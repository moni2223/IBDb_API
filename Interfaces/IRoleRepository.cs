using IBDb.Models;

namespace IBDb.Interfaces
{
    public interface IRoleRepository
    {
        ICollection<Role> GetRoles();
        Role GetRole(string roleName);
        Role GetRole(int id);
        bool CreateRole(Role role);
        bool Save();
    }
}
