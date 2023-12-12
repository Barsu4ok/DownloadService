using DownloadService.Config;
using DownloadService.Models;
using DownloadService.Services.Interfaces;
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
    public class DbDataTarget : IDataTarget
    {
        private readonly IOptionsMonitor<MySqlConnectionConfig> _connectionConfig;
        private string WRITE_ALL = "INSERT INTO towerdata(typeTower,countryCode,networkCode,lac,cellId,lon,lan)" +
                        " VALUES (@typeTower,@countryCode,@networkCode,@lac,@cellId,@lon,@lan)";
        private string UPDATE = "UPDATE towerdata SET typeTower = @typeTower, countryCode = @countryCode, networkCode=@networkCode," +
            "lac = @lac, cellId = @cellId, lon= @lon, lan=@lan  WHERE cellId = @number";
        private string DELETE_ALL = "DELETE FROM towerdata";
        public DbDataTarget(IOptionsMonitor<MySqlConnectionConfig> connectionConfig)
        {
            _connectionConfig = connectionConfig;
        }

        public void writeAllData(IEnumerable<CellInfo> cellInfoList)
        {
            var connectionString = $"server={_connectionConfig.CurrentValue.server};uid={_connectionConfig.CurrentValue.user};" +
                $"pwd={_connectionConfig.CurrentValue.password};database={_connectionConfig.CurrentValue.database}";
            try
            {
                using (var connection = new MySqlConnection())
                {
                    connection.ConnectionString = connectionString;
                    connection.Open();
                    var transaction = connection.BeginTransaction();
                    int batchSize = 100; 
                    int count = 0;
                    foreach (var cellObject in cellInfoList)
                    {
                        using (var command = new MySqlCommand(WRITE_ALL, connection))
                        {
                            command.Parameters.AddWithValue("@typeTower", cellObject.type);
                            command.Parameters.AddWithValue("@countryCode", cellObject.countryCode);
                            command.Parameters.AddWithValue("@networkCode", cellObject.networkCode);
                            command.Parameters.AddWithValue("@lac", cellObject.lac);
                            command.Parameters.AddWithValue("@cellId", cellObject.cellId);
                            command.Parameters.AddWithValue("@lon", cellObject.lon);
                            command.Parameters.AddWithValue("@lan", cellObject.lan);

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
        public void Update(CellInfo cellTower)
        {
            var connectionString = $"server={_connectionConfig.CurrentValue.server};uid={_connectionConfig.CurrentValue.user};" +
                $"pwd={_connectionConfig.CurrentValue.password};database={_connectionConfig.CurrentValue.database}";
            try
            {
                using (var connection = new MySqlConnection())
                {
                    connection.ConnectionString = connectionString;
                    connection.Open();
                    using (var command = new MySqlCommand(UPDATE, connection))
                    {
                        command.Parameters.AddWithValue("@typeTower", cellTower.type);
                        command.Parameters.AddWithValue("@countryCode", cellTower.countryCode);
                        command.Parameters.AddWithValue("@networkCode", cellTower.networkCode);
                        command.Parameters.AddWithValue("@lac", cellTower.lac);
                        command.Parameters.AddWithValue("@cellId", cellTower.cellId);
                        command.Parameters.AddWithValue("@lon", cellTower.lon);
                        command.Parameters.AddWithValue("@lan", cellTower.lan);
                        command.Parameters.AddWithValue("@number", cellTower.cellId);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("ERROR connection to MySQL Server: " + ex.ToString());
            }
        }
        public void deleteAll()
        {
            var connectionString = $"server={_connectionConfig.CurrentValue.server};uid={_connectionConfig.CurrentValue.user};" +
                $"pwd={_connectionConfig.CurrentValue.password};database={_connectionConfig.CurrentValue.database}";
            try
            {
                using (var connection = new MySqlConnection())
                {
                    connection.ConnectionString = connectionString;
                    connection.Open();
                    using (var command = new MySqlCommand(DELETE_ALL, connection))
                    {
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
