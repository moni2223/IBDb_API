using IBDb.Models;

namespace IBDb.Dto
{
    public class UserCreateDto
    {
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; } 
        public string Email { get; set; }
        public string RoleName { get; set; }
    }

    public class UserSuccessfullLoginDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string RoleName { get; set; }

    }
    public class UserListDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public int RoleId { get; set; }
        public Role Role { get; set; }

    }
    public class LoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
    public class PublisherDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
    }
}
