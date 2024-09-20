using MVC_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Threading.Tasks;


namespace MVC_Project.Controllers
{
    public class StoreMVCController : Controller
    {
        string baseURL = "https://localhost:44386/api/";//Base url common in all APIs

        public ActionResult Index(string searchType = null, string searchValue = null)
        {
            IEnumerable<Store> StoreList = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseURL);

                // Build the API URL based on the search type and value
                string apiUrl = "Stores"; // Default URL for getting all publishers

                if (!string.IsNullOrEmpty(searchType) && !string.IsNullOrEmpty(searchValue))
                {
                    switch (searchType.ToLower())
                    {
                        case "name":
                            apiUrl = $"Stores/name/{searchValue}";
                            break;
                        case "id":
                            apiUrl = $"Stores/id/{searchValue}";
                            break;
                        case "city":
                            apiUrl = $"Stores/city/{searchValue}";
                            break;
                        case "zipcode":
                            apiUrl = $"Stores/zip/{searchValue}";
                            break;
                        default:
                            apiUrl = $"Stores/";
                            break;
                    }
                }

                // HTTP GET
                var responseTask = client.GetAsync(apiUrl);
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<Store>>();
                    readTask.Wait();
                    StoreList = readTask.Result;
                }
                else
                {
                    StoreList = Enumerable.Empty<Store>();
                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }

            // Set ViewBag properties for use in the view
            ViewBag.SelectedValue = searchValue;
            ViewBag.SearchTypeName = searchType ?? "name"; // Default to "name"

