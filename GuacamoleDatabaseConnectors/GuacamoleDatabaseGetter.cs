using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using OVD.API.Dtos;

namespace OVD.API.GuacamoleDatabaseConnectors
{
    public class GuacamoleDatabaseGetter
    {
        /// <summary>
        /// Searchs for the name of a specified group in the connection group table.
        /// </summary>
        /// <returns><c>true</c>, if group name was found, <c>false</c> otherwise.</returns>
        /// <param name="groupName">Group name.</param>
        public int GetConnectionGroupId(string groupName, ref List<Exception> excepts)
        {
            const string queryString =
                "SELECT connection_group_id FROM guacamole_connection_group " +
                "WHERE connection_group_name=@name";

            try
            {
                using (GuacamoleDatabaseConnector gdbc = new GuacamoleDatabaseConnector(ref excepts))
                {
                    using (MySqlCommand query = new MySqlCommand(queryString, gdbc.getConnection()))
                    {
                        query.Prepare();

                        //Add the agruments given
                        query.Parameters.AddWithValue("@name", groupName);

                        //Collect the query result column
                        using (MySqlDataReader reader = query.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                return Int32.Parse(reader.GetValue(0).ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                excepts.Add(e);
            }
            return -1;
        }


        /// <summary>
        /// Searchs for the name of a specified group in the connection group table.
        /// </summary>
        /// <returns><c>true</c>, if group name was found, <c>false</c> otherwise.</returns>
        public List<ConnectionForListDto> GetAllConnectionInfo(ref List<Exception> excepts)
        {
            List<ConnectionForListDto> connectionInfo = new List<ConnectionForListDto>();

            const string queryString =
                "SELECT connection_id, connection_name, protocol, max_connections, parent_id FROM guacamole_connection";

            try
            {
                using (GuacamoleDatabaseConnector gdbc = new GuacamoleDatabaseConnector(ref excepts))
                {
                    using (MySqlCommand query = new MySqlCommand(queryString, gdbc.getConnection()))
                    {
                        query.Prepare();

                        //Collect the query result column
                        using (MySqlDataReader reader = query.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ConnectionForListDto infoDto = new ConnectionForListDto();
                                infoDto.Id = Int32.Parse(reader.GetValue(0).ToString());
                                infoDto.Name = reader.GetValue(1).ToString();
                                infoDto.Protocol = reader.GetValue(2).ToString();

                                if(reader.GetValue(3).ToString() != String.Empty)
                                {
                                    infoDto.MaxConnections = Int32.Parse(reader.GetValue(3).ToString());
                                }

                                if(reader.GetValue(4).ToString() != String.Empty)
                                {
                                    infoDto.HasGroup = true;
                                }
                                else
                                {
                                    infoDto.HasGroup = false;
                                }
                                
                                connectionInfo.Add(infoDto);
                            }
                        }
                        return connectionInfo;
                    }
                }
            }
            catch (Exception e)
            {
                excepts.Add(e);
                Console.WriteLine("\n\n\n\n" + e.Message);
                return null;
            }
        }


        /// <summary>
        /// Searchs for the name of a specified group in the connection group table.
        /// </summary>
        /// <returns><c>true</c>, if group name was found, <c>false</c> otherwise.</returns>
        public List<ConnectionForListDto> GetAllGroupConnections(int id, ref List<Exception> excepts)
        {
            List<ConnectionForListDto> connectionInfo = new List<ConnectionForListDto>();

            const string queryString =
                "SELECT connection_id, connection_name, protocol, max_connections, parent_id FROM guacamole_connection " +
                "WHERE parent_id = @id";

            try
            {
                using (GuacamoleDatabaseConnector gdbc = new GuacamoleDatabaseConnector(ref excepts))
                {
                    using (MySqlCommand query = new MySqlCommand(queryString, gdbc.getConnection()))
                    {
                        query.Prepare();

                        //Add the agruments given
                        query.Parameters.AddWithValue("@id", id);

                        //Collect the query result column
                        using (MySqlDataReader reader = query.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ConnectionForListDto infoDto = new ConnectionForListDto();
                                infoDto.Id = Int32.Parse(reader.GetValue(0).ToString());
                                infoDto.Name = reader.GetValue(1).ToString();
                                infoDto.Protocol = reader.GetValue(2).ToString();

                                if(reader.GetValue(3).ToString() != String.Empty)
                                {
                                    infoDto.MaxConnections = Int32.Parse(reader.GetValue(3).ToString());
                                }

                                if(reader.GetValue(4).ToString() != String.Empty)
                                {
                                    infoDto.HasGroup = true;
                                }
                                else
                                {
                                    infoDto.HasGroup = false;
                                }
                                
                                connectionInfo.Add(infoDto);
                            }
                        }
                        return connectionInfo;
                    }
                }
            }
            catch (Exception e)
            {
                excepts.Add(e);
                Console.WriteLine("\n\n\n\n" + e.Message);
                return null;
            }
        }



        /// <summary>
        /// Searchs for the name of a specified group in the connection group table.
        /// </summary>
        /// <returns><c>true</c>, if group name was found, <c>false</c> otherwise.</returns>
        public List<UserGroupForListDto> GetUserGroupInfo(string dawgtag, ref List<Exception> excepts)
        {
            List<UserGroupForListDto> userGroupInfo = new List<UserGroupForListDto>();

            const string queryString =
                "SELECT guacamole_user_group.user_group_id, e2.name FROM guacamole_entity AS e1, guacamole_entity AS e2, guacamole_user_group_member, guacamole_user_group " +
                "WHERE e1.name = @name AND e1.entity_id = member_entity_id AND guacamole_user_group_member.user_group_id = guacamole_user_group.user_group_id AND " +
                "guacamole_user_group.entity_id = e2.entity_id";

            try
            {
                using (GuacamoleDatabaseConnector gdbc = new GuacamoleDatabaseConnector(ref excepts))
                {
                    using (MySqlCommand query = new MySqlCommand(queryString, gdbc.getConnection()))
                    {
                        query.Prepare();

                        //Add the agruments given
                        query.Parameters.AddWithValue("@name", dawgtag);

                        //Collect the query result column
                        using (MySqlDataReader reader = query.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                UserGroupForListDto infoDto = new UserGroupForListDto();
                                infoDto.Id = Int32.Parse(reader.GetValue(0).ToString());
                                infoDto.Name = reader.GetValue(1).ToString();
                                userGroupInfo.Add(infoDto);
                            }
                        }
                        return userGroupInfo;
                    }
                }
            }
            catch (Exception e)
            {
                excepts.Add(e);
                Console.WriteLine("\n\n\n\n" + e.Message);
                return null;
            }
        }


        /// <summary>
        /// Searchs for the name of a specified group in the connection group table.
        /// </summary>
        /// <returns><c>true</c>, if group name was found, <c>false</c> otherwise.</returns>
        public List<GroupForListDto> GetConnectionGroupInfo(string dawgtag, ref List<Exception> excepts)
        {
            List<GroupForListDto> userGroupInfo = new List<GroupForListDto>();

            const string queryString =
                "SELECT guacamole_connection_group.connection_group_id, guacamole_connection_group.connection_group_name, guacamole_connection_group.max_connections, guacamole_connection_group.enable_session_affinity FROM guacamole_entity AS e1, guacamole_entity AS e2, guacamole_connection_group, guacamole_connection_group_permission, guacamole_user_group_member, guacamole_user_group " +
                "WHERE e1.name = @name AND e1.entity_id = member_entity_id AND guacamole_user_group_member.user_group_id = guacamole_user_group.user_group_id AND " +
                "guacamole_user_group.entity_id = e2.entity_id AND e2.entity_id = guacamole_connection_group_permission.entity_id AND guacamole_connection_group_permission.connection_group_id = guacamole_connection_group.connection_group_id";

            try
            {
                using (GuacamoleDatabaseConnector gdbc = new GuacamoleDatabaseConnector(ref excepts))
                {
                    using (MySqlCommand query = new MySqlCommand(queryString, gdbc.getConnection()))
                    {
                        query.Prepare();

                        //Add the agruments given
                        query.Parameters.AddWithValue("@name", dawgtag);

                        //Collect the query result column
                        using (MySqlDataReader reader = query.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                GroupForListDto infoDto = new GroupForListDto();
                                infoDto.Id = Int32.Parse(reader.GetValue(0).ToString());
                                infoDto.Name = reader.GetValue(1).ToString();

                                if(reader.GetValue(2).ToString() != String.Empty)
                                {
                                    infoDto.Max = Int32.Parse(reader.GetValue(2).ToString());
                                }

                                if(reader.GetValue(3).ToString() == "0")
                                {
                                    infoDto.Affinity = false;
                                }
                                else{
                                    infoDto.Affinity = true;
                                }

                                userGroupInfo.Add(infoDto);
                            }
                        }
                        return userGroupInfo;
                    }
                }
            }
            catch (Exception e)
            {
                excepts.Add(e);
                Console.WriteLine("\n\n\n\n" + e.Message);
                return null;
            }
        }


