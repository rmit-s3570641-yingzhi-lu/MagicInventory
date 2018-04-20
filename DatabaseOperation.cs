using System;
using System.Data;
using System.Data.SqlClient;

namespace MagicInventorySystem
{
    /// <summary>
    /// Database connection class
    /// </summary>
    public class DatabaseOperation
    {
        private SqlConnection connection = null;
        string Server = "server=wdt2018.australiaeast.cloudapp.azure.com;uid=s3570641;database=s3570641;pwd=abc123";

        /// <summary>
        /// Constructor
        /// </summary>
        public DatabaseOperation()
        {
            try
            {
                connection = new SqlConnection(Server);
            }
            catch
            {
                Console.WriteLine("Connect Error");
            }

        }

        /// <summary>
        /// public method for outside calling
        /// </summary>
        /// <param name="sql"></param>
        public void UpdateOperation(string sql, params SqlParameter[] parameters)
        {
            using (connection = new SqlConnection(Server))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = sql;
                command.Parameters.AddRange(parameters);

                var updates = command.ExecuteNonQuery();

                //Console.WriteLine($"{updates} rows updated.\n");
            }
        }

        /// <summary>
        /// public method for outside calling
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public DataTable getRequiredData(string sql, params SqlParameter[] parameters)
        {
            using (connection = new SqlConnection(Server))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = sql;
                command.Parameters.AddRange(parameters);

                var table = new DataTable();
                new SqlDataAdapter(command).Fill(table);

                return table;
            }   
        }

    }
}
