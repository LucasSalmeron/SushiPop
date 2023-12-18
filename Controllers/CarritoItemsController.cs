using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SushiPop.Models;

namespace SushiPOP_YA1A_2C2023_G2.Controllers
{
    public class CarritoItemsController : Controller
    {
        private readonly DBContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CarritoItemsController(DBContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: CarritoItems
        [Authorize(Roles = "CLIENTE")]
        public async Task<IActionResult> Index()
        {
            var dBContext = _context.CarritoItem.Include(c => c.Carrito).Include(c => c.Producto);
            return View(await dBContext.ToListAsync());
        }

        // GET: CarritoItems/Details/5
        [Authorize(Roles = "CLIENTE")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.CarritoItem == null)
            {
                return NotFound();
            }

            var carritoItem = await _context.CarritoItem
                .Include(c => c.Carrito)
                .Include(c => c.Producto)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (carritoItem == null)
            {
                return NotFound();
            }

            return View(carritoItem);
        }

        // GET: CarritoItems/Create
        [Authorize(Roles = "CLIENTE")]
        public IActionResult Create()
        {
            ViewData["CarritoId"] = new SelectList(_context.Carrito, "Id", "Id");
            ViewData["ProductoId"] = new SelectList(_context.Set<Producto>(), "Id", "Id");
            return View();
        }

        // POST: CarritoItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "CLIENTE")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PrecioUnitarioConDescuento,Cantidad,CarritoId,ProductoId")] CarritoItem carritoItem)
        {
            if (ModelState.IsValid)
            {
                _context.Add(carritoItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CarritoId"] = new SelectList(_context.Carrito, "Id", "Id", carritoItem.CarritoId);
            ViewData["ProductoId"] = new SelectList(_context.Set<Producto>(), "Id", "Id", carritoItem.ProductoId);
            return View(carritoItem);
        }

        // GET: CarritoItems/Edit/5
        [Authorize(Roles = "CLIENTE")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.CarritoItem == null)
            {
                return NotFound();
            }

            var carritoItem = await _context.CarritoItem.FindAsync(id);
            if (carritoItem == null)
            {
                return NotFound();
            }
            ViewData["CarritoId"] = new SelectList(_context.Carrito, "Id", "Id", carritoItem.CarritoId);
            ViewData["ProductoId"] = new SelectList(_context.Set<Producto>(), "Id", "Id", carritoItem.ProductoId);
            return View(carritoItem);
        }

        // POST: CarritoItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "CLIENTE")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PrecioUnitarioConDescuento,Cantidad,CarritoId,ProductoId")] CarritoItem carritoItem)
        {
            if (id != carritoItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(carritoItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarritoItemExists(carritoItem.Id))
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
            ViewData["CarritoId"] = new SelectList(_context.Carrito, "Id", "Id", carritoItem.CarritoId);
            ViewData["ProductoId"] = new SelectList(_context.Set<Producto>(), "Id", "Id", carritoItem.ProductoId);
            return View(carritoItem);
        }

        // GET: CarritoItems/Delete/5
        [Authorize(Roles = "CLIENTE")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.CarritoItem == null)
            {
                return NotFound();
            }

            var carritoItem = await _context.CarritoItem
                .Include(c => c.Carrito)
                .Include(c => c.Producto)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (carritoItem == null)
            {
                return NotFound();
            }

            return View(carritoItem);
        }

        // POST: CarritoItems/Delete/5
        [Authorize(Roles = "CLIENTE")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, bool returnUrl = false)
        {
            if (_context.CarritoItem == null)
            {
                return Problem("Entity set 'DBContext.CarritoItem'  is null.");
            }

            var user = await _userManager.GetUserAsync(User);
            var cliente = await _context.Cliente.Where(x => x.Email.ToUpper() == user.NormalizedEmail).FirstOrDefaultAsync();

            // Carrito item perteneciente al cliente
            var carritoItem = await _context.CarritoItem.Include(x => x.Carrito).FirstOrDefaultAsync(x => x.Id == id && x.Carrito.ClienteId == cliente.Id);

            if (carritoItem != null)
            {
                var producto = await _context.Producto.FirstOrDefaultAsync(x => x.Id == carritoItem.ProductoId);

                producto.Stock += carritoItem.Cantidad;

                _context.CarritoItem.Remove(carritoItem);
                _context.Update(producto);

                await _context.SaveChangesAsync();

            }
            else if (returnUrl && carritoItem == null)
            {
                TempData["SweetAlert"] = "error|El item no existe dentro de la lista de carrito.";
                
                return RedirectToAction("ConfirmarPedido", "Pedidos");
            }

            var carrito = await _context.Carrito.FirstOrDefaultAsync(x => x.ClienteId == cliente.Id && x.Id == carritoItem.CarritoId);

            var carritoAny = await _context.Carrito
                                        .Include(x => x.CarritosItems)
                                        .FirstOrDefaultAsync(x => 
                                                    x.Id == carritoItem.CarritoId && 
                                                    x.ClienteId == cliente.Id && 
                                                    x.CarritosItems.Count == 0
                                                );

            if (carritoAny != null)
            {
                _context.Carrito.Remove(carrito);

                await _context.SaveChangesAsync();
            }

            if (returnUrl)
            {
                TempData["SweetAlert"] = "success|El producto fue eliminado de la lista de carrito.";
                return RedirectToAction("ConfirmarPedido", "Pedidos");
                
            }

            return RedirectToAction(nameof(Index));
        }

        private bool CarritoItemExists(int id)
        {
          return (_context.CarritoItem?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
