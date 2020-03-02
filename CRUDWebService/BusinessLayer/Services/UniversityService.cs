using CRUDWebService.BusinessLayer.Contracts;
using CRUDWebService.BusinessLayer.DTO;
using CRUDWebService.DataLayer.Context;
using CRUDWebService.DataLayer.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRUDWebService.BusinessLayer.Services
{
    public class UniversityService : IUniversityService
    {
        private readonly UniversityContext _db;

        public UniversityService(UniversityContext db)
        {
            _db = db;
        }

        public async Task<UniversityDTO> AddAsync(AddUnivesityDTO addUnivesity)
        {
            _db.Universities.Add(new University
            {
                Address = addUnivesity.Address,
                Name = addUnivesity.Name
            });
            await _db.SaveChangesAsync();

            return new UniversityDTO { Address = addUnivesity.Address, Name = addUnivesity.Name };
        }

        public async Task<UniversityDTO> EditAsync(EditUniversityDTO editUniversity)
        {
            var entity = _db.Universities.Find(editUniversity.UniversityId);
            entity.Name = string.IsNullOrEmpty(editUniversity.Name) ? entity.Name : editUniversity.Name;
            entity.Address = string.IsNullOrEmpty(editUniversity.Name) ? entity.Address : editUniversity.Name;
            await _db.SaveChangesAsync();

            return new UniversityDTO { Address = editUniversity.Address, Name = editUniversity.Name };
        }

        public UniversityDTO Get(int id)
        {
            var entity = _db.Universities.Find(id);

            return new UniversityDTO { Address = entity.Address, Name = entity.Name } ;
        }

        public IEnumerable<UniversityDTO> GetAll()
        {
            return _db.Universities.Select(p => new UniversityDTO { Name = p.Name, Address = p.Address });
        }

        public async Task RemoveAsync(RemoveUniversityDTO addUnivesity)
        {
             await Task.Run(() => _db.Remove(addUnivesity.UniversityId));
        }
    }
}
