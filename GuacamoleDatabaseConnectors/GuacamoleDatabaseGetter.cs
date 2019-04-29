using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace OVD.API.GuacamoleDatabaseConnectors
{
    public class GuacamoleDatabaseGetter
    {
        /// <summary>
        /// Searchs for the name of a specified group in the connection group table.
        /// </summary>
        /// <returns><c>true</c>, if group name was found, <c>false</c> otherwise.</returns>
        /// <param name="groupName">Group name.</param>
        public Queue<int> GetConnectionGroupId(string groupName, ref List<Exception> excepts)
        {
            const string queryString =
                "SELECT connection_group_id FROM guacamole_connection_group " +
                "WHERE connection_group_name=@input";

            return GetIntegerQuery(queryString, groupName, ref excepts);
        }


        /// <summary>
        /// General format for running a search query on the guacamole database
        /// </summary>
        /// <returns><c>true</c>, if field was found, <c>false</c> otherwise.</returns>
        /// <param name="queryString">Query string.</param>
        /// <param name="arg">Argument.</param>
        /// <param name="exceptions">Exceptions.</param>
        private Queue<string> GetStringQuery(string queryString, string arg, ref List<Exception> excepts)
        {
            //Stores the Values found from the database
            Queue<string> queryResults = new Queue<string>();
            try
            {
                using (GuacamoleDatabaseConnector gdbc = new GuacamoleDatabaseConnector(ref excepts))
                {
                    using (MySqlCommand query = new MySqlCommand(queryString, gdbc.getConnection()))
                    {
                        query.Prepare();

                        //Add the agruments given if found
                        if (arg != null)
                        {
                            query.Parameters.AddWithValue("@input", arg);
                        }

                        //Collect the query result column
                        using (MySqlDataReader reader = query.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                queryResults.Enqueue((string)reader[0]);
                            }
                        }
                        return queryResults;
                    }
                }
            }
            catch (Exception e)
            {
                excepts.Add(e);
                Console.WriteLine("\n\n\n\n" + e.Message);
                return queryResults;
            }
        }


        /// <summary>
        /// General format for running a search query on the guacamole database
        /// </summary>
        /// <returns><c>true</c>, if field was found, <c>false</c> otherwise.</returns>
        /// <param name="queryString">Query string.</param>
        /// <param name="arg">Argument.</param>
        /// <param name="exceptions">Exceptions.</param>
        private Queue<int> GetIntegerQuery(string queryString, string arg, ref List<Exception> excepts)
        {
            //Stores the Values found from the database
            Queue<int> queryResults = new Queue<int>();
            try
            {
                using (GuacamoleDatabaseConnector gdbc = new GuacamoleDatabaseConnector(ref excepts))
                {
                    using (MySqlCommand query = new MySqlCommand(queryString, gdbc.getConnection()))
                    {
                        query.Prepare();

                        //Add the agruments given if found
                        if (arg != null)
                        {
                            query.Parameters.AddWithValue("@input", arg);
                        }

                        //Collect the query result column
                        using (MySqlDataReader reader = query.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                queryResults.Enqueue((int)reader[0]);
                            }
                        }
                        return queryResults;
                    }
                }
            }
            catch (Exception e)
            {
                excepts.Add(e);
                Console.WriteLine("\n\n\n\n" + e.Message);
                return queryResults;
            }
        }
    }
}