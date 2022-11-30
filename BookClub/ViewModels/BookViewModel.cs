using BookClub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookClub.ViewModels
{
    public class BookViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; } = "";
        public string Author { get; set; } = "";
        public string PublishHouse { get; set; } = "";
        public string PublishYear { get; set; } = "";
        public string Description { get; set; } = "";
        public string Tags { get; set; } = "";

        public Genre? Genre { get; set; }
        public List<Genre>? Genries { get; set; }

        public string CurrentImage { get; set; } = "";
        public IFormFile? Image { get; set; } = null;
    }
}
