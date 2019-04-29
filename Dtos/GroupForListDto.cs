using System;
using System.Collections.Generic;


namespace OVD.API.Dtos
{
    public class GroupForListDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Affinity { get; set; }
        public int Max { get; set; }
        public IList<ConnectionForListDto> Connections { get; set; }
        public bool AllUsers { get; set; }
        public IList<UserForListDto> Users { get; set; }
    }
}