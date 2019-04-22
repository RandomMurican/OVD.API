﻿﻿﻿using System;
using System.Collections.Generic;

namespace OVD.API.Dtos
{
    public class GroupForCreationDto
    {
        public String Name { get; set; }
        public String Template { get; set; }
        public string ServiceOffering { get; set; }
        public String Protocol { get; set; }
        public int Total { get; set; }
        public int Max {get; set;}
        public int Hotspares { get; set; }
        public IList<String> Dawgtags { get; set; }
    }
}