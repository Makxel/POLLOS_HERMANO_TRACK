using Microsoft.Data.SqlClient;
using PollosTrack.Catalogos.Models;
using PollosTrack.Utils;

namespace PollosTrack.Catalogos.Repositories
{
    public class CategoriaRepository
    {
        public List<Categoria> ObtenerTodas(bool soloActivas = false)
        {
            var lista = new List<Categoria>();
            using var conn = Conexion.ObtenerConexion();
            string sql = "SELECT CATEGORIA_ID, NOMBRE, ACTIVO FROM CAT.CATEGORIAS"
                       + (soloActivas ? " WHERE ACTIVO = 1" : "")
                       + " ORDER BY NOMBRE";
            using var cmd = new SqlCommand(sql, conn);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new Categoria
                {
                    CategoriaId = reader.GetInt32(0),
                    Nombre = reader.GetString(1),
                    Activo = reader.GetBoolean(2)
                });
            }
            return lista;
        }

        public void Insertar(Categoria categoria)
        {
            using var conn = Conexion.ObtenerConexion();
            using var cmd = new SqlCommand(
                "INSERT INTO CAT.CATEGORIAS (NOMBRE, ACTIVO) VALUES (@nombre, @activo)", conn);
            cmd.Parameters.AddWithValue("@nombre", categoria.Nombre);
            cmd.Parameters.AddWithValue("@activo", categoria.Activo);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public void Actualizar(Categoria categoria)
        {
            using var conn = Conexion.ObtenerConexion();
            using var cmd = new SqlCommand(
                "UPDATE CAT.CATEGORIAS SET NOMBRE = @nombre, ACTIVO = @activo WHERE CATEGORIA_ID = @id", conn);
            cmd.Parameters.AddWithValue("@id", categoria.CategoriaId);
            cmd.Parameters.AddWithValue("@nombre", categoria.Nombre);
            cmd.Parameters.AddWithValue("@activo", categoria.Activo);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        // "Eliminar" = desactivar, porque los productos referencian a la categoría
        public void CambiarEstado(int categoriaId, bool activo)
        {
            using var conn = Conexion.ObtenerConexion();
            using var cmd = new SqlCommand(
                "UPDATE CAT.CATEGORIAS SET ACTIVO = @activo WHERE CATEGORIA_ID = @id", conn);
            cmd.Parameters.AddWithValue("@id", categoriaId);
            cmd.Parameters.AddWithValue("@activo", activo);
            conn.Open();
            cmd.ExecuteNonQuery();
        }
    }
}