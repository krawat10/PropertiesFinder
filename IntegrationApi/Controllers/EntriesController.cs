using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatabaseConnection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

namespace IntegrationApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EntriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EntriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// GET: https://localhost:5001/api/Entities
        /// </summary>
        /// <returns> 200 - [{"id":1,"title":"Harry","author":"JK Rowling","isRented":false}]</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Entry>>> GetEntities()
        {
            return await _context.Entries.ToListAsync();
        }

        /// <summary>
        /// GET: https://localhost:5001/api/Entities/1
        /// </summary>
        /// <returns> 200 - [{"id":1,"title":"Harry","author":"JK Rowling","isRented":false}]</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Entry>> GetEntry(int id)
        {
            var Entry = await _context.Entries.FindAsync(id);

            if (Entry == null)
            {
                return NotFound();
            }

            return Entry;
        }

    
        /// <summary>
        /// PUT: https://localhost:5001/api/Entities/1
        /// {
        /// "Title": "Harry2",
        /// "Author": "JK Rowling"
        /// }
        /// </summary>
        /// <returns>204</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEntry(Guid id, Entry Entry)
        {
            Entry.Id = id;
            if (id != Entry.Id)
            {
                return BadRequest();
            }

            _context.Entry(Entry).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EntryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// POST: https://localhost:5001/api/Entities/
        /// {"title":"Harry","author":"JK Rowling"}
        /// </summary>
        /// <returns> 201 - {"id":1,"title":"Harry","author":"JK Rowling","isRented":false}</returns>
        [HttpPost]
        public async Task<ActionResult<Entry>> PostEntry(Entry Entry)
        {
            _context.Entries.Add(Entry);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEntry", new { id = Entry.Id }, Entry);
        }

        /// <summary>
        /// DELETE: https://localhost:5001/api/Entities/5
        /// </summary>
        /// <returns>
        /// 200 
        ///{
        /// "id": 1,
        /// "title": "Harry2",
        /// "author": "JK Rowling",
        /// "isRented": false
        /// }
        /// </returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<Entry>> DeleteEntry(int id)
        {
            var Entry = await _context.Entries.FindAsync(id);
            if (Entry == null)
            {
                return NotFound();
            }

            _context.Entries.Remove(Entry);
            await _context.SaveChangesAsync();

            return Entry;
        }

        private bool EntryExists(Guid id)
        {
            return _context.Entries.Any(e => e.Id == id);
        }
    }
}