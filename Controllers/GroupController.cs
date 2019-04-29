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
    public class GroupController : ControllerBase
    {

        private const string SERVER = "10.100.3.1";     //Guacamole SQL Docker Container IP
        private const string PORT = "3306";             //Guacamole SQL Docker Port
        private const string DATABASE = "guacamole_db";
        private const string USER = "root";
        private const string PASSWORD = "secret";


        [HttpGet("members/{id}")]
        public string[] GetGroupMembers(int id)
        { 
            List<string> results = new List<string>();
            string connectionString;
            connectionString = "Server=" + SERVER + ";" + "Port=" + PORT + ";" + "Database=" +
            DATABASE + ";" + "UID=" + USER + ";" + "Password=" + PASSWORD + ";";

            MySqlConnection connection = new MySqlConnection(connectionString);
            MySqlCommand command = new MySqlCommand("SELECT guacamole_entity.name FROM guacamole_db.guacamole_entity, guacamole_db.guacamole_user_group_member, guacamole_db.guacamole_user_group, guacamole_db.guacamole_connection_group_permission WHERE guacamole_entity.entity_id = guacamole_user_group_member.member_entity_id AND guacamole_user_group_member.user_group_id = guacamole_user_group.user_group_id AND guacamole_user_group.entity_id = guacamole_connection_group_permission.entity_id AND guacamole_connection_group_permission.connection_group_id =" + id + ";");
            command.Connection = connection;
            connection.Open();
            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read()) {
                results.Add(reader.GetValue(0).ToString());
            }
            connection.Close();
            string[] users = new string[results.Count()];
            results.CopyTo(users);
            return users;
        }


        [HttpGet("getallconnections")]
        public List<ConnectionForListDto> GetAllConnections()
        { 
            //Method Level Variable Declarations
            List<Exception> excepts = new List<Exception>();

            GuacamoleDatabaseGetter getter = new GuacamoleDatabaseGetter();
            return getter.GetAllConnectionInfo(ref excepts);
        }


        [HttpGet("getallconnectiongroups")]
        public List<GroupForListDto> GetAllConnectionGroups(string dawgtag)
        { 
            //Method Level Variable Declarations
            List<Exception> excepts = new List<Exception>();

            GuacamoleDatabaseGetter getter = new GuacamoleDatabaseGetter();
            List<GroupForListDto> groupList = getter.GetAllConnectionGroupInfo(ref excepts);

            foreach(GroupForListDto dto in groupList)
            {
                dto.Connections = getter.GetAllGroupConnections(dto.Id, ref excepts);
                dto.Users = getter.GetAllConnectionGroupUsers(dto.Id, ref excepts);
            }
            
            return groupList;
        }


        [HttpGet("getconnectiongroupinfo/{id}")]
        public List<GroupForListDto> GetConnectionGroupsFromId(int id)
        { 
            //Method Level Variable Declarations
            List<Exception> excepts = new List<Exception>();

            GuacamoleDatabaseGetter getter = new GuacamoleDatabaseGetter();
            List<GroupForListDto> groupList = getter.GetAllConnectionGroupInfo(id, ref excepts);

            foreach(GroupForListDto dto in groupList)
            {
                dto.Connections = getter.GetAllGroupConnections(dto.Id, ref excepts);
                dto.Users = getter.GetAllConnectionGroupUsers(dto.Id, ref excepts);
            }
            
            return groupList;
        }


        [HttpGet("getusergroups/{dawgtag}")]
        public List<UserGroupForListDto> GetUserGroups(string dawgtag)
        { 
            //Method Level Variable Declarations
            List<Exception> excepts = new List<Exception>();

            GuacamoleDatabaseGetter getter = new GuacamoleDatabaseGetter();
            return getter.GetUserGroupInfo(dawgtag, ref excepts);
        }


        [HttpGet("getconnectiongroups/{dawgtag}")]
        public List<GroupForListDto> GetConnectionGroups(string dawgtag)
        { 
            //Method Level Variable Declarations
            List<Exception> excepts = new List<Exception>();

            GuacamoleDatabaseGetter getter = new GuacamoleDatabaseGetter();
            List<GroupForListDto> groupList = getter.GetConnectionGroupInfo(dawgtag, ref excepts);

            foreach(GroupForListDto dto in groupList)
            {
                dto.Connections = getter.GetAllGroupConnections(dto.Id, ref excepts);
            }
            
            return groupList;
        }


        [HttpPost("creategroup")]
        public ActionResult CreateGroup(GroupForCreationDto groupForCreationDto) 
        {
            //Method Level Variable Declarations
            List<Exception> excepts = new List<Exception>();

            //Check if the group inupt parameters are valid
            using (Validator checker = new Validator())
            {
                checker.ValidateGroupName(groupForCreationDto.Name, ref excepts);
                checker.ValidateVmTotal(groupForCreationDto.Max, ref excepts);
            }

            if(excepts.Count != 0)
            {
                var message = HandleErrors(excepts);
                return Ok(null);
            }

            //Format connection group type
            using (Formatter styler = new Formatter())
            {
                groupForCreationDto.Type = styler.FormatName(groupForCreationDto.Type);
            }

            //Get affinity bool
            string affinityBool = "0";
            if(groupForCreationDto.Affinity)
            {
                affinityBool = "1";
            }

            //Create connection group
            GuacamoleDatabaseInserter inserter = new GuacamoleDatabaseInserter();
            if(!inserter.InsertConnectionGroup(groupForCreationDto.Name, groupForCreationDto.Type, groupForCreationDto.Max, affinityBool, ref excepts))
            {
                var message = HandleErrors(excepts);
                return Ok(null);
            }

            //Get the newly created group id
            GuacamoleDatabaseGetter getter = new GuacamoleDatabaseGetter();
            groupForCreationDto.Id = getter.GetConnectionGroupId(groupForCreationDto.Name, ref excepts);
            return Ok(groupForCreationDto);
        }


        [HttpPost("createusergroup")]
        public ActionResult CreateUserGroup(UserGroupForCreationDto userGroupForCreationDto) 
        {
            //Method Level Variable Declarations
            List<Exception> excepts = new List<Exception>();
            GuacamoleDatabaseInserter inserter = new GuacamoleDatabaseInserter();

            if(!userGroupForCreationDto.AllGroups)
            {
                //Create user group
                Console.WriteLine("Create User Group.\n");
                if(!inserter.InsertUserGroup(userGroupForCreationDto.Id, ref excepts))
                {
                    var message = HandleErrors(excepts);
                    return Ok(excepts);
                }

                //Insert users into the user group
                foreach(string dawgtag in userGroupForCreationDto.Dawgtags)
                {
                    //Insert users that are not in the system
                    if(!InitializeUser(dawgtag, ref excepts))
                    {
                        var message = HandleErrors(excepts);
                        return Ok(excepts);
                    }
                    inserter.InsertUserIntoUserGroup(userGroupForCreationDto.Id, dawgtag, ref excepts);
                }
            }
            
            //Connect connection group and user group
            Console.WriteLine("Connect the User Group and Connection Group.\n");
            if(!inserter.InsertConnectionGroupIntoUserGroup(userGroupForCreationDto.Id, userGroupForCreationDto.AllGroups,ref excepts))
            {
                var message = HandleErrors(excepts);
                return Ok(excepts);
            }
            return Ok(userGroupForCreationDto);
        }


        /// <summary>
        /// Initializes the user by checking if they exist and creates a user
        /// if that user does not exist yet.
        /// </summary>
        /// <returns><c>true</c>, if user was initialized, <c>false</c> otherwise.</returns>
        /// <param name="dawgtag">Dawgtag.</param>
        /// <param name="excepts">Excepts.</param>
        private bool InitializeUser(string dawgtag, ref List<Exception> excepts)
        {
            GuacamoleDatabaseInserter inserter = new GuacamoleDatabaseInserter();
            GuacamoleDatabaseSearcher searcher = new GuacamoleDatabaseSearcher();

            //Check if the user already exists
            if (!searcher.SearchUserName(dawgtag, ref excepts))
            {
                if(excepts.Count != 0)
                {
                    return false;
                }

                //Add the user if it was not found
                return inserter.InsertUser(dawgtag, ref excepts);
            }
            return true;
        }


        /// <summary>
        /// Creates the virtual machine connections and adds them to the
        /// Guacamole database.
        /// </summary>
        /// <returns><c>true</c>, if connections were created, <c>false</c> otherwise.</returns>
        /// <param name="groupForCreationDto">Group for creation dto.</param>
        /// <param name="excepts">Excepts.</param>
        /* private bool CreateConnection(GroupForCreationDto groupForCreationDto, ref List<Exception> excepts)
        {
            Calculator calculator = new Calculator();
            CloudmonkeyParser jsonParser = new CloudmonkeyParser();
            ScriptExecutor executor = new ScriptExecutor();
            GuacamoleDatabaseInserter inserter = new GuacamoleDatabaseInserter();

            string connectionName, templateInfo, templateId, zoneInfo, zoneId, serviceOfferingInfo, serviceOfferingId;

            //Get the unique name-id that is associated with each new connection
            using (Formatter styler = new Formatter())
            {
                connectionName = styler.FormatVmName(groupForCreationDto.Name, ref excepts);
                if(connectionName == null)
                {
                    return false;
                }
            }

            //Get the virtual machine template information
            templateInfo = executor.GetTemplateStats();
            templateId = jsonParser.ParseTemplateId(templateInfo, groupForCreationDto.Template);
            Console.WriteLine(templateId);

            //Get the virtual machine service offering info
            serviceOfferingInfo = executor.GetServiceOfferingStats();
            serviceOfferingId = jsonParser.ParseServiceOfferingId(serviceOfferingInfo, groupForCreationDto.ServiceOffering);

            //Get the zone information 
            zoneInfo = executor.GetZoneStats();
            zoneId = jsonParser.ParseZoneId(zoneInfo);

            //Deploy the new virtual machine
            string vmInfo = executor.DeployVirtualMachine(connectionName, templateId, serviceOfferingId, zoneId);
            string vmId = jsonParser.ParseVmId(vmInfo);

            //Accquire a public ip address for the virtual machine
            string associatedIpInfo = executor.AccquireIp();
            string associatedIp = jsonParser.ParseAssociatedIpInfo(associatedIpInfo);
            string associatedIpId = jsonParser.ParseAssociatedIpId(associatedIpInfo);

            //Setup the static nat for the accquired vm and ip
            executor.SetStaticNat(vmId, associatedIpId);

            //Get the associated port
            string port = getPort(groupForCreationDto.Protocol);

            //Insert the new connection into the guacamole database
            if (!inserter.InsertConnection(groupForCreationDto.Name, connectionName, groupForCreationDto.Protocol, associatedIp, port, ref excepts))
            {
                return false;
            }
            return true;
        }*/


        /// <summary>
        /// Gets the port associted with the given protocol.
        /// </summary>
        /// <returns>Port number</returns>
        /// <param name="protocol">Protocol specified</param>
        private string getPort(string protocol){
            return ("rdp".Equals(protocol)) ? "3389" : "22";
        }


        /// <summary>
        /// Handles getting the error messages formatted.
        /// </summary>
        /// <returns>A string containing the error messages</returns>
        /// <param name="excepts">Exceptions.</param>
        private String HandleErrors(List<Exception> excepts)
        {
            String exceptionMessage = "";
            foreach (Exception e in excepts)
            {
                Console.Error.Write(e.Message);
                exceptionMessage += e;
            }
            return exceptionMessage;
        }
        
    }
}