using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper.Execution;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using SushiPop.Models;
using SushiPOP_YA1A_2C2023_G2.DTO;

namespace SushiPOP_YA1A_2C2023_G2.Controllers
{
    public class PedidosController : Controller
    {
        private readonly DBContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public PedidosController(DBContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Pedidos
        public async Task<IActionResult> Index()
        {
            if (_context.Pedido == null)
            {
                //return NotFound();
                ModelState.AddModelError(string.Empty, "La conexión a la base de datos está interrumpida. Comuniquese con el administrador.");
                return RedirectToAction(nameof(Index));
            }

            var lPedidos = new List<Pedido>();

            // el cliente unicamente ve los pedidos que son suyos
            if (User.IsInRole("CLIENTE"))
            {
                var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

                lPedidos = await _context.Pedido
                    .Include(p => p.Carrito).ThenInclude(x => x.CarritosItems).ThenInclude(x => x.Producto)
                    .Include(p => p.Carrito).ThenInclude(x => x.Cliente)
                    .Where(x => x.Carrito.Cliente.Email == email)
                    .ToListAsync();
            }
            else if (User.IsInRole("EMPLEADO"))
            {
                lPedidos = await _context.Pedido.Include(p => p.Carrito).ThenInclude(x => x.CarritosItems).ThenInclude(x => x.Producto).ToListAsync();
            }

            return View(lPedidos);
        }

        // GET: Pedidos/Details/5
        [Authorize(Roles = "CLIENTE, EMPLEADO")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Pedido == null)
            {
                //return NotFound();
                ModelState.AddModelError(string.Empty, "La conexión a la base de datos está interrumpida. Comuniquese con el administrador.");
                return RedirectToAction(nameof(Index));
            }

            var pedido = new Pedido();

            // el cliente unicamente ve los pedidos que son suyos
            if (User.IsInRole("CLIENTE"))
            {
                var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

                pedido = await _context.Pedido
                    .Include(p => p.Carrito).ThenInclude(x => x.Cliente)
                    .Include(p => p.Carrito).ThenInclude(x => x.CarritosItems).ThenInclude(x => x.Producto)
                    .FirstOrDefaultAsync(x => x.Carrito.Cliente.Email == email && x.Id == id);
            }
            else if (User.IsInRole("EMPLEADO"))
            {
                pedido = await _context.Pedido
                    .Include(p => p.Carrito).ThenInclude(x => x.CarritosItems).ThenInclude(x => x.Producto)
                    .FirstOrDefaultAsync(m => m.Id == id);

            }

            if (pedido == null)
            {
                //return NotFound();
                ModelState.AddModelError(string.Empty, "El pedido no existe actualmente.");
                return RedirectToAction(nameof(Index));
            }

            return View(pedido);
        }

        // GET: Pedidos/Create
        [Authorize(Roles = "CLIENTE")]
        public async Task<IActionResult> Create()
        {
            await ModelView(null);

            return View();
        }

        // POST: Pedidos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "CLIENTE")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NroPedido,FechaCompra,Subtotal,GastoEnvio,Total,Estado,CarritoId")] Pedido pedido)
        {
            if (ModelState.IsValid)
            {
                _context.Add(pedido);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            await ModelView(pedido);

            return View(pedido);
        }

        // GET: Pedidos/Edit/5
        [Authorize(Roles = "EMPLEADO")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Pedido == null)
            {
                //return NotFound();
                ModelState.AddModelError(string.Empty, "La conexión a la base de datos está interrumpida. Comuniquese con el administrador.");
                return RedirectToAction(nameof(Index));
            }

            var pedido = await _context.Pedido.FindAsync(id);

            if (pedido == null)
            {
                //return NotFound();
                ModelState.AddModelError(string.Empty, "El pedido no existe actualmente.");
                return RedirectToAction(nameof(Index));
            }

            ModelViewEditar(pedido);

            return View(pedido);
        }

        // POST: Pedidos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "EMPLEADO")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NroPedido,FechaCompra,Subtotal,GastoEnvio,Total,Estado,CarritoId")] Pedido pedido)
        {
            if (id != pedido.Id)
            {
                //return NotFound();
                ModelState.AddModelError(string.Empty, "El pedido no existe actualmente.");
                return RedirectToAction(nameof(Index));
            }

            if (pedido.Estado == (int)EstadoPedidoEnum.SinConfirmar)
            {
                ModelState.AddModelError("Estado", "El pedido no puede pasar a estado sin confirmar.");
                ModelViewEditar(pedido);

                return View(pedido);
            }
            if (pedido.Estado == (int)EstadoPedidoEnum.Cancelado)
            {
                ModelState.AddModelError("Estado", "Solo el cliente puede cancelar el pedido.");
                ModelViewEditar(pedido);

                return View(pedido);
            }

            try
            {
                var pedidoValido = await _context.Pedido.Include(x => x.Carrito).Include(x => x.Reclamo).FirstOrDefaultAsync(x => x.Id == pedido.Id);

                pedidoValido.Estado = pedido.Estado;

                _context.Update(pedidoValido);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PedidoExists(pedido.Id))
                {
                    //return NotFound();
                    ModelState.AddModelError(string.Empty, "El pedido no existe actualmente.");
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    throw;
                }
            }
        }

