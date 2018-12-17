using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fameUploadConsole
{
    public class DemoLibrary
    {
        /// <summary>
        /// Retrieve connection string from app.config
        /// </summary>
        /// <param name="connectionName">Connection string name</param>
        /// <returns>string representing the connection string</returns>
        public static string GetConnectionString(string connectionName = "wacFameDB")
        {
            return ConfigurationManager.ConnectionStrings[connectionName].ConnectionString;
        }

        /// <summary>
        /// Get pk_farmBusiness for associated farm
        /// </summary>
        /// <param name="farmID">Farm ID </param>
        /// <returns></returns>
        public static int GetFarmBusinessByFarmId(string farmID)
        {
            int pk_FarmBusiness = 0;

            using (SqlConnection cnn = new SqlConnection(GetConnectionString()))
            {

                try
                {
                    cnn.Open();
                    SqlCommand sql = new SqlCommand($@"SELECT pk_farmBusiness FROM dbo.farmBusiness WHERE farmID = '{ farmID }'", cnn);
                    Int32 pkFarmBusiness = (Int32)sql.ExecuteScalar();
                    pk_FarmBusiness = pkFarmBusiness;

                                        
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

            }

            return pk_FarmBusiness;

        }
    }
}