        /// <summary>
        /// Searchs for the name of a specified group in the connection group table.
        /// </summary>
        /// <returns><c>true</c>, if group name was found, <c>false</c> otherwise.</returns>
        public IList<GroupForListDto> GetAllConnectionGroupInfo(ref List<Exception> excepts)
        {
            IList<GroupForListDto> userGroupInfo = new List<GroupForListDto>();

            const string queryString =
                "SELECT guacamole_connection_group.connection_group_id, guacamole_connection_group.connection_group_name, guacamole_connection_group.max_connections, guacamole_connection_group.enable_session_affinity FROM guacamole_connection_group";

            try
            {
                using (GuacamoleDatabaseConnector gdbc = new GuacamoleDatabaseConnector(ref excepts))
                {
                    using (MySqlCommand query = new MySqlCommand(queryString, gdbc.getConnection()))
                    {
                        query.Prepare();

                        //Collect the query result column
                        using (MySqlDataReader reader = query.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                GroupForListDto infoDto = new GroupForListDto();
                                infoDto.Id = Int32.Parse(reader.GetValue(0).ToString());
                                infoDto.Name = reader.GetValue(1).ToString();

                                if(reader.GetValue(2).ToString() != String.Empty)
                                {
                                    infoDto.Max = Int32.Parse(reader.GetValue(2).ToString());
                                }

                                if(reader.GetValue(3).ToString() == "0")
                                {
                                    infoDto.Affinity = false;
                                }
                                else{
                                    infoDto.Affinity = true;
                                }

                                userGroupInfo.Add(infoDto);
                            }
                        }
                        return userGroupInfo;
                    }
                }
            }
            catch (Exception e)
            {
                excepts.Add(e);
                Console.WriteLine("\n\n\n\n" + e.Message);
                return null;
            }
        }


