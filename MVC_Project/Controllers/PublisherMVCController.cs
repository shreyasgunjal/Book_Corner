using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
///using System.Security.Policy;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using MVC_Project.Models;
using Newtonsoft.Json;

namespace MVC_Project.Controllers
{
    public class PublisherMVCController : Controller
    {
        // GET: PublisherMVC
        string baseURL = "https://localhost:44386/api/";


        #region Index
        [Authorize(Roles = "Publisher,Store")]
        public ActionResult Index(string searchType = null, string searchValue = null)
        {
            IEnumerable<Publisher> publisherList = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseURL);

                // Build the API URL based on the search type and value
                string apiUrl = "Publishers"; // Default URL for getting all publishers

                if (!string.IsNullOrEmpty(searchType) && !string.IsNullOrEmpty(searchValue))
                {
                    switch (searchType.ToLower())
                    {
                        case "name":
                            apiUrl = $"publishers/pubname/{searchValue}";
                            break;
                        case "id":
                            apiUrl = $"publishers/{searchValue}";
                            break;
                        case "city":
                            apiUrl = $"publishers/city/{searchValue}";
                            break;
                        case "state":
                            apiUrl = $"publishers/state/{searchValue}";
                            break;
                        case "country":
                            apiUrl = $"publishers/country/{searchValue}";
                            break;
                    }
                }

                // HTTP GET
                var responseTask = client.GetAsync(apiUrl);
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<Publisher>>();
                    readTask.Wait();
                    publisherList = readTask.Result;
                }
                else
                {
                    publisherList = Enumerable.Empty<Publisher>();
                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }

            // Set ViewBag properties for use in the view
            ViewBag.SelectedValue = searchValue;
            ViewBag.SearchTypeName = searchType ?? "name"; // Default to "name"

            return View(publisherList);
        }
        #endregion

        #region Reject Request
        [HttpPost]
        [Authorize(Roles = "Publisher")]
        public async Task<ActionResult> RejectRequest(int id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseURL);

                // Step 1: Delete the StoreRequest and get the TitleId as a string
                var response = await client.DeleteAsync($"StoreRequests/{id}");

            
                if (response.IsSuccessStatusCode)
                {
                    // Read the TitleId from the response as a string
                    var titleId = await response.Content.ReadAsStringAsync();
                    var cleanedResponse = titleId.Trim('\"');

                    // Step 2: Delete the Title using TitleId
                    var titleResponse = await client.DeleteAsync($"titles/{cleanedResponse}");

                    if (titleResponse.IsSuccessStatusCode)
                    {
                        TempData["SuccessMessage"] = "Request and associated title deleted successfully!";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Request deleted, but failed to delete the associated title.";
                    }

                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorMessage"] = "Server Error. Please contact administrator.";
                }
            }

            return RedirectToAction("Index");
        }
        #endregion

        #region Accept Request
        [HttpPost]
        [Authorize(Roles = "Publisher")]
        public async Task<ActionResult> AcceptRequest(int id)
        {
            // Call the API to delete the request
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseURL);

                var response = await client.DeleteAsync($"StoreRequests/{id}");

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Request Accepted successfully!";
                    return RedirectToAction("Index"); // Redirect to the list of requests or another appropriate view
                }
                else
                {
                    TempData["ErrorMessage"] = "Server Error. Please contact administrator.";
                }
            }

            return RedirectToAction("Index");
        }
        #endregion

        #region Details
        // GET: PublisherMVC/Details/5
        public async Task<ActionResult> Details(string id)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri(baseURL); 

                    HttpResponseMessage response = await client.GetAsync($"publishers/{id}");
                    response.EnsureSuccessStatusCode();

                    string responseData = await response.Content.ReadAsStringAsync();
                    var publisher = JsonConvert.DeserializeObject<Publisher>(responseData);

                    if (publisher == null)
                    {
                        return HttpNotFound();
                    }

                    return View(publisher);
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
        #endregion

        #region Create(GET)
        //GET: PublisherMVC/Create
        [Authorize(Roles ="Publisher")]
        public ActionResult Create()
        {
            return View();
        }
        #endregion

        #region Create(POST)

        [HttpPost]
        [Authorize(Roles = "Publisher")]
        public ActionResult Create(Publisher pubObj)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseURL);
                var postTask = client.PostAsJsonAsync<Publisher>("publishers/post", pubObj);
                postTask.Wait();
                var result = postTask.Result;

                if (result.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Publisher created successfully!";
                    
                }
                else
                {
                    TempData["ErrorMessage"] = "Server Error. Please contact administrator.";
                }
            }
            
            return View(pubObj);
            
        }
        #endregion

        #region Edit(GET)
        // GET: PublisherMVC/Edit/5
        [Authorize(Roles = "Publisher")]
        public async Task<ActionResult> Edit(string id)
        {
            Publisher pubOBJ = null;

            using (var client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri(baseURL); // Ensure baseURL is set correctly

                    HttpResponseMessage response = await client.GetAsync($"publishers/{id}" );
                    if (response.IsSuccessStatusCode)
                    {
                        pubOBJ = await response.Content.ReadAsAsync<Publisher>();
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

            if (pubOBJ == null)
            {
                return HttpNotFound();
            }

         
            return View(pubOBJ);

        }
        #endregion

        #region Delete(GET)
        // GET: TitlesMVC/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            Publisher publisher = null;

            using (var client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri(baseURL); // Ensure baseURL is set correctly

                    HttpResponseMessage response = await client.GetAsync($"publishers/{id}");
                    if (response.IsSuccessStatusCode)
                    {
                        publisher = await response.Content.ReadAsAsync<Publisher>();
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

            if (publisher == null)
            {
                return HttpNotFound();
            }

            return View(publisher);
        }
        #endregion

        #region Delete(POST)
        // POST: TitlesMVC/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri(baseURL); // Ensure baseURL is set correctly

                    HttpResponseMessage response = await client.DeleteAsync($"publishers/{id}");
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

        #endregion

        #region Show All Requests
        public async Task<ActionResult> ShowRequests()
        {
            IEnumerable<StoreRequest> requestList = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseURL);

                // HTTP GET to fetch all store requests
                HttpResponseMessage response = await client.GetAsync("storerequests");
                if (response.IsSuccessStatusCode)
                {
                    // Deserialize the response to get the list of store requests
                    string responseData = await response.Content.ReadAsStringAsync();
                    requestList = JsonConvert.DeserializeObject<List<StoreRequest>>(responseData);
                }
                else
                {
                    requestList = Enumerable.Empty<StoreRequest>();
                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }

            return View(requestList);
        }
        #endregion

        #region Edit(POST)
        // POST: PublisherMVC/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string id,Publisher publisher)
        {
            if (!ModelState.IsValid)
            {
                return View(publisher);
            }

            using (var client = new HttpClient())
            {
                try
                {
                    publisher.pub_id =id;
                    client.BaseAddress = new Uri(baseURL); // Ensure baseURL is set correctly

                    HttpResponseMessage response = await client.PutAsJsonAsync($"publishers/"+publisher.pub_id,publisher);
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
        #endregion


    }
}
