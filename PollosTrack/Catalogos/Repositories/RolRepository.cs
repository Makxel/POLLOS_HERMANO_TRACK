using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using PollosTrack.Catalogos.Models;
using PollosTrack.Utils;

namespace PollosTrack.Catalogos.Repositories
{
    public class RolRepository
    {
        public List<Rol> ObtenerTodos()
        {
            var lista = new List<Rol>();
            using var conn = Conexion.ObtenerConexion();
            using var cmd = new SqlCommand(
                "SELECT ROL_ID, NOMBRE, DESCRIPCION FROM SEG.ROLES ORDER BY NOMBRE", conn);
            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new Rol
                {
                    RolId = reader.GetInt32(0),
                    Nombre = reader.GetString(1),
                    Descripcion = reader.IsDBNull(2) ? null : reader.GetString(2)
                });
            }
            return lista;
        }

        public void Insertar(Rol rol)
        {
            using var conn = Conexion.ObtenerConexion();
            using var cmd = new SqlCommand(
                "INSERT INTO SEG.ROLES (NOMBRE, DESCRIPCION) VALUES (@nombre, @descripcion)", conn);
            cmd.Parameters.AddWithValue("@nombre", rol.Nombre);
            cmd.Parameters.AddWithValue("@descripcion", (object?)rol.Descripcion ?? DBNull.Value);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public void Actualizar(Rol rol)
        {
            using var conn = Conexion.ObtenerConexion();
            using var cmd = new SqlCommand(
                "UPDATE SEG.ROLES SET NOMBRE = @nombre, DESCRIPCION = @descripcion WHERE ROL_ID = @id", conn);
            cmd.Parameters.AddWithValue("@id", rol.RolId);
            cmd.Parameters.AddWithValue("@nombre", rol.Nombre);
            cmd.Parameters.AddWithValue("@descripcion", (object?)rol.Descripcion ?? DBNull.Value);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public void Eliminar(int rolId)
        {
            using var conn = Conexion.ObtenerConexion();
            using var cmd = new SqlCommand("DELETE FROM SEG.ROLES WHERE ROL_ID = @id", conn);
            cmd.Parameters.AddWithValue("@id", rolId);
            conn.Open();
            cmd.ExecuteNonQuery();
        }
    }
}
