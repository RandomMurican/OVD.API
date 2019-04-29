using System;
using System.Collections.Generic;


namespace OVD.API.Dtos
{
    public class UserForListDto
    {
        	public int Id { get; set; }

            public List<string> Users { get; set; }
    }
}