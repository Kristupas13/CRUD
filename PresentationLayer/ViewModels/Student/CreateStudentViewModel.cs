using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRUDWebService.PresentationLayer.ViewModels
{
    public class CreateStudentViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int AverageGrade { get; set; }
    }
}
