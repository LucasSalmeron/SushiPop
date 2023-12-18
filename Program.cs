using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using SushiPop.Models;
using SushiPOP_YA1A_2C2023_G2.DTO.Empleados;
using SushiPOP_YA1A_2C2023_G2.DTO.Productos;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<DBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DBContext") ?? throw new InvalidOperationException("Connection string 'DBContext' not found.")));

 //options.UseInMemoryDatabase("MemoryDb"));

builder.Services.AddAutoMapper(config =>
{
    config.CreateMap<Empleado, CrearEmpleadoDTO>()
        .ForMember(x => x.FechaNacimiento, y => y.MapFrom(y => y.FechaNacimiento.ToString("dd/MM/yyyy")));
    config.CreateMap<CrearEmpleadoDTO, Empleado>()
        .ForMember(dest => dest.FechaNacimiento, opt => opt.MapFrom(src => DateTime.Parse(src.FechaNacimiento)));
    config.CreateMap<Empleado, EditarEmpleadoDTO>()
        .ForMember(x => x.FechaNacimiento, y => y.MapFrom(y => y.FechaNacimiento.ToString("dd/MM/yyyy")));
    config.CreateMap<EditarEmpleadoDTO, Empleado>()
        .ForMember(dest => dest.FechaNacimiento, opt => opt.MapFrom(src => DateTime.Parse(src.FechaNacimiento)));


    config.CreateMap<Empleado, EmpleadoDTO>().ReverseMap();

    config.CreateMap<Producto, CardProductoDTO>().ReverseMap();
    config.CreateMap<Producto, ProductoDTO>()
        .ForMember(x => x.Categoria, y => y.MapFrom(y => y.Categoria.Nombre))
        .ReverseMap();
    config.CreateMap<Producto, CrearProductoDTO>()
        .ReverseMap();
    config.CreateMap<Producto, EditarProductoDTO>().ReverseMap();
});

builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddDefaultTokenProviders().AddEntityFrameworkStores<DBContext>();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();

app.UseAuthorization();
app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
