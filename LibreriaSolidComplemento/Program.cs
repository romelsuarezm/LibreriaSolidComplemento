using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibreriaSolidComplemento
{
    // ISP: INTERFASE DE SEGREGACION, Esta interfaz solo requiere que el producto pueda mostrar su información
    public interface IProducto
    {
        void MostrarInformacion();
    }

    // OCP + LSP: OPEN-CLOSED + LISKOV; Esta clase base puede ser extendida sin modificarse
    public abstract class ProductoBase : IProducto
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public CategoriaUso Categoria { get; set; }
        public Marca Marca { get; set; }
        public int Stock { get; set; }
        public decimal Precio { get; set; }

        public abstract void MostrarInformacion();
    }

    // LSP LISKOV: Producto físico hereda de ProductoBase y funciona correctamente en su lugar
    public class ProductoFisico : ProductoBase
    {
        public override void MostrarInformacion()
        {
            Console.WriteLine("[Físico] Código: " + Codigo + " | Nombre: " + Nombre + " | Categoría: " + Categoria + " | Marca: " + Marca + " | Stock: " + Stock + " | Precio: S/. " + Precio);
        }
    }

    // LSP LISKOV: Producto virtual también hereda ProductoBase
    public class ProductoVirtual : ProductoBase
    {
        public string EnlaceDescarga { get; set; }
        public override void MostrarInformacion()
        {
            Console.WriteLine("[Virtual] Código: " + Codigo + " | Nombre: " + Nombre + " | Precio: S/. " + Precio + " | Link: " + EnlaceDescarga);
        }
    }

    // ISP INTERFASE DE SEGREGACION: Esta interfaz define operaciones de inventario básicas
    public interface IInventario
    {
        void InsertarProducto(ProductoBase producto);
        void MostrarTodos();
        ProductoBase BuscarProducto(string codigo);
    }

    // SRP SINGLE RESPONSABILIDAD: Esta clase solo gestiona el inventario
    public class Inventario : IInventario
    {
        private List<ProductoBase> productos = new List<ProductoBase>();

        public void InsertarProducto(ProductoBase producto)
        {
            productos.Add(producto);
        }

        // 
        public void MostrarTodos()
        {
            Console.WriteLine("\n--- Inventario Completo ---");
            foreach (var p in productos)
                p.MostrarInformacion();
        }

        public ProductoBase BuscarProducto(string codigo)
        {
            foreach (var p in productos)
            {
                if (p.Codigo == codigo) return p;
            }
            return null;
        }
    }

    // ISP INTERFACE DE SEGREGACION: Interfaz para la abstracción del método de pago
    public interface IPago
    {
        void ProcesarPago(decimal monto);
    }

    // OCP OPEN CLOSED: Nueva pasarela se puede agregar sin modificar código existente
    public class PasarelaYape : IPago
    {
        public void ProcesarPago(decimal monto)
        {
            Console.WriteLine("Pago de S/. " + monto + " realizado por Yape.");
        }
    }

    public class PasarelaPlin : IPago
    {
        public void ProcesarPago(decimal monto)
        {
            Console.WriteLine("Pago de S/. " + monto + " realizado por Plin.");
        }
    }

    // SRP SINGLE RESPONSIBILITY: Esta clase se encarga solo de calcular descuentos
    public class Descuento
    {
        public static decimal AplicarDescuento(ProductoBase producto)
        {
            if (producto.Categoria == CategoriaUso.Colorear)
            {
                Console.WriteLine("Descuento aplicado del 10%");
                return producto.Precio * 0.9m;
            }
            return producto.Precio;
        }
    }

    // SRP + DIP: Solo gestiona productos y depende de interfaces
    /// <summary>
    /// 
    /// </summary>
    public class CarritoCompras
    {
        private Dictionary<ProductoBase, int> carrito = new Dictionary<ProductoBase, int>();

        public void AgregarProducto(ProductoBase producto, int cantidad)
        {
            if (carrito.ContainsKey(producto))
                carrito[producto] += cantidad;
            else
                carrito.Add(producto, cantidad);
        }

        public void MostrarCarrito()
        {
            if (carrito.Count == 0)
            {
                Console.WriteLine("Carrito vacío. Agregue productos antes de continuar.");
                return;
            }

            Console.WriteLine("\n Carrito de Compras:");
            foreach (var item in carrito)
            {
                var producto = item.Key;
                var cantidad = item.Value;
                decimal precioFinal = Descuento.AplicarDescuento(producto) * cantidad;
                Console.WriteLine(producto.Nombre + " x " + cantidad + " - Total: S/. " + precioFinal);
            }
            Console.WriteLine("Total: S/. " + ObtenerTotal());
        }

        public decimal ObtenerTotal()
        {
            decimal total = 0;
            foreach (var item in carrito)
            {
                total += Descuento.AplicarDescuento(item.Key) * item.Value;
            }
            return total;
        }

        public void ProcesarPago(IPago metodo)
        {
            if (carrito.Count == 0)
            {
                Console.WriteLine(" No hay productos en el carrito. Seleccione un producto antes de pagar.");
                return;
            }
            MostrarCarrito();
            metodo.ProcesarPago(ObtenerTotal());
        }
    }

    // ISP: Rol del usuario solo requiere la gestión de pedidos
    public interface IUsuario
    {
        void GestionarPedido();
    }

    // LSP: Estas clases pueden sustituir a IUsuario
    // mejorar gestionar pedido
    public class Proveedor : IUsuario
    {
        public void GestionarPedido()
        {
            Console.WriteLine("Proveedor gestiona el envío de productos al inventario.");
        }
    }

    public class Cliente : IUsuario
    {
        public void GestionarPedido()
        {
            Console.WriteLine("Cliente realiza un pedido desde el carrito de compras.");
        }
    }

    public class Vendedor : IUsuario
    {
        public void GestionarPedido()
        {
            Console.WriteLine("Vendedor revisa y valida pedidos para despacho.");
        }
    }

    // SRP: Clase principal que coordina todo el sistema
    class Program
    {
        static void Main(string[] args)
        {
            IInventario inventario = new Inventario(); // DIP: se usa abstracción IInventario
            CarritoCompras carrito = new CarritoCompras();
            // crear un inventario para proveedor y vendedor
            // Insertamos algunos productos al inventario
            inventario.InsertarProducto(new ProductoFisico { Codigo = "P001", Nombre = "Cuaderno A4", Categoria = CategoriaUso.PapelYCuadernos, Marca = Marca.Artesco, Stock = 50, Precio = 5 });
            inventario.InsertarProducto(new ProductoFisico { Codigo = "P002", Nombre = "Lapicero Azul", Categoria = CategoriaUso.Escritura, Marca = Marca.Universal, Stock = 100, Precio = 1.2m });
            inventario.InsertarProducto(new ProductoVirtual { Codigo = "V001", Nombre = "E-book: Aprende C#", Precio = 10, EnlaceDescarga = "http://descarga.com/ebook-csharp" });

            // Selección de rol
            Console.WriteLine("Seleccione su rol:");
            Console.WriteLine("1. Proveedor\n2. Cliente\n3. Vendedor");
            Console.Write("Opción: ");
            string rol = Console.ReadLine();
            IUsuario usuario;
            bool mostrarMenuCompleto = true;
            switch (rol)
            {
                case "1": usuario = new Proveedor(); mostrarMenuCompleto = false; break;
                case "2": usuario = new Cliente(); break;
                case "3": usuario = new Vendedor(); break;
                default:
                    Console.WriteLine("Rol inválido. Terminando programa.");
                    return;
            }
            usuario.GestionarPedido();

            bool continuar = true;
            while (continuar)
            {
                Console.WriteLine("\n===== MENÚ PRINCIPAL =====");
                Console.WriteLine("1. Ver productos");
                Console.WriteLine("2. Buscar producto por código");
                if (mostrarMenuCompleto)
                {
                    Console.WriteLine("3. Agregar producto al carrito");
                    Console.WriteLine("4. Ver carrito");
                    Console.WriteLine("5. Pagar");
                    Console.WriteLine("6. Ver gestión de pedidos por roles");
                }
                Console.WriteLine("7. Salir");
                Console.Write("Opción: ");

                string opcion = Console.ReadLine();
                switch (opcion)
                {
                    case "1": inventario.MostrarTodos(); break;
                    case "2":
                        Console.Write("Código del producto: ");
                        var buscado = inventario.BuscarProducto(Console.ReadLine());
                        if (buscado != null) buscado.MostrarInformacion();
                        else Console.WriteLine("Producto no encontrado");
                        break;
                    case "3":
                        if (!mostrarMenuCompleto) break;
                        Console.Write("Código del producto: ");
                        var p = inventario.BuscarProducto(Console.ReadLine());
                        if (p != null)
                        {
                            Console.Write("Cantidad: ");
                            int cantidad;
                            if (int.TryParse(Console.ReadLine(), out cantidad) && cantidad > 0)
                            {
                                carrito.AgregarProducto(p, cantidad);
                                Console.WriteLine("Producto agregado al carrito");
                            }
                            else
                            {
                                Console.WriteLine("Cantidad inválida");
                            }
                        }
                        else Console.WriteLine("Producto no encontrado");
                        break;
                    case "4":
                        if (!mostrarMenuCompleto) break;
                        carrito.MostrarCarrito();
                        break;
                    case "5":
                        if (!mostrarMenuCompleto) break;
                        Console.WriteLine("Método de pago: 1. Yape  2. Plin");
                        string metodo = Console.ReadLine();
                        IPago pasarela = metodo == "1" ? (IPago)new PasarelaYape() : new PasarelaPlin();
                        carrito.ProcesarPago(pasarela);
                        break;
                    case "6":
                        if (!mostrarMenuCompleto) break;
                        Console.WriteLine("\n=== Gestión de pedidos por roles ===");
                        List<IUsuario> usuarios = new List<IUsuario>()
                        {
                            new Proveedor(),
                            new Cliente(),
                            new Vendedor()
                        };
                        foreach (var u in usuarios)
                        {
                            u.GestionarPedido();
                        }
                        break;
                    case "7":
                        continuar = false;
                        Console.WriteLine("Gracias por usar el sistema");
                        break;
                    default:
                        Console.WriteLine("Opción no válida");
                        break;
                }
            }
        }
    }
    public enum CategoriaUso

    {
        Lectura, Escritura, Colorear, Gomas, Accesorios, Archivado, PapelYCuadernos
    }

    public enum Marca
    {
        Artesco, FaberCastell, OVE, Universal, Staedtler, Standford
    }
}