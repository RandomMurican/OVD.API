using System;
using System.Collections.Generic;

namespace OVD.API.GuacamoleDatabaseConnectors
{
    public interface IGuacInserter
    {
        bool InsertConnectionGroup(string groupName, int max, ref List<Exception> excepts);
        bool InsertUserGroup(string groupName, ref List<Exception> excepts);
        bool InsertUser(string dawgtag, ref List<Exception> excepts);
        bool InsertUserIntoUserGroup(string groupName, string dawgtag, ref List<Exception> excepts);
        bool InsertConnectionGroupIntoUserGroup(string groupName, ref List<Exception> excepts);
    }
}