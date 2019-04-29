using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using OVD.API.Exceptions;

namespace OVD.API.GuacamoleDatabaseConnectors
{
    public class GuacamoleDatabaseUpdater
    {

        /// <summary>
        /// Searchs for the name of a specified group in the connection group table.
        /// </summary>
        /// <returns><c>true</c>, if group name was found, <c>false</c> otherwise.</returns>
        public bool UpdateAddConnectionGroupConnection(int connectionGroupId, int connectionId, ref List<Exception> excepts)
        {
            const string queryString =
                "UPDATE guacamole_connection SET parent_id = @groupId WHERE connection_id = @connectionId";

            try
            {
                using (GuacamoleDatabaseConnector gdbc = new GuacamoleDatabaseConnector(ref excepts))
                {
                    using (MySqlCommand query = new MySqlCommand(queryString, gdbc.getConnection()))
                    {
                        query.Prepare();

                        //Add the agruments given
                        query.Parameters.AddWithValue("@groupId", connectionGroupId);
                        query.Parameters.AddWithValue("@connectionId", connectionId);

                        return (query.ExecuteNonQuery() > 0);
                    }
                }
            }
            catch (Exception e)
            {
                excepts.Add(e);
                return false;
            }
        }


        /// <summary>
        /// Searchs for the name of a specified group in the connection group table.
        /// </summary>
        /// <returns><c>true</c>, if group name was found, <c>false</c> otherwise.</returns>
        public bool UpdateRemoveConnectionGroupConnection(int connectionId, ref List<Exception> excepts)
        {
            const string queryString =
                "UPDATE guacamole_connection SET parent_id = NULL WHERE connection_id = @connectionId";

            try
            {
                using (GuacamoleDatabaseConnector gdbc = new GuacamoleDatabaseConnector(ref excepts))
                {
                    using (MySqlCommand query = new MySqlCommand(queryString, gdbc.getConnection()))
                    {
                        query.Prepare();

                        //Add the agruments given
                        query.Parameters.AddWithValue("@connectionId", connectionId);

                        return (query.ExecuteNonQuery() > 0);
                    }
                }
            }
            catch (Exception e)
            {
                excepts.Add(e);
                return false;
            }
        }
    }
}