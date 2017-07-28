﻿using System;
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

        public async Task<BaseResponse<int>> AddRecord(Record record)
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
                command.Parameters.Add(new SqlParameter("@Capacity", record.Capacity));
                command.Parameters.Add(new SqlParameter("@Cost", record.Cost));
                command.Parameters.Add(new SqlParameter("@DailyAdded", record.DailyAdded));
                command.Parameters.Add(new SqlParameter("@FoodType", record.FoodType));
                command.Parameters.Add(new SqlParameter("@OilType", record.OilType));
                command.Parameters.Add(new SqlParameter("@FryerBrand", record.FryerBrand));
                await connection.OpenAsync();
                var result = (int)(await command.ExecuteScalarAsync());
                var response = new BaseResponse<int>()
                {
                    Data = result
                };
                switch (response.Data)
                {
                    case 0:
                        response.ErrorMessage = "Record Added";
                        break;
                    default:
                        response.ErrorMessage = "Wrong Password";
                        break;
                }
                return response;
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

        public async Task<BaseResponse<List<Record>>> GetRecords(RequestRecords request)
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
                command.Parameters.Add(new SqlParameter("@HardwareId", request.Id));
                command.Parameters.Add(new SqlParameter("@HardwareName", request.Name));
                command.Parameters.Add(new SqlParameter("@CountryISO", request.Country));
                command.Parameters.Add(new SqlParameter("@City", request.City));
                command.Parameters.Add(new SqlParameter("@Branch", request.Branch));
                command.Parameters.Add(new SqlParameter("@FryerNum", request.Fryer));
                command.Parameters.Add(new SqlParameter("@Quality", request.Quality));
                command.Parameters.Add(new SqlParameter("@FromDate", request.From));
                command.Parameters.Add(new SqlParameter("@ToDate", request.To));
                command.Parameters.Add(new SqlParameter("@FoodType", request.FoodType));
                command.Parameters.Add(new SqlParameter("@OilType", request.OilType));
                command.Parameters.Add(new SqlParameter("@FryerBrand", request.FryerBrand));
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
                            DateTimeStamp = Convert.ToDateTime(reader["DateTimeStamp"]),
                            DateStamp = Convert.ToDateTime(reader["DateTimeStamp"]).ToShortDateString(),
                            Capacity = reader["Capacity"].ToString(),
                            Cost = reader["Cost"].ToString(),
                            DailyAdded = reader["DailyAdded"].ToString(),
                            TimeStamp = Convert.ToDateTime(reader["DateTimeStamp"]).ToString("HH:mm"),
                            FoodType = reader["FoodType"].ToString(),
                            OilType = reader["OilType"].ToString(),
                            FryerBrand = reader["FryerBrand"].ToString()
                        });
                    }
                }
            }
            return response;
        }

        public async Task<BaseResponse<List<Record>>> GetLastFryerRecordByHardware(FryerGetRequest request)
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
                    CommandText = "Fryer_Get_Last_Record"
                };
                if (request.HardwareId.HasValue)
                    command.Parameters.AddWithValue("@HardwareId", request.HardwareId);

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
                            DateTimeStamp = Convert.ToDateTime(reader["DateTimeStamp"]),
                            DateStamp = Convert.ToDateTime(reader["DateTimeStamp"]).ToShortDateString(),
                            Capacity = reader["Capacity"].ToString(),
                            Cost = reader["Cost"].ToString(),
                            DailyAdded = reader["DailyAdded"].ToString(),
                            TimeStamp = Convert.ToDateTime(reader["DateTimeStamp"]).ToString("HH:mm"),
                            FoodType = reader["FoodType"].ToString(),
                            OilType = reader["OilType"].ToString(),
                            FryerBrand = reader["FryerBrand"].ToString()
                        });
                    }
                }
            }
            return response;
        }
    }
}