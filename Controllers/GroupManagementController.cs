﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using OVD.API.Dtos;
using OVD.API.Helpers;
using OVD.API.ScriptConnectors;

namespace OVD.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupManagementController
    {
        /// <summary>
        /// Gets all of the template names.
        /// </summary>
        /// <returns>The template names.</returns>
        [HttpGet("template/{templates}")]
        public List<string> GetTemplateNames()
        {
            // ScriptExecutor executor = new ScriptExecutor();
            // CloudmonkeyParser parser = new CloudmonkeyParser();

            // string templateJson = executor.GetTemplateStats();
            // return parser.ParseTemplateNames(templateJson);
            
            List<string> testList = new List<string>();
            testList.Add("Template 1");
            testList.Add("Template 2");
            testList.Add("Template 3");
            return testList;
        }


        /// <summary>
        /// Gets all of the service offering names.
        /// </summary>
        /// <returns>The service offering names.</returns>
        [HttpGet("offering/{serviceofferings}")]
        public List<string> GetServiceOfferingNames()
        {
            // ScriptExecutor executor = new ScriptExecutor();
            // CloudmonkeyParser parser = new CloudmonkeyParser();

            // string serviceOfferingJson = executor.GetServiceOfferingStats();
            // return parser.ParseServiceOfferingNames(serviceOfferingJson);

            List<string> testList = new List<string>();
            testList.Add("ServiceOffering 1");
            testList.Add("ServiceOffering 2");
            testList.Add("ServiceOffering 3");
            return testList;
        }

        /// <summary>
        /// Gets all of the protocol names.
        /// </summary>
        /// <returns>The protocol names.</returns>
        [HttpGet("protocol/{protocols}")]
        public List<string> GetProtocolNames()
        {
            List<string> testList = new List<string>();
            testList.Add("SSH");
            testList.Add("RDP");
            testList.Add("VNC");
            return testList;
        }
    }
}