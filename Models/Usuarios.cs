using Dapper;
using login.Utils;
using Microsoft.Data.SqlClient;
using System.Data;

namespace login.Models
{
    public class Usuarios
    {
        private readonly string _connectionString;

        public Usuarios(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Conexion");
        }

        public async Task<RespuestasApi> modeloUsuarios(int opcion)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    var spParams = new DynamicParameters();
                    spParams.Add("@opcion",opcion,DbType.Int16,ParameterDirection.Input);
                    var resultado = await conn.QueryAsync(
                        "uspUsuarios", 
                        spParams,
                        commandType: CommandType.StoredProcedure);
                    return new RespuestasApi
                    {
                        StatusCode = System.Net.HttpStatusCode.OK,
                        IsSuccess = true,
                        Results = resultado
                    };
                }
            }
            catch (Exception ex)
            {
                return new RespuestasApi
                {
                    IsSuccess = false,
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    ErrorMessages = [ex.Message]
                };
            }
        }
    }
}
