using System;

namespace OVD.API.Dtos
{
    public class ConnectionForListDto
    {
        public int Id { get; set; }
        public string Name { get; set;}
        public int MaxConnections { get; set; }
        public string Template { get; set; }
        public string Service { get; set; }
        public string Protocol { get; set;}
        public bool HasGroup { get; set;}
    }
}