﻿﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using OVD.API.Dtos;
using OVD.API.Helpers;
using OVD.API.GuacamoleDatabaseConnectors;
using OVD.API.ScriptConnectors;

namespace OVD.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupManagementController : ControllerBase
    {
        /// <summary>
        /// Gets all of the template names.
        /// </summary>
        /// <returns>The template names.</returns>
        [HttpGet("template/{templates}")]
        public List<string> GetTemplateNames()
        {
            List<string> templateNames = new List<string>();
            templateNames.Add("CentOS 5.5(64-bit) no GUI (KVM)");
            templateNames.Add("Ubuntu 19 v1.2");
            templateNames.Add("Ubuntu 19.04 XRDP v1.1");
            return templateNames;
        }


        /// <summary>
        /// Gets all of the service offering names.
        /// </summary>
        /// <returns>The service offering names.</returns>
        [HttpGet("offering/{serviceofferings}")]
        public List<string> GetServiceOfferingNames()
        {
            List<string> serviceOfferingNames = new List<string>();
            serviceOfferingNames.Add("1GHz @ 1xCPU, 1GB of Ram");
            serviceOfferingNames.Add("1GHz @ 4xCPU, 4GB of Ram");
            return serviceOfferingNames;
        }


        /// <summary>
        /// Gets all of the protocol names.
        /// </summary>
        /// <returns>The protocol names.</returns>
        [HttpGet("protcol/{protocols}")]
        public List<string> GetProtocols()
        {
            List<string> protocolNames = new List<string>();
            protocolNames.Add("ssh");
            protocolNames.Add("rdp");
            return protocolNames;
        }


        [HttpPost]
        public async Task<IActionResult> CreateGroup(GroupForCreationDto groupForCreationDto)
        {
            //Method Level Variable Declarations
            List<Exception> excepts = new List<Exception>();

            //Format the given input
            Console.WriteLine("Formatting User Input.\n");
            if (!FormatUserInput(groupForCreationDto, ref excepts))
            {
                var message = HandleErrors(excepts);
                return Ok(false);
            }

            //Validate group input parameters
            Console.WriteLine("Validate User Group Inputs.\n");
            if (!ValidateInputForGroup(groupForCreationDto, ref excepts))
            {
                var message = HandleErrors(excepts);
                return Ok(false);
            }

            //Validate user input parameters
            Console.WriteLine("Validate User Inputs.\n");
            if (!ValidateInputForUsers(groupForCreationDto, ref excepts))
            {
                var message = HandleErrors(excepts);
                return Ok(false);
            }

            //Create user group
            Console.WriteLine("Create User Group.\n");
            if (!CreateUserGroup(groupForCreationDto, ref excepts))
            {
                var message = HandleErrors(excepts);
                return Ok(false);
            }

            //Create users if they do not exist in the system and add them to the created user group
            foreach(string dawgtag in groupForCreationDto.Dawgtags)
            {
                //Verify a user exists and create them if they do not
                Console.WriteLine("Initialize User " + dawgtag + "\n");
                if(!InitializeUser(dawgtag, ref excepts))
                {
                    var message = HandleErrors(excepts);
                    return Ok(false);
                } 
                else
                {
                    if(!AddUserToUserGroup(groupForCreationDto.Name, dawgtag, ref excepts)){
                        var message = HandleErrors(excepts);
                        return Ok(false);
                    }
                }
            }

            //Create Connection Group
            Console.WriteLine("Create Connection Group.\n");
            if (!CreateConnectionGroup(groupForCreationDto, ref excepts))
            {
                var message = HandleErrors(excepts);
                return Ok(false);
            }

            //Connect the user group to the connection group
            Console.WriteLine("Add Connection Group to User Group.\n");
            if(!AddConnectionGroupToUserGroup(groupForCreationDto.Name, ref excepts))
            {
                var message = HandleErrors(excepts);
                return Ok(false);
            }

            //Create the desired number of connections and add them to guacamole
            Console.WriteLine("Create Connections.\n");
            for(int i = 0; i < groupForCreationDto.Total; i++)
            {
                if(!CreateConnection(groupForCreationDto, ref excepts)){
                    var message = HandleErrors(excepts);
                    return Ok(false);
                }
            }

            //Final exception check
            if (excepts.Count != 0)
            {
                var message = HandleErrors(excepts);
                return Ok(false);
            }
            return Ok(true);
        }


        /// <summary>
        /// Formats the given user inputs to ensure data consistancy when stored.
        /// </summary>
        /// <returns><c>true</c>, if the input was formated, <c>false</c> otherwise.</returns>
        /// <param name="groupForCreationDto">Group for creation dto.</param>
        /// <param name="excepts">Excepts.</param>
        private bool FormatUserInput(GroupForCreationDto groupForCreationDto, ref List<Exception> excepts)
        {
            // Format User Text Input to be standardized to the following:
            //EX. test_group_1, ubuntu_16.04
            using (Formatter styler = new Formatter())
            {
                groupForCreationDto.Name = styler.FormatGroupName(groupForCreationDto.Name);
                groupForCreationDto.Protocol = styler.FormatName(groupForCreationDto.Protocol);

                for (int i = 0; i < groupForCreationDto.Dawgtags.Count; i++)
                {
                    groupForCreationDto.Dawgtags = styler.FormatDawgtagList(groupForCreationDto.Dawgtags);
                }
            }
            return true;
        }


        /// <summary>
        /// Validates the user input for group parameters.
        /// </summary>
        /// <returns><c>true</c>, if input parameters for the group is valid, <c>false</c> otherwise.</returns>
        /// <param name="groupForCreationDto">Group for creation dto.</param>
        /// <param name="excepts">Excepts.</param>
        private bool ValidateInputForGroup(GroupForCreationDto groupForCreationDto, ref List<Exception> excepts)
        {
            using (Validator checker = new Validator())
            {
                //Check if the group inupt parameters are valid
                checker.ValidateGroupName(groupForCreationDto.Name, ref excepts);
                checker.ValidateVmTotal(groupForCreationDto.Total, ref excepts);
                checker.ValidateHotspares(groupForCreationDto.Hotspares, ref excepts);
            }
            return excepts.Count == 0;
        }


        /// <summary>
        /// Validates the input for the list of dawgtags.
        /// </summary>
        /// <returns><c>true</c>, if the dawgtags were valid, <c>false</c> otherwise.</returns>
        /// <param name="groupForCreationDto">Group for creation dto.</param>
        /// <param name="excepts">Excepts.</param>
        private bool ValidateInputForUsers(GroupForCreationDto groupForCreationDto, ref List<Exception> excepts)
        {
            using (Validator checker = new Validator())
            {
                //Check if the dawgtags are in the proper format
                foreach (string dawgtag in groupForCreationDto.Dawgtags)
                {
                    checker.ValidateDawgtag(dawgtag, ref excepts);
                }
            }
            return excepts.Count == 0;
        }


        /// <summary>
        /// Creates the new user group.
        /// </summary>
        /// <returns><c>true</c>, if user group was created, <c>false</c> otherwise.</returns>
        /// <param name="groupForCreationDto">Group for creation dto.</param>
        /// <param name="excepts">Excepts.</param>
        private bool CreateUserGroup(GroupForCreationDto groupForCreationDto, ref List<Exception> excepts)
        {
            GuacamoleDatabaseInserter inserter = new GuacamoleDatabaseInserter();
            return inserter.InsertUserGroup(groupForCreationDto.Name, ref excepts);
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
                if(excepts.Count != 0){
                    return false;
                }

                //Add the user if it was not found
                return inserter.InsertUser(dawgtag, ref excepts);
            }
            return true;
        }


        /// <summary>
        /// Adds the user to the user group.
        /// </summary>
        /// <returns><c>true</c>, if user was added to the user group, <c>false</c> otherwise.</returns>
        /// <param name="groupName">Group name.</param>
        /// <param name="dawgtag">Dawgtag.</param>
        /// <param name="excepts">Excepts.</param>
        private bool AddUserToUserGroup(string groupName, string dawgtag, ref List<Exception> excepts)
        {
            GuacamoleDatabaseInserter inserter = new GuacamoleDatabaseInserter();
            return inserter.InsertUserIntoUserGroup(groupName, dawgtag, ref excepts);
        }


        /// <summary>
        /// Creates the new connection group.
        /// </summary>
        /// <returns><c>true</c>, if the connection group was created, <c>false</c> otherwise.</returns>
        /// <param name="groupForCreationDto">Group for creation dto.</param>
        /// <param name="excepts">Excepts.</param>
        private bool CreateConnectionGroup(GroupForCreationDto groupForCreationDto, ref List<Exception> excepts)
        {
            GuacamoleDatabaseInserter inserter = new GuacamoleDatabaseInserter();
            return inserter.InsertConnectionGroup(groupForCreationDto.Name, groupForCreationDto.Total, ref excepts);
        }


        /// <summary>
        /// Adds the connection group to user group.
        /// </summary>
        /// <returns><c>true</c>, if connection group to user group was added, <c>false</c> otherwise.</returns>
        /// <param name="groupName">Group name.</param>
        /// <param name="excepts">Excepts.</param>
        private bool AddConnectionGroupToUserGroup(string groupName, ref List<Exception> excepts)
        {
            GuacamoleDatabaseInserter inserter = new GuacamoleDatabaseInserter();
            return inserter.InsertConnectionGroupIntoUserGroup(groupName, ref excepts);
        }


        /// <summary>
        /// Removes the given users from the user group.
        /// </summary>
        /// <returns><c>true</c>, if the users where removed <c>false</c> otherwise.</returns>
        /// <param name="groupForEditDto">Group information.</param>
        /// <param name="excepts">Excepts.</param>
        private bool RemoveUsersFromUserGroup(GroupForEditDto groupForEditDto, ref List<Exception> excepts)
        {
            GuacamoleDatabaseDeleter deleter = new GuacamoleDatabaseDeleter();
            foreach(string dawgtag in groupForEditDto.RemoveDawgtags){
                deleter.DeleteUserFromUserGroup(groupForEditDto.Name, dawgtag, ref excepts);
            }
            return (excepts.Count == 0);
        }


        /// <summary>
        /// Creates the virtual machine connections and adds them to the
        /// Guacamole database.
        /// </summary>
        /// <returns><c>true</c>, if connections were created, <c>false</c> otherwise.</returns>
        /// <param name="groupForCreationDto">Group for creation dto.</param>
        /// <param name="excepts">Excepts.</param>
        private bool CreateConnection(GroupForCreationDto groupForCreationDto, ref List<Exception> excepts)
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
        }


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