using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Services;
using Microsoft.AspNetCore.Mvc;

namespace App.Controllers
{
    [Area("ProductManage")]
    public class ProductController : Controller
    {
        private readonly ILogger<ProductController> _logger;
        private readonly ProductService _productService;

        public ProductController(ILogger<ProductController> logger, ProductService productService)
        {
            _logger = logger;
            _productService = productService;
        }

        /**
            Có area: 
            Tìm file theo đường dẫn: /Areas/{AreasName}/Views/{ControllerName}/Action.cshtml
        */
        [HttpGet("danhsachsanpham.html", Name = "danhsachsanpham")]
        public IActionResult Index()
        {
            var products = _productService.ToList();
            return View(products); // => /Areas/ProductManage/Views/Product/Index.cshtml
        }
    }
}