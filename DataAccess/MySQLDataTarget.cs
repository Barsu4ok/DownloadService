using DownloadService.Config;
using DownloadService.Interfaces;
using DownloadService.Models;
using DownloadService.Validators;
using FluentValidation;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace DownloadService.DataAccess
{
    public class MySQLDataTarget : IDataTarget
    {
        private readonly IOptionsMonitor<MySqlConnectionConfig> _connectionConfig;
        private readonly IValidator<MySqlConnectionConfig> _mySqlConnectionConfigValidator;
        private const string WRITE_ALL = "INSERT INTO towerdata(Radio,MCC,MNC,LAC,CID,LON,LAN) VALUES (@Radio,@MCC,@MNC,@LAC,@CID,@LON,@LAN);";
        private const string UPDATE = "UPDATE towerdata SET Radio = @Radio, MCC = @MCC, MNC=@MNC, LAC = @LAC, CID = @CID, LON= @LON, LAN=@LAN  WHERE id = @id;";
        private const string DELETE_ALL = "DELETE FROM towerdata;";
        private const string GET_COORDINATES_BY_LBS = "SELECT LON, LAN FROM towerdata  WHERE MCC = @MCC AND MNC=@MNC AND LAC = @LAC AND CID = @CID;";
        private const string GET_LBS = "SELECT MCC, MNC, LAC, CID FROM towerdata WHERE LON = @LON AND LAN = @LAN";

        private const string CREATE_STATEMENT = "CREATE TABLE `towerdata`(" +
                                                " `id` int NOT NULL AUTO_INCREMENT," +
                                                " `Radio` varchar(20) NOT NULL," +
                                                "  `MCC` int NOT NULL," +
                                                " `MNC` int NOT NULL," +
                                                " `LAC` int NOT NULL," +
                                                "  `CID` int NOT NULL," +
                                                "  `LON` varchar(30) NOT NULL," +
                                                "  `LAN` varchar(30) NOT NULL," +
                                                "  PRIMARY KEY (`id`)," +
                                                "  UNIQUE KEY `id_UNIQUE` (`id`)," +
                                                "  KEY `idx_towerdata_CID` (`CID`)," +
                                                "  KEY `idx_LBS` (`Radio`,`MCC`,`MNC`,`LAC`,`CID`)," +
                                                "  KEY `idx_LON` (`LON`)," +
                                                "  KEY `idx_LAN` (`LAN`)," +
                                                "  KEY `idx_LON_LAN` (`LON`,`LAN`)," +
                                                "  CONSTRAINT `chk_Radio` CHECK ((`Radio` in (_utf8mb4\\'GSM\\',_utf8mb4\\'UMTS\\')))" +
                                                ") ENGINE=InnoDB AUTO_INCREMENT=654447 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci'";
        public MySQLDataTarget(IOptionsMonitor<MySqlConnectionConfig> connectionConfig, IValidator<MySqlConnectionConfig> mySqlConnectionConfigValidator)
        {
            _connectionConfig = connectionConfig;
            _mySqlConnectionConfigValidator = mySqlConnectionConfigValidator;
        }

        public void writeData(IEnumerable<CellInfo> cellInfoList)
        {
            var resultValidation = _mySqlConnectionConfigValidator.Validate(_connectionConfig.CurrentValue);
            if (resultValidation.IsValid)
            {
                using (var connection = new MySqlConnection())
                {
                    connection.ConnectionString = _connectionConfig.CurrentValue.connectionString;
                    connection.Open();
                    using (var command = new MySqlCommand(DELETE_ALL, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                    var transaction = connection.BeginTransaction();
                    int batchSize = 10;
                    int count = 0;
                    foreach (var cellTower in cellInfoList)
                    {
                        try
                        {
                            using (var command = new MySqlCommand(WRITE_ALL, connection))
                            {
                                command.Parameters.AddWithValue("@Radio", cellTower.Radio);
                                command.Parameters.AddWithValue("@MCC", cellTower.MCC);
                                command.Parameters.AddWithValue("@MNC", cellTower.MNC);
                                command.Parameters.AddWithValue("@LAC", cellTower.LAC);
                                command.Parameters.AddWithValue("@CID", cellTower.CID);
                                command.Parameters.AddWithValue("@LON", cellTower.LON);
                                command.Parameters.AddWithValue("@LAN", cellTower.LAN);

                                command.ExecuteNonQuery();
                                count++;

                                if (count % batchSize != 0) continue;
                                transaction.Commit();
                                transaction = connection.BeginTransaction();
                            }
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                        }
                    }
                    transaction.Commit();
                }
            }
            else throw new Exception("Invalid connection string");
        }
        public void Update(CellInfo cellTower, int id)
        {

            using (var connection = new MySqlConnection())
            {
                connection.ConnectionString = _connectionConfig.CurrentValue.connectionString;
                connection.Open();
                using (var command = new MySqlCommand(UPDATE, connection))
                {
                    command.Parameters.AddWithValue("@Radio", cellTower.Radio);
                    command.Parameters.AddWithValue("@MCC", cellTower.MCC);
                    command.Parameters.AddWithValue("@MNC", cellTower.MNC);
                    command.Parameters.AddWithValue("@LAC", cellTower.LAC);
                    command.Parameters.AddWithValue("@CID", cellTower.CID);
                    command.Parameters.AddWithValue("@LON", cellTower.LON);
                    command.Parameters.AddWithValue("@LAN", cellTower.LAN);
                    command.Parameters.AddWithValue("@id", id);

                    command.ExecuteNonQuery();
                }
            }
        }
        public Coordinates GetCoordinatesByLbs(CellInfo cellTower)
        {
            var coordinates = new Coordinates();
            var resultValidation = _mySqlConnectionConfigValidator.Validate(_connectionConfig.CurrentValue);
            if (resultValidation.IsValid)
            {
                using (var connection = new MySqlConnection())
                {
                    connection.ConnectionString = _connectionConfig.CurrentValue.connectionString;
                    connection.Open();

                    using (var command = new MySqlCommand(GET_COORDINATES_BY_LBS, connection))
                    {
                        command.Parameters.AddWithValue("@MCC", cellTower.MCC);
                        command.Parameters.AddWithValue("@MNC", cellTower.MNC);
                        command.Parameters.AddWithValue("@LAC", cellTower.LAC);
                        command.Parameters.AddWithValue("@CID", cellTower.CID);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                if (double.TryParse(reader["LON"].ToString(), out var lon))
                                {
                                    coordinates.Lon = lon;
                                }
                                if (double.TryParse(reader["LAN"].ToString(), out var lan))
                                {
                                    coordinates.Lan = lan;
                                }
                            }
                        }
                        return coordinates;
                    }
                }
            }
            else throw new Exception("Invalid connection string");
        }
        public LbsInfo GetLsb(CellInfo cellTower)
        {
            var info = new LbsInfo();
            var resultValidation = _mySqlConnectionConfigValidator.Validate(_connectionConfig.CurrentValue);
            if (resultValidation.IsValid)
            {
                using (var connection = new MySqlConnection())
                {
                    connection.ConnectionString = _connectionConfig.CurrentValue.connectionString;
                    connection.Open();

                    using (var command = new MySqlCommand(GET_LBS, connection))
                    {
                        command.Parameters.AddWithValue("@LON", cellTower.LON);
                        command.Parameters.AddWithValue("@LAN", cellTower.LAN);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                info.Mcc = (Convert.ToInt32(reader["MCC"]));
                                info.Mnc = Convert.ToInt32(reader["MNC"]);
                                info.Lac = Convert.ToInt32(reader["LAC"]);
                                info.Cid = Convert.ToInt32(reader["CID"]);
                            }
                        }
                        return info;
                    }
                }
            }
            else throw new Exception("Invalid connection string");
        }
    }
}
