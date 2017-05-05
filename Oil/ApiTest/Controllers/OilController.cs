using ApiTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace ApiTest.Controllers
{
    public class OilController : ApiController
    {
        [HttpGet]
        public async Task<string> AddRecord(string id, string name, string branch, string country, string city, string fryer, string quality, string time, string password)
        {
            if (string.IsNullOrEmpty(id))
                return "Hardware ID cannot be empty";
            if (string.IsNullOrEmpty(name))
                return "Hardware Name cannot be empty";
            if (string.IsNullOrEmpty(branch))
                return "Branch cannot be empty";
            if (string.IsNullOrEmpty(country))
                return "Country cannot be empty";
            if (string.IsNullOrEmpty(city))
                return "City cannot be empty";
            if (string.IsNullOrEmpty(fryer))
                return "Fryer cannot be empty";
            if (string.IsNullOrEmpty(quality))
                return "Quality cannot be empty";
            if (string.IsNullOrEmpty(time))
                return "Time cannot be empty";
            if (string.IsNullOrEmpty(password))
                return "Password cannot be empty";

            var db = new DatabaseManager();

            var parts = time.Split('.');

            var newTime = DateTime.UtcNow;

            if (parts.Length >= 2)
            {
                var minutes = (Convert.ToInt16(parts[0]) * 60) + Convert.ToInt16(parts[1]);
                newTime = newTime.AddMinutes(minutes);
            }
            else
            {
                newTime = newTime.AddMinutes(Convert.ToInt32(time) * 60);
            }

            var record = new Record
            {
                HardwareId = id,
                HardwareName = name,
                HardwareBranch = branch,
                CountryISO = country,
                City = city,
                FryerNum = fryer,
                Quality = quality,
                DateTimeStamp = newTime,
                Password = password
            };
            await db.AddRecord(record);
            return "Record Added";
        }

        [HttpPost]
        public async Task<BaseResponse<int>> Login([FromBody]Login login)
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
        public async Task<BaseResponse<int>> Register([FromBody]Register register)
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

            var db = new DatabaseManager();

            return await db.Register(register);
        }

        [HttpPost]
        public async Task<BaseResponse<List<Record>>> GetRecords([FromBody] RequestRecords request)
        {
            var db = new DatabaseManager();
            if(request != null)
            {
                return await db.GetRecords(request.Id, request.Name, request.Country, request.City, request.Branch, request.Fryer, request.Quality, request.From, request.To);
            }
            return await db.GetRecords();
        }
    }
}