        // GET: Pedidos/Delete/5
        [Authorize(Roles = "CLIENTE")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Pedido == null)
            {
                //return NotFound();
                ModelState.AddModelError(string.Empty, "La conexión a la base de datos está interrumpida. Comuniquese con el administrador.");
                return RedirectToAction(nameof(Index));
            }


            var user = await _userManager.GetUserAsync(User);
            var cliente = await _context.Cliente.Where(x => x.Email.ToUpper() == user.NormalizedEmail).FirstOrDefaultAsync();

            var pedido = await _context.Pedido
                    .Include(p => p.Carrito).ThenInclude(x => x.CarritosItems).ThenInclude(x => x.Producto)
                    .FirstOrDefaultAsync(m =>
                                m.Id == id &&
                                m.Carrito.ClienteId == cliente.Id
                            );

            if (pedido == null)
            {
                //return NotFound();
                ModelState.AddModelError(string.Empty, "El pedido no existe actualmente.");
                return RedirectToAction(nameof(Index));
            }

            return View(pedido);
        }

        // POST: Pedidos/Delete/5
        [Authorize(Roles = "CLIENTE")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Pedido == null)
            {
                return Problem("Entity set 'DBContext.Pedido'  is null.");
            }

            var user = await _userManager.GetUserAsync(User);
            var cliente = await _context.Cliente.Where(x => x.Email.ToUpper() == user.NormalizedEmail).FirstOrDefaultAsync();

            var pedido = await _context.Pedido
                                    .Include(x => x.Carrito)
                                    .ThenInclude(p => p.CarritosItems)
                                    .FirstOrDefaultAsync(x =>
                                            x.Id == id &&
                                            x.Carrito.ClienteId == cliente.Id
                                        );
            if (pedido != null)
            {

                if (pedido.Estado != (int)EstadoPedidoEnum.SinConfirmar)
                {
                    ModelState.AddModelError(string.Empty, "Solo puedes cancelar un pedido que este en estado sin confirmar.");

                }
                if (ModelState.IsValid)
                {
                    pedido.Estado = (int)EstadoPedidoEnum.Cancelado;
                    _context.Update(pedido);

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }

                return View("Delete", pedido);
            }

            return RedirectToAction(nameof(Index));
        }

        private bool PedidoExists(int id)
        {
            return (_context.Pedido?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        [Authorize(Roles = "CLIENTE")]
        public async Task<IActionResult> ConfirmarPedido()
        {
            ViewBag.confirmado = false;
            var user = await _userManager.GetUserAsync(User);
            var cliente = await _context.Cliente.Where(x => x.Email.ToUpper() == user.NormalizedEmail).FirstOrDefaultAsync();


            var pedido = new Pedido();

            var carrito = await _context.Carrito
                            .Include(x => x.CarritosItems)
                            .ThenInclude(x => x.Producto)
                            .FirstOrDefaultAsync(x =>
                                    x.ClienteId == cliente.Id &&
                                    x.Procesado == false &&
                                    x.Cancelado == false
                                );

            if (carrito == null)
            {
                ViewBag.HayCarrito = false;
                return View(pedido);
            }
            else
            {
                ViewBag.HayCarrito = true;
            }

            var cantPedidos = _context.Pedido.Where(x => x.Carrito.ClienteId == carrito.ClienteId
                        && x.FechaCompra.Date > DateTime.Now.Date.AddDays(-30))
                        .Count();


            if (cantPedidos < 10)
            {
                pedido.GastoEnvio = 50;
                //aca va la api de temperatura

                //d47debca079f43d5b3b210622232111

            }
            else
            {
                pedido.GastoEnvio = 0;
            }


            if (carrito != null)
            {
                pedido.Subtotal = 0;
                foreach (var item in carrito.CarritosItems)
                {
                    pedido.Subtotal += item.Cantidad * item.PrecioUnitarioConDescuento;
                }
            }
            else
            {
                return NotFound();
            }

            pedido.Total = pedido.Subtotal + pedido.GastoEnvio;
            pedido.Estado = 1;
            pedido.Carrito = carrito;
            pedido.CarritoId = carrito.Id;
            pedido.FechaCompra = DateTime.Now;

            try
            {
                var nroPedidoMasAltoEnBase = await _context.Pedido.MaxAsync(x => x.NroPedido);
                if (nroPedidoMasAltoEnBase == 0)
                {
                    pedido.NroPedido = 30000;
                }
                else
                {
                    pedido.NroPedido = nroPedidoMasAltoEnBase + 5;
                }
            }
            catch (Exception e)
            {
                pedido.NroPedido = 30000;
            }



            return View(pedido);
        }

        [Authorize(Roles = "CLIENTE")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarPedido([Bind("Id,NroPedido,FechaCompra,Subtotal,GastoEnvio,Total,Estado,CarritoId")] Pedido pedido)
        {
            var user = await _userManager.GetUserAsync(User);
            var cliente = await _context.Cliente.Where(x => x.Email.ToUpper() == user.NormalizedEmail).FirstOrDefaultAsync();

            var carrito = await _context.Carrito.Include(x => x.CarritosItems).FirstOrDefaultAsync(x => x.Id == pedido.CarritoId);

            try
            {

                // solo puede haber max 3 pedidos en un día
                var cantPedidoDia = await _context.Pedido.Include(x => x.Carrito)
                        .Where(x =>
                            x.Carrito.ClienteId == cliente.Id &&
                            x.FechaCompra.Date == DateTime.Now.Date &&
                            x.Carrito.Cancelado == false)
                        .ToListAsync();

                if (cantPedidoDia.Count >= 3)
                {
                    TempData["SweetAlert"] = "error|Solo puede hacer 3 pedidos por día.";

                    return RedirectToAction(nameof(ConfirmarPedido));
                }

                ViewBag.Confirmado = true;
                ViewBag.HayCarrito = true;
                carrito.Procesado = true;
                _context.Add(pedido);
                _context.Update(carrito);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                ViewBag.Confirmado = false;
                ViewBag.HayCarrito = false;
                carrito.Cancelado = true;
                _context.Update(carrito);
                await _context.SaveChangesAsync();
                return NotFound();
            }

            return View(pedido);



        }

        [Authorize(Roles = "CLIENTE")]
        public IActionResult SeguirPedido()
        {
            var carrito = ViewData["Carrito"] as Carrito;

            ViewBag.HayCarrito = carrito != null;

            return View();
        }

        [Authorize(Roles = "CLIENTE")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SeguirPedido(string NroPedido)
        {
            if (string.IsNullOrEmpty(NroPedido))
            {
                ModelState.AddModelError("NroPedido", "El número de pedido es obligatorio.");
                return View();
            }

            // Lógica para validar el número de pedido (US12)
            var pedido = await _context.Pedido
                .FirstOrDefaultAsync(m => m.NroPedido == Convert.ToInt32(NroPedido));

            if (pedido == null)
            {
                ViewBag.Error = "El número de pedido ingresado no pertenece a ningún pedido.";
                return View();
            }
            ViewBag.EstadoPedido = ObtenerEstadoPedido(pedido.Estado);

            // Renderiza directamente la vista SeguirPedido con el modelo del pedido
            return View(pedido);
        }

        private string ObtenerEstadoPedido(int estadoPedido)
        {
            switch (estadoPedido)
            {
                case 1: return "Sin confirmar";
                case 2: return "Confirmado";
                case 3: return "En preparación";
                case 4: return "En reparto";
                case 5: return "Entregado";
                case 6: return "Cancelado";
                default: return "Desconocido";
            }
        }


        [NonAction]
        private async Task ModelView(Pedido? pedido)
        {
            if (pedido == null) pedido = new Pedido();

            var lCarritos = new List<SelectListItem>();

            var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;

            var carritosDelCliente = await _context.Set<Carrito>().Include(x => x.Cliente).Where(x => x.Cliente.Email == email).ToListAsync();

            if (carritosDelCliente.Count > 0)
            {
                lCarritos.AddRange(carritosDelCliente.Select(x => new SelectListItem
                {
                    Text = x.Id.ToString(),
                    Value = x.Id.ToString(),
                    Selected = x.Id == pedido.CarritoId

                }));

            }
            else
            {
                ModelState.AddModelError("CarritoId", "El cliente no posee carrito");
            }

            lCarritos.Insert(0, new SelectListItem("-Seleccione-", ""));

            ViewData["CarritoId"] = lCarritos;

        }

        [NonAction]
        private void ModelViewEditar(Pedido? pedido)
        {
            var lEstados = new List<SelectListItem>();

            foreach (var estadoPedido in Enum.GetValues(typeof(EstadoPedidoEnum)))
            {
                int valor = (int)estadoPedido;

                var displayAttribute = (DisplayAttribute)Attribute.GetCustomAttribute(
                    estadoPedido.GetType().GetField(estadoPedido.ToString()),
                    typeof(DisplayAttribute));

                string nombre = displayAttribute != null ? displayAttribute.Name : estadoPedido.ToString();

                var item = new SelectListItem
                {
                    Text = nombre,
                    Value = valor.ToString(),
                    Selected = valor == pedido.Estado,
                };

                lEstados.Add(item);
            }


            ViewData["EstadoId"] = lEstados;

        }
    }


}
