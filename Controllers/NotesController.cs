using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; 
using NotinLite.Data;
using NotinLite.Models;
using System.Linq; 
using System.Security.Claims; 
using System.Threading.Tasks; 

namespace NotinLite.Controllers
{
    [Authorize]
    public class NotesController : Controller
    {
        private readonly ApplicationDbContext _context; 

        public NotesController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdString))
            {
                return Unauthorized("User ID not found in claims.");
            }

            if (!int.TryParse(userIdString, out var userId))
            {
                return BadRequest("Invalid User ID format.");
            }

        
            var userNotes = await _context.Notes
                                          .Where(note => note.UserId == userId)
                                          .OrderByDescending(note => note.ModifiedDate)
                                          .ToListAsync(); 

            return View(userNotes);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound(); 
            }

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var userId))
            {
                return Unauthorized("User ID not found or invalid.");
            }

            var note = await _context.Notes
                                     .FirstOrDefaultAsync(n => n.NoteId == id && n.UserId == userId);

            if (note == null)
            {
                return NotFound();
            }

            return View(note); 
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var userId))
            {
                return Unauthorized("User ID not found or invalid.");
            }

            var note = await _context.Notes
                                     .FirstOrDefaultAsync(m => m.NoteId == id && m.UserId == userId);

            if (note == null)
            {
                return NotFound();
            }

            return View(note); 
        }

       
        [HttpPost, ActionName("Delete")] 
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) 
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var userId))
            {
                return Unauthorized("User ID not found or invalid during delete confirmation.");
            }

            var note = await _context.Notes
                                     .FirstOrDefaultAsync(n => n.NoteId == id && n.UserId == userId);

            if (note == null)
            {
                
                return RedirectToAction(nameof(Index)); 
            }

            try
            {
                _context.Notes.Remove(note); 
                await _context.SaveChangesAsync(); 
                TempData["SuccessMessage"] = "Note deleted successfully!";
                return RedirectToAction(nameof(Index)); 
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = "Unable to delete note. Please try again.";
                return RedirectToAction(nameof(Delete), new { id = id });
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("NoteId,Title,Content")] Note note) 
        {
            if (id != note.NoteId)
            {
                return NotFound("Mismatched note ID.");
            }

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var userId))
            {
                return Unauthorized("User ID not found or invalid.");
            }

         
            var noteToUpdate = await _context.Notes
                                             .FirstOrDefaultAsync(n => n.NoteId == id && n.UserId == userId);

            if (noteToUpdate == null)
            {
                return NotFound("Note not found or access denied.");
            }

            noteToUpdate.Title = note.Title;
            noteToUpdate.Content = note.Content;
            noteToUpdate.ModifiedDate = DateTime.UtcNow; 

            ModelState.Remove(nameof(note.UserId));
            ModelState.Remove(nameof(note.CreatedDate));
            ModelState.Remove(nameof(note.ModifiedDate));
            ModelState.Remove(nameof(note.User));


            if (ModelState.IsValid) 
            {
                try
                {
                    _context.Update(noteToUpdate); 
                    await _context.SaveChangesAsync(); 
                    TempData["SuccessMessage"] = "Note updated successfully!";
                    return RedirectToAction(nameof(Index)); 
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NoteExists(note.NoteId, userId)) 
                    {
                        return NotFound();
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "The note was modified by another user. Please try again.");
                    }
                }
            }

          
            return View(noteToUpdate);
        }

        private bool NoteExists(int id, int userId)
        {
            return _context.Notes.Any(e => e.NoteId == id && e.UserId == userId);
        }


        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var userId))
            {
                return Unauthorized("User ID not found or invalid.");
            }

           
            var note = await _context.Notes
                                     .FirstOrDefaultAsync(m => m.NoteId == id && m.UserId == userId);

            if (note == null)
            {
                return NotFound();
            }

            return View(note); 
        }

        [HttpGet] 
        public IActionResult Create()
        {
            
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken] 
        public async Task<IActionResult> Create([Bind("Title,Content")] Note note) 
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var userId))
            {
                ModelState.AddModelError(string.Empty, "Unable to identify user.");
                return View(note); 
            }

            note.UserId = userId; 
            note.CreatedDate = DateTime.UtcNow;
            note.ModifiedDate = DateTime.UtcNow;

            ModelState.Remove(nameof(note.UserId));
            ModelState.Remove(nameof(note.CreatedDate));
            ModelState.Remove(nameof(note.ModifiedDate));
            ModelState.Remove(nameof(note.User));


            if (ModelState.IsValid)
            {
                _context.Add(note);

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Note created successfully!"; 
                return RedirectToAction(nameof(Index)); 
            }
            return View(note);
        }
    }
}