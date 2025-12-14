using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyCodeManager.Data;
using MyCodeManager.Models;

namespace MyCodeManager.Controllers
{
    public class SnippetsController : Controller
    {
        private readonly MyCodeManagerContext _context;

        public SnippetsController(MyCodeManagerContext context)
        {
            _context = context;
        }

        // هذه الدالة الجديدة تقبل كلمة بحث
        // 1. دالة العرض المحدثة (تقبل بحث، لغة، ومفضلة)
        public async Task<IActionResult> Index(string searchString, string lang, bool showFavorites)
        {
            var snippets = from s in _context.Snippet
                           select s;

            // فلتر المفضلة
            if (showFavorites)
            {
                snippets = snippets.Where(s => s.IsFavorite == true);
                ViewData["CurrentFilter"] = "Favorites"; // لتمييز العنوان
            }

            // فلتر اللغة (من القائمة الجانبية)
            if (!string.IsNullOrEmpty(lang))
            {
                snippets = snippets.Where(s => s.Language == lang);
                ViewData["CurrentFilter"] = lang;
            }

            // البحث العام
            if (!string.IsNullOrEmpty(searchString))
            {
                snippets = snippets.Where(s => s.Title.Contains(searchString) || s.Code.Contains(searchString));
                ViewData["CurrentFilter"] = $"Search: {searchString}";
            }

            return View(await snippets.ToListAsync());
        }

        // 2. دالة جديدة: تبديل حالة المفضلة عند الضغط على النجمة
        public async Task<IActionResult> ToggleFavorite(int id)
        {
            var snippet = await _context.Snippet.FindAsync(id);
            if (snippet != null)
            {
                // عكس الحالة (إذا كانت true تصبح false والعكس)
                snippet.IsFavorite = !snippet.IsFavorite;
                await _context.SaveChangesAsync();
            }
            // العودة لنفس الصفحة
            return RedirectToAction(nameof(Index));
        }
        // GET: Snippets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var snippet = await _context.Snippet
                .FirstOrDefaultAsync(m => m.Id == id);
            if (snippet == null)
            {
                return NotFound();
            }

            return View(snippet);
        }

        // GET: Snippets/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Snippets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Language,Code")] Snippet snippet)
        {
            if (ModelState.IsValid)
            {
                _context.Add(snippet);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(snippet);
        }

        // GET: Snippets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var snippet = await _context.Snippet.FindAsync(id);
            if (snippet == null)
            {
                return NotFound();
            }
            return View(snippet);
        }

        // POST: Snippets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Language,Code")] Snippet snippet)
        {
            if (id != snippet.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(snippet);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SnippetExists(snippet.Id))
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
            return View(snippet);
        }

        // GET: Snippets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var snippet = await _context.Snippet
                .FirstOrDefaultAsync(m => m.Id == id);
            if (snippet == null)
            {
                return NotFound();
            }

            return View(snippet);
        }

        // POST: Snippets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var snippet = await _context.Snippet.FindAsync(id);
            if (snippet != null)
            {
                _context.Snippet.Remove(snippet);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SnippetExists(int id)
        {
            return _context.Snippet.Any(e => e.Id == id);
        }
    }
}
