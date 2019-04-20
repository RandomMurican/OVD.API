using System;

namespace OVD.API.Dtos
{
    public class GroupForListDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Total { get; set; }
        public int Active { get; set; }
        public double Cpu { get; set; }
        public double Ram { get; set; }
        public double Memory { get; set; }
        public string ServiceOffering { get; set; }
        public string Protocol { get; set; }
        public string Template { get; set; }
        public int Hotspares { get; set; }
    }
}