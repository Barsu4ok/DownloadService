using DownloadService.Config;
using DownloadService.Models;
using DownloadService.Services.Interfaces;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using System;
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
        public DbDataTarget(IOptionsMonitor<MySqlConnectionConfig> connectionConfig)
        {
            _connectionConfig = connectionConfig;
        }

        public void writeAllData(IEnumerable<CellInfo> cellInfoList)
        {
            var myConnectionString = $"server={_connectionConfig.CurrentValue.server};uid={_connectionConfig.CurrentValue.user};" +
                $"pwd={_connectionConfig.CurrentValue.password};database={_connectionConfig.CurrentValue.database}";
            try
            {
                using (var connection = new MySqlConnection())
                {
                    connection.ConnectionString = myConnectionString;
                    connection.Open();
                    var transaction = connection.BeginTransaction();
                    int batchSize = 100; 
                    int count = 0;
                    string QUERY = "INSERT INTO towerdata(typeTower,countryCode,networkCode,lac,cellId,lon,lan)" +
                        " VALUES (@typeTower,@countryCode,@networkCode,@lac,@cellId,@lon,@lan)";
                    foreach (var cellObject in cellInfoList)
                    {
                        using (var command = new MySqlCommand(QUERY, connection))
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
    }
}
