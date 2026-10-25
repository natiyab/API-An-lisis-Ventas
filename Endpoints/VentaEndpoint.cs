using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using tarea2.Data;
using tarea2.Models;

namespace tarea2.Endpoints;

public static class VentaEndpoint
{
    public static void MapVentaEndpoints(this WebApplication app)
    {
        var grupo = app.MapGroup("/api")
            .WithTags("Ventas");

        grupo.MapPost("/cargar-datos", async ([FromServices] MongoDbContext db) =>
        {
            var ventas = GenerarDatosPrueba();
            await db.Ventas.DeleteManyAsync(_ => true);
            await db.Ventas.InsertManyAsync(ventas);
            return Results.Ok(new { mensaje = $"Se cargaron {ventas.Count} ventas ✅" });
        });

        grupo.MapGet("/reporte", async ([FromServices] MongoDbContext db) =>
        {
            var todas = await db.Ventas.Find(_ => true).ToListAsync();

            var facturacionTotal = todas.Sum(v => v.Total);
            var ticketPromedio = todas.Average(v => v.Total);
            var totalTransacciones = todas.Count;

            var productosMasVendidos = todas
                .SelectMany(v => v.Productos)
                .GroupBy(p => new { p.Nombre, p.Categoria })
                .Select(g => new
                {
                    producto = g.Key.Nombre!,
                    categoria = g.Key.Categoria!,
                    cantidadVendida = g.Sum(p => p!.Cantidad),
                    ingresos = g.Sum(p => p.Cantidad * p.PrecioUnitario * (1 - (decimal)p.Descuento / 100m))
                })
                .OrderByDescending(x => x.cantidadVendida)
                .ToList();

            var ventasPorCiudad = todas
                .GroupBy(v => v.Cliente!.Ciudad)
                .Select(g => new
                {
                    ciudad = g.Key!,
                    cantidadVentas = g.Count(),
                    facturacion = g.Sum(v => v.Total)
                })
                .OrderByDescending(x => x.facturacion)
                .ToList();

            var desempeñoVendedores = todas
                .GroupBy(v => new { v.Vendedor!.Nombre, v.Vendedor.Sucursal })
                .Select(g => new
                {
                    vendedor = g.Key.Nombre!,
                    sucursal = g.Key.Sucursal!,
                    ventasRealizadas = g.Count(),
                    totalVendido = g.Sum(v => v.Total),
                    promedioVenta = g.Average(v => v.Total)
                })
                .OrderByDescending(x => x.totalVendido)
                .ToList();

            var topClientes = todas
                .GroupBy(v => new { v.Cliente!.Nombre, v.Cliente.Categoria })
                .Select(g => new
                {
                    cliente = g.Key.Nombre!,
                    categoria = g.Key.Categoria!,
                    compras = g.Count(),
                    gastoTotal = g.Sum(v => v.Total)
                })
                .OrderByDescending(x => x.gastoTotal)
                .Take(5)
                .ToList();

            return Results.Ok(new
            {
                facturacionTotal,
                ticketPromedio,
                totalTransacciones,
                productosMasVendidos,
                ventasPorCiudad,
                desempeñoVendedores,
                topClientes
            });
        });
    }

