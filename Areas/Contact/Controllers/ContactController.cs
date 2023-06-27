using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using App.Models;
using App.Models.Contact;
using Microsoft.AspNetCore.Authorization;

namespace App.Areas.Contact.Controllers
{
    [Area("Contact")]
    public class ContactController : Controller
    {
        private readonly AppDbContext _context;

        public ContactController(AppDbContext context)
        {
            _context = context;
        }

        [TempData]
        public string StatusMessage { get; set; }

        [HttpGet("/admin/contact")]
        public async Task<IActionResult> Index()
        {
            return _context.ContactModel != null ?
                        View(await _context.ContactModel.ToListAsync()) :
                        Problem("Entity set 'AppDbContext.ContactModel'  is null.");
        }

        [HttpGet("/admin/contact/detail/{id:int}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ContactModel == null)
            {
                return NotFound();
            }

            var contactModel = await _context.ContactModel
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contactModel == null)
            {
                return NotFound();
            }

            return View(contactModel);
        }

        [HttpGet("/contact/")]
        [AllowAnonymous] // => Không cần kiểm tra role của user
        public IActionResult SendContact()
        {
            return View();
        }

        [HttpPost("/contact/")]
        [AllowAnonymous] // => Không cần kiểm tra role của user
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendContact([Bind("FullName,Phone,Email,Message")] ContactModel contactModel)
        {
            if (ModelState.IsValid)
            {
                contactModel.DateSent = DateTime.Now;
                _context.Add(contactModel);
                await _context.SaveChangesAsync();
                StatusMessage = "Cảm ơn bạn đã gửi liên hệ!";
                return RedirectToAction("Index", "Home");
            }
            
            return View(contactModel);
        }

        [HttpGet("/admin/contact/delete/{id:int}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ContactModel == null)
            {
                return NotFound();
            }

            var contactModel = await _context.ContactModel
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contactModel == null)
            {
                return NotFound();
            }

            return View(contactModel);
        }

        [HttpPost("/admin/contact/delete/{id:int}", Name = "Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ContactModel == null)
            {
                return Problem("Entity set 'AppDbContext.ContactModel'  is null.");
            }
            var contactModel = await _context.ContactModel.FindAsync(id);
            if (contactModel != null)
            {
                _context.ContactModel.Remove(contactModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
