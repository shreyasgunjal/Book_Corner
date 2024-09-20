using MVC_Project.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls.Expressions;

namespace MVC_Project.Controllers
{
    public class AuthorsMVCController : Controller
    {
        string baseURL = "https://localhost:44386/api/";     // base URL for accessing the Web APIs 
                                                             // GET: AuthorsMVC

        [Authorize(Roles ="Publisher,Store")]
        public ActionResult Index(string searchType = null, string searchValue = null)  // Get AuthorsMVC/Index 
        {
            IEnumerable<Author> authorList = null;     // Creating Author List to store the output 

            using (var client = new HttpClient())      // CReating a new instance or object of http client 
            {
                client.BaseAddress = new Uri(baseURL);         // set the base address for HTTP Requests 

                // Build the API URL based on the search type and value
                string apiUrl = "Authors"; // Default URL for getting all the authors 

                if (!string.IsNullOrEmpty(searchType) && !string.IsNullOrEmpty(searchValue)) 
                {
                    switch (searchType.ToLower())
                    {

                        case "id":
                            apiUrl = $"authors/id/{searchValue}"; 
                            break;

                        case "fname":
                            apiUrl = $"authors/fname/{searchValue}";
                            break;

                        case "lname":
                            apiUrl = $"authors/lname/{searchValue}";
                            break;
                            
                        case "phone":
                            apiUrl = $"authors/phone/{searchValue}";
                            break;

                        case "zip":
                            apiUrl = $"authors/zip/{searchValue}"; 
                            break; 

                        case "city":
                            apiUrl = $"authors/city/{searchValue}";
                            break;

                        case "state":
                            apiUrl = $"authors/state/{searchValue}";
                            break;

                        default:
                            //  fetch all authors 
                            apiUrl = "authors";
                            break;
                    }
                }

                // HTTP GET
                var responseTask = client.GetAsync(apiUrl);   // send get request to the constructed url 
                responseTask.Wait();             // Wait for the ResPoNse 

                var result = responseTask.Result;        
                if (result.IsSuccessStatusCode)         
                {
                    var readTask = result.Content.ReadAsAsync<IList<Author>>();
                    readTask.Wait();
                    authorList = readTask.Result;
                }
                else
                {
                    authorList = Enumerable.Empty<Author>();
                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }

            // Set ViewBag properties for use in the view
            ViewBag.SelectedValue = searchValue;
            ViewBag.SearchTypeName = searchType ?? "name"; // Default to "name"

            return View(authorList);
        }

        // GET: AuthorsMVC/Details/5
        public async Task<ActionResult> Details(string id)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri(baseURL);

                    HttpResponseMessage response = await client.GetAsync($"authors/id/{id}");
                    response.EnsureSuccessStatusCode();

                    string responseData = await response.Content.ReadAsStringAsync();
                    var author = JsonConvert.DeserializeObject<Author>(responseData);

                    if (author == null)
                    {
                        return HttpNotFound();
                    }

                    return View(author);
                }
                catch (HttpRequestException ex)
                {
                    //List<Author> authors = new List<Author>(); 
                    // Log the exception or handle as needed
                    ViewBag.ErrorMessage = $"Error fetching data: {ex.Message}";
                    //ViewData["ErrorMessage"] = authors;
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

        // GET: AuthorsMVC/Create 
        //[Authorize]
        public ActionResult Create()
        {
             return View();
        }

        // POST: AuthorsMVC/Create
        [HttpPost]
        // [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Author author)
        {
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseURL);

                    HttpResponseMessage response = await client.PostAsJsonAsync("authors/add", author);
                    if (response.IsSuccessStatusCode)
                    {
                        TempData["SuccessMessage"] = "Author created successfully!";

                        
                    //    return RedirectToAction("Index"); // Redirect to Index after successful creation
                    }
                    else
                    {
                        
                        TempData["ErrorMessage"] = "Server Error. Please contact administrator.";
                        
                        //ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                    }
                }
            }
            return View(author); // If validation fails, redisplay the form with the entered data
        }

        //PREVIOUS 
        // GET: PublisherMVC/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            Author authOBJ = null;

