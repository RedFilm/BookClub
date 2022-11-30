using BookClub.Data;
using BookClub.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookClub.Controllers
{
    [Authorize]
    public class LibController : Controller
    {
        private UserManager<AppUser> _userMnr;
        private AppDbContext _ctx;

        public LibController(UserManager<AppUser> userMnr, AppDbContext ctx)
        {
            _userMnr = userMnr;
            _ctx = ctx;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userMnr.GetUserAsync(HttpContext.User);

            if (user == null)
                return BadRequest("Something went wrong");

            var user_books = _ctx.Users.Include(u => u.Books)
                .Single(u => u.Id == user.Id);

            return View(user_books);
        }

        [HttpGet]
        public async Task<IActionResult> AddBook(int? id)
        {
            var book = _ctx.Books.FirstOrDefault(b => b.Id == id);

            if (book == null)
                return BadRequest("Something went wrong");

            var user = await _userMnr.GetUserAsync(HttpContext.User);

            if (user == null)
                return BadRequest("Something went wrong");

            var user_books = _ctx.Users.Include(u => u.Books)
                .Single(u => u.Id == user.Id);

            user.Books = user.Books ?? new List<Book>();

            if (user_books.Books.Contains(book))
            {
                return Ok("You already have this book");
            }

            user.Books.Add(book);

            await _ctx.SaveChangesAsync();

            return RedirectToAction("Index","Home");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveBook(int? id)
        {
            var book = _ctx.Books.FirstOrDefault(b => b.Id == id);

            if (book == null)
                return BadRequest("Something went wrong");

            var user = await _userMnr.GetUserAsync(HttpContext.User);

            if (user == null)
                return BadRequest("Something went wrong");

            var user_books = _ctx.Users.Include(u => u.Books)
                .Single(u => u.Id == user.Id);

            if (user_books.Books == null)
                return BadRequest();

            if (user_books.Books.Contains(book))
            {
                user_books.Books.Remove(book);
                await _ctx.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

    }
}
