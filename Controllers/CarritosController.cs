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
    public class CarritosController : Controller
    {
        private readonly DBContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CarritosController(DBContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Carritos
        [Authorize(Roles = "CLIENTE")]
        public async Task<IActionResult> Index()
        {
            var dBContext = _context.Carrito.Include(c => c.Cliente);
            return View(await dBContext.ToListAsync());
        }

        // GET: Carritos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Carrito == null)
            {
                return NotFound();
            }

            var carrito = await _context.Carrito
                .Include(c => c.Cliente)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (carrito == null)
            {
                return NotFound();
            }

            return View(carrito);
        }

        // GET: Carritos/Create
        [Authorize(Roles = "CLIENTE")]
        public IActionResult Create()
        {
            ViewData["ClienteId"] = new SelectList(_context.Set<Cliente>(), "Id", "Id");
            return View();
        }


        [Authorize(Roles = "CLIENTE")]
        private async Task<int> Create([Bind("Id,Procesado,Cancelado,ClienteId")] Carrito carrito)
        {
            int resultado = -1;
            if (ModelState.IsValid)
            {
                _context.Add(carrito);
                await _context.SaveChangesAsync();

                resultado = carrito.Id;
            }
            return resultado;
        }

        // GET: Carritos/Edit/5
        [Authorize(Roles = "CLIENTE")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Carrito == null)
            {
                return NotFound();
            }

            var carrito = await _context.Carrito.FindAsync(id);
            if (carrito == null)
            {
                return NotFound();
            }
            ViewData["ClienteId"] = new SelectList(_context.Set<Cliente>(), "Id", "Id", carrito.ClienteId);
            return View(carrito);
        }

        // POST: Carritos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "CLIENTE")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Procesado,Cancelado,ClienteId")] Carrito carrito)
        {
            if (id != carrito.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(carrito);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarritoExists(carrito.Id))
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
            ViewData["ClienteId"] = new SelectList(_context.Set<Cliente>(), "Id", "Id", carrito.ClienteId);
            return View(carrito);
        }

        // GET: Carritos/Delete/5
        [Authorize(Roles = "CLIENTE")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Carrito == null)
            {
                return NotFound();
            }

            var carrito = await _context.Carrito
                .Include(c => c.Cliente)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (carrito == null)
            {
                return NotFound();
            }

            return View(carrito);
        }

        // POST: Carritos/Delete/5
        [Authorize(Roles = "CLIENTE")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, bool returnUrl = false)
        {
            if (_context.Carrito == null)
            {
                return Problem("Entity set 'DBContext.Carrito'  is null.");
            }

            var user = await _userManager.GetUserAsync(User);
            var cliente = await _context.Cliente.Where(x => x.Email.ToUpper() == user.NormalizedEmail).FirstOrDefaultAsync();

            // el carrito debe pertenecer al cliente
            var carrito = await _context.Carrito.Include(x => x.CarritosItems).FirstOrDefaultAsync(x => x.Id == id && x.ClienteId == cliente.Id);

            if (carrito != null)
            {
                if (!carrito.Procesado)
                {
                    var producto = new Producto();

                    foreach (var prod in carrito.CarritosItems)
                    {
                        producto = await _context.Producto.FirstOrDefaultAsync(x => x.Id == prod.ProductoId);

                        if (producto != null)
                        {
                            producto.Stock += prod.Cantidad;

                            _context.Update(producto);
                        }

                    }

                    _context.Carrito.Remove(carrito);

                    await _context.SaveChangesAsync();

                    TempData["SweetAlert"] = "success|Elimino el carrito de forma exitosa.";

                    return RedirectToAction("ConfirmarPedido", "Pedidos");
                }
                else if (returnUrl)
                {
                    TempData["SweetAlert"] = "error|No puedes eliminar un carrito procesado.";

                    return RedirectToAction("ConfirmarPedido", "Pedidos");
                }

            }

            ModelState.AddModelError(string.Empty, "Ocurrio un error al borrar el carrito. Comuniquese con el administrador.");

            return RedirectToAction(nameof(Index));
        }

        private bool CarritoExists(int id)
        {
            return (_context.Carrito?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        [Authorize(Roles = "CLIENTE")]
        public async Task<IActionResult> AgregarACarrito(int idProducto, int cantidad)
        {
            ViewBag.agregado = false;
            try
            {
                if (idProducto == null)
                {
                    ModelState.AddModelError(string.Empty, "No se envio el id del producto, reintentar.");
                    return RedirectToAction(nameof(Index), "Home");
                }

                var user = await _userManager.GetUserAsync(User);
                var cliente = await _context.Cliente.Where(x => x.Email.ToUpper() == user.NormalizedEmail).FirstOrDefaultAsync();

                // solo puede haber un pedido pendiente
                var pedido = await _context.Pedido.Include(x => x.Carrito)
                        .Where(x => x.Carrito.ClienteId == cliente.Id && x.Estado == 1) // Estado 1 pendiente
                        .FirstOrDefaultAsync();

                if (pedido != null)
                {
                    TempData["SweetAlert"] = "error|Solo puede haber un pedido pendiente.";

                    return RedirectToAction(nameof(Details), "Productos", new { id = idProducto });
                    
                }

                var cantPedidoDia = await _context.Pedido.Include(x => x.Carrito)
                        .Where(x =>
                            x.Carrito.ClienteId == cliente.Id &&
                            x.FechaCompra.Date == DateTime.Now.Date &&
                            x.Carrito.Cancelado == false)
                        .ToListAsync();

                if (cantPedidoDia.Count >= 3)
                {
                    TempData["SweetAlert"] = "error|Solo puede hacer 3 pedidos por día.";

                    return RedirectToAction(nameof(Details), "Productos", new { id = idProducto }); 
                }

                // Stock y cantidad solicitada
                var producto = await _context.Producto.FindAsync(idProducto);

                if (producto == null)
                {
                    TempData["SweetAlert"] = "error|El producto no existe actualmente";
                    
                    return RedirectToAction(nameof(Details), "Productos", new { id = idProducto }); 
                }

                if (producto.Stock < cantidad)
                {
                    TempData["SweetAlert"] = "error|El producto no cuenta con el stock suficiente para la compra.";
                    
                    return RedirectToAction(nameof(Details), "Productos", new { id = idProducto }); 
                }


                var carrito = await _context.Carrito.Include(x => x.CarritosItems).FirstOrDefaultAsync(x => x.ClienteId == cliente.Id
                    && x.Procesado == false && x.Cancelado == false);
                // suponiendo que si no esta procesado ni cancelado es que esta pendiente


                if (carrito == null)
                {
                    var carritoId = await Create(new Carrito()
                    {
                        Id = 0,
                        Procesado = false,
                        Cancelado = false,
                        ClienteId = cliente.Id,
                        CarritosItems = new List<CarritoItem>()
                    });

                    if (carritoId == -1)
                    {
                        TempData["SweetAlert"] = "error|No fue posible crerar el carrito. Comuniquese con el administrador.";
                        
                        return RedirectToAction(nameof(Details), "Productos", new { id = idProducto });
                    }

                    carrito = await _context.Carrito.FirstOrDefaultAsync(x => x.Id == carritoId);

                }


                var item = carrito.CarritosItems.Where(i => i.ProductoId == idProducto).FirstOrDefault();

                if (item == null)
                {
                    item = new CarritoItem();
                    item.CarritoId = carrito.Id;
                    item.ProductoId = idProducto;
                    item.PrecioUnitarioConDescuento = producto.Precio;
                    item.Cantidad = cantidad;

                    var intDayWeek = (int)DateTime.Now.DayOfWeek;

                    var promocion = await _context.Descuento
                                                .Include(d => d.Producto)
                                                .Where(d =>
                                                    d.ProductoId == producto.Id &&
                                                    d.Activo == true &&
                                                    d.Dia == intDayWeek)
                                                .FirstOrDefaultAsync();

                    var precioFinalProducto = producto.Precio;

                    if (promocion != null)
                    {
                        var precioDescuento = (producto.Precio * promocion.Porcentaje) / 100;

                        if (precioDescuento > promocion.DescuentoMaximo)
                        {
                            precioDescuento = promocion.DescuentoMaximo;
                        }

                        precioFinalProducto = producto.Precio - precioDescuento;

                        item.PrecioUnitarioConDescuento = precioFinalProducto;
                    }

                    _context.Add(item);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    item.Cantidad += cantidad;
                    _context.Update(item);
                    await _context.SaveChangesAsync();
                }

                producto.Stock -= cantidad;

                carrito.CarritosItems.Add(item);
                _context.Update(carrito);

                _context.Update(producto);
                await _context.SaveChangesAsync();


                ViewBag.Agregado = true;

                TempData["SweetAlert"] = "success|El producto fue agregado de la lista de carrito.";

                return RedirectToAction(nameof(Details), "Productos", new { id = idProducto });


            }
            catch (Exception)
            {

                ViewBag.agregado = false;
                return View();
            }


        }

    }
}
