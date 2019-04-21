﻿using System;
using System.Diagnostics;
using System.Net.Security;
using Renci.SshNet;

namespace OVD.API.ScriptConnectors
{
    public class ScriptExecutor
    {
        //Cloudmonkey virtual machine ssh credentials
        private const string REMOTE_ADDRESS = "10.100.64.5";
        private const string REMOTE_USERNAME = "ovdadmin";
        private const string REMOTE_PASSWORD = "8UtJitxCnuHuiBsW";

        //Status info cloudmonkey scripts
        private const string TEMPLATE_SCRIPT_PATH = "~/cloudmonkey_scripts/info_scripts/get_template_stats.sh";
        private const string SERVICE_OFFERING_SCRIPT_PATH = "~/cloudmonkey_scripts/info_scripts/get_service_offering_stats.sh";
        private const string VM_STATS_SCRIPT_PATH = "~/cloudmonkey_scripts/info_scripts/get_vm_stats.sh";
        private const string ZONE_SCRIPT_PATH = "~/cloudmonkey_scripts/info_scripts/get_zone_stats.sh";

        //Control cloudmonkey scripts
        private const string ACCQUIRE_IP_SCRIPT_PATH = "~/cloudmonkey_scripts/control_scripts/accquire_public_ip.sh";
        private const string DEPLOY_VIRTUAL_MACHINE_PATH = "~/cloudmonkey_scripts/control_scripts/deploy_vm.sh";
        private const string SET_STATIC_NAT_PATH = "~/cloudmonkey_scripts/control_scripts/set_static_nat.sh";


        /// <summary>
        /// Gets the template info from the cloudmonkey api script.
        /// </summary>
        /// <returns>The template info in the format of json.</returns>
        public string GetTemplateStats()
        {
            return ExecuteRemoteScript(TEMPLATE_SCRIPT_PATH, null);
        }


        /// <summary>
        /// Gets the template info from the cloudmonkey api script.
        /// </summary>
        /// <returns>The template info in the format of json.</returns>
        public string GetServiceOfferingStats()
        {
            return ExecuteRemoteScript(SERVICE_OFFERING_SCRIPT_PATH, null);
        }


        /// <summary>
        /// Gets the status information from the vms.
        /// </summary>
        /// <returns>The vm info.</returns>
        public string GetVmStats()
        {
            return ExecuteRemoteScript(VM_STATS_SCRIPT_PATH, null);
        }

        
        /// <summary>
        /// Gets the zone info from the cloudmonkey api script.
        /// </summary>
        /// <returns>The zone info in the format of json.</returns>
        public string GetZoneStats()
        {
            return ExecuteRemoteScript(ZONE_SCRIPT_PATH, null);
        }


        /// <summary>
        /// Deploys a virtual machine with the given parameters.
        /// </summary>
        /// <returns>The virtual machine output information.</returns>
        /// <param name="connectionName">Connection name.</param>
        /// <param name="templateId">Template identifier.</param>
        /// <param name="serviceOfferingId">Service offering identifier.</param>
        /// <param name="zoneId">Zone identifier.</param>
        public string DeployVirtualMachine(string connectionName, string templateId,
            string serviceOfferingId, string zoneId)
        {
            //NOTE: The following line must stay on one line due to the variable insertion.
            //An error occurs if it is broken up with concatenation.
            string argumentString = $"displayname={connectionName} zoneid={zoneId} templateid={templateId} serviceofferingid={serviceOfferingId}";
            return ExecuteRemoteScript(DEPLOY_VIRTUAL_MACHINE_PATH, argumentString);
        }


        /// <summary>
        /// Accquires a public ip address in CloudStack.
        /// </summary>
        /// <returns>The public ip info.</returns>
        public string AccquireIp()
        {
            return ExecuteRemoteScript(ACCQUIRE_IP_SCRIPT_PATH, null);
        }

         
        /// <summary>
        /// Sets the associated ip as part of the static nat.
        /// </summary>
        /// <returns>The script output.</returns>
        /// <param name="vmId">Vm identifier.</param>
        /// <param name="ipId">Ip identifier.</param>
        public string SetStaticNat(string vmId, string ipId)
        {
            //NOTE: The following line must stay on one line due to the variable insertion.
            //An error occurs if it is broken up with concatenation.
            string argumentString = $"ipaddressid={ipId} virtualmachineid={vmId}";
            return ExecuteRemoteScript(SET_STATIC_NAT_PATH, argumentString);
        }


        /// <summary>
        /// Executes the given remote script on the cloudmonkey virutal machine
        /// found within the CloudStack OVD setup.
        /// </summary>
        /// <returns>The remote script output.</returns>
        /// <param name="scriptName">Script name.</param>
        /// <param name="argumentString">Argument string.</param>
        private string ExecuteRemoteScript(string scriptName, string argumentString)
        {
            string stdoutput = string.Empty;
            string scriptString = scriptName;
            using (SshClient client = new SshClient(REMOTE_ADDRESS, REMOTE_USERNAME, REMOTE_PASSWORD))
            {
                client.Connect();
                if(argumentString != null)
                {
                    scriptString += " " + argumentString;
                }
                SshCommand command = client.CreateCommand(scriptString);
                stdoutput = command.Execute();
                client.Disconnect();
            }
            return stdoutput;
        }


        /// <summary>
        /// Executes an arbitrary script based off of the script location and any
        /// desired arguments. A new process is created for every new script and is
        /// collected when execution is complete.
        /// </summary>
        /// <returns>Status information from the standard output/error stream.</returns>
        /// <param name="scriptName">Script name or Path.</param>
        /// <param name="argumentString">
        /// A string containing the dash seperated arguments.
        /// Pass the argument string as null if not arguments are required.
        /// </param>
        private string ExecuteScript(string scriptName, string argumentString)
        {
            try
            {
                string standardErrorOutput = "Success";

                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = scriptName;
                psi.UseShellExecute = false;
                psi.RedirectStandardOutput = true;

                if (argumentString != null)
                {
                    psi.Arguments = argumentString;
                }

                Process p = Process.Start(psi);
                standardErrorOutput = p.StandardOutput.ReadToEnd();
                p.WaitForExit();
                return standardErrorOutput;

            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }
    }
}