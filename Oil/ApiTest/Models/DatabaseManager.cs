using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace ApiTest.Models
{
    public class DatabaseManager
    {
        private string _connectionString = @"Server=b8b4887c-cc91-4515-a21e-a75b0144e564.sqlserver.sequelizer.com;Database=dbb8b4887ccc914515a21ea75b0144e564;User ID=yisltebdgsewobue;Password=snE72bEZjZCbfHxECDGCzEeHWhYNZsyLNmXpMTR6V3p5PbbNtLydBtTVi6jTkZ8Z;";

        public void AddRecord(Record record)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand();
                command.Connection = connection;
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.CommandText = "Insert_Record";
                command.Parameters.Add(new SqlParameter("@HardwareId", record.HardwareId));
                command.Parameters.Add(new SqlParameter("@HardwareName", record.HardwareName));
                command.Parameters.Add(new SqlParameter("@CountryISO", record.CountryISO));
                command.Parameters.Add(new SqlParameter("@City", record.City));
                command.Parameters.Add(new SqlParameter("@Branch", record.HardwareBranch));
                command.Parameters.Add(new SqlParameter("@FryerNum", record.FryerNum));
                command.Parameters.Add(new SqlParameter("@Quality", record.Quality));
                command.Parameters.Add(new SqlParameter("@Password", record.Password));
                command.Parameters.Add(new SqlParameter("@DateTimeStamp", record.DateTimeStamp));

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public BaseResponse<int> Login(Login login)
        {
            var response = new BaseResponse<int>();
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand();
                command.Connection = connection;
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.CommandText = "User_Login";
                command.Parameters.Add(new SqlParameter("@Id", login.Id));
                command.Parameters.Add(new SqlParameter("@Password", login.Password));
                connection.Open();
                response.Data = Convert.ToInt32(command.ExecuteScalar());
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

        public async Task<BaseResponse<List<Record>>> GetRecords(string id = null, string name = null, string country = null, string city = null, string branch = null, string fryer = null, string quality = null, DateTime? from = null, DateTime? to = null)
        {
            var response = new BaseResponse<List<Record>>();
            response.Data = new List<Record>();
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

                        var v1 = reader["HardwareId"].ToString();
                        var v2 = reader["HardwareName"].ToString();
                        var v3 = reader["HardwareBranch"].ToString();
                        var v4 = reader["CountryISO"].ToString();
                        var v5 = reader["City"].ToString();
                        var v6 = reader["FryerNum"].ToString();
                        var v7 = reader["Quality"].ToString();
                        var v8 = Convert.ToDateTime(reader["DateTimeStamp"]);

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