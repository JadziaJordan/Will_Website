using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Will_Practice.Models;

namespace Will_Practice.Controllers;

    public class DonateController : Controller
    {
        // GET: AboutUs
        public ActionResult Index()
        {
            ViewBag.Title = "Donate";
            return View();
        }
    }
