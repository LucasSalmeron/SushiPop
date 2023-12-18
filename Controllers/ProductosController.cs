using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SushiPop.Models;
using SushiPOP_YA1A_2C2023_G2.DTO;
using SushiPOP_YA1A_2C2023_G2.DTO.Productos;


namespace SushiPOP_YA1A_2C2023_G2.Controllers
{
    public class ProductosController : Controller
    {
        private readonly DBContext _context;
        private readonly IMapper _mapper;

        public ProductosController(DBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: Productos
        public async Task<IActionResult> Index(string nombreCategoria)
        {
            if (_context.Producto == null)
            {
                //return NotFound();
                ModelState.AddModelError(string.Empty, "La conexión a la base de datos está interrumpida. Comuniquese con el administrador.");
                return RedirectToAction(nameof(Index));
            }

            var lProductosDto = await ModelViewIndex(nombreCategoria);

            return View(lProductosDto);
        }

        // GET: Productos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Producto == null)
            {
                //return NotFound();
                ModelState.AddModelError(string.Empty, "La conexión a la base de datos está interrumpida. Comuniquese con el administrador.");
                return RedirectToAction(nameof(Index));
            }

            var producto = await _context.Producto
                .Include(p => p.Categoria)
                .Include(p => p.Descuentos)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (producto == null)
            {
                //return NotFound();
                ModelState.AddModelError(string.Empty, "El producto no existe actualmente.");
                return RedirectToAction(nameof(Index));
            }

            var productoDto = ModelViewDetails(producto);

            return View(productoDto);
        }

        // GET: Productos/Create
        [Authorize(Roles = "EMPLEADO")]
        public async Task<IActionResult> Create()
        {
            await ModelViewCreate(null);

            return View();
        }

