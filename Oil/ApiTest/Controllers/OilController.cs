using ApiTest.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace ApiTest.Controllers
{
    public class OilController : ApiController
    {
        [HttpGet]
        public async Task<BaseResponse<string>> AddRecord(string id, string name, string branch, string country, string city, string fryer, string quality, string time, string password,string capacity, string cost, string dailyAdded, string foodType, string oilType, string fryerBrand)
        {
            if (string.IsNullOrEmpty(id))
                return new BaseResponse<string>() { ErrorMessage = "Hardware ID cannot be empty", Data = "-1" };
            if (string.IsNullOrEmpty(name))
                return new BaseResponse<string>() { ErrorMessage = "Hardware Name cannot be empty", Data = "-1" };
            if (string.IsNullOrEmpty(branch))
                return new BaseResponse<string>() { ErrorMessage = "Branch Name cannot be empty", Data = "-1" };
            if (string.IsNullOrEmpty(country))
                return new BaseResponse<string>() { ErrorMessage = "Country cannot be empty", Data = "-1" };
            if (string.IsNullOrEmpty(city))
                return new BaseResponse<string>() { ErrorMessage = "City cannot be empty", Data = "-1" };
            if (string.IsNullOrEmpty(fryer))
                return new BaseResponse<string>() { ErrorMessage = "Fryer cannot be empty", Data = "-1" };
            if (string.IsNullOrEmpty(quality))
                return new BaseResponse<string>() { ErrorMessage = "Quality cannot be empty", Data = "-1" };
            if (string.IsNullOrEmpty(time))
                return new BaseResponse<string>() { ErrorMessage = "Time cannot be empty", Data = "-1" };
            if (string.IsNullOrEmpty(password))
                return new BaseResponse<string>() { ErrorMessage = "Password cannot be empty", Data = "-1" };
            if (string.IsNullOrEmpty(capacity))
                return new BaseResponse<string>() { ErrorMessage = "Capacity cannot be empty", Data = "-1" };
            if (string.IsNullOrEmpty(cost))
                return new BaseResponse<string>() { ErrorMessage = "Cost cannot be empty", Data = "-1" };
            if (string.IsNullOrEmpty(dailyAdded))
                return new BaseResponse<string>() { ErrorMessage = "Daily added cannot be empty", Data = "-1" };
            if (string.IsNullOrEmpty(foodType))
                return new BaseResponse<string>() { ErrorMessage = "foodType added cannot be empty", Data = "-1" };
            if (string.IsNullOrEmpty(oilType))
                return new BaseResponse<string>() { ErrorMessage = "oilType added cannot be empty", Data = "-1" };
            if (string.IsNullOrEmpty(fryerBrand))
                return new BaseResponse<string>() { ErrorMessage = "fryerBrand added cannot be empty", Data = "-1" };

            var db = new DatabaseManager();

            var record = new Record
            {
                HardwareId = id,
                HardwareName = name,
                HardwareBranch = branch,
                CountryISO = country,
                City = city,
                FryerNum = fryer,
                Quality = quality,
                DateTimeStamp = Convert.ToDateTime(time),
                Password = password,
                Capacity = capacity,
                Cost = cost,
                DailyAdded = dailyAdded,
                FoodType = foodType,
                OilType = oilType,
                FryerBrand = fryerBrand
            };
            var response = await db.AddRecord(record);
            return response;
        }

        [HttpPost]
        public async Task<BaseResponse<int>> Login(Login login)
        {
            if (login == null)
                return new BaseResponse<int> { Data = 0, ErrorMessage = "ID and password are required" };

            if (string.IsNullOrEmpty(login.Id))
                return new BaseResponse<int> { Data = 0, ErrorMessage = "ID is required" };

            if (string.IsNullOrEmpty(login.Password))
                return new BaseResponse<int> { Data = 0, ErrorMessage = "Password is required" };

            var db = new DatabaseManager();

            return await db.Login(login);
        }

        [HttpPost]
        public async Task<BaseResponse<int>> Register(Register register)
        {
            if (register == null)
                return new BaseResponse<int> { Data = 0, ErrorMessage = "ID, Name, Country, Branch and Password are required" };

            if (string.IsNullOrEmpty(register.Id))
                return new BaseResponse<int> { Data = 0, ErrorMessage = "ID is required" };

            if (string.IsNullOrEmpty(register.Name))
                return new BaseResponse<int> { Data = 0, ErrorMessage = "Name is required" };

            if (string.IsNullOrEmpty(register.Country))
                return new BaseResponse<int> { Data = 0, ErrorMessage = "Country is required" };

            if (string.IsNullOrEmpty(register.Branch))
                return new BaseResponse<int> { Data = 0, ErrorMessage = "Branch is required" };

            if (string.IsNullOrEmpty(register.Password))
                return new BaseResponse<int> { Data = 0, ErrorMessage = "Password is required" };

            if (string.IsNullOrEmpty(register.RegisterId))
                return new BaseResponse<int> { Data = 0, ErrorMessage = "RegisterId is required" };

            var db = new DatabaseManager();

            return await db.Register(register);
        }

        [HttpPost]
        public async Task<BaseResponse<List<Record>>> GetRecords(RequestRecords request)
        {
            var db = new DatabaseManager();
            if(request != null)
            {
                return await db.GetRecords(request);
            }
            return await db.GetRecords(new RequestRecords());
        }

        [HttpPost]
        public async Task<BaseResponse<List<Record>>> GetLastFryerRecordByHardware(FryerGetRequest request)
        {
            var db = new DatabaseManager();
            return await db.GetLastFryerRecordByHardware(request);
        }

        [HttpPost]
        public async Task<BaseResponse<QuantityCostResponse>> GetQuantityAndCost(QuantityAndCostRequest request)
        {
            var db = new DatabaseManager();

            return await db.GetQuantityAndCost(request);
        }
    }
}