            return View(StoreList);
        }

        // GET: StoreMVC/Details/5
        public ActionResult Details(int id)
        {
            // variable to hold the store details retrieved from WebApi
            Store StoreObj = null;

            using (var client = new HttpClient())
            {
                // Url of Webapi project
                client.BaseAddress = new Uri(baseURL);
                //HTTP GET
                var responseTask = client.GetAsync("Stores/" + id.ToString());
                // wait for task to complete
                responseTask.Wait();
                // retrieve the result
                var result = responseTask.Result;
                // check the status code for success
                if (result.IsSuccessStatusCode)
                {
                    // read the result
                    var readTask = result.Content.ReadAsAsync<IList<Store>>();
                    readTask.Wait();
                    // fill the store vairable created above with the returned result
                    StoreObj = readTask.Result.FirstOrDefault();
                }
            }
            return View(StoreObj);
        }



        // GET: StoreMVC/Create
        [Authorize(Roles = "Store")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: StoreMVC/Create
        [HttpPost]
        [Authorize(Roles ="Store")]
        public ActionResult Create(Store obj)
        {
            using (var client = new HttpClient())
            {
                // Url of Webapi project
                client.BaseAddress = new Uri(baseURL);
                //HTTP POST
                var postTask = client.PostAsJsonAsync<Store>("Stores", obj);
                // wait for task to complete
                postTask.Wait();
                // retrieve the result
                var result = postTask.Result;
                // check the status code for success
                if (result.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Store created successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Server Error. Please contact administrator.";
                }
               
            }
            // send the view back with model error
            return View(obj);
        }

        // GET: StoreMVC/Edit/5
        [Authorize(Roles ="Store")]
        public ActionResult Edit(int id)
        {
            // variable to hold the store details retrieved from WebApi
            Store StoreObj = null;

            using (var client = new HttpClient())
            {
                // Url of Webapi project
                client.BaseAddress = new Uri(baseURL);
                //HTTP GET
                var responseTask = client.GetAsync("Stores/" + id.ToString());
                // wait for task to complete
                responseTask.Wait();
                // retrieve the result
                var result = responseTask.Result;
                // check the status code for success
                if (result.IsSuccessStatusCode)
                {
                    // read the result
                    var readTask = result.Content.ReadAsAsync<List<Store>>();
                    readTask.Wait();
                    // fill the StoreObj  vairable created above with the returned result
                    StoreObj = readTask.Result.FirstOrDefault();
                }
            }
            return View(StoreObj);
        }

        
        // POST: StoreMVC/Edit/5
        [HttpPost]
        [Authorize(Roles ="Store")]
        public ActionResult Edit(string id, Store obj)
        {
            using (var client = new HttpClient())
            {
                obj.stor_id = id;
                // Url of Webapi project
                client.BaseAddress = new Uri(baseURL);
                //HTTP POST
                var putTask = client.PutAsJsonAsync<Store>("Stores/update/" + obj.stor_id, obj);
                // wait for task to complete
                putTask.Wait();
                // retrieve the result
                var result = putTask.Result;
                // check the status code for success
                if (result.IsSuccessStatusCode)
                {
                    // Return to Index
                    return RedirectToAction("Index");
                }
            }
            // Add model error
            ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");
            // send the view back with model error 
            return View(obj);
        }

        // GET: StoreMVC/Delete/5
        public ActionResult Delete(int id)
        {
            Store StoreObj = null;

            using (var client = new HttpClient())
            {
                // Url of Webapi project
                client.BaseAddress = new Uri(baseURL);
                //HTTP GET
                var responseTask = client.GetAsync("Stores/" + id.ToString());
                // wait for task to complete
                responseTask.Wait();
                // retrieve the result
                var result = responseTask.Result;
                // check the status code for success
                if (result.IsSuccessStatusCode)
                {
                    // read the result
                    var readTask = result.Content.ReadAsAsync<List<Store>>();
                    readTask.Wait();
                    // fill the StoreObj vairable created above with the returned result
                    StoreObj = readTask.Result.FirstOrDefault();
                }
            }
            return View(StoreObj);
        }

        // POST: StoreMVC/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, Store StorObj)
        {
            // variable to hold the store details retrieved from WebApi
            //Store StorObj = null;

            using (var client = new HttpClient())
            {
                // Url of Webapi project
                client.BaseAddress = new Uri(baseURL);
                //HTTP Delete
                var responseTask = client.DeleteAsync("Stores/Delete/" + id.ToString());
                // wait for task to complete
                responseTask.Wait();
                // retrieve the result
                var deleteTask = responseTask.Result;
                // check the status code for success
                if (deleteTask.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
            }
            return RedirectToAction("Index");
        }



        [HttpGet]
        [Authorize(Roles = "Store")]
        public ActionResult SendTitleRequest(string storeId)
        {
            if (string.IsNullOrEmpty(storeId))
            {
                ModelState.AddModelError(string.Empty, "Store ID is required.");
                return RedirectToAction("Index");
            }

            // Initialize a new StoreRequest model
            var storeRequest = new StoreRequest
            {
                StoreId = storeId
            };

            return View(storeRequest);
        }




        [HttpPost]
        [Authorize(Roles = "Store")]
        public async Task<ActionResult> SendTitleRequest(StoreRequest model)
        {
            if (ModelState.IsValid)
            {
                var storeRequest = new StoreRequest
                {
                    StoreId = model.StoreId,
                    PublisherId = model.PublisherId,
                    TitleId = model.TitleId,
                    RequestDetails = model.RequestDetails,
                    Status = "Pending",
                    RequestDate = DateTime.Now
                };

                try
                {
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(baseURL);

                        // Create the API endpoint URL for sending the request
                        string apiUrl = "StoreRequests"; // Ensure this is the correct endpoint

                        var response = await client.PostAsJsonAsync(apiUrl, storeRequest);

                        if (response.IsSuccessStatusCode)
                        {
                            TempData["SuccessMessage"] = "Request sent successfully!";
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            // Optionally, log the error response details here
                            var errorMessage = await response.Content.ReadAsStringAsync();
                            TempData["ErrorMessage"] = $"Server Error: {errorMessage}";
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Handle exceptions (e.g., network issues) here
                    TempData["ErrorMessage"] = $"Exception: {ex.Message}";
                }
            }

            return View(model);
        }
    }
}




            











        
    
