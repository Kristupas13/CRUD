using System;
using System.ComponentModel.DataAnnotations;

namespace CRUDWebService.DataLayer.Entities
{
    public class UniversityBook
    {
        [Key]
        public int Id { get; set; }
        public int UniversityId { get; set; }
        public string BookISBN { get; set; }
        public bool IsAvailable { get; set; }
        public DateTime AvailableFrom { get; set; }
    }
}
