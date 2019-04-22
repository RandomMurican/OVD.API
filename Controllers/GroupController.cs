using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using OVD.API.Dtos;

namespace OVD.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupController : ControllerBase
    {

        private const string SERVER = "10.100.3.1";     //Guacamole SQL Docker Container IP
        private const string PORT = "3306";             //Guacamole SQL Docker Port
        private const string DATABASE = "guacamole_db";
        private const string USER = "root";
        private const string PASSWORD = "secret";


        [HttpGet]
        public ActionResult<IEnumerable<string>> GetGroups()
        {
            List<GroupForListDto> groups = new List<GroupForListDto>();

            string connectionString;
            connectionString = "Server=" + SERVER + ";" + "Port=" + PORT + ";" + "Database=" +
            DATABASE + ";" + "UID=" + USER + ";" + "Password=" + PASSWORD + ";";

            MySqlConnection connection = new MySqlConnection(connectionString);

            MySqlCommand command = new MySqlCommand("SELECT guacamole_connection_group.connection_group_id AS id, guacamole_connection_group.connection_group_name AS name, COUNT(guacamole_connection.connection_name) AS VMs FROM guacamole_connection_group LEFT JOIN guacamole_connection ON guacamole_connection.parent_id=guacamole_connection_group.connection_group_id GROUP BY guacamole_connection_group.connection_group_id;");
            command.Connection = connection;
            connection.Open();
            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read()) {
                GroupForListDto group = new GroupForListDto();
                group.Id = Convert.ToInt32(reader.GetValue(0).ToString());
                group.Name = reader.GetValue(1).ToString();
                group.Total = Convert.ToInt32(reader.GetValue(2).ToString());
                group.Active = 0;
                group.Cpu = 0;
                group.Ram = 0;
                group.Memory = 0;
                group.ServiceOffering = "1GB @ 4xCPU";
                group.Protocol = "ssh";
                group.Template = "Ubuntu";
                group.Hotspares = 0;

                groups.Add(group);
            }
            connection.Close();
            return Ok(groups);
        }

        [HttpGet("{id}")]
        public ActionResult<string> GetGroup(int id)
        { 
            string connectionString;
            connectionString = "Server=" + SERVER + ";" + "Port=" + PORT + ";" + "Database=" +
            DATABASE + ";" + "UID=" + USER + ";" + "Password=" + PASSWORD + ";";

            MySqlConnection connection = new MySqlConnection(connectionString);
            MySqlCommand command = new MySqlCommand("SELECT guacamole_connection_group.connection_group_id AS id, guacamole_connection_group.connection_group_name AS name, COUNT(guacamole_connection.connection_name) AS VMs FROM guacamole_connection_group LEFT JOIN guacamole_connection ON guacamole_connection.parent_id=guacamole_connection_group.connection_group_id WHERE guacamole_connection_group.connection_group_id='" + id + "' GROUP BY guacamole_connection_group.connection_group_id;");
            command.Connection = connection;
            connection.Open();
            MySqlDataReader reader = command.ExecuteReader();
            if (reader.Read()) {
                GroupForListDto group = new GroupForListDto();
                group.Id = Convert.ToInt32(reader.GetValue(0).ToString());
                group.Name = reader.GetValue(1).ToString();
                group.Total = Convert.ToInt32(reader.GetValue(2).ToString());
                group.Active = 0;
                group.Cpu = 0;
                group.Ram = 0;
                group.Memory = 0;
                group.ServiceOffering = "1GB @ 4xCPU";
                group.Protocol = "ssh";
                group.Template = "Ubuntu";
                group.Hotspares = 0;
                connection.Close();
                return Ok(group);
            }
            connection.Close();
            return BadRequest("Group does not exist");
        }

        [HttpPost]
        public ActionResult<Boolean> Create(GroupForListDto group)
        {
            return BadRequest("Not implemented");
        }

        [HttpDelete("{id}")]
        public ActionResult<Boolean> Delete(int id)
        {
            return BadRequest("Not implemented");
        }
        
    }
}