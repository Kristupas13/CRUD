using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRUDWebService.BusinessLayer.DTO
{
    public class EditUniversityDTO
    {
        public int UniversityId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
    }
}
