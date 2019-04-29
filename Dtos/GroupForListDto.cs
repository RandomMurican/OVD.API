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
        public List<ConnectionForListDto> Connections { get; set; }
        public bool AllUsers { get; set; }
        public List<UserForListDto> Users { get; set; }
    }
}