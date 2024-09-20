using MVC_Project.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MVC_Project.Controllers
{
    public class SalesMVCController : Controller
    {
        string baseURL = "https://localhost:44386/api/";

        public ActionResult Index(string searchType = null, string searchValue = null)
        {
            IEnumerable<Sale> saleList = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseURL);

                string apiUrl = "Sales"; // Default URL for getting all sales

                if (!string.IsNullOrEmpty(searchType) && !string.IsNullOrEmpty(searchValue))
                {
                    switch (searchType.ToLower())
                    {
                        case "titleid":
                            apiUrl = $"sales/titleid/{searchValue}";
                            break;
                        case "ordernum":
                            apiUrl = $"sales/{searchValue}";
                            break;
                        case "orderdate":
                    // Ensure the date format is acceptable by the API (e.g., "yyyy-MM-dd")
                    DateTime orderDate;
                            if (DateTime.TryParse(searchValue, out orderDate))
                            {
                                string formattedDate = orderDate.ToString("yyyy-MM-dd");
                                apiUrl = $"sales/orderdate/{formattedDate}";
                            }
                            else
                            {
                                ModelState.AddModelError(string.Empty, "Invalid date format.");
                                return View(Enumerable.Empty<Sale>());
                            }
                            break;
                        //case "orderdate":
                        //    apiUrl = $"sales/orderdate/{searchValue}";
                        //    break;
                        case "storeid":
                            apiUrl = $"sales/storeid/{searchValue}";
                            break;
                    }
                }

                var responseTask = client.GetAsync(apiUrl);
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var jsonString = result.Content.ReadAsStringAsync().Result;

                    // Handle both cases: single Sale object or array of Sales
                    if (jsonString.Trim().StartsWith("["))
                    {
                        // JSON array (e.g., when searching by titleid)
                        saleList = JsonConvert.DeserializeObject<IList<Sale>>(jsonString);
                    }
                    else
                    {
                        // Single object (e.g., when searching by ordernum)
                        var singleSale = JsonConvert.DeserializeObject<Sale>(jsonString);
                        saleList = new List<Sale> { singleSale };
                    }
                }
                else
                {
                    saleList = Enumerable.Empty<Sale>();
                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }

            // Set ViewBag properties for use in the view
            ViewBag.SelectedValue = searchValue;
            ViewBag.SearchTypeName = searchType ?? "OrderNum";

            return View(saleList);
        }


        

        // GET: SalesMVC/Details/5
        public async Task<ActionResult> Details(string id, string titleId)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri(baseURL);

                    HttpResponseMessage response = await client.GetAsync($"sales/orderandtitle?id={id}&titleId={titleId}");
                    response.EnsureSuccessStatusCode();

                    string responseData = await response.Content.ReadAsStringAsync();
                    var sale = JsonConvert.DeserializeObject<Sale>(responseData);

                    if (sale == null)
                    {
                        return HttpNotFound();
                    }

                    return View(sale);
                }
                catch (HttpRequestException ex)
                {
                    // Log the exception or handle as needed
                    ViewBag.ErrorMessage = $"Error fetching data: {ex.Message}";
                    return View("Error");
                }
                catch (Exception ex)
                {
                    // Log the exception or handle as needed
                    ViewBag.ErrorMessage = $"An unexpected error occurred: {ex.Message}";
                    return View("Error");
                }
            }
        }
        //return View();


        // GET: SalesMVC/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SalesMVC/Create
        [HttpPost]
        public ActionResult Create(Sale obj)
        {
            using (var client = new HttpClient())
            {
                // Url of Webapi project
                client.BaseAddress = new Uri(baseURL);
                //HTTP POST
                var postTask = client.PostAsJsonAsync<Sale>("sales", obj);
                // wait for task to complete
                postTask.Wait();
                // retrieve the result
                var result = postTask.Result;
                // check the status code for success
                if (result.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Record created successfully"; 

                    //return RedirectToAction("Index");
                } 
                else
                {
                    TempData["ErrorMessage"] = "Server Error. Please Contact Administrator";
                }
            }
            // Add model error
           // ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");
            // send the view back with model error
            return View(obj);
        }




       
        // GET: SalesMVC/Edit/5
        public async Task<ActionResult> Edit(string id, string titleId)
        {
            Sale saleOBJ = null;

            using (var client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri(baseURL); // Ensure baseURL is set correctly

                    HttpResponseMessage response = await client.GetAsync($"sales/orderandtitle?id={id}&titleId={titleId}");
                    if (response.IsSuccessStatusCode)
                    {
                        saleOBJ = await response.Content.ReadAsAsync<Sale>();
                    }
                    else
                    {
                        // Handle non-success status codes
                        ViewBag.ErrorMessage = $"Error: {response.ReasonPhrase}";
                        return View("Error");
                    }
                }
                catch (Exception ex)
                {
                    // Handle any exceptions that occur during the HTTP request
                    ViewBag.ErrorMessage = $"An error occurred: {ex.Message}";
                    return View("Error");
                }
            }

            if (saleOBJ == null)
            {
                return HttpNotFound();
            }


            return View(saleOBJ);

        }






        // POST: SalesMVC/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string id, Sale sale)
        {
            if (!ModelState.IsValid)
            {
                return View(sale);
            }

            using (var client = new HttpClient())
            {
                try
                {
                    sale.ord_num = id;
                    client.BaseAddress = new Uri(baseURL); // Ensure baseURL is set correctly

                    HttpResponseMessage response = await client.PutAsJsonAsync($"sales/" + sale.ord_num, sale);
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.ErrorMessage = $"Error: {response.ReasonPhrase}";
                        return View("Error");
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = $"An error occurred: {ex.Message}";
                    return View("Error");
                }
            }
        }

        // GET: SalesMVC/Delete/5
        public async Task<ActionResult> Delete(string id,string titleId)
        {
            Sale sale = null;

            using (var client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri(baseURL); // Ensure baseURL is set correctly

                    HttpResponseMessage response = await client.GetAsync($"sales/orderandtitle?id={id}&titleId={titleId}");
                    if (response.IsSuccessStatusCode)
                    {
                        sale = await response.Content.ReadAsAsync<Sale>();
                    }
                    else
                    {
                        // Handle non-success status codes
                        ViewBag.ErrorMessage = $"Error: {response.ReasonPhrase}";
                        return View("Error");
                    }
                }
                catch (Exception ex)
                {
                    // Handle any exceptions that occur during the HTTP request
                    ViewBag.ErrorMessage = $"An error occurred: {ex.Message}";
                    return View("Error");
                }
            }

            if (sale == null)
            {
                return HttpNotFound();
            }

            return View(sale);
        }


        
        // POST: SaleMVC/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id, string titleId)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri(baseURL); // Ensure baseURL is set correctly

                    HttpResponseMessage response = await client.DeleteAsync($"sales/Delete?id={id}&titleId={titleId}");
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        // Handle non-success status codes
                        ViewBag.ErrorMessage = $"Error: {response.ReasonPhrase}";
                        return View("Error");
                    }
                }
                catch (Exception ex)
                {
                    // Handle any exceptions that occur during the HTTP request
                    ViewBag.ErrorMessage = $"An error occurred: {ex.Message}";
                    return View("Error");
                }
            }
        }
        


    }
}
