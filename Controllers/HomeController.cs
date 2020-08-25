using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using EgyptTourism.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace EgyptTourism.Controllers
{
    public class HomeController : Controller
    {
        private EgyptTourismContext db;
        private int? uid
        {
            get
            {
                return HttpContext.Session.GetInt32("UserId");
            }
        }

        private bool isLoggedIn
        {
            get
            {
                return uid != null;
            }
        }
        public HomeController(EgyptTourismContext context)
        {
            db = context;
        }

        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            return View("Index");
        }

        [HttpPost]
        [Route("/register")]
        public IActionResult Register(User newUser)
        {
            // validations
            if (ModelState.IsValid)
            {
                if (db.Users.Any(u => u.Email == newUser.Email))
                {
                    ModelState.AddModelError("Email", "already exists!");
                }
            }
            if (ModelState.IsValid == false)
            {
                return View("Index");
            }
            // hash password
            PasswordHasher<User> hasher = new PasswordHasher<User>();
            newUser.Password = hasher.HashPassword(newUser, newUser.Password);
            // create db entry
            db.Users.Add(newUser);
            db.SaveChanges();
            // add to session
            HttpContext.Session.SetInt32("UserId", newUser.UserId);
            HttpContext.Session.SetString("UserName", newUser.FirstName);
            return RedirectToAction("Landing");
        }

        [HttpPost]
        [Route("/login")]
        public IActionResult Login(LoginUser loginUser)
        {
            // validations
            if (ModelState.IsValid == false)
            {
                return View("Index");
            }
            // retrieve User from db
            User dbUser = db.Users.FirstOrDefault(u => u.Email == loginUser.LoginEmail);
            if (dbUser == null)
            {
                ModelState.AddModelError("LoginEmail", "Invalid E-Mail/Password combination");
                return View("Index");
            }
            // hash password and compare
            PasswordHasher<LoginUser> hasher = new PasswordHasher<LoginUser>();
            PasswordVerificationResult pwCompareResult = hasher.VerifyHashedPassword(loginUser, dbUser.Password, loginUser.LoginPassword);
            if (pwCompareResult == 0)
            {
                ModelState.AddModelError("LoginEmail", "Invalid E-Mail/Password combination");
                return View("Index");
            }
            // add to session
            HttpContext.Session.SetInt32("UserId", dbUser.UserId);
            HttpContext.Session.SetString("UserName", dbUser.FirstName);
            return RedirectToAction("Landing");
        }
        [HttpGet]
        [Route("/landing")]
        public IActionResult Landing()
        {
            if (!isLoggedIn)
            {
                return RedirectToAction("Index");
            }
            ViewBag.Pharaoh = db.Destinations
            .Where(dest => dest.Type == "Pharaoh")
            .ToList();

            ViewBag.Coptic = db.Destinations
            .Where(dest => dest.Type == "Coptic")
            .ToList();

            ViewBag.Islamic = db.Destinations
            .Where(dest => dest.Type == "Islamic")
            .ToList();

            ViewBag.Recreation = db.Destinations
            .Where(dest => dest.Type == "Recreation")
            .ToList();

            return View("Landing");

        }

        [HttpGet("/destination/{id}")]
        public IActionResult DestinationDetail(int id)
        {
             if (!isLoggedIn)
            {
                return RedirectToAction("Index");
            }

            ViewBag.SelectedDestination = db.Destinations
            .FirstOrDefault(dest => dest.DestinationId == id);
            return View("DestinationDetail");
        }

        [HttpGet]
        [Route("/logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");

        }
        [HttpGet]
        [Route("/destination/new")]
        public IActionResult NewDestination()
        {
            if (!isLoggedIn)
            {
                return RedirectToAction("Index");
            }
            return View("DestCreator");
        }

        [HttpPost]
        [Route("/destination/create")]
        public IActionResult CreateDest(Destination newDestination)
        {
            if (!isLoggedIn)
            {
                return RedirectToAction("Index");
            }
            if (ModelState.IsValid == false)
            {
                return View("DestCreator");
            }
            db.Destinations.Add(newDestination);
            db.SaveChanges();
            return RedirectToAction("Landing");
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
