﻿﻿﻿using System;
using System.Collections.Generic;

namespace OVD.API.Dtos
{
    public class GroupForCreationDto
    {
        public String Name { get; set; }
        public bool Affinity { get; set;}
        public String Type { get; set;}
        public int Max {get; set;}
        public int Id { get; set; }
    }
}