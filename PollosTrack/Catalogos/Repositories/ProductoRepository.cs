using Microsoft.Data.SqlClient;
using PollosTrack.Catalogos.Models;
using PollosTrack.Utils;

namespace PollosTrack.Catalogos.Repositories
{
    public class ProductoRepository
    {
        private const string Columnas =
            "PRODUCTO_ID, CATEGORIA_ID, NOMBRE, DESCRIPCION, PRECIO_UNITARIO, APLICA_IVA, ACTIVO";

        private static Producto Mapear(SqlDataReader r) => new Producto
        {
            ProductoId = r.GetInt32(0),
            CategoriaId = r.GetInt32(1),
            Nombre = r.GetString(2),
            Descripcion = r.IsDBNull(3) ? null : r.GetString(3),
            PrecioUnitario = r.GetDecimal(4),
            AplicaIva = r.GetBoolean(5),
            Activo = r.GetBoolean(6)
        };

        public List<Producto> ObtenerTodos(bool soloActivos = false)
        {
            var lista = new List<Producto>();
            using var conn = Conexion.ObtenerConexion();
            string sql = $"SELECT {Columnas} FROM CAT.PRODUCTOS"
                       + (soloActivos ? " WHERE ACTIVO = 1" : "")
                       + " ORDER BY NOMBRE";
            using var cmd = new SqlCommand(sql, conn);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) lista.Add(Mapear(reader));
            return lista;
        }

        // Devuelve el ID del producto nuevo
        public int Insertar(Producto p)
        {
            using var conn = Conexion.ObtenerConexion();
            using var cmd = new SqlCommand(
                @"INSERT INTO CAT.PRODUCTOS (CATEGORIA_ID, NOMBRE, DESCRIPCION, PRECIO_UNITARIO, APLICA_IVA, ACTIVO)
                  OUTPUT INSERTED.PRODUCTO_ID
                  VALUES (@categoria, @nombre, @descripcion, @precio, @iva, @activo)", conn);
            cmd.Parameters.AddWithValue("@categoria", p.CategoriaId);
            cmd.Parameters.AddWithValue("@nombre", p.Nombre);
            cmd.Parameters.AddWithValue("@descripcion", (object?)p.Descripcion ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@precio", p.PrecioUnitario);
            cmd.Parameters.AddWithValue("@iva", p.AplicaIva);
            cmd.Parameters.AddWithValue("@activo", p.Activo);
            conn.Open();
            return (int)cmd.ExecuteScalar()!;
        }

        public void Actualizar(Producto p)
        {
            using var conn = Conexion.ObtenerConexion();
            using var cmd = new SqlCommand(
                @"UPDATE CAT.PRODUCTOS
                  SET CATEGORIA_ID = @categoria, NOMBRE = @nombre, DESCRIPCION = @descripcion,
                      PRECIO_UNITARIO = @precio, APLICA_IVA = @iva, ACTIVO = @activo
                  WHERE PRODUCTO_ID = @id", conn);
            cmd.Parameters.AddWithValue("@id", p.ProductoId);
            cmd.Parameters.AddWithValue("@categoria", p.CategoriaId);
            cmd.Parameters.AddWithValue("@nombre", p.Nombre);
            cmd.Parameters.AddWithValue("@descripcion", (object?)p.Descripcion ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@precio", p.PrecioUnitario);
            cmd.Parameters.AddWithValue("@iva", p.AplicaIva);
            cmd.Parameters.AddWithValue("@activo", p.Activo);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        // "Eliminar" = desactivar, porque los pedidos ya los referencian
        public void CambiarEstado(int productoId, bool activo)
        {
            using var conn = Conexion.ObtenerConexion();
            using var cmd = new SqlCommand(
                "UPDATE CAT.PRODUCTOS SET ACTIVO = @activo WHERE PRODUCTO_ID = @id", conn);
            cmd.Parameters.AddWithValue("@id", productoId);
            cmd.Parameters.AddWithValue("@activo", activo);
            conn.Open();
            cmd.ExecuteNonQuery();
        }


        // Crea el producto y, si controla inventario, su fila en INV.INVENTARIO
        public int Insertar(Producto p, bool controlaStock, int stockMinimo)
        {
            using var conn = Conexion.ObtenerConexion();
            conn.Open();
            using var tx = conn.BeginTransaction();
            try
            {
                int id;
                using (var cmd = new SqlCommand(
                    @"INSERT INTO CAT.PRODUCTOS (CATEGORIA_ID, NOMBRE, DESCRIPCION, PRECIO_UNITARIO, APLICA_IVA, ACTIVO)
                      OUTPUT INSERTED.PRODUCTO_ID
                      VALUES (@categoria, @nombre, @descripcion, @precio, @iva, @activo)", conn, tx))
                {
                    cmd.Parameters.AddWithValue("@categoria", p.CategoriaId);
                    cmd.Parameters.AddWithValue("@nombre", p.Nombre);
                    cmd.Parameters.AddWithValue("@descripcion", (object?)p.Descripcion ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@precio", p.PrecioUnitario);
                    cmd.Parameters.AddWithValue("@iva", p.AplicaIva);
                    cmd.Parameters.AddWithValue("@activo", p.Activo);
                    id = (int)cmd.ExecuteScalar()!;
                }

                if (controlaStock)
                {
                    using var inv = new SqlCommand(
                        @"INSERT INTO INV.INVENTARIO (PRODUCTO_ID, STOCK_ACTUAL, STOCK_MINIMO, CONTROLA_STOCK)
                          VALUES (@id, 0, @minimo, 1)", conn, tx);
                    inv.Parameters.AddWithValue("@id", id);
                    inv.Parameters.AddWithValue("@minimo", stockMinimo);
                    inv.ExecuteNonQuery();
                }

                tx.Commit();
                return id;
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }
    }
}