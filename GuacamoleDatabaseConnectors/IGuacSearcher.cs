using System;
using System.Collections.Generic;

namespace OVD.API.GuacamoleDatabaseConnectors
{
    public interface IGuacSearcher
    {
        bool SearchConnectionGroupName(string groupName, ref List<Exception> excepts);
        bool SearchUserGroupName(string groupName, ref List<Exception> excepts);
        bool SearchUserName(string dawgtag, ref List<Exception> excepts);
        bool SearchVmName(string vmName, ref List<Exception> excepts);
    }
}