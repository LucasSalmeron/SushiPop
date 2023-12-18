using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SushiPop.Models;

namespace SushiPOP_YA1A_2C2023_G2.Controllers
{
    public class DescuentosController : Controller
    {
        private readonly DBContext _context;
        private readonly CultureInfo _culture;

        public DescuentosController(DBContext context)
        {
            _context = context;
            _culture = CultureInfo.GetCultureInfo("es-ES");
        }

        // GET: Descuentos
        [Authorize(Roles = "EMPLEADO")]
        public async Task<IActionResult> Index()
        {
            var dBContext = _context.Descuento.Include(d => d.Producto);
            return View(await dBContext.ToListAsync());
        }

        // GET: Descuentos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Descuento == null)
            {
                //return NotFound();
                ModelState.AddModelError(string.Empty, "La conexión a la base de datos esta interrumpida. Comunicarse con el administrador.");
                return RedirectToAction(nameof(Index));
            }

            var descuento = await _context.Descuento
                .Include(d => d.Producto)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (descuento == null)
            {
                //return NotFound();
                ModelState.AddModelError(string.Empty, "El descuento no existe actualmente.");
                return RedirectToAction(nameof(Index));
            }

            return View(descuento);
        }

        // GET: Descuentos/Create
        [Authorize(Roles = "EMPLEADO")]
        public IActionResult Create()
        {

            ModelViewData(null);

            return View();
        }

        // POST: Descuentos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "EMPLEADO")]
        [HttpPost]
        [ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Dia,Porcentaje,DescuentoMaximo,Activo,ProductoId")] Descuento descuento)
        {


            var existeDescuento = await _context.Descuento.AnyAsync(x => x.Activo == true &&
                                                                       x.Dia == descuento.Dia
                                                                      );

            // no puede existir mas de un descuento por producto con las siguientes caracteristicas repetidas: dia y estado (activo) 
            if (existeDescuento)
            {
                ModelState.AddModelError("Dia", string.Format("Existe un descuento activo el día {0}.",
                    _culture.TextInfo.ToTitleCase(_culture.DateTimeFormat.DayNames[descuento.Dia])));
            }

            //var descuentoDia = await _context.Descuento.AnyAsync(x => x.Activo == true && x.Dia == descuento.Dia);
            //if (descuentoDia) return NotFound();

            if (ModelState.IsValid)
            {
                if (descuento.DescuentoMaximo > 1000)
                {

                    descuento.DescuentoMaximo = 1000;

                }

                if (descuento.Porcentaje > 50)
                {
                    descuento.Porcentaje = 50;
                }
                else if (descuento.Porcentaje < 1)
                {
                    descuento.Porcentaje = 1;
                }

                _context.Add(descuento);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

            }

            ModelViewData(descuento);

            return View(descuento);
        }

        // GET: Descuentos/Edit/5
        [Authorize(Roles = "EMPLEADO")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Descuento == null)
            {
                //return NotFound();
                ModelState.AddModelError(string.Empty, "La conexión a la base de datos esta interrumpida. Comunicarse con el administrador.");
                return RedirectToAction(nameof(Index));
            }

            var descuento = await _context.Descuento.FindAsync(id);
            if (descuento == null)
            {
                //return NotFound();
                ModelState.AddModelError(string.Empty, "El descuento no existe actualmente.");
                return RedirectToAction(nameof(Index));
            }

            ModelViewData(null);

            return View(descuento);
        }

        // POST: Descuentos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "EMPLEADO")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Dia,Porcentaje,DescuentoMaximo,Activo,ProductoId")] Descuento descuento)
        {
            if (id != descuento.Id)
            {
                //return NotFound();
                ModelState.AddModelError(string.Empty, "El descuento no existe actualmente.");
                return RedirectToAction(nameof(Index));
            }


            var existeDescuento = await _context.Descuento.AnyAsync(x => x.Activo == true &&
                                                                     x.Dia == descuento.Dia &&
                                                                     x.Id != descuento.Id
                                                                     );


            // no puede existir mas de un descuento por producto con las siguientes caracteristicas repetidas: dia y estado (activo)
            if (existeDescuento)
            {
                ModelState.AddModelError("Dia", string.Format("Existe un descuento activo el día {0}",
                    _culture.TextInfo.ToTitleCase(_culture.DateTimeFormat.DayNames[descuento.Dia])));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(descuento);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DescuentoExists(descuento.Id))
                    {
                        //return NotFound();
                        ModelState.AddModelError(string.Empty, "El descuento no existe actualmente.");
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            
            ModelViewData(descuento);

            return View(descuento);
        }

        // GET: Descuentos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Descuento == null)
            {
                //return NotFound();
                ModelState.AddModelError(string.Empty, "La conexión a la base de datos esta interrumpida. Comunicarse con el administrador.");
                return RedirectToAction(nameof(Index));
            }

            var descuento = await _context.Descuento
                .Include(d => d.Producto)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (descuento == null)
            {
                //return NotFound();
                ModelState.AddModelError(string.Empty, "El descuento no existe actualmente.");
                return RedirectToAction(nameof(Index));
            }

            return View(descuento);
        }

        // POST: Descuentos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Descuento == null)
            {
                return Problem("Entity set 'DBContext.Descuento'  is null.");
            }
            var descuento = await _context.Descuento.FindAsync(id);
            if (descuento != null)
            {
                _context.Descuento.Remove(descuento);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DescuentoExists(int id)
        {
            return (_context.Descuento?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        [NonAction]
        private void ModelViewData(Descuento? descuento)
        {
            if (descuento == null) descuento = new Descuento()
            {
                Dia = -1,
                ProductoId = -1
            };

            var diasDeSemana = new List<SelectListItem>();
            var productos = new List<SelectListItem>();

            foreach (var i in Enum.GetValues(typeof(DayOfWeek)))
            {
                var number = Convert.ToInt32(i);
                diasDeSemana.Add(new SelectListItem
                {
                    Text = _culture.TextInfo.ToTitleCase(_culture.DateTimeFormat.DayNames[number]),
                    Value = number.ToString(),
                    Selected = number == descuento.Dia
                });
            }

            productos.AddRange(_context.Set<Producto>().Select(x => new SelectListItem
            {
                Text = x.Nombre,
                Value = x.Id.ToString(),
                Selected = x.Id == descuento.ProductoId

            }));

            diasDeSemana.Insert(0, new SelectListItem("-Seleccione-", ""));
            productos.Insert(0, new SelectListItem("-Seleccione-", ""));

            ViewData["DiasDeSemana"] = diasDeSemana;
            ViewData["ProductoId"] = productos;
        }
    }
}
