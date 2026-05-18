using login.Dto;
using login.Utils;
using Dapper;
using Microsoft.Data.SqlClient;

namespace login.Models
{
    public class OrdenCompra
    {
        private readonly string _coneccionString;
        public OrdenCompra(IConfiguration config)
        {
            _coneccionString = config.GetConnectionString("Conexion2");
        }

        public async Task<RespuestasApi> modeloOrdenCompra(OrdenCompraRequest parametros)
        {
            try
            {
                using (var conn = new SqlConnection(_coneccionString))
                {
                    await conn.OpenAsync();
                    var spParams = new DynamicParameters();
                    spParams.Add("@Opcion", parametros.Opcion, System.Data.DbType.Int16);
                    spParams.Add("@Emisor", parametros.Emisor ?? "", System.Data.DbType.String);
                    spParams.Add("@Proveedor", parametros.Proveedor ?? "", System.Data.DbType.String);
                    spParams.Add("@Correlativo", parametros.Correlativo ?? "", System.Data.DbType.String);
                    spParams.Add("@TipoDocumento", parametros.TipoDocumento, System.Data.DbType.Int16);
                    spParams.Add("@FechaInicio", parametros.FechaInicio, System.Data.DbType.DateTime);
                    spParams.Add("@FechaFin", parametros.FechaFin, System.Data.DbType.DateTime);
                    var resultado = await conn.QueryAsync("Usp_GetOrdenCompra", spParams, commandType: System.Data.CommandType.StoredProcedure);
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
