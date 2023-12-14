using DownloadService.Config;
using DownloadService.Interfaces;
using DownloadService.Models;
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
        private const string WRITE_ALL = "INSERT INTO towerdata(Radio,MCC,MNC,LAC,CID,LON,LAN) VALUES (@Radio,@MCC,@MNC,@LAC,@CID,@LON,@LAN)";
        private const string UPDATE = "UPDATE towerdata SET Radio = @Radio, MCC = @MCC, MNC=@MNC, LAC = @LAC, CID = @CID, LON= @LON, LAN=@LAN  WHERE id = @number";
        private const string DELETE_ALL = "DELETE FROM towerdata";
        public MySQLDataTarget(IOptionsMonitor<MySqlConnectionConfig> connectionConfig)
        {
            _connectionConfig = connectionConfig;
        }

        public void writeAllData(IEnumerable<CellInfo> cellInfoList)
        {
            try
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
                    int batchSize = 100; 
                    int count = 0;
                    foreach (var cellTower in cellInfoList)
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

                            if (count % batchSize == 0) 
                            {
                                transaction.Commit();
                                transaction = connection.BeginTransaction(); 
                            }
                        }
                    }
                    transaction.Commit();
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("ERROR connection to MySQL Server: " + ex.ToString());
            }
        }
        public void Update(CellInfo cellTower, int number)
        {
            try
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
                        command.Parameters.AddWithValue("@number", number);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("ERROR connection to MySQL Server: " + ex.ToString());
            }
        }
    }
}
