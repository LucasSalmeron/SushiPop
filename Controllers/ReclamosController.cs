using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SushiPop.Models;

namespace SushiPOP_YA1A_2C2023_G2.Controllers
{
    public class ReclamosController : Controller
    {
        private readonly DBContext _context;
        private readonly UserManager<IdentityUser> _userManager;


        public ReclamosController(DBContext context, UserManager<IdentityUser> userManager )
        {
            _context = context;
            _userManager = userManager;
        }


        // GET: Reclamos
        [Authorize(Roles = "EMPLEADO")]
        public async Task<IActionResult> Index()
        {
            var dBContext = _context.Reclamo.Include(r => r.Pedido);
            return View(await dBContext.ToListAsync());
        }

        // GET: Reclamos/Details/5
        [Authorize(Roles = "EMPLEADO")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Reclamo == null)
            {
                return NotFound();
            }

            var reclamo = await _context.Reclamo
                .Include(r => r.Pedido)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reclamo == null)
            {
                return NotFound();
            }

            return View(reclamo);
        }


        // GET: Reclamos/Create
        public async Task<IActionResult> Create()
        {
            Reclamo reclamo = new Reclamo();

            if (User.IsInRole("Cliente"))
              
            {
                var usuario = await _userManager.GetUserAsync(User);
                var cliente = await _context.Cliente.Where(c => c.Email.ToUpper() == usuario.NormalizedEmail).FirstOrDefaultAsync();

                if (cliente != null)
                {
                    reclamo.NombreCompleto = cliente.Nombre + " " + cliente.Apellido;
                    reclamo.Email = cliente.Email;
                    reclamo.Telefono = cliente.Telefono;
                }
            }
            ViewData["PedidoId"] = new SelectList(_context.Pedido, "Id", "Id");
            return View(reclamo);
        }

        // POST: Reclamos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NombreCompleto,Email,Telefono,DetalleReclamo,PedidoId")] Reclamo reclamo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(reclamo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PedidoId"] = new SelectList(_context.Pedido, "Id", "Id", reclamo.PedidoId);
            return View(reclamo);
        }

        // GET: Reclamos/Edit/5
        //NO SE USA
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Reclamo == null)
            {
                return NotFound();
            }

            var reclamo = await _context.Reclamo.FindAsync(id);
            if (reclamo == null)
            {
                return NotFound();
            }
            ViewData["PedidoId"] = new SelectList(_context.Pedido, "Id", "Id", reclamo.PedidoId);
            return View(reclamo);
        }

        // POST: Reclamos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NombreCompleto,Email,Telefono,DetalleReclamo,PedidoId")] Reclamo reclamo)
        {
            if (id != reclamo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reclamo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReclamoExists(reclamo.Id))
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
            ViewData["PedidoId"] = new SelectList(_context.Pedido, "Id", "Id", reclamo.PedidoId);
            return View(reclamo);
        }

        // GET: Reclamos/Delete/5
        //NO SE USA
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Reclamo == null)
            {
                return NotFound();
            }

            var reclamo = await _context.Reclamo
                .Include(r => r.Pedido)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reclamo == null)
            {
                return NotFound();
            }

            return View(reclamo);
        }

        // POST: Reclamos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Reclamo == null)
            {
                return Problem("Entity set 'DBContext.Reclamo'  is null.");
            }
            var reclamo = await _context.Reclamo.FindAsync(id);
            if (reclamo != null)
            {
                _context.Reclamo.Remove(reclamo);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReclamoExists(int id)
        {
          return (_context.Reclamo?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
