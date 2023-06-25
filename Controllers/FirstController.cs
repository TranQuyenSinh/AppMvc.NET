using App.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App.Controllers
{
    public class FirstController : Controller
    {
        private readonly ILogger<FirstController> _logger;
        private readonly IWebHostEnvironment _env;
        private readonly ProductService _productService;
        public FirstController(ILogger<FirstController> logger, IWebHostEnvironment env, ProductService productService)
        {
            _logger = logger;
            _env = env;
            _productService = productService;
        }

        [TempData]
        public string StatusMessage { get; set; }

        public string Index()
        {
            _logger.LogInformation("Index Action");
            return "I'm index of FirstController";
        }

        public IActionResult Image()
        {
            var filePath = Path.Combine(_env.ContentRootPath, "Files", "moto.png");
            var bytes = System.IO.File.ReadAllBytes(filePath);

            return File(bytes, "image/png");
        }

        public IActionResult Hello(string name)
        {
            if (string.IsNullOrEmpty(name))
                name = "Khách";
            return View((object)name);
        }

        public IActionResult Product(int? id)
        {
            var product = _productService.Where(x => x.Id == id).FirstOrDefault();
            if (product == null)
            {
                StatusMessage = "Error: Product not found";
                return LocalRedirect("/");
            }

            ViewData["TotalProduct"] = _productService.Count();


            return View(product);
        }
    }
}

/**
    Một số cách truyền dữ liệu từ controller sang view
    1. ViewData
            - ViewData["key"] = product;
            - var product = ViewData["key"] as ProductModel;
    2. ViewBag => dynamic
            - ViewBag.productXYZ = product;
            - ViewBag.productXYZ
    3. TempData => đọc 1 lần sẽ bị xóa
            - TempData["StatusMessage"] = "Success";
            - var thongbao = TempData["StatusMessage"];

*/