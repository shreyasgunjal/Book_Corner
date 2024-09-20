using MVC_Project.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MVC_Project.Controllers
{
    public class JobsController : Controller
    {
        string baseURL = "https://localhost:44386/api/";
        // GET: JobsMVC
        public ActionResult Index(string searchType = null, string searchValue = null)
        {
            IEnumerable<Job> jobList = null;
            Job singleJob = null; // Handle a single job object

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseURL);

                string apiUrl = "Jobs"; // Default URL for getting all jobs

                if (!string.IsNullOrEmpty(searchType) && !string.IsNullOrEmpty(searchValue))
                {
                    switch (searchType.ToLower())
                    {
                        case "id":
                            apiUrl = $"jobs/{searchValue}";
                            break;
                        case "minlvl":
                            apiUrl = $"jobs/minlvl/{searchValue}";
                            break;
                        case "maxlvl":
                            apiUrl = $"jobs/maxlvl/{searchValue}";
                            break;
                    }
                }

                // HTTP GET
                var responseTask = client.GetAsync(apiUrl);
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    if (searchType?.ToLower() == "id")
                    {
                        // Handle single job response for Job ID search
                        var readTask = result.Content.ReadAsAsync<Job>();
                        readTask.Wait();
                        singleJob = readTask.Result;

                        jobList = new List<Job> { singleJob }; // Convert to list for the view
                    }
                    else
                    {
                        var readTask = result.Content.ReadAsAsync<IList<Job>>();
                        readTask.Wait();
                        jobList = readTask.Result;
                    }
                }
                else
                {
                    jobList = Enumerable.Empty<Job>();
                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }

            ViewBag.SelectedValue = searchValue;
            ViewBag.SearchTypeName = searchType ?? "jobs";

            return View(jobList);
        }




        // GET: Job/Details/5
        public async Task<ActionResult> Details(string id)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri(baseURL);

                    HttpResponseMessage response = await client.GetAsync($"jobs/{id}");
                    response.EnsureSuccessStatusCode();

                    string responseData = await response.Content.ReadAsStringAsync();
                    var job = JsonConvert.DeserializeObject<Job>(responseData);

                    if (job == null)
                    {
                        return HttpNotFound();
                    }

                    return View(job);
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

        // GET: Job/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Job/Create
        [HttpPost]
        public ActionResult Create(Job obj)
        {
            using (var client = new HttpClient())
            {
                // Url of Webapi project
                client.BaseAddress = new Uri(baseURL);
                //HTTP POST
                var postTask = client.PostAsJsonAsync<Job>("Jobs", obj);
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
            //ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");
            // send the view back with model error
            return View(obj);
        }

        // GET: JobsMVC/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            Job jobOBJ = null;

            using (var client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri(baseURL); // Ensure baseURL is set correctly

                    HttpResponseMessage response = await client.GetAsync($"Jobs/{id}");
                    if (response.IsSuccessStatusCode)
                    {
                        jobOBJ = await response.Content.ReadAsAsync<Job>();
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

            if (jobOBJ == null)
            {
                return HttpNotFound();
            }


            return View(jobOBJ);

        }

        // POST: JobsMVC/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(short id, Job job)
        {
            if (!ModelState.IsValid)
            {
                return View(job);
            }

            using (var client = new HttpClient())
            {
                try
                {
                    job.job_id = id;
                    client.BaseAddress = new Uri(baseURL); // Ensure baseURL is set correctly

                    HttpResponseMessage response = await client.PutAsJsonAsync($"jobs/" + job.job_id, job);
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


        // GET: AuthorsMVC/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            Job job = null;

            using (var client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri(baseURL); // Ensure baseURL is set correctly

                    HttpResponseMessage response = await client.GetAsync($"jobs/{id}");
                    if (response.IsSuccessStatusCode)
                    {
                        job = await response.Content.ReadAsAsync<Job>();
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

            if (job == null)
            {
                return HttpNotFound();
            }

            return View(job);
        }


        
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

                    HttpResponseMessage response = await client.DeleteAsync($"jobs/Delete/{id}");
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
