using IBDb.Data;
using IBDb.Interfaces;
using IBDb.Models;
using Microsoft.EntityFrameworkCore;

namespace IBDb.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;

        public UserRepository(DataContext context)
        {
            _context = context;
        }
        public ICollection<User> GetUsers()  // ICollection you can add / remove but only to the end
        {
            return _context.Users.OrderBy(u => u.FullName).ToList();
        }
        public User GetUser(int id)
        {
            return _context.Users.Where(u => u.Id == id).FirstOrDefault();
        }

        public ICollection<User> GetUsersByQuery(string query){
            return _context.Set<User>()
              .Where(u => u.FullName.Contains(query))
              .OrderBy(u => u.FullName)
              .ToList();
        }

        public User GetUserByEmail(string email)
        {
            return _context.Users.Where(u => u.Email == email).Include(u => u.Role).FirstOrDefault();
        }

        public bool CreateUser(User user)
        {
            _context.Add(user);
            return Save();
        }
        public bool UpdateUser(User user)
        {
            _context.Update(user);
            return Save();
        }

        public bool DeleteUser(User user)
        {
            _context.Remove(user);
            return Save();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0;
        }

       
    }
}
