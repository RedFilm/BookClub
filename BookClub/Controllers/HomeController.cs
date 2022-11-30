using BookClub.Data;
using BookClub.Data.FileManager;
using BookClub.Models;
using BookClub.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BookClub.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _ctx;
        private readonly IFileManager _fileManager;

        public HomeController(ILogger<HomeController> logger, AppDbContext ctx, 
            IFileManager fileManager, UserManager<AppUser> userManager)
        {
            _logger = logger;
            _ctx = ctx;
            _fileManager = fileManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var books = _ctx.Books.ToList();
            return View(books);
        }

        [HttpGet]
        public IActionResult Book(int? id)
        {
            if (id == null)
            {
                return View(new BookViewModel());
            }
            
            var book = _ctx.Books.First(x => x.Id == id);

            return View(new BookViewModel()
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                CurrentImage = book.Image,
                Description = book.Description,
            });
        }

        [HttpGet]
        public IActionResult Genries()
        {
            var genries = _ctx.Genries.ToList();

            return View(genries);
        }

        [HttpGet("/Image/{image}")]
        public IActionResult Image(string image)
        {
            var mime = image.Substring(image.LastIndexOf("."));
            return new FileStreamResult(_fileManager.ImageStream(image), $"image/{mime}");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}