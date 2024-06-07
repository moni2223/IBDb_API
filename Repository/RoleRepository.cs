using AutoMapper;
using IBDb.Data;
using IBDb.Interfaces;
using IBDb.Models;

namespace IBDb.Repository
{
    public class RoleRepository : IRoleRepository
    {
        private readonly DataContext _context;

        public RoleRepository(DataContext context)
        {
            _context = context;
        }
        public Role GetRole(string roleName)
        {
            return _context.Roles.Where(r => r.RoleName == roleName).FirstOrDefault();
        }
        public Role GetRole(int id)
        {
            return _context.Roles.Where(r => r.Id == id).FirstOrDefault();
        }
        public ICollection<Role> GetRoles() 
        {
            return _context.Roles.OrderBy(r => r.RoleName).ToList();
        }
        public bool CreateRole(Role role)
        {
            _context.Add(role);
            return Save();
        }
       public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0;
        }
    }
}
