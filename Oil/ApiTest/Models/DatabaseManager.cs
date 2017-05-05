using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace ApiTest.Models
{
    public class DatabaseManager
    {
        private string _connectionString = ConfigurationManager.ConnectionStrings["OilApiDatabase"].ConnectionString;

        public async Task AddRecord(Record record)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand()
                {
                    Connection = connection,
                    CommandType = System.Data.CommandType.StoredProcedure,
                    CommandText = "Insert_Record"
                };
                command.Parameters.Add(new SqlParameter("@HardwareId", record.HardwareId));
                command.Parameters.Add(new SqlParameter("@HardwareName", record.HardwareName));
                command.Parameters.Add(new SqlParameter("@CountryISO", record.CountryISO));
                command.Parameters.Add(new SqlParameter("@City", record.City));
                command.Parameters.Add(new SqlParameter("@Branch", record.HardwareBranch));
                command.Parameters.Add(new SqlParameter("@FryerNum", record.FryerNum));
                command.Parameters.Add(new SqlParameter("@Quality", record.Quality));
                command.Parameters.Add(new SqlParameter("@Password", record.Password));
                command.Parameters.Add(new SqlParameter("@DateTimeStamp", record.DateTimeStamp));

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task<BaseResponse<int>> Login(Login login)
        {
            var response = new BaseResponse<int>();
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand()
                {
                    Connection = connection,
                    CommandType = System.Data.CommandType.StoredProcedure,
                    CommandText = "User_Login"
                };
                command.Parameters.Add(new SqlParameter("@Id", login.Id));
                command.Parameters.Add(new SqlParameter("@Password", login.Password));
                await connection.OpenAsync();
                response.Data = Convert.ToInt32(await command.ExecuteScalarAsync());
                switch (response.Data)
                {
                    case 0:
                        response.ErrorMessage = "No such hardware with this ID: " + login.Id;
                        break;
                    case -1:
                        response.ErrorMessage = "Wrong Password";
                        break;
                }
            }
            return response;
        }

        public async Task<BaseResponse<int>> Register(Register register)
        {
            var response = new BaseResponse<int>();
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand()
                {
                    Connection = connection,
                    CommandType = System.Data.CommandType.StoredProcedure,
                    CommandText = "User_Register"
                };
                command.Parameters.Add(new SqlParameter("@Id", register.Id));
                command.Parameters.Add(new SqlParameter("@Name", register.Name));
                command.Parameters.Add(new SqlParameter("@Country", register.Country));
                command.Parameters.Add(new SqlParameter("@Branch", register.Branch));
                command.Parameters.Add(new SqlParameter("@Password", register.Password));
                await connection.OpenAsync();
                response.Data = Convert.ToInt32(await command.ExecuteScalarAsync());
                switch (response.Data)
                {
                    case 0:
                        response.ErrorMessage = "This Hardware Already Exsits ";
                        break;
                    case 1:
                        response.ErrorMessage = "Hardware Added";
                        break;
                }
            }
            return response;
        }

        public async Task<BaseResponse<List<Record>>> GetRecords(string id = null, string name = null, string country = null, string city = null, string branch = null, string fryer = null, string quality = null, DateTime? from = null, DateTime? to = null)
        {
            var response = new BaseResponse<List<Record>>()
            {
                Data = new List<Record>()
            };
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand()
                {
                    Connection = connection,
                    CommandType = System.Data.CommandType.StoredProcedure,
                    CommandText = "Get_Records"
                };
                command.Parameters.Add(new SqlParameter("@HardwareId", id));
                command.Parameters.Add(new SqlParameter("@HardwareName", name));
                command.Parameters.Add(new SqlParameter("@CountryISO", country));
                command.Parameters.Add(new SqlParameter("@City", city));
                command.Parameters.Add(new SqlParameter("@Branch", branch));
                command.Parameters.Add(new SqlParameter("@FryerNum", fryer));
                command.Parameters.Add(new SqlParameter("@Quality", quality));
                command.Parameters.Add(new SqlParameter("@FromDate", from));
                command.Parameters.Add(new SqlParameter("@ToDate", to));

                await connection.OpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        response.Data.Add(new Record
                        {
                            HardwareId = reader["HardwareId"].ToString(),
                            HardwareName = reader["HardwareName"].ToString(),
                            HardwareBranch = reader["HardwareBranch"].ToString(),
                            CountryISO = reader["CountryISO"].ToString(),
                            City = reader["City"].ToString(),
                            FryerNum = reader["FryerNum"].ToString(),
                            Quality = reader["Quality"].ToString(),
                            DateTimeStamp = Convert.ToDateTime(reader["DateTimeStamp"])
                        });
                    }
                }
            }
            return response;
        }
    }
}