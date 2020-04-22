using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CRUDWebService.DataLayer.Entities
{
    public class University
    {
        [Key]
        public int UniversityId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }

        public List<Student> Students { get; set; }
        public List<UniversityBook> UniversityBooks { get; set; }
    }
}
