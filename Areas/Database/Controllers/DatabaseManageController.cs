using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App.Areas.Database.Controllers
{
    [Area("Database")]
    [Route("/database-manage/[action]")]
    public class DatabaseManageController : Controller
    {
        private readonly AppDbContext _dbContext;

        public DatabaseManageController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [TempData]
        public string StatusMessage { get; set;}

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult DeleteDb()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteDbAsync() {
            var success = await _dbContext.Database.EnsureDeletedAsync();
            StatusMessage = success ? "Xóa database thành công": "Xóa thất bại";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Migrate() {
            await _dbContext.Database.MigrateAsync();
            StatusMessage = "Cập nhật database thành công";
            return RedirectToAction(nameof(Index));
        }
    }
}