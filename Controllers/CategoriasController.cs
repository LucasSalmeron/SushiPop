using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SushiPop.Models;

namespace SushiPOP_YA1A_2C2023_G2.Controllers
{
    public class CategoriasController : Controller
    {
        private readonly DBContext _context;

        public CategoriasController(DBContext context)
        {
            _context = context;
        }

        // GET: Categorias
        public async Task<IActionResult> Index()
        {
              var categoriasConProductos = await _context.Categoria
                                                          .Where(c => c.Productos.Any())
                                                          .ToListAsync();
              return _context.Categoria != null ? 
                          View(categoriasConProductos) :
                          Problem("Entity set 'DBContext.Categoria'  is null.");
        }

        // GET: Categorias/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Categoria == null)
            {
                //return NotFound();
                ModelState.AddModelError(string.Empty, "La conexión a la base de datos está interrumpida. Comuniquese con el administrador.");
                return RedirectToAction(nameof(Index));
            }

            var categoria = await _context.Categoria
                .FirstOrDefaultAsync(m => m.Id == id);
            if (categoria == null)
            {
                //return NotFound();
                ModelState.AddModelError(string.Empty, "La categoría no existe actualmente.");
                return RedirectToAction(nameof(Index));
            }

            return View(categoria);
        }

        // GET: Categorias/Create
        [Authorize(Roles = "EMPLEADO")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categorias/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Descripcion")] Categoria categoria)
        {
            if (ModelState.IsValid)
            {

                if (await _context.Categoria.AnyAsync(c => c.Nombre == categoria.Nombre))
                {
                    ModelState.AddModelError("Nombre", "Ya existe otra categoria con este nombre");
                    return View(categoria);
                }

                _context.Add(categoria);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(categoria);
        }

        // GET: Categorias/Edit/5
        [Authorize(Roles = "EMPLEADO")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Categoria == null)
            {
                //return NotFound();
                ModelState.AddModelError(string.Empty, "La conexión a la base de datos está interrumpida. Comuniquese con el administrador.");
                return RedirectToAction(nameof(Index));
            }

            var categoria = await _context.Categoria.FindAsync(id);
            if (categoria == null)
            {
                //return NotFound();
                ModelState.AddModelError(string.Empty, "La categoría no existe actualmente.");
                return RedirectToAction(nameof(Index));
            }
            return View(categoria);
        }

        // POST: Categorias/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "EMPLEADO")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Descripcion")] Categoria categoria)
        {
            if (id != categoria.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(categoria);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoriaExists(categoria.Id))
                    {
                        //return NotFound();
                        ModelState.AddModelError(string.Empty, "La categoría no existe actualmente.");
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(categoria);
        }

        // GET: Categorias/Delete/5
        [Authorize(Roles = "EMPLEADO")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Categoria == null)
            {
                //return NotFound();
                ModelState.AddModelError(string.Empty, "La conexión a la base de datos está interrumpida. Comuniquese con el administrador.");
                return RedirectToAction(nameof(Index));
            }

            var categoria = await _context.Categoria
                .FirstOrDefaultAsync(m => m.Id == id);
            if (categoria == null)
            {
                //return NotFound();
                ModelState.AddModelError(string.Empty, "La categoría no existe actualmente.");
                return RedirectToAction(nameof(Index));
            }

            var categoriasConProductos = categoria.Productos.Any();

            ViewData["CategoriaConProductos"] = categoriasConProductos;

            return View(categoria);
        }

        // POST: Categorias/Delete/5
        [Authorize(Roles = "EMPLEADO")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Categoria == null)
            {
                return Problem("Entity set 'DBContext.Categoria'  is null.");
            }

            var categoriasConProductos = await _context.Producto.AnyAsync(p => p.CategoriaId == id);
            
            if (categoriasConProductos)
            {
                //return NotFound();
                ModelState.AddModelError(string.Empty, "La categoría cuenta con productos relacionados. No se puede eliminar.");
                return RedirectToAction(nameof(Delete), new { id = id });
            }

            var categoria = await _context.Categoria.FindAsync(id);
            if (categoria == null)
            {
                //return NotFound();
                ModelState.AddModelError(string.Empty, "La categoría no existe actualmente.");
                return RedirectToAction(nameof(Index));
            }
            
            
            _context.Categoria.Remove(categoria);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoriaExists(int id)
        {
          return (_context.Categoria?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
