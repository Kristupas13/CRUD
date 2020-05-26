using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRUDWebService.BusinessLayer.DTO
{
    public class AddBookDto : BookDTO
    {
        public int UniversityId { get; set; }
        public DateTime AvailableFrom { get; set; }
        public bool IsAvailable { get; set; }
    }
}
