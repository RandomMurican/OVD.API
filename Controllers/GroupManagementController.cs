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
            ScriptExecutor executor = new ScriptExecutor();
            CloudmonkeyParser parser = new CloudmonkeyParser();

            string templateJson = executor.GetTemplateStats();
            return parser.ParseTemplateNames(templateJson);
        }


        /// <summary>
        /// Gets all of the service offering names.
        /// </summary>
        /// <returns>The service offering names.</returns>
        [HttpGet("offering/{serviceofferings}")]
        public List<string> GetServiceOfferingNames()
        {
            ScriptExecutor executor = new ScriptExecutor();
            CloudmonkeyParser parser = new CloudmonkeyParser();

            string serviceOfferingJson = executor.GetServiceOfferingStats();
            return parser.ParseServiceOfferingNames(serviceOfferingJson);
        }
    }
}