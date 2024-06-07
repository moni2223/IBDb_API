using System.Text.Json.Serialization;

namespace IBDb.Models
{
    public class Role
    {
        public int Id { get; set; }
        public string RoleName { get; set; }

        [JsonIgnore]
        public ICollection<User> Users { get; set; }
    }
}
