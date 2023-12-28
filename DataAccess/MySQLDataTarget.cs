using DownloadService.Config;
using DownloadService.Interfaces;
using DownloadService.Models;
using FluentValidation;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using System.Globalization;
using System.Text;


namespace DownloadService.DataAccess
{
    public class MySqlDataTarget : IDataTarget
    {
        private readonly ILoggerService _logger;
        private readonly IOptionsMonitor<MySqlConnectionConfig> _connectionConfig;
        private readonly IValidator<MySqlConnectionConfig> _mySqlConnectionConfigValidator;
        private const string GetLatAndLon = "SELECT lng, lat FROM @tableName  WHERE mcc = @MCC AND mnc = @MNC AND lac = @LAC AND cid = @CID;";
        private const string GetLbs = "SELECT mcc, mnc, lac, cid FROM @tableName WHERE lng = @LON AND lat = @LAT";

        private const string Query = "INSERT INTO towerdata(radio,mcc,mnc,lac,cid,lng,lat) VALUES ";
        
        public MySqlDataTarget(ILoggerService logger,IOptionsMonitor<MySqlConnectionConfig> connectionConfig, IValidator<MySqlConnectionConfig> mySqlConnectionConfigValidator)
        {
            _logger = logger;
            _connectionConfig = connectionConfig;
            _mySqlConnectionConfigValidator = mySqlConnectionConfigValidator;
        }

        public void WriteData(IEnumerable<CellInfo> cellInfoList)
        {
            ValidateConfig(_mySqlConnectionConfigValidator, _connectionConfig); 
            const int batchSize = 20;
            var count = 0;

            using var connection = new MySqlConnection(_connectionConfig.CurrentValue.ConnectionString);
            connection.Open();
            var commandCheckState = new MySqlCommand($"SELECT COUNT(*) FROM {_connectionConfig.CurrentValue.TableName};", connection);

            _logger.Log(LogLevel.Information, "number of records in the database at the beginning: " +(long)commandCheckState.ExecuteScalar());
            try
            {
                var delete = new MySqlCommand("DELETE FROM towerdata", connection);
                delete.ExecuteNonQuery();
                _logger.Log(LogLevel.Information, "the number of records in the database after clearing: " + (long)commandCheckState.ExecuteScalar());
                var sb = new StringBuilder(500);
                sb.Append(Query);
                foreach (var cellTower in cellInfoList)
                {
                    sb.Append("('").Append(cellTower.Radio).Append("'").Append(",");
                    sb.Append(cellTower.Mcc).Append(",");
                    sb.Append(cellTower.Mnc).Append(",");
                    sb.Append(cellTower.Lac).Append(",");
                    sb.Append(cellTower.Cid).Append(",");
                    sb.Append(cellTower.Lng.ToString(CultureInfo.InvariantCulture)).Append(",");
                    sb.Append(cellTower.Lat.ToString(CultureInfo.InvariantCulture)).Append("),");
                    count++;
                    if (count % batchSize == 0)
                    {
                        sb.Remove(sb.Length - 1, 1);
                        using var command = new MySqlCommand(sb.ToString(),connection);
                        command.ExecuteNonQuery();
                        sb.Clear();
                        sb.Append(Query);
                    }
                }
                _logger.Log(LogLevel.Information, "number of records in the database at the end: " + (long)commandCheckState.ExecuteScalar());
            }
            catch (MySqlException ex)
            {
                _logger.Log(LogLevel.Error,ex.Message);
            }
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
                info.Lng = lon;
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
            command.Parameters.AddWithValue("@LON", cellTower.Lng);
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
