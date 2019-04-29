﻿﻿using System;
using System.Collections.Generic;

namespace OVD.API.Dtos
{
    public class UserGroupForCreationDto
    {
        public bool AllGroups { get; set; }
        public int Id { get; set; }
        public IList<string> Dawgtags {get; set;}
    }
}