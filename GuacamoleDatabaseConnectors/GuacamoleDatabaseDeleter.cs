﻿using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using OVD.API.Exceptions;

namespace OVD.API.GuacamoleDatabaseConnectors
{
    public class GuacamoleDatabaseDeleter
    {

        /// <summary>
        /// Deletes the user group associated with the given name.
        /// All members associated with the group are removed.
        /// </summary>
        /// <returns><c>true</c>, if user group was deleted, <c>false</c> otherwise.</returns>
        /// <param name="groupName">Group name.</param>
        /// <param name="excepts">Exceptions.</param>
        /*public bool DeleteUserGroup(string groupName, ref List<Exception> excepts)
        {
            const string queryString =
                "DELETE FROM guacamole_entity " +
                "WHERE name=@groupname AND type='USER_GROUP'";

            Queue<string> argNames = new Queue<string>();
            argNames.Enqueue("@groupname");

            Queue<string> args = new Queue<string>();
            args.Enqueue(groupName);

            //Insert the usergroup into the entity table
            return DeleteQuery(queryString, argNames, args, ref excepts);
        }*/


        /// <summary>
        /// Deletes the connection group associated with the given group name.
        /// All connections associated with this connection group are auto deleted
        /// </summary>
        /// <returns><c>true</c>, if connection group was deleted, <c>false</c> otherwise.</returns>
        /// <param name="groupName">Group name.</param>
        /// <param name="excepts">Exceptions.</param>
        public bool DeleteConnectionGroup(string groupId, ref List<Exception> excepts)
        {
            const string queryString =
                "DELETE FROM guacamole_connection_group " +
                "WHERE connection_group_id=@id";

            try
            {
                using (GuacamoleDatabaseConnector gdbc = new GuacamoleDatabaseConnector(ref excepts))
                {
                    using (MySqlCommand query = new MySqlCommand(queryString, gdbc.getConnection()))
                    {
                        query.Prepare();

                        //Add the agrument names and values
                        query.Parameters.AddWithValue("@id", groupId);

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
        /// Deletes the user from user group.
        /// </summary>
        /// <returns><c>true</c>, if user was deleted from the user group, <c>false</c> otherwise.</returns>
        /// <param name="groupName">Group name.</param>
        /// <param name="dawgtag">Dawgtag.</param>
        /// <param name="excepts">Excepts.</param>
        public bool DeleteUserFromUserGroup(int groupId, string dawgtag, ref List<Exception> excepts)
        {
            const string nameQueryString =
                "(SELECT connection_group_name FROM guacamole_connection_group " +
                 "WHERE connection_group_id=@id)";

            const string groupIdQueryString =
                "(SELECT user_group_id FROM guacamole_user_group, guacamole_entity " +
                "WHERE name =" +  nameQueryString + " AND type = 'USER_GROUP' " +
                "AND guacamole_user_group.entity_id=guacamole_entity.entity_id)";

            const string userIdQueryString =
                "(SELECT entity_id FROM guacamole_entity " +
                "WHERE name = @username AND type = 'USER')";

            const string memberQueryString =
                "DELETE FROM guacamole_user_group_member " +
                "WHERE user_group_id=" + groupIdQueryString + " AND member_entity_id =" + userIdQueryString;

            try
            {
                using (GuacamoleDatabaseConnector gdbc = new GuacamoleDatabaseConnector(ref excepts))
                {
                    using (MySqlCommand query = new MySqlCommand(memberQueryString, gdbc.getConnection()))
                    {
                        query.Prepare();

                        //Add the agrument names and values
                        query.Parameters.AddWithValue("@id", groupId);
                        query.Parameters.AddWithValue("@username", dawgtag);

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