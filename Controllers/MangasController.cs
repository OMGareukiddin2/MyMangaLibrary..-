using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyMangaLibrary.Data;
using MyMangaLibrary.Models;

namespace MyMangaLibrary.Controllers
{
    public class MangasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MangasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Mangas
        public async Task<IActionResult> Index()
        {
            var ctx = _context.Manga.Include(m => m.Mangaka);
            return View(await ctx.ToListAsync());
        }
        public async Task<IActionResult> ShowSearchForm()
        {
            return View();
        }
        public async Task<IActionResult> ShowSearchResults(string SearchPhrase)
        {
            var ctx = _context.Manga.Include(m => m.Mangaka);
            return View("Index", await ctx.Where( m => m.Name.Contains(SearchPhrase)).ToListAsync());
        }
        // GET: Mangas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var manga = await _context.Manga
                .Include(m => m.Mangaka)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (manga == null)
            {
                return NotFound();
            }

            return View(manga);
        }

        // GET: Mangas/Create
        public IActionResult Create()
        {
            ViewData["MangakaID"] = new SelectList(_context.Mangaka, "ID", "Name");
            return View();
        }

        // POST: Mangas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Rating,Name,ChapterCount,MangakaID")] Manga manga)
        {
            if (ModelState.IsValid)
            {
                _context.Add(manga);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MangakaID"] = new SelectList(_context.Mangaka, "ID", "Name", manga.MangakaID);
            return View(manga);
        }

        // GET: Mangas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var manga = await _context.Manga.FindAsync(id);
            if (manga == null)
            {
                return NotFound();
            }
            return View(manga);
        }

        [HttpPost]
        public ActionResult DecreaseChapter(int id) {
            if (id == null) {
                return NotFound();
            }
            var manga =  _context.Manga.Where(m => m.ID == id).FirstOrDefault();
            if (manga == null) {
                return NotFound();
            }

            if (manga.ChapterCount > 0) {
                manga.ChapterCount--;
            } 
            _context.Update(manga);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult IncreaseChapter(int id) {
            if (id == null) {
                return NotFound();
            }
            var manga =  _context.Manga.Where(m => m.ID == id).FirstOrDefault();
            if (manga == null) {
                return NotFound();
            }

            manga.ChapterCount++;
            _context.Update(manga);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // POST: Mangas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Rating,Name,ChapterCount,Summary")] Manga manga)
        {
            if (id != manga.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(manga);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MangaExists(manga.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(manga);
        }

        // GET: Mangas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var manga = await _context.Manga
                .Include(m => m.Mangaka)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (manga == null)
            {
                return NotFound();
            }

            return View(manga);
        }

        // POST: Mangas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var manga = await _context.Manga.FindAsync(id);
            _context.Manga.Remove(manga);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MangaExists(int id)
        {
            return _context.Manga.Any(e => e.ID == id);
        }
    }
}
