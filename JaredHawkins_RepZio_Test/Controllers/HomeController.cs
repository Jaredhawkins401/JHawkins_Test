using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using JaredHawkins_RepZio_Test.Models;
using System.Net.Http;
using Newtonsoft.Json;
using System.IO;
using System.Runtime.Serialization.Json;

namespace JaredHawkins_RepZio_Test.Controllers
{
    public class HomeController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult TaxRate()
        {
            return View();
        }

        public IActionResult OrderRate()
        {
            return View();
        }

        /// <summary>
        /// Returns tax needed for an order.
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> OrderTax(Order order)
        {
            OrderResponse orderResponse = new OrderResponse();
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization
                    = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "2ddb34bc5852bd9acf8ed22e95076484");
                var orderSerialized = JsonConvert.SerializeObject(order);
                var buffer = System.Text.Encoding.UTF8.GetBytes(orderSerialized);
                var byteReturn = new ByteArrayContent(buffer);
                using (var response = await httpClient.PostAsync("https://api.taxjar.com/v2/taxes", byteReturn))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    orderResponse = JsonConvert.DeserializeObject<OrderResponse>(apiResponse);
                }
            }
            return View("OrderResponse", orderResponse);
        }

        /// <summary>
        /// Checks tax rate for a given zipcode.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> CheckTaxRate(Rate rate)
        {
            RateResponse rateResponse = new RateResponse();
            using (var httpClient = new HttpClient())
            {
                string url = string.Format("https://api.taxjar.com/v2/rates/{0}", rate.Zip);
                httpClient.DefaultRequestHeaders.Authorization
                    = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "2ddb34bc5852bd9acf8ed22e95076484");
                using (var response = await httpClient.GetAsync(url))
                {

                    string apiResponse = await response.Content.ReadAsStringAsync();
                    rateResponse = JsonConvert.DeserializeObject<RateResponse>(apiResponse);
                }
            }
            return View("TaxResponse", rateResponse);
        }
    }
}
