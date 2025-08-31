using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Myblog.ViewModel;

[Authorize(Roles = "superadmin")]
public class SuperAdminController : Controller
{
    private readonly ApplicationDbContext _context;

    public SuperAdminController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> SuperAdminIndex()
    {
        var blogs = await _context.Blogs.Where(b => b.Status == "Pending" || b.Status == "Approved").ToListAsync();        
        return View(blogs);
    }

    public IActionResult Review(int id)
    {
        var blog = _context.Blogs
            .Where(b => b.Id == id)
            .Select(b => new BlogFormModel
            {
                Id = b.Id,
                Title = b.Title,
                Author = b.Author,
                Content = b.Content,
                CreatedAt = b.CreatedAt
            })
            .FirstOrDefault();

        if (blog == null)
        {
            return NotFound();
        }

        return View(blog);
    }

    [HttpPost]
    public async Task<IActionResult> Approve(int id)
    {
        var blog = await _context.Blogs.FindAsync(id);
        if (blog != null)
        {
            blog.Status = "Approved";
            blog.RejectionReason = null; // cler this 
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(SuperAdminIndex));
    }


    //[HttpPost]
    //public async Task<IActionResult> Reject(int id, string rejectionReason)
    //{
    //    var blog = await _context.Blogs.FindAsync(id);
    //    if (blog != null)
    //    {
    //        blog.Status = "Rejected";
    //        blog.RejectionReason = rejectionReason; // Save the reason
    //        await _context.SaveChangesAsync();
    //    }
    //    return RedirectToAction("SuperAdminIndex");
    //}


    [HttpPost]
    public async Task<IActionResult> Reject(int id, string rejectionReason)
    {
        if (string.IsNullOrWhiteSpace(rejectionReason)) // Ensure reason is provided
        {
            ModelState.AddModelError("RejectionReason", "Rejection reason is required.");
            return View("Review", new { id = id }); // Reload the review page with error
        }

        var blog = await _context.Blogs.FindAsync(id);
        if (blog != null)
        {
            blog.Status = "Rejected";
            blog.RejectionReason = rejectionReason; // Save the rejection reason
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("SuperAdminIndex");
    }


}
