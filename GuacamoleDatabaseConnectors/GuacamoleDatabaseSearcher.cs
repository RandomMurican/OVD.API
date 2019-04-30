﻿using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace OVD.API.GuacamoleDatabaseConnectors
{
    public class GuacamoleDatabaseSearcher : IGuacSearcher
    {
        /// <summary>
        /// Searchs for the name of a specified group in the connection group table.
        /// </summary>
        /// <returns><c>true</c>, if group name was found, <c>false</c> otherwise.</returns>
        /// <param name="groupName">Group name.</param>
        public bool SearchConnectionGroupName(string groupName, ref List<Exception> excepts)
        {
            const string queryString =
                "SELECT COUNT(*) FROM guacamole_connection_group " +
                "WHERE connection_group_name=@input";

            return SearchQuery(queryString, groupName, ref excepts);
        }


        public bool SearchConnectedUserGroup(string connectionGroupId, ref List<Exception> excepts)
        {
            const string queryString =
                "SELECT COUNT(*) FROM guacamole_connection_group_permission " +
                "WHERE connection_group_id=@input";

            return SearchQuery(queryString, connectionGroupId, ref excepts);
        }


        /// <summary>
        /// Searchs for the name of a specified group in the user group table.
        /// </summary>
        /// <returns><c>true</c>, if group name was found, <c>false</c> otherwise.</returns>
        /// <param name="groupName">Group name.</param>
        public bool SearchUserGroupName(string groupName, ref List<Exception> excepts)
        {
            const string queryString =
                "SELECT COUNT(*) FROM guacamole_entity " +
                "WHERE name=@input AND type='USER_GROUP'";

            return SearchQuery(queryString, groupName, ref excepts);
        }

        /// <summary>
        /// Searchs for the dawgtag of the user.
        /// </summary>
        /// <returns><c>true</c>, if user name was found, <c>false</c> otherwise.</returns>
        /// <param name="dawgtag">Dawgtag.</param>
        public bool SearchUserName(string dawgtag, ref List<Exception> excepts)
        {
            const string queryString =
                "SELECT COUNT(*) FROM guacamole_entity " +
                "WHERE type='USER' AND name=@input";

            return SearchQuery(queryString, dawgtag, ref excepts);
        }
        

        /// <summary>
        /// Searchs for the name of the vm.
        /// </summary>
        /// <returns><c>true</c>, if vm name was found, <c>false</c> otherwise.</returns>
        /// <param name="vmName">Vm name.</param>
        public bool SearchVmName(string vmName, ref List<Exception> excepts)
        {
            const string queryString =
                 "SELECT COUNT(*) FROM guacamole_connection " +
                 "WHERE connection_name=@input";

            return SearchQuery(queryString, vmName, ref excepts);
        }


        /// <summary>
        /// General format for running a search query on the guacamole database
        /// </summary>
        /// <returns><c>true</c>, if field was found, <c>false</c> otherwise.</returns>
        /// <param name="queryString">Query string.</param>
        /// <param name="arg">Argument.</param>
        /// <param name="excepts">Exceptions.</param>
        private bool SearchQuery(string queryString, string arg, ref List<Exception> excepts)
        {
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
                        return (Convert.ToInt32(query.ExecuteScalar()) > 0) ? true : false;
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