using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Myblog.Models;
using Myblog.ViewModel;

namespace Myblog.Controllers
{

    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var approvedBlogs = await _context.Blogs
                .Where(b => b.Status == "Approved")
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

            return View(approvedBlogs);
        }


        public async Task<IActionResult> Details(int id)
        {
            var blog = await _context.Blogs.FindAsync(id);
            if (blog == null || blog.Status != "Approved") return NotFound();

            return View(blog);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
