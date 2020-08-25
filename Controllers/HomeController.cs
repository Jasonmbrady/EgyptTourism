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
using Microsoft.EntityFrameworkCore;

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
            ViewBag.DestinationComments = db.Comments.Where(com => com.DestinationId == id).Include(com => com.User).ToList();
            Wishlist wish = db.Wishlists.FirstOrDefault(w => w.DestinationId == id && w.UserId == (int)HttpContext.Session.GetInt32("UserId"));
            if (wish != null)
            {
                ViewBag.IsWishlisted = true;
                ViewBag.WishId = (int)wish.WishlistId;
            }
            else
            {
                ViewBag.IsWishlisted = false;
                ViewBag.WishId = 0;
            }
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

        [HttpPost]
        [Route("/comment/create/{DestId}")]
        public IActionResult CreateComment(int DestId, Comment newComment)
        {
            if (!isLoggedIn)
            {
                return RedirectToAction("Index");
            }
            if (ModelState.IsValid == false)
            {
                return View("DestinationDetail");
            }
            newComment.UserId = (int)uid;
            newComment.DestinationId = DestId;
            db.Comments.Add(newComment);
            db.SaveChanges();
            return RedirectToAction("DestinationDetail", new { id = DestId });

        }

        [HttpGet]
        [Route("/user/{UserId}/wishlist")]
        public IActionResult MyWishlist(int UserId)
        {
            if (!isLoggedIn)
            {
                return RedirectToAction("Index");
            }
            List<Wishlist> myWishlist = new List<Wishlist>();
            myWishlist = db.Wishlists.Where(w => w.UserId == UserId).Include(w => w.Destination).ToList();
            return View("Wishlist", myWishlist);
        }

        [HttpPost]
        [Route("/User/{UserId}/wishlist/add/{DestId}")]
        public IActionResult AddWishlist(int UserId, int DestId)
        {
            if (!isLoggedIn)
            {
                return RedirectToAction("Index");
            }
            Wishlist exists = db.Wishlists.FirstOrDefault(w => w.UserId == UserId && w.DestinationId == DestId);
            if (exists == null)
            {
                Wishlist newWishlist = new Wishlist();
                newWishlist.UserId = UserId;
                newWishlist.DestinationId = DestId;

                db.Wishlists.Add(newWishlist);
                db.SaveChanges();
            }
            return RedirectToAction("Landing");
        }

        [HttpGet]
        [Route("/wishlist/remove/{WishId}")]
        public IActionResult RemoveWishlist(int WishId)
        {
            Wishlist selectedWishlist = db.Wishlists.FirstOrDefault(w => w.WishlistId == WishId);
            if (selectedWishlist != null)
            {
                db.Wishlists.Remove(selectedWishlist);
                db.SaveChanges();
            }

            return RedirectToAction("Landing");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
