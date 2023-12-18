
using System;
using System.Collections.Generic;
using System.Data;
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
    public class ReservasController : Controller
    {
        private readonly DBContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ReservasController(DBContext context, UserManager<IdentityUser> userManager )
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Reservas
        [Authorize(Roles = "ADMIN, EMPLEADO, CLIENTE")]
        public async Task<IActionResult> Index()
        {
            var dBContext = _context.Reserva.Include(r => r.Cliente);
            return View(await dBContext.ToListAsync());
        }

        // GET: Reservas/Details/5
        [Authorize(Roles = "EMPLEADO")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Reserva == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reserva
                .Include(r => r.Cliente)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reserva == null)
            {
                return NotFound();
            }

            return View(reserva);
        }

        // GET: Reservas/Create
        [Authorize(Roles = "CLIENTE")]
        public async Task<IActionResult> Create()
        {
            var usuario = await _userManager.GetUserAsync(User); 
            var cliente = await _context.Cliente.Where(c => c.Email.ToUpper() == usuario.NormalizedEmail).FirstOrDefaultAsync();

            Reserva reserva = new Reserva()
            {
                ClienteId = cliente.Id,
                Nombre = cliente.Nombre,
                Apellido = cliente.Apellido
                
            };
           
            return View(reserva);
        }

        // POST: Reservas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "CLIENTE")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Local,FechaHora,Confirmada,Nombre,Apellido,ClienteId")] Reserva reserva)
        {
            var usuario = await _userManager.GetUserAsync(User);
            reserva.Cliente = await _context.Cliente.Where(c => c.Email.ToUpper() == usuario.NormalizedEmail).FirstOrDefaultAsync();
            reserva.ClienteId = reserva.Cliente.Id;
            
            
            if (ModelState.IsValid)
            {
                if (!fechaRepetida(reserva))
                {
                    _context.Add(reserva);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(actionName: "Index", controllerName: "Home");
                }
                else
                {
                    ModelState.AddModelError("FechaHora", "Ya tenés una reserva creada para este día.");
                }
               
            }
            ViewData["ClienteId"] = new SelectList(_context.Cliente, "Id", "Id", reserva.ClienteId);
            return View(reserva);
        }

        public bool fechaRepetida(Reserva reserva)
        {
            bool result = false;


            var fechasDelUsuario = _context.Reserva.Where(r => r.ClienteId == reserva.ClienteId).Select(r => r.FechaHora).ToList();

          
            foreach(DateTime fecha in fechasDelUsuario)
            {
                if(fecha.DayOfYear == reserva.FechaHora.DayOfYear)
                {
                    result = true;
                    return result;
                }
            }




            return result;
        }


        // GET: Reservas/Edit/5
        [Authorize(Roles = "EMPLEADO")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Reserva == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reserva.FindAsync(id);
            if (reserva == null)
            {
                return NotFound();
            }
            ViewData["ClienteId"] = new SelectList(_context.Cliente, "Id", "Id", reserva.ClienteId);
            return View(reserva);
        }

        // POST: Reservas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "EMPLEADO")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Local,FechaHora,Confirmada,Nombre,Apellido,ClienteId")] Reserva reserva)
        {

            

            if (id != reserva.Id)
            {
                return NotFound();
            }

          

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reserva);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservaExists(reserva.Id))
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
            ViewData["ClienteId"] = new SelectList(_context.Cliente, "Id", "Id", reserva.ClienteId);
            return View(reserva);
        }

        

        private bool ReservaExists(int id)
        {
          return (_context.Reserva?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
