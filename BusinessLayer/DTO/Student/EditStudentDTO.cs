namespace CRUDWebService.BusinessLayer.DTO.Student
{
    public class EditStudentDTO
    {
        public int StudentId { get; set; }
        public int? UniversityId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? AverageGrade { get; set; }
    }
}
