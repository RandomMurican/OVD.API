using System;
using System.Collections.Generic;


namespace OVD.API.Dtos
{
    public class GroupsToAddDto
    {
       public int Id { get; set; }
       public String[] AddIds { get; set; }
       public String[] RemoveIds { get; set; }
    }
}