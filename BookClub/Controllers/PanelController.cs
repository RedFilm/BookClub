using BookClub.Data;
using BookClub.Data.FileManager;
using BookClub.Models;
using BookClub.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookClub.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class PanelController : Controller
    {
        private AppDbContext _ctx;
        private IFileManager _fileManager;

        public PanelController(AppDbContext ctx, IFileManager fileManager)
        {
            _ctx = ctx; ;
            _fileManager = fileManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var books = _ctx.Books.ToList();
            return View(books);
        }

        [HttpGet]
        public IActionResult AddBook()
        {
            var genries = _ctx.Genries.ToList();

            var bookVm = new BookViewModel 
            { 
                Genries = genries
            };

            return View(bookVm);
        }
    
        [HttpPost]
        public async Task<IActionResult> AddBook(BookViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(new BookViewModel());
            }

            var book = new Book()
            {
                Title = vm.Title,
                Author = vm.Author,
                PublishHouse = vm.PublishHouse,
                PublishYear = vm.PublishYear,
                Description = vm.Description,
                Tags = vm.Tags,
            };
            var genre = _ctx.Genries.FirstOrDefault(g => g.Id == vm.Genre.Id);

            if (genre == null)
                book.Genre = vm.Genre;
            else
                book.Genre = genre;

            if (vm.Image == null)
                return BadRequest("Upload the image!");

            book.Image = await _fileManager.SaveImage(vm.Image);

            _ctx.Add(book);
            await _ctx.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult EditBook(int id)
        {
             var book = _ctx.Books.FirstOrDefault(b => b.Id == id);
            return View(new BookViewModel
            {
                Title = book.Title,
                Author = book.Author,
                PublishHouse = book.PublishHouse,
                PublishYear = book.PublishYear,
                Description = book.Description,
                Tags = book.Tags,
                CurrentImage = book.Image,
                Genre = book.Genre,
                Genries = _ctx.Genries.ToList()
            });
        }

        [HttpPost]
        public async Task<IActionResult> EditBook(BookViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var book = new Book
            {
                Id = vm.Id,
                Title = vm.Title,
                Author = vm.Author,
                PublishHouse = vm.PublishHouse,
                PublishYear = vm.PublishYear,
                Description = vm.Description,
                Tags = vm.Tags,
            };
            var genre = _ctx.Genries.FirstOrDefault(g => g.Id == vm.Genre.Id);

            if (genre == null)
                book.Genre = vm.Genre;
            else
                book.Genre = genre;

            if (vm.Image == null)
                book.Image = vm.CurrentImage;
            else
            {
                if (!String.IsNullOrEmpty(vm.CurrentImage))
                    _fileManager.RemoveImage(vm.CurrentImage);

                book.Image = await _fileManager.SaveImage(vm.Image);
            }
             
            _ctx.Books.Update(book);
            await _ctx.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> RemoveBook(int id)
        {
            var book = _ctx.Books.FirstOrDefault(b => b.Id == id);

            _fileManager.RemoveImage(book.Image);
            _ctx.Books.Remove(book);
            await _ctx.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
