using login.Dto;
using login.Utils;
using Dapper;
using Microsoft.Data.SqlClient;

namespace login.Models
{
    public class Proveedor
    {
        private readonly string _coneccionString;
        public Proveedor(IConfiguration config)
        {
            _coneccionString = config.GetConnectionString("Conexion2");
        }
        public async Task<RespuestasApi> modeloProveedor(int opcion)
        {
            try
            {
                using (var conn = new SqlConnection(_coneccionString))
                {
                    await conn.OpenAsync();
                    var spParams = new DynamicParameters();
                    spParams.Add("@Opcion", opcion, System.Data.DbType.Int16);
                    var resultado = await conn.QueryAsync("USP_GETPROVEEDOR", spParams, commandType: System.Data.CommandType.StoredProcedure);
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
