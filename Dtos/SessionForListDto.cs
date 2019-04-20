using System;

namespace OVD.API.Dtos
{
    public class SessionForListDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string User { get; set; }
        public int GroupId { get; set; }
        public string Group { get; set; }
        public bool Active { get; set; }
        public int Time { get; set; }
        public DateTime Start { get; set; }
    }
}