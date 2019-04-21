using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace OVD.API.Helpers
{
    public class CloudmonkeyParser
    {
        /// <summary>
        /// Parses the template info json string to retrieve the id for the vm
        /// template specified by the user.
        /// </summary>
        /// <returns>The template id.</returns>
        /// <param name="templateJsonString">Template json string.</param>
        /// <param name="templateChoice">Template choice.</param>
        public string ParseTemplateId(string templateJsonString, string templateChoice)
        {
            JObject jo = JObject.Parse(templateJsonString);
            JToken token = (jo["template"] as JArray).FirstOrDefault(x => x.Value<string>("name") == templateChoice);
            return token.Value<string>("id");
        }


        /// <summary>
        /// Parses the template info json string to retrieve all the names.
        /// </summary>
        /// <returns>The template names.</returns>
        /// <param name="templateJsonString">Template json string.</param>
        public List<string> ParseTemplateNames(string templateJsonString)
        {
            JObject jo = JObject.Parse(templateJsonString);
            JArray templates = (JArray)jo["template"];
            List<string> templateNames = new List<string>();

            foreach (JToken nameToken in templates)
            {
                templateNames.Add((string)nameToken["name"]);
            }
            return templateNames;
        }


        /// <summary>
        /// Parses the service offering info json string to retrieve all the names.
        /// </summary>
        /// <returns>The service offering names.</returns>
        /// <param name="serviceOfferingJsonString">Service offering json string.</param>
        public List<string> ParseServiceOfferingNames(string serviceOfferingJsonString)
        {
            JObject jo = JObject.Parse(serviceOfferingJsonString);
            JArray serviceOfferings = (JArray)jo["serviceoffering"];
            List<string> serviceOfferingNames = new List<string>();

            foreach (JToken nameToken in serviceOfferings)
            {
                serviceOfferingNames.Add((string)nameToken["displaytext"]);
            }
            return serviceOfferingNames;
        }


        /// <summary>
        /// Parses the service offering info json string to retrieve the id for the vm
        /// memory specified by the user.
        /// </summary>
        /// <returns>The service offering id.</returns>
        /// <param name="serviceOfferingJsonString">Service offering json string.</param>
        /// <param name="memoryChoice">Memory choice.</param>
        public string ParseServiceOfferingId(string serviceOfferingJsonString, string memoryChoice)
        {
            JObject jo = JObject.Parse(serviceOfferingJsonString);
            JToken token = (jo["serviceoffering"] as JArray).FirstOrDefault(x => x.Value<string>("displaytext") == memoryChoice);
            return token.Value<string>("id");
        }


        /// <summary>
        /// Parses the associated ip info using regex. Regex is used as the json
        /// parsing method incorrectly ids the ipaddress field due to its structure
        /// </summary>
        /// <returns>The associated ip.</returns>
        /// <param name="ipJsonString">Ip json string.</param>
        public string ParseAssociatedIpInfo(string ipJsonString)
        {
            Regex regex = new Regex(@"\""ipaddress\"":\s\""(.*)\"",", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            return regex.Match(ipJsonString).Groups[1].Value;
        }


        /// <summary>
        /// Parses the associated ip info using regex. Regex is used as the json
        /// parsing method incorrectly ids the ipaddress field due to its structure
        /// </summary>
        /// <returns>The associated ip identifier.</returns>
        /// <param name="ipJsonString">Ip json string.</param>
        public string ParseAssociatedIpId(string ipJsonString)
        {
            Regex regex = new Regex(@"\""id\"":\s\""(.*)\"",", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            return regex.Match(ipJsonString).Groups[1].Value;
        }


        /// <summary>
        /// Parses the zone info json string to retrieve the id for the vm zone.
        /// </summary>
        /// <returns>The zone id.</returns>
        /// <param name="zoneJsonString">Zone json string.</param>
        public string ParseZoneId(string zoneJsonString)
        {
            JObject jo = JObject.Parse(zoneJsonString);
            JToken token = (jo["zone"] as JArray).FirstOrDefault(x => x.Value<string>("id") != null);
            return token.Value<string>("id");
        }


        /// <summary>
        /// Parses the vm info json object to retrieve the vm id.
        /// </summary>
        /// <returns>The vm id.</returns>
        /// <param name="vmInfoJsonString">Vm info json string.</param>
        public string ParseVmId(string vmInfoJsonString)
        {
            Regex regex = new Regex(@"\""id\"":\s\""(.*)\"",", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            return regex.Match(vmInfoJsonString).Groups[1].Value;
        }
    }
}