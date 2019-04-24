using System;
using System.Collections.Generic;

namespace OVD.API.Dtos
{
    public class GroupForEditDto
    {
        public string Name { get; set; }
        public int TotalNumber { get; set; }
        public int NumHotspares { get; set; }
        public IList<string> NewDawgtags { get; set; }
        public IList<string> RemoveDawgtags { get; set; }
    }
}