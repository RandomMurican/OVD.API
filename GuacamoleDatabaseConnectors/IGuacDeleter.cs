﻿using System;
using System.Collections.Generic;

namespace OVD.API.GuacamoleDatabaseConnectors
{
    public interface IGuacDeleter
    {
        bool DeleteUserGroup(string groupName, ref List<Exception> excepts);
        bool DeleteConnectionGroup(string groupName, ref List<Exception> excepts);
        bool DeleteUserFromUserGroup(string groupName, string dawgtag, ref List<Exception> excepts);
    }
}