        // POST: Productos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "EMPLEADO")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nombre,Descripcion,Precio,Foto,Stock,Costo,CategoriaId")] CrearProductoDTO productoDto)
        {
            var nombreInvalido = await _context.Producto.AnyAsync(x => x.Nombre == productoDto.Nombre);

            if (nombreInvalido)
                ModelState.AddModelError("Nombre", "El producto ingresado ya existe en stock.");

            if (ModelState.IsValid)
            {
                var producto = _mapper.Map<Producto>(productoDto);

                if (string.IsNullOrEmpty(producto.Foto))
                {
                    producto.Foto = "https://img.freepik.com/vector-premium/cocinar-logo-restaurante-logo-cuchara-tenedor-plato_690577-587.jpg?w=740";
                }

                _context.Add(producto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            await ModelViewCreate(productoDto);

            return View("Create", productoDto);
        }

        // GET: Productos/Edit/5
        [Authorize(Roles = "EMPLEADO")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Producto == null)
            {
                //return NotFound();
                ModelState.AddModelError(string.Empty, "La conexión a la base de datos está interrumpida. Comuniquese con el administrador.");
                return RedirectToAction(nameof(Index));
            }

            var producto = await _context.Producto.FindAsync(id);

            if (producto == null)
            {
                //return NotFound();
                ModelState.AddModelError(string.Empty, "El pedido no existe actualmente.");
                return RedirectToAction(nameof(Index));
            }

            var productoDto = _mapper.Map<EditarProductoDTO>(producto);

            ModelViewPedidoPendiente(producto);

            ViewData["Nombre"] = producto.Nombre;

            return View(productoDto);
        }

        // POST: Productos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "EMPLEADO")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Descripcion,Precio,Stock,Costo")] EditarProductoDTO productoDto)
        {
            if (id != productoDto.Id)
            {
                return NotFound();
            }

            Producto producto = await _context.Producto.FirstOrDefaultAsync(x => x.Id == id);

            if (ModelState.IsValid)
            {
                try
                {

                    producto.Descripcion = productoDto.Descripcion;
                    producto.Precio = productoDto.Precio;
                    producto.Stock = productoDto.Stock;
                    producto.Costo = productoDto.Costo;

                    _context.Entry<Producto>(producto).State = EntityState.Modified;

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductoExists(productoDto.Id))
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

            ModelViewPedidoPendiente(producto);

            ViewData["Nombre"] = producto.Nombre;
            return View(productoDto);
        }

        // GET: Productos/Delete/5
        [Authorize(Roles = "EMPLEADO")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Producto == null)
            {
                //return NotFound();
                ModelState.AddModelError(string.Empty, "La conexión a la base de datos está interrumpida. Comuniquese con el administrador.");
                return RedirectToAction(nameof(Index));
            }

            var producto = await _context.Producto
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (producto == null)
            {
                //return NotFound();
                ModelState.AddModelError(string.Empty, "El pedido no existe actualmente.");
                return RedirectToAction(nameof(Index));
            }

            ModelViewPedidoPendiente(producto);

            return View(producto);
        }

        // POST: Productos/Delete/5
        [Authorize(Roles = "EMPLEADO")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Producto == null)
            {
                return Problem("Entity set 'DBContext.Producto'  is null.");
            }

            var producto = await _context.Producto.FindAsync(id);

            // TODO validar: producto con pedidos pendientes
            //          de confirmacion, en proceso, en reparto
            //          no deberian ser borrables
            if (producto != null)
            {
                bool productoConPedidoPendiente = false;
                if (producto.CarritosItems != null)
                {
                    productoConPedidoPendiente = producto.CarritosItems.Any(x =>
                              x.Carrito.Pedido.Estado != (int)EstadoPedidoEnum.Confirmado
                              && x.Carrito.Pedido.Estado != (int)EstadoPedidoEnum.Entregado
                              && x.Carrito.Pedido.Estado != (int)EstadoPedidoEnum.Cancelado
                          );
                }


                if (productoConPedidoPendiente)
                {
                    ModelState.AddModelError(string.Empty, "No puede borrar un producto con un pedido pendiente.");

                    ViewData["PedidoPendiente"] = productoConPedidoPendiente;

                    return View(nameof(Delete), new { id = id });

                }

                _context.Producto.Remove(producto);
                await _context.SaveChangesAsync();
            }
            else
            {
                ModelState.AddModelError(string.Empty, "El producto no existe actualmente.");

                return View(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ProductoExists(int id)
        {
            return (_context.Producto?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        [NonAction]
        private async Task<List<CardProductoDTO>> ModelViewIndex(string nombreCategoria)
        {
            var intDayWeek = (int)DateTime.Now.DayOfWeek;
            var lProductos = new List<Producto>();

            if (User.IsInRole("EMPLEADO") && string.IsNullOrEmpty(nombreCategoria))
            {
                lProductos = await _context.Producto.Include(p => p.Categoria).ToListAsync();
            }
            else
            {
                lProductos = await _context.Producto.Include(p => p.Categoria).Where(p => p.Categoria.Nombre == nombreCategoria).ToListAsync();
            }

            var lDescuentos = await _context.Descuento.Include(x => x.Producto).Where(d =>
                                            d.Dia == intDayWeek
                                            && d.Activo == true).ToListAsync();

            var lProductosDto = _mapper.Map<List<CardProductoDTO>>(lProductos);

            if (lDescuentos.Count > 0)
            {
                lProductosDto.Select(x =>
                {
                    if (lDescuentos.Any(d => d.ProductoId == x.Id))
                    {
                        decimal decDescuento = 0;
                        var descuento = lDescuentos.Find(d => d.ProductoId == x.Id);

                        if (descuento != null)
                        {
                            // verifica que descuento total sea menor al tope maximo, asigna el tope maximo si lo es
                            decDescuento = ((x.Precio * descuento?.Porcentaje) / 100) > descuento?.DescuentoMaximo ?
                                descuento.DescuentoMaximo :
                                ((decimal)(x.Precio * descuento.Porcentaje) / 100);

                        }

                        x.Descuento = x.Precio - decDescuento;

                    }
                    return x;

                }).ToList();
            }

            return lProductosDto.OrderByDescending(x => x.Nombre).ToList();
        }

        [NonAction]
        private ProductoDTO ModelViewDetails(Producto producto)
        {
            var productoDto = _mapper.Map<ProductoDTO>(producto);

            var intDayWeek = (int)DateTime.Now.DayOfWeek;

            if (producto.Descuentos.Any(d => d.Dia == intDayWeek && d.Activo == true))
            {
                var descuento = producto.Descuentos.FirstOrDefault(d =>
                                            d.Dia == intDayWeek
                                            && d.Activo == true);

                // verifica que descuento total sea menor al tope maximo, asigna el tope maximo si lo es
                decimal decDescuento = ((productoDto.Precio * descuento.Porcentaje) / 100) > descuento.DescuentoMaximo ?
                    descuento.DescuentoMaximo :
                    ((decimal)(productoDto.Precio * descuento.Porcentaje) / 100);

                productoDto.Descuento = productoDto.Precio - decDescuento;
            }

            return productoDto;
        }

        [NonAction]
        private async Task ModelViewCreate(CrearProductoDTO? producto)
        {
            if (producto == null) producto = new CrearProductoDTO();

            var categorias = await _context.Categoria.ToListAsync();
            var productos = await _context.Producto.ToListAsync();

            var selectList = new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "- Seleccione -" }
            };

            selectList.AddRange(categorias.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Nombre.ToString() }));

            ViewData["CategoriaId"] = new SelectList(selectList, "Value", "Text", producto.CategoriaId);
            ViewData["Nombre"] = productos.Select(x => x.Nombre).ToList();

        }

        [NonAction]
        private void ModelViewPedidoPendiente(Producto producto)
        {

            bool productoConPedidoPendiente = false;
            if (producto.CarritosItems != null)
            {
                productoConPedidoPendiente = producto.CarritosItems.Any(x =>
                          x.Carrito.Pedido.Estado != (int)EstadoPedidoEnum.Confirmado
                          && x.Carrito.Pedido.Estado != (int)EstadoPedidoEnum.Entregado
                          && x.Carrito.Pedido.Estado != (int)EstadoPedidoEnum.Cancelado
                      );
            }

            
            ViewData["PedidoPendiente"] = productoConPedidoPendiente;

        }
    }
}
