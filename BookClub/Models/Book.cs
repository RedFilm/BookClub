using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookClub.Models
{
    public class Book
    {
        public int Id { get; set; }

        public string Title { get; set; } = "";
        public string Author { get; set; } = "";
        public string PublishHouse { get; set; } = "";
        public string PublishYear { get; set; } = "";

        public string Image { get; set; } = "";

        public string Description { get; set; } = "";
        public string Tags { get; set; } = "";

        public int GenreId { get; set; }
        public Genre? Genre { get; set; }

        public List<AppUser>? Users { get; set; }
    }
}
