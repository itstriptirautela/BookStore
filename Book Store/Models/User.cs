using System;
using System.Collections.Generic;
#nullable disable
namespace Book_Store.Models
{
 
  
    public partial class User
    {
        public User()
        {
            Books = new HashSet<Book>();
        }

        public int UserId { get; set; }
        public string Name { get; set; } 
        public string Address { get; set; } 
        public string Role { get; set; } 
        public string Username { get; set; } 
        public string Password { get; set; }
        public long? PhoneNumber { get; set; }
        public string Email { get; set; } 

        public virtual ICollection<Book> Books { get; set; }
    }
}
