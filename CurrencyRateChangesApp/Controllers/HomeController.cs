using CurrencyRateChangesApp.Models;
using CurrencyRateChangesApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyRateChangesApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ExchangeRateService _exchangeRateService;

        public HomeController(ExchangeRateService exchangeRateService)
        {
            _exchangeRateService = exchangeRateService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Changes(string date)
        {
            return View(_exchangeRateService.GetRateChanges(date));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
