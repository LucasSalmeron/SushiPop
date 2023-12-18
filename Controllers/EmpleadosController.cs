using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SushiPop.Models;
using AutoMapper;
using SushiPOP_YA1A_2C2023_G2.DTO.Empleados;
using SushiPOP_YA1A_2C2023_G2.DTO;

namespace SushiPOP_YA1A_2C2023_G2.Controllers
{
    public class EmpleadosController : Controller
    {
        private readonly DBContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IMapper _mapper;

        public EmpleadosController(
            DBContext context,
            UserManager<IdentityUser> userManager,
            IMapper mapper)
        {
            _context = context;
            _userManager = userManager;
            _mapper = mapper;
        }

        // GET: Empleados
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Index()
        {
            if (_context.Empleado == null)
            {
                Problem("Entity set 'DBContext.Empleado'  is null.");
            }

            var lEmpleados = await _context.Empleado.ToListAsync();

            var lEmpleadosDto = _mapper.Map<List<EmpleadoDTO>>(lEmpleados);

            return View(lEmpleadosDto.OrderByDescending(x => x.FechaAlta));

        }
        [Authorize(Roles ="EMPLEADO")]
        public async Task<IActionResult> Principal(String userEmail)
        {
                var empleado = await _context.Empleado.FirstOrDefaultAsync(e => e.Email == userEmail);
                if (empleado == null)
                {
                    return NotFound();
                }
                return View(empleado);


            
        }

        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> AdminDashboard()
        {
            var modelo = new DashboardViewModel();

            //modelo.topProductos = await _context.Producto.FromSqlRaw("SELECT TOP(10) * FROM T_PRODUCTO P INNER JOIN T_CARRITOITEM CI ON CI.ProductoId = P.Id INNER JOIN T_CARRITO C ON C.Id = CI.CarritoId INNER JOIN T_PEDIDO PE ON PE.CarritoId = C.Id WHERE PE.FechaCompra >= GETDATE()- 90 ").ToListAsync();


            //CREO QUE ESTA FUNCIONA, HAY QUE CHEQUEAR CON COSAS EN LA BASE DE DATOS
            // var topIdProductos = await _context.Producto.FromSqlRaw("SELECT * FROM T_PRODUCTO P INNER JOIN T_CARRITOITEM CI ON CI.ProductoId = P.Id INNER JOIN T_CARRITO C ON C.Id = CI.CarritoId INNER JOIN T_PEDIDO PE ON PE.CarritoId = C.Id\r\nWHERE PE.FechaCompra >= GETDATE()- 90 AND P.Id IS NOT NULL  ").ToListAsync();
          
            return View(modelo);

        }


        // GET: Empleados/Details/5
        [Authorize(Roles = "EMPLEADO")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Empleado == null)
            {
                return NotFound();
            }

            var empleado = await _context.Empleado
                .FirstOrDefaultAsync(m => m.Id == id);
            if (empleado == null)
            {
                return NotFound();
            }
            
            var empleadoDto = _mapper.Map<EmpleadoDTO>(empleado);

            return View(empleadoDto);
        }

        // GET: Empleados/Create
        
        public IActionResult Create()
        {
            return View();
        }

        // POST: Empleados/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nombre,Apellido,Direccion,Telefono,FechaNacimiento,Email")] CrearEmpleadoDTO empleadoDto)
        {
            if (ModelState.IsValid)
            {
                IdentityUser user = new IdentityUser();

                var empleado = _mapper.Map<Empleado>(empleadoDto);

                user.Email = empleadoDto.Email;
                user.UserName = empleadoDto.Email;

                var result = await _userManager.CreateAsync(user, "Password1!");

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "EMPLEADO");

                    empleado.Activo = true;
                    empleado.FechaAlta = DateTime.Now;
                    empleado.Legajo = await _context.Empleado.MaxAsync(e => e.Legajo) + 1;
                    // TODO ver en caso de uso que debe tener el legajo como valor por defecto

                    _context.Add(empleado);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
            }
            return View(empleadoDto);
        }

        // GET: Empleados/Edit/5
        [Authorize(Roles = "EMPLEADO")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Empleado == null)
            {
                return NotFound();
            }

            var empleado = await _context.Empleado.FindAsync(id);

            // quito seguimiento de entidad en el contexto
            _context.Entry(empleado).State = EntityState.Detached; 

            if (empleado == null)
            {
                return NotFound();
            }

            var empleadoDto = _mapper.Map<EditarEmpleadoDTO>(empleado);

            return View(empleadoDto);
        }

        // POST: Empleados/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "EMPLEADO")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Legajo,Nombre,Apellido,Direccion,Telefono,FechaNacimiento,Activo,Email")] EditarEmpleadoDTO empleadoDto)
        {
            if (id != empleadoDto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var empleado = await _context.Empleado.FirstOrDefaultAsync(x => x.Id == id);

                    // quito seguimiento de entidad en el contexto
                    _context.Entry(empleado).State = EntityState.Detached;
                    
                    var fechaAlta = empleado.FechaAlta;
                    
                    empleado = _mapper.Map<Empleado>(empleadoDto);
                    empleado.FechaAlta = fechaAlta;

                    _context.Update(empleado);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmpleadoExists(empleadoDto.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Details), routeValues: new {id = empleadoDto.Id});
            }
            return View(empleadoDto);
        }

        // GET: Empleados/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Empleado == null)
            {
                return NotFound();
            }

            var empleado = await _context.Empleado
                .FirstOrDefaultAsync(m => m.Id == id);
            if (empleado == null)
            {
                return NotFound();
            }

            return View(empleado);
        }

        // POST: Empleados/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Empleado == null)
            {
                return Problem("Entity set 'DBContext.Empleado'  is null.");
            }
            var empleado = await _context.Empleado.FindAsync(id);
            if (empleado != null)
            {
                _context.Empleado.Remove(empleado);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmpleadoExists(int id)
        {
            return (_context.Empleado?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
