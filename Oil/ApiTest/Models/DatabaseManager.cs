using FCM.Net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ApiTest.Models
{
    public class DatabaseManager
    {
        private string _connectionString = ConfigurationManager.ConnectionStrings["OilApiDatabase"].ConnectionString;

        public async Task<BaseResponse<string>> AddRecord(Record record)
        {
            try
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
                    var result = "";
                    var regId = "";
                    var res = await command.ExecuteReaderAsync();
                    while(await res.ReadAsync())
                    {
                        regId = res["RegisterId"].ToString();
                        result = res["Result"].ToString();
                    }
                    var response = new BaseResponse<string>()
                    {
                        Data = result
                    };

                    if (record.Quality == "3")
                    {
                        await SendNotification(record.FryerNum, regId);
                    }

                    if(response.Data == "0")
                    {
                        response.ErrorMessage = "Record Added";
                    }
                    else
                    {
                        response.ErrorMessage = "Wrong Password";
                    }

                    return response;
                }
            }
            catch (Exception e)
            {
                return new BaseResponse<string>
                {
                    ErrorMessage = e.Message
                };
            }
        }

        private async Task SendNotification(string fryerNum, string reg_id)
        {
            var registrationId = reg_id;
            var serverKey = ConfigurationManager.AppSettings.Get("android_serverKey");

            using (var sender = new Sender(serverKey))
            {
                var message = new Message
                {
                    RegistrationIds = new List<string> { registrationId },
                    Priority = Priority.High,
                    Data = new Notification
                    {
                        Title = string.Format("Fryer: {0}",fryerNum),
                        Body = "Time to change oil !"
                    }
                };
                var result = await sender.SendAsync(message);
                //Console.WriteLine($"Success: {result.MessageResponse.Success}");

                //var json = "{\"notification\":{\"title\":\"json message\",\"body\":\"works like a charm!\"},\"to\":\"" + registrationId + "\"}";
                //result = await sender.SendAsync(json);
            }
        }

        public async Task<BaseResponse<int>> Login(Login login)
        {
            var response = new BaseResponse<int>();
            try
            {
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
                    command.Parameters.Add(new SqlParameter("@RegisterId", login.RegisterId));
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
            catch (Exception e)
            {
                response.ErrorMessage = e.Message;
                return response;
            }
        }

        public async Task<BaseResponse<int>> Register(Register register)
        {
            var response = new BaseResponse<int>();
            try
            {
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
                    command.Parameters.Add(new SqlParameter("@RegisterId", register.RegisterId));
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
            catch (Exception e)
            {
                response.ErrorMessage = e.Message;
                return response;
            }
        }

        public async Task<BaseResponse<List<Record>>> GetRecords(RequestRecords request)
        {
            var response = new BaseResponse<List<Record>>()
            {
                Data = new List<Record>()
            };
            try
            {
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
                                FryerBrand = reader["FryerBrand"].ToString(),
                                OilCycle = Convert.ToInt32(reader["OilDuration"])
                            });
                        }
                    }
                }
                return response;
            }
            catch (Exception e)
            {
                response.ErrorMessage = e.Message;
                return response;
            }
        }

        public async Task<BaseResponse<List<Record>>> GetLastFryerRecordByHardware(FryerGetRequest request)
        {
            var response = new BaseResponse<List<Record>>()
            {
                Data = new List<Record>()
            };
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var command = new SqlCommand()
                    {
                        Connection = connection,
                        CommandType = System.Data.CommandType.StoredProcedure,
                        CommandText = "Fryer_Get_Last_Record"
                    };
                    if (string.IsNullOrEmpty(request.HardwareId))
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
            catch (Exception e)
            {
                response.ErrorMessage = e.Message;
                return response;
            }
        }

        public async Task<BaseResponse<QuantityCostResponse>> GetQuantityAndCost(QuantityAndCostRequest request)
        {
            var response = new BaseResponse<QuantityCostResponse>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var command = new SqlCommand()
                    {
                        Connection = connection,
                        CommandType = System.Data.CommandType.StoredProcedure,
                        CommandText = "Oil_Quantity_Cost_Get"
                    };

                    command.Parameters.AddWithValue("@HardwareId", request.HardwareId);
                    command.Parameters.AddWithValue("@FryerId", request.FryerId);
                    command.Parameters.AddWithValue("@From", request.From);
                    command.Parameters.AddWithValue("@To", request.To);

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.Data = new QuantityCostResponse
                            {
                                Quantity = Convert.ToDecimal(reader["Quantity"]),
                                Cost = Convert.ToDecimal(reader["Cost"])
                            };
                        }
                    }
                }
                return response;
            }
            catch (Exception e)
            {

                response.ErrorMessage = e.Message;
                return response;
            }
        }
    }
}