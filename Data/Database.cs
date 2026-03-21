using Microsoft.Data.Sqlite;
using CalculadoraCripto.Models;

namespace CalculadoraCripto.Data;

public static class Database
{
    private static string connectionString = "Data Source=cripto.db";

    public static void Inicializar()
    {
        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        // Crear tabla Usuarios
        var sqlUsuarios = @"
            CREATE TABLE IF NOT EXISTS Usuarios (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                NombreUsuario TEXT NOT NULL UNIQUE,
                Email TEXT NOT NULL,
                Password TEXT NOT NULL
            )";
        using var cmdUsuarios = new SqliteCommand(sqlUsuarios, connection);
        cmdUsuarios.ExecuteNonQuery();

        // Crear tabla Operaciones
        var sqlOperaciones = @"
            CREATE TABLE IF NOT EXISTS Operaciones (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                IdUsuario INTEGER NOT NULL,
                Criptomoneda TEXT NOT NULL,
                PrecioCompra REAL NOT NULL,
                PrecioVenta REAL NOT NULL,
                Cantidad REAL NOT NULL,
                Invertido REAL NOT NULL,
                ValorFinal REAL NOT NULL,
                Ganancia REAL NOT NULL,
                Porcentaje REAL NOT NULL,
                Fecha TEXT NOT NULL,
                FOREIGN KEY(IdUsuario) REFERENCES Usuarios(Id)
            )";
        using var cmdOperaciones = new SqliteCommand(sqlOperaciones, connection);
        cmdOperaciones.ExecuteNonQuery();
    }

