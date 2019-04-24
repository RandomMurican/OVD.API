using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using OVD.API.Dtos;
using OVD.API.Helpers;
using OVD.API.GuacamoleDatabaseConnectors;
using OVD.API.ScriptConnectors;

namespace OVD.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private const string SERVER = "10.100.3.1";     //Guacamole SQL Docker Container IP
        private const string PORT = "3306";             //Guacamole SQL Docker Port
        private const string DATABASE = "guacamole_db";
        private const string USER = "root";
        private const string PASSWORD = "secret";

        [HttpGet("{id}")]
        public ActionResult<IEnumerable<GroupForUserDto>> GetGroups(int id)
        {
            List<GroupForUserDto> groups = new List<GroupForUserDto>();

            string connectionString;
            connectionString = "Server=" + SERVER + ";" + "Port=" + PORT + ";" + "Database=" +
            DATABASE + ";" + "UID=" + USER + ";" + "Password=" + PASSWORD + ";";

            MySqlConnection connection = new MySqlConnection(connectionString);

            MySqlCommand command = new MySqlCommand("SELECT guacamole_connection_group.connection_group_id, guacamole_connection_group.connection_group_name, 'ssh' AS protocol FROM guacamole_db.guacamole_connection_group, guacamole_db.guacamole_connection_group_permission, guacamole_db.guacamole_user_group, guacamole_db.guacamole_user_group_member WHERE guacamole_user_group_member.member_entity_id = " + id + " AND guacamole_user_group_member.user_group_id = guacamole_user_group.user_group_id AND guacamole_user_group.entity_id = guacamole_connection_group_permission.entity_id AND guacamole_connection_group_permission.connection_group_id = guacamole_connection_group.connection_group_id;");
            command.Connection = connection;
            connection.Open();
            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read()) {
                GroupForUserDto group = new GroupForUserDto();
                group.Id = Convert.ToInt32(reader.GetValue(0).ToString());
                group.Name = reader.GetValue(1).ToString();
                group.Protocol = reader.GetValue(1).ToString();
                groups.Add(group);
            }
            connection.Close();
            return Ok(groups);
        }
    }
}