    private static List<Venta> GenerarDatosPrueba()
    {
        return new List<Venta>
        {
            new Venta
            {
                Fecha = new DateTime(2026,9,15,10,30,0),
                Estado = "Completada",
                MetodoPago = "Tarjeta",
                Moneda = "CRC",
                Cliente = new Cliente
                {
                    Identificacion = "C1001",
                    Nombre = "Ana Rodríguez",
                    Ciudad = "San José",
                    Provincia = "San José",
                    Categoria = "Premium"
                },
                Vendedor = new VendedorInfo
                {
                    Identificacion = "V015",
                    Nombre = "Carlos Méndez",
                    Sucursal = "Escazú",
                    Region = "Central"
                },
                Productos = new List<Producto>
                {
                    new Producto { IdProducto = "P001", Nombre = "Laptop", Categoria = "Tecnología", Cantidad = 2, PrecioUnitario = 450000m, Descuento = 5 },
                    new Producto { IdProducto = "P002", Nombre = "Mouse", Categoria = "Tecnología", Cantidad = 3, PrecioUnitario = 12000m, Descuento = 0 }
                },
                Subtotal = 936000m,
                DescuentoTotal = 45000m,
                Impuesto = 115830m,
                Total = 1006830m,
                CanalVenta = "Sitio web",
                TienePromocion = true
            },
            new Venta
            {
                Fecha = new DateTime(2026,9,16,14,20,0),
                Estado = "Completada",
                MetodoPago = "Efectivo",
                Moneda = "CRC",
                Cliente = new Cliente
                {
                    Identificacion = "C1002",
                    Nombre = "Manuel Quesada",
                    Ciudad = "Alajuela",
                    Provincia = "Alajuela",
                    Categoria = "Premium"
                },
                Vendedor = new VendedorInfo
                {
                    Identificacion = "V015",
                    Nombre = "Carlos Méndez",
                    Sucursal = "Escazú",
                    Region = "Central"
                },
                Productos = new List<Producto>
                {
                    new Producto { IdProducto = "P003", Nombre = "Monitor 24\"", Categoria = "Tecnología", Cantidad = 3, PrecioUnitario = 180000m, Descuento = 10 },
                    new Producto { IdProducto = "P004", Nombre = "Teclado Mecánico", Categoria = "Tecnología", Cantidad = 2, PrecioUnitario = 45000m, Descuento = 5 }
                },
                Subtotal = 576000m,
                DescuentoTotal = 54000m,
                Impuesto = 62100m,
                Total = 584100m,
                CanalVenta = "Tienda física",
                TienePromocion = true
            },
            new Venta
            {
                Fecha = new DateTime(2026,9,17,11,45,0),
                Estado = "Completada",
                MetodoPago = "Tarjeta",
                Moneda = "CRC",
                Cliente = new Cliente
                {
                    Identificacion = "C1003",
                    Nombre = "Jorge Vargas",
                    Ciudad = "Cartago",
                    Provincia = "Cartago",
                    Categoria = "Regular"
                },
                Vendedor = new VendedorInfo
                {
                    Identificacion = "V022",
                    Nombre = "Andrea Ruiz",
                    Sucursal = "Cartago",
                    Region = "Este"
                },
                Productos = new List<Producto>
                {
                    new Producto { IdProducto = "P005", Nombre = "Silla Ergonómica", Categoria = "Muebles", Cantidad = 3, PrecioUnitario = 85000m, Descuento = 0 },
                    new Producto { IdProducto = "P006", Nombre = "Escritorio Ejecutivo", Categoria = "Muebles", Cantidad = 2, PrecioUnitario = 150000m, Descuento = 0 }
                },
                Subtotal = 555000m,
                DescuentoTotal = 0m,
                Impuesto = 61050m,
                Total = 616050m,
                CanalVenta = "Tienda física",
                TienePromocion = false
            },
            new Venta
            {
                Fecha = new DateTime(2026,9,18,9,15,0),
                Estado = "Completada",
                MetodoPago = "Transferencia",
                Moneda = "CRC",
                Cliente = new Cliente
                {
                    Identificacion = "C1004",
                    Nombre = "Roberto Solís",
                    Ciudad = "Heredia",
                    Provincia = "Heredia",
                    Categoria = "Premium"
                },
                Vendedor = new VendedorInfo
                {
                    Identificacion = "V018",
                    Nombre = "María López",
                    Sucursal = "Heredia",
                    Region = "Central"
                },
                Productos = new List<Producto>
                {
                    new Producto { IdProducto = "P007", Nombre = "Notebook Gamer", Categoria = "Tecnología", Cantidad = 1, PrecioUnitario = 870000m, Descuento = 8 },
                    new Producto { IdProducto = "P008", Nombre = "Impresora Láser", Categoria = "Oficina", Cantidad = 2, PrecioUnitario = 27000m, Descuento = 0 }
                },
                Subtotal = 924000m,
                DescuentoTotal = 69600m,
                Impuesto = 94704m,
                Total = 949104m,
                CanalVenta = "Sitio web",
                TienePromocion = true
            },
            new Venta
            {
                Fecha = new DateTime(2026,9,19,16,30,0),
                Estado = "Completada",
                MetodoPago = "Efectivo",
                Moneda = "CRC",
                Cliente = new Cliente
                {
                    Identificacion = "C1005",
                    Nombre = "Victor Espinoza",
                    Ciudad = "Liberia",
                    Provincia = "Guanacaste",
                    Categoria = "Regular"
                },
                Vendedor = new VendedorInfo
                {
                    Identificacion = "V030",
                    Nombre = "Pedro Jiménez",
                    Sucursal = "Liberia",
                    Region = "Norte"
                },
                Productos = new List<Producto>
                {
                    new Producto { IdProducto = "P009", Nombre = "Cafetera Industrial", Categoria = "Oficina", Cantidad = 3, PrecioUnitario = 12000m, Descuento = 5 },
                    new Producto { IdProducto = "P010", Nombre = "Escritorio Estándar", Categoria = "Muebles", Cantidad = 4, PrecioUnitario = 55000m, Descuento = 0 }
                },
                Subtotal = 252000m,
                DescuentoTotal = 1800m,
                Impuesto = 27522m,
                Total = 277722m,
                CanalVenta = "Tienda física",
                TienePromocion = false
            }
        };
    }
}