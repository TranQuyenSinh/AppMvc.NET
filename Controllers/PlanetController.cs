using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using App.Services;

namespace App.Controllers
{
    [Route("he-mat-troi")]
    /**
        Route ở Controller này sẽ kết hợp với route ở action
        /he-mat-troi/[action]/...
    */
    public class PlanetController : Controller
    {
        private readonly ILogger<PlanetController> _logger;
        private readonly PlanetService _planetService;

        public PlanetController(ILogger<PlanetController> logger, PlanetService planetService)
        {
            _logger = logger;
            _planetService = planetService;
        }

        [Route("danhsachhanhtinh.html")]
        public IActionResult Index()
        {
            return View(_planetService);
        }

        [HttpGet("traidat.html")]
        public IActionResult Earth(int? id)
        {
            var planet = _planetService.Where(x => x.Id == id).FirstOrDefault();
            return View("Details", planet);
        }

        [Route("hanhtinh/[action]/{id:int?}", Name = "PlanetDetail")]
        public IActionResult Details(int? id)
        {
            var planet = _planetService.Where(x => x.Id == id).FirstOrDefault();
            return View(planet);
        }
    }
}

/**
    Controller, Action, Area => [controller], [action], [area]
    vd: [Route("[controller]-[action].html")] => /Planet-Details.html
    [Route("[controller]-[action].html")]
    Name = "PlanetDetail" => @Url.RouteUrl(Name) => Url của action này
*/