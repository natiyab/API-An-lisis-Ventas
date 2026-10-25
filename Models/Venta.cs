using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace tarea2.Models;

public class Venta
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("fecha")]
    public DateTime Fecha { get; set; }

    [BsonElement("estado")]
    public string? Estado { get; set; }

    [BsonElement("metodoPago")]
    public string? MetodoPago { get; set; }

    [BsonElement("moneda")]
    public string? Moneda { get; set; }

    [BsonElement("cliente")]
    public Cliente? Cliente { get; set; }

    [BsonElement("vendedor")]
    public VendedorInfo? Vendedor { get; set; }

    [BsonElement("productos")]
    public List<Producto> Productos { get; set; } = new();

    [BsonElement("subtotal")]
    public decimal Subtotal { get; set; }

    [BsonElement("descuentoTotal")]
    public decimal DescuentoTotal { get; set; }

    [BsonElement("impuesto")]
    public decimal Impuesto { get; set; }

    [BsonElement("total")]
    public decimal Total { get; set; }

    [BsonElement("canalVenta")]
    public string? CanalVenta { get; set; }

    [BsonElement("tienePromocion")]
    public bool TienePromocion { get; set; }
}

public class Cliente
{
    [BsonElement("identificacion")]
    public string? Identificacion { get; set; }

    [BsonElement("nombre")]
    public string? Nombre { get; set; }

    [BsonElement("ciudad")]
    public string? Ciudad { get; set; }

    [BsonElement("provincia")]
    public string? Provincia { get; set; }

    [BsonElement("categoria")]
    public string? Categoria { get; set; }
}

public class VendedorInfo
{
    [BsonElement("identificacion")]
    public string? Identificacion { get; set; }

    [BsonElement("nombre")]
    public string? Nombre { get; set; }

    [BsonElement("sucursal")]
    public string? Sucursal { get; set; }

    [BsonElement("region")]
    public string? Region { get; set; }
}

public class Producto
{
    [BsonElement("idProducto")]
    public string? IdProducto { get; set; }

    [BsonElement("nombre")]
    public string? Nombre { get; set; }

    [BsonElement("categoria")]
    public string? Categoria { get; set; }

    [BsonElement("cantidad")]
    public int Cantidad { get; set; }

    [BsonElement("precioUnitario")]
    public decimal PrecioUnitario { get; set; }

    [BsonElement("descuento")]
    public int Descuento { get; set; }
}