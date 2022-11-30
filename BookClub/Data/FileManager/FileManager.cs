using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookClub.Data.FileManager
{
    public class FileManager : IFileManager
    {
        private string _imagePath;
        public FileManager(IConfiguration conf)
        {
            _imagePath = conf["Path:Images"];
        }

        public FileStream ImageStream(string image)
        {
            return new FileStream(Path.Combine(_imagePath, image),FileMode.Open,FileAccess.Read);  
        }

        public bool RemoveImage(string image)
        {
            try
            {
                var file = Path.Combine(_imagePath, image);
                if (File.Exists(file))
                    File.Delete(file);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public async Task<string> SaveImage(IFormFile image)
        {
            var dir_path = Path.Combine(_imagePath);

            if (!Directory.Exists(dir_path))
            {
                Directory.CreateDirectory(dir_path);
            }

            var mime = image.FileName.Substring(image.FileName.LastIndexOf("."));

            var img_name = $"img_{DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss")}{mime}";


            using (var imageStream = new FileStream(Path.Combine(dir_path, img_name), FileMode.Create))
            {
                await image.CopyToAsync(imageStream);
            }

            return img_name;
        }
    }
}
