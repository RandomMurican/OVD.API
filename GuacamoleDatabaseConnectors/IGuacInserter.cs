using System;
using System.Collections.Generic;

namespace OVD.API.GuacamoleDatabaseConnectors
{
    public interface IGuacInserter
    {
        bool InsertConnectionGroup(string groupName, string type, int max, string affinity, ref List<Exception> excepts);
        bool InsertUserGroup(string groupName, ref List<Exception> excepts);
        bool InsertUser(string dawgtag, ref List<Exception> excepts);
        bool InsertUserIntoUserGroup(string groupName, string dawgtag, ref List<Exception> excepts);
        bool InsertConnectionGroupIntoUserGroup(string groupName, int groupId, ref List<Exception> excepts);
    }
}