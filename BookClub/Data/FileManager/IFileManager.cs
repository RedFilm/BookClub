using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookClub.Data.FileManager
{
    public interface IFileManager
    {
        public FileStream ImageStream(string image);
        public Task<string> SaveImage(IFormFile image);
        public bool RemoveImage(string image);

    }
}
