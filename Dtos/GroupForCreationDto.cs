namespace OVD.API.Dtos
{
    public class GroupForCreationDto
    {
        public string Name { get; set; }
        public int Total { get; set; }
        public string ServiceOffering { get; set; }
        public string Protocol { get; set; }
        public string Template { get; set; }
        public int Hotspares { get; set; }
        public int Max { get; set; }
    }
}