    // Usuarios
    public static Usuario ObtenerUsuarioPorNombre(string nombreUsuario)
    {
        using var connection = new SqliteConnection(connectionString);
        connection.Open();
        var sql = "SELECT Id, NombreUsuario, Email, Password FROM Usuarios WHERE NombreUsuario = @nombre";
        using var cmd = new SqliteCommand(sql, connection);
        cmd.Parameters.AddWithValue("@nombre", nombreUsuario);
        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return new Usuario
            {
                Id = reader.GetInt32(0),
                NombreUsuario = reader.GetString(1),
                Email = reader.GetString(2),
                Password = reader.GetString(3)
            };
        }
        return null;
    }

    public static void InsertarUsuario(Usuario usuario, string passwordHash)
    {
        using var connection = new SqliteConnection(connectionString);
        connection.Open();
        var sql = "INSERT INTO Usuarios (NombreUsuario, Email, Password) VALUES (@nombre, @email, @pass)";
        using var cmd = new SqliteCommand(sql, connection);
        cmd.Parameters.AddWithValue("@nombre", usuario.NombreUsuario);
        cmd.Parameters.AddWithValue("@email", usuario.Email);
        cmd.Parameters.AddWithValue("@pass", passwordHash);
        cmd.ExecuteNonQuery();
    }

    // Operaciones
    public static List<Operacion> ObtenerOperacionesPorUsuario(int idUsuario)
    {
        var lista = new List<Operacion>();
        using var connection = new SqliteConnection(connectionString);
        connection.Open();
        var sql = "SELECT Id, IdUsuario, Criptomoneda, PrecioCompra, PrecioVenta, Cantidad, Invertido, ValorFinal, Ganancia, Porcentaje, Fecha FROM Operaciones WHERE IdUsuario = @idUsuario ORDER BY Fecha DESC";
        using var cmd = new SqliteCommand(sql, connection);
        cmd.Parameters.AddWithValue("@idUsuario", idUsuario);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            lista.Add(new Operacion
            {
                Id = reader.GetInt32(0),
                IdUsuario = reader.GetInt32(1),
                Criptomoneda = reader.GetString(2),
                PrecioCompra = reader.GetDecimal(3),
                PrecioVenta = reader.GetDecimal(4),
                Cantidad = reader.GetDecimal(5),
                Invertido = reader.GetDecimal(6),
                ValorFinal = reader.GetDecimal(7),
                Ganancia = reader.GetDecimal(8),
                Porcentaje = reader.GetDecimal(9),
                Fecha = DateTime.Parse(reader.GetString(10))
            });
        }
        return lista;
    }

    public static void InsertarOperacion(Operacion op)
    {
        using var connection = new SqliteConnection(connectionString);
        connection.Open();
        var sql = @"
            INSERT INTO Operaciones 
            (IdUsuario, Criptomoneda, PrecioCompra, PrecioVenta, Cantidad, Invertido, ValorFinal, Ganancia, Porcentaje, Fecha)
            VALUES (@idUsuario, @cripto, @pc, @pv, @cant, @inv, @vf, @gan, @porc, @fecha)";
        using var cmd = new SqliteCommand(sql, connection);
        cmd.Parameters.AddWithValue("@idUsuario", op.IdUsuario);
        cmd.Parameters.AddWithValue("@cripto", op.Criptomoneda);
        cmd.Parameters.AddWithValue("@pc", op.PrecioCompra);
        cmd.Parameters.AddWithValue("@pv", op.PrecioVenta);
        cmd.Parameters.AddWithValue("@cant", op.Cantidad);
        cmd.Parameters.AddWithValue("@inv", op.Invertido);
        cmd.Parameters.AddWithValue("@vf", op.ValorFinal);
        cmd.Parameters.AddWithValue("@gan", op.Ganancia);
        cmd.Parameters.AddWithValue("@porc", op.Porcentaje);
        cmd.Parameters.AddWithValue("@fecha", op.Fecha.ToString("yyyy-MM-dd"));
        cmd.ExecuteNonQuery();
    }

    public static void ActualizarOperacion(Operacion op)
    {
        using var connection = new SqliteConnection(connectionString);
        connection.Open();
        var sql = @"
            UPDATE Operaciones SET
                Criptomoneda = @cripto,
                PrecioCompra = @pc,
                PrecioVenta = @pv,
                Cantidad = @cant,
                Invertido = @inv,
                ValorFinal = @vf,
                Ganancia = @gan,
                Porcentaje = @porc,
                Fecha = @fecha
            WHERE Id = @id AND IdUsuario = @idUsuario";
        using var cmd = new SqliteCommand(sql, connection);
        cmd.Parameters.AddWithValue("@cripto", op.Criptomoneda);
        cmd.Parameters.AddWithValue("@pc", op.PrecioCompra);
        cmd.Parameters.AddWithValue("@pv", op.PrecioVenta);
        cmd.Parameters.AddWithValue("@cant", op.Cantidad);
        cmd.Parameters.AddWithValue("@inv", op.Invertido);
        cmd.Parameters.AddWithValue("@vf", op.ValorFinal);
        cmd.Parameters.AddWithValue("@gan", op.Ganancia);
        cmd.Parameters.AddWithValue("@porc", op.Porcentaje);
        cmd.Parameters.AddWithValue("@fecha", op.Fecha.ToString("yyyy-MM-dd"));
        cmd.Parameters.AddWithValue("@id", op.Id);
        cmd.Parameters.AddWithValue("@idUsuario", op.IdUsuario);
        cmd.ExecuteNonQuery();
    }

    public static void EliminarOperacion(int idOperacion, int idUsuario)
    {
        using var connection = new SqliteConnection(connectionString);
        connection.Open();
        var sql = "DELETE FROM Operaciones WHERE Id = @id AND IdUsuario = @idUsuario";
        using var cmd = new SqliteCommand(sql, connection);
        cmd.Parameters.AddWithValue("@id", idOperacion);
        cmd.Parameters.AddWithValue("@idUsuario", idUsuario);
        cmd.ExecuteNonQuery();
    }

    public static Operacion ObtenerOperacionPorId(int idOperacion, int idUsuario)
    {
        using var connection = new SqliteConnection(connectionString);
        connection.Open();
        var sql = "SELECT Id, IdUsuario, Criptomoneda, PrecioCompra, PrecioVenta, Cantidad, Invertido, ValorFinal, Ganancia, Porcentaje, Fecha FROM Operaciones WHERE Id = @id AND IdUsuario = @idUsuario";
        using var cmd = new SqliteCommand(sql, connection);
        cmd.Parameters.AddWithValue("@id", idOperacion);
        cmd.Parameters.AddWithValue("@idUsuario", idUsuario);
        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return new Operacion
            {
                Id = reader.GetInt32(0),
                IdUsuario = reader.GetInt32(1),
                Criptomoneda = reader.GetString(2),
                PrecioCompra = reader.GetDecimal(3),
                PrecioVenta = reader.GetDecimal(4),
                Cantidad = reader.GetDecimal(5),
                Invertido = reader.GetDecimal(6),
                ValorFinal = reader.GetDecimal(7),
                Ganancia = reader.GetDecimal(8),
                Porcentaje = reader.GetDecimal(9),
                Fecha = DateTime.Parse(reader.GetString(10))
            };
        }
        return null;
    }

    // Resumen (totales)
    public static (decimal totalInvertido, decimal totalFinal, decimal gananciaTotal) ObtenerTotales(int idUsuario)
    {
        using var connection = new SqliteConnection(connectionString);
        connection.Open();
        var sql = "SELECT SUM(Invertido), SUM(ValorFinal), SUM(Ganancia) FROM Operaciones WHERE IdUsuario = @idUsuario";
        using var cmd = new SqliteCommand(sql, connection);
        cmd.Parameters.AddWithValue("@idUsuario", idUsuario);
        using var reader = cmd.ExecuteReader();
        if (reader.Read() && !reader.IsDBNull(0))
        {
            return (reader.GetDecimal(0), reader.GetDecimal(1), reader.GetDecimal(2));
        }
        return (0, 0, 0);
    }
}