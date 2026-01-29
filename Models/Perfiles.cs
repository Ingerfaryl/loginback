using Dapper;
using login.Dto;
using login.Utils;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal;
using Microsoft.Extensions.Configuration;

namespace login.Models
{
    public class Perfiles
    {
        private readonly string _connectionString;

        public Perfiles(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Conexion");
        }
        public async Task<RespuestasApi> modeloPerfiles(PerfilRequest parametros)
        {
			try
			{
                using (var conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    var spParams = new DynamicParameters();

                    spParams.Add("@opcion", parametros.opcion, System.Data.DbType.Int32);
                    spParams.Add("@perfil", parametros.perfil, System.Data.DbType.String);

                    var resultado = await conn.QueryAsync("spPerfiles", spParams, commandType: System.Data.CommandType.StoredProcedure);

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
