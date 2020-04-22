using System.ComponentModel.DataAnnotations;

namespace CRUDWebService.DataLayer.Entities
{
    public class UniversityBook
    {
        public int UniversityId { get; set; }
        public string BookISBN { get; set; }
    }
}
