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
    public class TitleMVCController : Controller
    {
        string baseURL = "https://localhost:44386/api/";

        // GET: TitleMVC
        [Authorize(Roles = "Publisher,Store")]
        public ActionResult Index(string searchType = null, string searchValue = null)
        {
            IEnumerable<Title> titleList = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseURL);

                // Build the API URL based on the search type and value
                string apiUrl = "Titles"; // Default URL for getting all titles

                if ((!string.IsNullOrEmpty(searchType)) && (!string.IsNullOrEmpty(searchValue) || string.IsNullOrEmpty(searchValue)))
                {

                    switch (searchType)
                    {
                        case "title":
                            apiUrl = $"Titles/title/{searchValue}";
                            break;
                        case "type":
                            apiUrl = $"Titles/type/{searchValue}";
                            break;
                        case "pubid":
                            apiUrl = $"Titles/pubid/{searchValue}";
                            break;
                        //case "pubdate":
                        //    apiUrl = $"Titles/pubdate/{searchValue}";
                        //    break;
                        case "pubdate":
                            // Ensure the date format is acceptable by the API (e.g., "yyyy-MM-dd")
                            DateTime pubdate;
                            if (DateTime.TryParse(searchValue, out pubdate))
                            {
                                string formattedDate = pubdate.ToString("yyyy-MM-dd");
                                apiUrl = $"Titles/pubdate/{formattedDate}";
                            }
                            else
                            {
                                ModelState.AddModelError(string.Empty, "Invalid date format.");
                                return View(Enumerable.Empty<Title>());
                            }
                            break;
                        case "authorname":
                            apiUrl = $"Titles/authorname/{searchValue}";
                            break;
                        //case "top5titles":
                        //    apiUrl = "titles/top5titles";
                        //    break;
                        default:
                            //apiUrl = ("Titles");
                            apiUrl = "Titles/top5titles";
                            break;
                    }
                }

                    // HTTP GET
                var responseTask = client.GetAsync(apiUrl);
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<Title>>();
                    readTask.Wait();
                    titleList = readTask.Result;
                }
                else
                {
                    titleList = Enumerable.Empty<Title>();
                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }

            // Set ViewBag properties for use in the view
            ViewBag.SelectedValue = searchValue;
            ViewBag.SearchTypeName = searchType ?? "name"; // Default to "name"

            return View(titleList);
        }


        // GET: TitleMVC/Details/5
        public async Task<ActionResult> Details(string id)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri(baseURL);

                    HttpResponseMessage response = await client.GetAsync($"titles/{id}");
                    response.EnsureSuccessStatusCode();

                    string responseData = await response.Content.ReadAsStringAsync();
                    var title = JsonConvert.DeserializeObject<Title>(responseData);

                    if (title == null)
                    {
                        return HttpNotFound();
                    }

                    return View(title);
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

        // GET: TitleMVC/Create
        [Authorize(Roles = "Publisher")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: TitleMVC/Create
        [Authorize(Roles = "Publisher")]
        [HttpPost]
        public ActionResult Create(Title obj)
        {
            using (var client = new HttpClient())
            {
                // Url of Webapi project
                client.BaseAddress = new Uri(baseURL);
                //HTTP POST
                var postTask = client.PostAsJsonAsync<Title>("titles/post", obj);
                // wait for task to complete
                postTask.Wait();
                // retrieve the result
                var result = postTask.Result;
                // check the status code for success
                if (result.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Title created successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Server Error. Please contact administrator.";
                }  
            }
            return View(obj);
        }

        // GET: TitleMVC/Edit/5
        [Authorize(Roles = "Publisher")]
        public async Task<ActionResult> Edit(string id)
        {
            Title titleOBJ = null;

            using (var client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri(baseURL); // Ensure baseURL is set correctly

                    HttpResponseMessage response = await client.GetAsync($"titles/{id}");
                    if (response.IsSuccessStatusCode)
                    {
                        titleOBJ = await response.Content.ReadAsAsync<Title>();
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

            if (titleOBJ == null)
            {
                return HttpNotFound();
            }
            return View(titleOBJ);
        }


        // POST: TitleMVC/Edit/5
        [Authorize(Roles = "Publisher")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string id, Title title)
        {
            if (!ModelState.IsValid)
            {
                return View(title);
            }

            using (var client = new HttpClient())
            {
                try
                {
                    title.title_id = id;
                    client.BaseAddress = new Uri(baseURL); // Ensure baseURL is set correctly

                    HttpResponseMessage response = await client.PutAsJsonAsync($"titles/" + title.title_id, title);
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


        // GET: TitlesMVC/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            Title title = null;

            using (var client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri(baseURL); // Ensure baseURL is set correctly

                    HttpResponseMessage response = await client.GetAsync($"titles/{id}");
                    if (response.IsSuccessStatusCode)
                    {
                        title = await response.Content.ReadAsAsync<Title>();
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

            if (title == null)
            {
                return HttpNotFound();
            }

            return View(title);
        }

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

                    HttpResponseMessage response = await client.DeleteAsync($"titles/{id}");
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