            using (var client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri(baseURL); // Ensure baseURL is set correctly

                    HttpResponseMessage response = await client.GetAsync($"authors/id/{id}");
                    if (response.IsSuccessStatusCode)
                    {
                        authOBJ = await response.Content.ReadAsAsync<Author>();
                        //TempData["SuccessMessage"] = "Author created successfully!";                         
                    }
                    else
                    {
                        // Handle non-success status codes
                        ViewBag.ErrorMessage = $"Error: {response.ReasonPhrase}";
                        return View("Error");
                        //TempData["ErrorMessage"] = "Server Error. Please contact administrator.";
                    }
                }
                catch (Exception ex)
                {
                    // Handle any exceptions that occur during the HTTP request
                    ViewBag.ErrorMessage = $"An error occurred: {ex.Message}";
                    return View("Error");
                }
            }

            if (authOBJ == null)
            {
                return HttpNotFound();
            }


            return View(authOBJ);

        }

        // NEED TO CHECK AGAIN 
        //public async Task<ActionResult> Edit(string id)
        //{
        //    Author authOBJ = null;

        //    using (var client = new HttpClient())
        //    {
        //        try
        //        {
        //            client.BaseAddress = new Uri(baseURL);

        //            HttpResponseMessage response = await client.GetAsync($"authors/id/{id}");
        //            if (response.IsSuccessStatusCode)
        //            {
        //                authOBJ = await response.Content.ReadAsAsync<Author>();
        //            }
        //            else
        //            {
        //                ViewBag.ErrorMessage = $"Error: {response.ReasonPhrase}";
        //                return View("Error");
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            ViewBag.ErrorMessage = $"An error occurred: {ex.Message}";
        //            return View("Error");
        //        }
        //    }

        //    if (authOBJ == null)
        //    {
        //        return HttpNotFound();
        //    }

        //    TempData["SuccessMessage"] = "Author updated successfully!";
        //    return View(authOBJ);
        //}

        //[HttpPost]
        //public async Task<ActionResult> Edit(Author model)
        //{
        //    using (var client = new HttpClient())
        //    {
        //        client.BaseAddress = new Uri(baseURL);

        //        try
        //        {
        //            HttpResponseMessage response = await client.PutAsJsonAsync($"authors/id/{model.au_id}", model);
        //            if (response.IsSuccessStatusCode)
        //            {
        //                TempData["SuccessMessage"] = "Author updated successfully!";
        //                return RedirectToAction("Index");
        //            }
        //            else
        //            {
        //                TempData["ErrorMessage"] = "Failed to update the author. Please try again.";
        //                return View(model);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
        //            return View(model);
        //        }
        //    }
        //}


        // POST: PublisherMVC/Edit/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Edit(string id, Author author)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(author);
        //    }

        //    using (var client = new HttpClient())
        //    {
        //        try
        //        {
        //            author.au_id = id;
        //            client.BaseAddress = new Uri(baseURL); // Ensure baseURL is set correctly

        //            HttpResponseMessage response = await client.PutAsJsonAsync($"authors/" + author.au_id, author);
        //            if (response.IsSuccessStatusCode)
        //            {
        //                return RedirectToAction("Index");
        //            }
        //            else
        //            {
        //                ViewBag.ErrorMessage = $"Error: {response.ReasonPhrase}";
        //                return View("Error");
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            ViewBag.ErrorMessage = $"An error occurred: {ex.Message}";
        //            return View("Error");
        //        }
        //    }
        //}


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string id, Author author)
        {
            if (!ModelState.IsValid)
            {
                return View(author);
            }
            using (var client = new HttpClient())
            {
                try
                {
                    author.au_id = id;
                    client.BaseAddress = new Uri(baseURL); // Ensure baseURL is set correctly

                    HttpResponseMessage result = await client.PutAsJsonAsync($"authors/update/" + author.au_id, author);
                    if (result.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.ErrorMessage = $"Error: {result.ReasonPhrase}";
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



        // GET: AuthorsMVC/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}
        // GET: AuthorsMVC/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            Author author = null;

            using (var client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri(baseURL); // Ensure baseURL is set correctly

                    HttpResponseMessage response = await client.GetAsync($"authors/id/{id}");
                    if (response.IsSuccessStatusCode)
                    {
                        author = await response.Content.ReadAsAsync<Author>();
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

            if (author == null)
            {
                return HttpNotFound();
            }

            return View(author);
        }


        // POST: AuthorsMVC/Delete/5
        //[HttpPost]
        //public ActionResult Delete(int id, FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add delete logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //} 
        // POST: AuthorsMVC/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri(baseURL); // Ensure baseURL is set correctly

                    HttpResponseMessage response = await client.DeleteAsync($"authors/{id}");
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

        //// GET 
        //public ActionResult SearchByFirstName()
        //{
        //    return View();
        //}

        //// POST: AuthorsMVC/SearchByFirstName
        //[HttpPost]
        //public async Task<ActionResult> SearchByFirstName(string firstName)
        //{
        //    if (string.IsNullOrEmpty(firstName))
        //    {
        //        ModelState.AddModelError("", "First Name is required");
        //        return View();
        //    }

        //    IEnumerable<Author> authors = new List<Author>();

        //    using (var client = new HttpClient())
        //    {
        //        client.BaseAddress = new Uri(baseURL);
        //        HttpResponseMessage response = await client.GetAsync($"api/authors/fname/{firstName}");

        //        if (response.IsSuccessStatusCode)
        //        {
        //            var responseData = await response.Content.ReadAsStringAsync();
        //            authors = JsonConvert.DeserializeObject<IEnumerable<Author>>(responseData);
        //        }
        //        else
        //        {
        //            ViewBag.ErrorMessage = "No authors found with that first name.";
        //            return View("SearchResults", authors); // Return empty view if no results
        //        }
        //    }

        //    return View("SearchResults", authors); // Pass authors to the SearchResults view
        //}


    }


}

