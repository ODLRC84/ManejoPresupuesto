using Dapper;
using ManejoPresupuestos.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuestos.Servicios
{
    public interface IRepositorioTransacciones
    {
        Task Actualizar(Transaccion transaccion, decimal montoAnterior, int cuentaAnterior);
        Task Borrar(int id);
        Task Crear(Transaccion transaccion);
        Task<IEnumerable<Transaccion>> ObtenerPorCuentaId(ObtenerTransaccionesPorCuenta modelo);
        Task<Transaccion> ObtenerPorId(int id, int usuarioId);
        Task<IEnumerable<ResultadoObtenerPorSemana>> ObtenerPorSemanas(ParametroObtenerTransaccionesPorUsuario modelo);
        Task<IEnumerable<Transaccion>> ObtenerPorUsuarioId(ParametroObtenerTransaccionesPorUsuario modelo);
    }
    public class RepositorioTransacciones: IRepositorioTransacciones
    {
        private readonly string connectionString;
        public RepositorioTransacciones(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Crear(Transaccion transaccion)
        {
            using var connection  = new SqlConnection(connectionString);
            var id = await connection.ExecuteAsync("Transaccion_Insertar",
                new { 
                        transaccion.UsuarioId, 
                        transaccion.FechaTransaccion, 
                        transaccion.Monto, 
                        transaccion.CategoriaId, 
                        transaccion.CuentaId, 
                        transaccion.Nota 
                    }, 
                commandType: System.Data.CommandType.StoredProcedure);

            transaccion.Id = id;

        }

        public async Task Actualizar(Transaccion transaccion, decimal montoAnterior, int cuentaAnteriorId)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.ExecuteAsync("Transaccion_Actualizar",
                new
                {
                    transaccion.Id,
                    transaccion.FechaTransaccion,
                    transaccion.Monto,
                    transaccion.CategoriaId,
                    transaccion.CuentaId,
                    transaccion.Nota,
                    montoAnterior,
                    cuentaAnteriorId
                },
                commandType: System.Data.CommandType.StoredProcedure);

            transaccion.Id = id;
            
        }

        public async Task<Transaccion> ObtenerPorId(int id, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Transaccion>(
                                    @"SELECT Transacciones.*, cat.TipoOperacionId 
                                    FROM Transacciones 
                                    INNER JOIN Categorias cat ON cat.Id = Transacciones.CategoriaId
                                    WHERE Transacciones.id=@Id AND Transacciones.UsuarioId=@UsuarioId", new { id, usuarioId });
        }

        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("Transaccion_Borrar", new { id }, commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Transaccion>> ObtenerPorCuentaId(ObtenerTransaccionesPorCuenta modelo)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Transaccion>(@"SELECT t.Id, t.Monto, t.FechaTransaccion, Categoria =c.Nombre,Cuenta = cu.Nombre, c.TipoOperacionId                                                   
                                                    FROM Transacciones t 
                                                    INNER JOIN Categorias c ON c.Id = t.CategoriaId
                                                    INNER JOIN Cuentas cu ON cu.Id = t.CuentaId
                                                    WHERE t.CuentaId = @CuentaId AND t.UsuarioId = @UsuarioId
                                                    AND FechaTransaccion BETWEEN @FechaInicio AND @FechaFin", modelo);
        }

        public async Task<IEnumerable<Transaccion>> ObtenerPorUsuarioId(ParametroObtenerTransaccionesPorUsuario modelo)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Transaccion>(@"SELECT t.Id, t.Monto, t.FechaTransaccion, Categoria =c.Nombre,Cuenta = cu.Nombre, c.TipoOperacionId                                                   
                                                    FROM Transacciones t 
                                                    INNER JOIN Categorias c ON c.Id = t.CategoriaId
                                                    INNER JOIN Cuentas cu ON cu.Id = t.CuentaId
                                                    WHERE  t.UsuarioId = @UsuarioId
                                                    AND FechaTransaccion BETWEEN @FechaInicio AND @FechaFin
                                                    ORDER BY t.FechaTransaccion DESC", modelo);
        }

        public async Task<IEnumerable<ResultadoObtenerPorSemana>> ObtenerPorSemanas(ParametroObtenerTransaccionesPorUsuario modelo)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<ResultadoObtenerPorSemana>(@"SELECT Semana=DATEDIFF(d,@fechaInicio,FechaTransaccion) /  7, cat.TipoOperacionId,Monto = SUM(Monto)
                                            FROM Transacciones t
                                            INNER JOIN Categorias cat ON cat.Id = CategoriaId
                                            WHERE FechaTransaccion BETWEEN @fechaInicio AND @fechaFin AND t.UsuarioId = @UsuarioId
                                            GROUP BY DATEDIFF(d,@fechaInicio,FechaTransaccion) /  7, cat.TipoOperacionId", modelo);
        }
    }
}
