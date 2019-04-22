using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using OVD.API.GuacamoleDatabaseConnectors;
using OVD.API.Exceptions;

namespace OVD.API.Helpers
{
    public class Validator : IDisposable
    {
        private bool isDisposed = false;


        /// <summary>
        /// Releases all resource used by the <see cref="T:test_OVD_clientless.Helpers.Validator"/> object.
        /// </summary>
        /// <remarks>Call <see cref="Dispose"/> when you are finished using the
        /// <see cref="T:test_OVD_clientless.Helpers.Validator"/>. The <see cref="Dispose"/> method leaves the
        /// <see cref="T:test_OVD_clientless.Helpers.Validator"/> in an unusable state. After calling
        /// <see cref="Dispose"/>, you must release all references to the
        /// <see cref="T:test_OVD_clientless.Helpers.Validator"/> so the garbage collector can reclaim the memory that
        /// the <see cref="T:test_OVD_clientless.Helpers.Validator"/> was occupying.</remarks>
        public void Dispose()
        {
            ReleaseResources(true);
            GC.SuppressFinalize(this);
        }


        /// <summary>
        /// Releases the managed and unmanaged resources.
        /// </summary>
        /// <param name="isFromDispose">If set to <c>true</c> is from dispose.</param>
        protected void ReleaseResources(bool isFromDispose)
        {
            if (!isDisposed)
            {
                if (isFromDispose)
                {
                    // TODO: Release managed resources here
                }
                //TODO: Release unmanaged resources here
            }
            isDisposed = true;
        }


        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="T:test_OVD_clientless.Helpers.Validator"/> is reclaimed by garbage collection.
        /// </summary>
        ~Validator()
        {
            ReleaseResources(false);
        }


        /*******************************************************************************
         *------------------------Primary Validator Methods----------------------------*
         ******************************************************************************/
        /// <summary>
        /// Validates the name of the group by checking if the given name exists within
        /// the guacamole database.
        /// </summary>
        /// <param name="groupName">Group name.</param>
        public bool ValidateGroupName(string groupName, ref List<Exception> exceptions)
        {
            string execptMessage = $"The given group name {groupName} already " +
                "exists. Please choose another name or edit the existing group.";

            GuacamoleDatabaseSearcher searcher = new GuacamoleDatabaseSearcher();
            if (searcher.SearchConnectionGroupName(groupName, ref exceptions))
            {
                exceptions.Add(new ValidationException(execptMessage));
                return false;
            }
            if (searcher.SearchUserGroupName(groupName, ref exceptions))
            {
                exceptions.Add(new ValidationException(execptMessage));
                return false;
            }
            return true;
        }


        /// <summary>
        /// Validates the total vm number provided.
        /// </summary>
        /// <param name="total">vm total.</param>
        public bool ValidateVmTotal(int total, ref List<Exception> exceptions)
        {
            const string exceptMessage = "The vm total number specified " +
                "must be greater than or equal to zero.";
            if (!CheckPositiveInputNumber(total))
            {
                exceptions.Add(new ValidationException(exceptMessage));
                return false;
            }
            return true;
        }


        /// <summary>
        /// Validates the hotspare number provided.
        /// </summary>
        /// <param name="hotspareNumber">Hotspare number.</param>
        public bool ValidateHotspares(int hotspareNumber, ref List<Exception> exceptions)
        {
            const string exceptMessage = "The number of hotspares provided must be " +
                "greater than or equal to zero.";

            if (!CheckPositiveInputNumber(hotspareNumber))
            {
                exceptions.Add(new ValidationException(exceptMessage));
                return false;
            }
            return true;
        }


        /// <summary>
        /// Ensures that the dawgtag given is in the proper format.
        /// </summary>
        /// <param name="dawgtag">Dawgtag.</param>
        public bool ValidateDawgtag(string dawgtag, ref List<Exception> exceptions)
        {
            string exceptMessage = $"The given dawgtag {dawgtag} is not in the proper " +
                "format.";

            //Ensure dawg tag is in the proper format
            Regex regex = new Regex(@"siu85\d{7}\z", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            if (!regex.Match(dawgtag).Success)
            {
                exceptions.Add(new ValidationException(exceptMessage));
                return false;
            }
            return true;
        }


        /// <summary> 
        /// Checks if the provided integer is greater than or equal to zero.
        /// This is used for validating user integer input. 
        /// </summary>
        /// <returns><c>true</c>, if input number was validated, <c>false</c> otherwise.</returns>
        /// <param name="number">The integer to validate.</param>
        private bool CheckPositiveInputNumber(int number)
        {
            return (number >= 0);
        }
    }
}