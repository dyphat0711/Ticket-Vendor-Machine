using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BenThanhMetro.Models;

namespace BenThanhMetro.Controllers
{
    public class HomeController : Controller
    {
        private MetroTicketDBEntities db = new MetroTicketDBEntities();

        public ActionResult Index()
        {
            var destinations = db.Destinations.ToList();

            return View(destinations);
        }
    }
}