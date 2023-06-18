using System;
using System.Collections.Generic;
#nullable disable
namespace Book_Store.Models
{

    public partial class Book
    {
        public int BookId { get; set; }
        public string BookTitle { get; set; } 
        public string Author { get; set; } 
        public string Description { get; set; } 
        public string Category { get; set; } 
        public int BookPrice { get; set; }
        public string Publisher { get; set; } 
        public bool IsDeleted { get; set; }
        public int NumberOfCopies { get; set; }
        public int UserId { get; set; }

        //public virtual User User { get; set; } 
    }
}
