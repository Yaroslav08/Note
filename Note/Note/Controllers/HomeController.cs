using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Note.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Note.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly NoteService db;

        public HomeController(ILogger<HomeController> logger, NoteService _db)
        {
            _logger = logger;
            db = _db;
        }

        [HttpGet("/")]
        public async Task<IActionResult> Index()
        {
            return View(await db.GetAllNotes());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetails(string id)
        {
            var note = await db.GetById(id);
            if (note == null)
            {
                ViewBag.Error = "Note by id not found";
                return LocalRedirect("~/");
            }
            return View(note);
        }



        [HttpGet("new")]
        public IActionResult CreateNew()
        {
            return View();
        }

        [HttpPost("new")]
        public async Task<IActionResult> CreateNew(Models.Note note)
        {
            if (ModelState.IsValid)
            {
                await db.CreateNote(note);
                return LocalRedirect("~/");
            }
            return View(note);
        }



        [HttpGet("edit")]
        public async Task<IActionResult> Update(string id)
        {
            var note = await db.GetById(id);
            if (note == null)
            {
                ViewBag.Error = "Note by id not found";
                return LocalRedirect("~/");
            }
            return View(note);
        }

        [HttpPost("edit")]
        public async Task<IActionResult> Update(Models.Note note)
        {
            await db.UpdateNote(note);
            return LocalRedirect("~/");
        }

        [HttpGet("remove")]
        public async Task<IActionResult> Remove(string id)
        {
            await db.Remove(id);
            return LocalRedirect("~/");
        }
    }
}
