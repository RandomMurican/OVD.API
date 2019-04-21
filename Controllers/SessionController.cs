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
    public class SessionController : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<string>> GetSessions()
        {
            List<SessionForListDto> sessions = new List<SessionForListDto>();
            MySqlConnection connection = new MySqlConnection("Server=10.100.3.1;Port=3306;Uid=root;Pwd=secret");
            MySqlCommand command = new MySqlCommand("SELECT history_id AS id, user_id, username AS user, connection_id AS group_id, connection_group_name AS group_name, 0 AS active, (end_date - start_date) AS time, start_date AS start FROM guacamole_db.guacamole_connection_history LEFT JOIN guacamole_db.guacamole_connection_group ON connection_id=connection_group_id;");
            command.Connection = connection;
            connection.Open();
            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read()) {
                SessionForListDto session = new SessionForListDto();
                session.Id = Convert.ToInt32(reader.GetValue(0).ToString());
                session.UserId = Convert.ToInt32(reader.GetValue(1).ToString());
                session.User = reader.GetValue(2).ToString();
                session.GroupId = Convert.ToInt32(reader.GetValue(3).ToString());
                session.Group = reader.GetValue(4).ToString();
                if (session.Group == "") {
                    session.Group = "Deleted Group";
                }
                if (Convert.ToInt32(reader.GetValue(5).ToString()) > 0) {
                    session.Active = true;
                } else {
                    session.Active = false;
                }
                session.Time = Convert.ToInt32(reader.GetValue(6).ToString());
                session.Start = DateTime.Parse(reader.GetValue(7).ToString());
                sessions.Add(session);
            }
            connection.Close();
            return Ok(sessions);
        }
    }
}