using DownloadService.Config;
using DownloadService.Interfaces;
using DownloadService.Models;
using FluentValidation;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using System.Data;



namespace DownloadService.DataAccess
{
    public class MySqlDataTarget : IDataTarget
    {
        private readonly IOptionsMonitor<MySqlConnectionConfig> _connectionConfig;
        private readonly IValidator<MySqlConnectionConfig> _mySqlConnectionConfigValidator;
        private const string GetLatAndLon = "SELECT LON, LAT FROM @tableName  WHERE MCC = @MCC AND MNC=@MNC AND LAC = @LAC AND CID = @CID;";
        private const string GetLbs = "SELECT MCC, MNC, LAC, CID FROM @tableName WHERE LON = @LON AND LAT = @LAT";
        private const string CreateTable =
            "'CREATE TABLE `towerdata` (" +
            " `id` int NOT NULL AUTO_INCREMENT," +
            "  `Act` enum(\\'GSM\\',\\'UMTS\\') NOT NULL," +
            "`MCC` smallint NOT NULL," +
            " `MNC` smallint NOT NULL," +
            " `LAC` mediumint NOT NULL," +
            "  `CID` int NOT NULL," +
            " `LON` double NOT NULL," +
            " `LAT` double NOT NULL," +
            " PRIMARY KEY (`id`)," +
            "  KEY `idx_LON_LAN` (`LON`,`LAT`)," +
            "  KEY `idx_LBS` (`MCC`,`MNC`,`LAC`,`CID`))" +
            " ENGINE=InnoDB AUTO_INCREMENT=1757506 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci'";
        public MySqlDataTarget(IOptionsMonitor<MySqlConnectionConfig> connectionConfig, IValidator<MySqlConnectionConfig> mySqlConnectionConfigValidator)
        {
            _connectionConfig = connectionConfig;
            _mySqlConnectionConfigValidator = mySqlConnectionConfigValidator;
        }

        public void WriteData(IEnumerable<CellInfo> cellInfoList)
        {
            ValidateConfig(_mySqlConnectionConfigValidator, _connectionConfig);

            const int batchSize = 10;
            var count = 0;

            using var connection = new MySqlConnection(_connectionConfig.CurrentValue.ConnectionString);
            connection.Open();

            var transaction = connection.BeginTransaction();
            try
            {
                var delete = new MySqlCommand("DELETE FROM towerdata", connection);
                delete.ExecuteNonQuery();

                var command = new MySqlCommand($"SELECT *  FROM {_connectionConfig.CurrentValue.TableName} WHERE 1=0;", connection);
                var adapter = new MySqlDataAdapter(command);
                var builder = new MySqlCommandBuilder(adapter);
                var dataset = new DataSet();
                adapter.Fill(dataset);
                var insert = builder.GetInsertCommand();
                
                foreach (var cellTower in cellInfoList)
                {
                    insert.Parameters.Clear();

                    insert.Parameters.Add("@p1", MySqlDbType.VarChar).Value = cellTower.Act;
                    insert.Parameters.Add("@p2", MySqlDbType.Int16).Value = cellTower.Mcc;
                    insert.Parameters.Add("@p3", MySqlDbType.Int16).Value = cellTower.Mnc;
                    insert.Parameters.Add("@p4", MySqlDbType.Int32).Value = cellTower.Lac;
                    insert.Parameters.Add("@p5", MySqlDbType.Int32).Value = cellTower.Cid;
                    insert.Parameters.Add("@p6", MySqlDbType.Double).Value = cellTower.Lon;
                    insert.Parameters.Add("@p7", MySqlDbType.Double).Value = cellTower.Lat;

                    insert.ExecuteNonQuery();
                    count++;

                    if (count % batchSize != 0) continue;
                    adapter.Fill(dataset);
                    transaction.Commit();
                    transaction = connection.BeginTransaction();
                }
            }
            catch (MySqlException ex)
            {
                transaction.Rollback();
                Console.WriteLine($"Error MySQL: {ex.Message}");
            }
            transaction.Commit();
        }
        public CellInfo GetCoordinatesByLbs(CellInfo cellTower)
        {
            var info = new CellInfo();
            ValidateConfig(_mySqlConnectionConfigValidator, _connectionConfig);
            using var connection = new MySqlConnection(_connectionConfig.CurrentValue.ConnectionString);
            connection.Open();

            using var command = new MySqlCommand(GetLatAndLon, connection);
            command.Parameters.AddWithValue("@tableName", _connectionConfig.CurrentValue.TableName);
            command.Parameters.AddWithValue("@MCC", cellTower.Mcc);
            command.Parameters.AddWithValue("@MNC", cellTower.Mnc);
            command.Parameters.AddWithValue("@LAC", cellTower.Lac);
            command.Parameters.AddWithValue("@CID", cellTower.Cid);
            using var reader = command.ExecuteReader();
            if (!reader.Read())
            {
                return info;
            }

            if (double.TryParse(reader["LON"].ToString(), out var lon))
            {
                info.Lon = lon;
            }

            if (double.TryParse(reader["LAT"].ToString(), out var lan))
            {
                info.Lat = lan;
            }

            return info;
        }
        public CellInfo GetLsb(CellInfo cellTower)
        {
            var info = new CellInfo();
            ValidateConfig(_mySqlConnectionConfigValidator, _connectionConfig);
            using var connection = new MySqlConnection(_connectionConfig.CurrentValue.ConnectionString);
            connection.Open();

            using var command = new MySqlCommand(GetLbs, connection);
            command.Parameters.AddWithValue("@tableName", _connectionConfig.CurrentValue.TableName);
            command.Parameters.AddWithValue("@LON", cellTower.Lon);
            command.Parameters.AddWithValue("@LAT", cellTower.Lat);

            using var reader = command.ExecuteReader();
            if (!reader.Read()) return info;
            info.Mcc = (Convert.ToInt32(reader["MCC"]));
            info.Mnc = Convert.ToInt32(reader["MNC"]);
            info.Lac = Convert.ToInt32(reader["LAC"]);
            info.Cid = Convert.ToInt32(reader["CID"]);

            return info;
        }

        private static void ValidateConfig(IValidator<MySqlConnectionConfig> validator, IOptionsMonitor<MySqlConnectionConfig> config)
        {
            var resultValidation = validator.Validate(config.CurrentValue);
            if (!resultValidation.IsValid)
            {
                throw new ValidationException($"Validation failed: {resultValidation.Errors}");
            }
        }
    }
}
