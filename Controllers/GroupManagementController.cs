﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;


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
            templateNames.Add("Barkdoll-Guacamole-v1.4");
            templateNames.Add("Ubuntu 19.04 XRDP v1.1");
            templateNames.Add("Ubuntu 19 v1.1");
            templateNames.Add("Win2k3");
            templateNames.Add("BT5R3");
            templateNames.Add("WinXP");

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
        [HttpGet("protocol/{protocols}")]
        public List<string> GetProtocolNames()
        {
            List<string> protocolNames = new List<string>();
            protocolNames.Add("ssh");
            protocolNames.Add("rdp");
            return protocolNames;
        }
    }
}