        /// <summary>
        /// Searchs for the name of a specified group in the connection group table.
        /// </summary>
        /// <returns><c>true</c>, if group name was found, <c>false</c> otherwise.</returns>
        public List<UserForListDto> GetAllConnectionGroupUsers(int id, ref List<Exception> excepts)
        {
            List<UserForListDto> userInfo = new List<UserForListDto>();
            UserForListDto infoDto = new UserForListDto();
            List<string> dawgtags = new List<string>();

            const string queryString = 
                 "SELECT guacamole_entity.name, guacamole_entity.entity_id FROM guacamole_connection_group_permission AS permiss, guacamole_user_group, guacamole_user_group_member AS mem, guacamole_entity " + 
                 "WHERE permiss.connection_group_id = @id AND permiss.entity_id = guacamole_user_group.entity_id AND mem.user_group_id = guacamole_user_group.user_group_id AND guacamole_entity.entity_id = mem.member_entity_id";

            try
            {
                using (GuacamoleDatabaseConnector gdbc = new GuacamoleDatabaseConnector(ref excepts))
                {
                    using (MySqlCommand query = new MySqlCommand(queryString, gdbc.getConnection()))
                    {
                        query.Prepare();

                        //Add the agruments given
                        query.Parameters.AddWithValue("@id", id);

                        //Collect the query result column
                        using (MySqlDataReader reader = query.ExecuteReader())
                        {
                            while (reader.Read())
                            {                                
                                dawgtags.Add(reader.GetValue(0).ToString());
                                infoDto.Id = Int32.Parse(reader.GetValue(1).ToString());
                            }
                        }
                        infoDto.Users = dawgtags;
                        userInfo.Add(infoDto);
                        return userInfo;
                    }
                }
            }
            catch (Exception e)
            {
                excepts.Add(e);
                Console.WriteLine("\n\n\n\n" + e.Message);
                return null;
            }    
        }
    }
}