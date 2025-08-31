using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Myblog.Models;
using Myblog.ViewModel;
using System.Reflection.Metadata;

[Authorize(Roles = "admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> AdminIndex()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();
        var blogs = await _context.Blogs
            .Where(b => b.AuthorId == user.Id)
            .ToListAsync();

        return View(blogs);
    }

    public IActionResult Create() => View();

    [HttpPost]

    public async Task<IActionResult> Create(BlogFormModel blog, string actionType)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized(); // Ensure the user is authenticated

        var newBlog = new BlogFormModel
        {
            Title = blog.Title,
            Content = blog.Content,
            AuthorId = user.Id, // Assign the authenticated user's ID
            CreatedAt = DateTime.Now,
            Status = actionType == "SaveDraft" ? "Draft" : "Pending" // Save as Draft or Submit for Approval
        };

        _context.Blogs.Add(newBlog);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(AdminIndex));
    }

    //public async Task<IActionResult> Create(BlogFormModel blog)
    //{
    //    var user = await _userManager.GetUserAsync(User);
    //    blog.AuthorId = user.Id;
    //    blog.Status = "Pending";
    //    _context.Blogs.Add(blog);
    //    await _context.SaveChangesAsync();
    //    return RedirectToAction(nameof(AdminIndex));
    //}

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var data = await _context.Blogs.FirstOrDefaultAsync(x => x.Id == id);
        if (data == null)
        {
            return NotFound();
        }

        _context.Blogs.Remove(data);
        await _context.SaveChangesAsync();
        ViewBag.Message = " Record Delete Successfully";
        //return RedirectToAction(nameof(AdminIndex));
        return View(data);
    }

    public async Task<IActionResult> Details(int id)
    {
        var blog = await _context.Blogs.FindAsync(id);
        return blog == null ? NotFound() : View(blog);
    }



    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var blog = await _context.Blogs.FindAsync(id);
        if (blog == null || blog.Status == "Approved") // Prevent editing approved blogs
        {
            return NotFound();
        }
        return View(blog);
    }

     [HttpPost]
       public async Task<IActionResult> Edit(int id, BlogFormModel model)
        {
            //if (!ModelState.IsValid) return View(model);
     
            var blog = await _context.Blogs.FindAsync(id);
            if (blog != null)
            {
                blog.Title = model.Title;
                blog.Content = model.Content;
                blog.Status = "Pending"; // Resend for approval
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("AdminIndex");
        }
}




