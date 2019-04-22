using System;
using System.Collections.Generic;

namespace OVD.API.Dtos
{
    public class GroupForEditDto
    {
        public String Name { get; set; }
        public int TotalNumber { get; set; }
        public int NumHotspares { get; set; }
        public IList<String> NewDawgtags { get; set; }
        public IList<String> RemoveDawgtags { get; set; }
    }
}