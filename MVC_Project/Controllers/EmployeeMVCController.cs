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
    public class EmployeeMVCController : Controller
    {
        string baseURL = "https://localhost:44386/api/";

        [Authorize(Roles ="Publisher")]
        public ActionResult Index(string searchType = null, string searchValue = null)
        {
            IEnumerable<Employee> EmpList = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseURL);

                // Build the API URL based on the search type and value
                string apiUrl = "employees"; // Default URL for getting all publishers

                if (!string.IsNullOrEmpty(searchType) && !string.IsNullOrEmpty(searchValue))
                {
                    switch (searchType.ToLower())
                    {
                        case "emp_id":
                            apiUrl = $"employees/{searchValue}";
                            break;
                        case "pubid":
                            apiUrl = $"employees/pubid/{searchValue}";
                            break;
                        case "fname":
                            apiUrl = $"employees/fname/{searchValue}";
                            break;
                        case "lname":
                            apiUrl = $"employees/lname/{searchValue}";
                            break;
                        case "hiredate":
                    // Ensure the date format is acceptable by the API (e.g., "yyyy-MM-dd")
                    DateTime hiredate;
                            if (DateTime.TryParse(searchValue, out hiredate))
                            {
                                string formattedDate = hiredate.ToString("yyyy-MM-dd");
                                apiUrl = $"employees/hiredate/{searchValue}";
                                //apiUrl = $"employees/hiredate/{formattedDate}";
                            }
                            else
                            {
                                ModelState.AddModelError(string.Empty, "Invalid date format.");
                                return View(Enumerable.Empty<Employee>());
                            }
                            break;
                             
                    }
                }

                // HTTP GET
                var responseTask = client.GetAsync(apiUrl);
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<Employee>>();
                    readTask.Wait();
                    EmpList = readTask.Result;
                }
                else
                {
                    EmpList = Enumerable.Empty<Employee>();
                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }

            // Set ViewBag properties for use in the view
            ViewBag.SelectedValue = searchValue;
            ViewBag.SearchTypeName = searchType ?? "name"; // Default to "name"

            return View(EmpList);
        }

        // GET: EmployeeMVC/Details/5
        public async Task<ActionResult> Details(string id)
        {
            //return View();
           using (var client = new HttpClient())
                {
                    try
                    {
                        client.BaseAddress = new Uri(baseURL);

                        HttpResponseMessage response = await client.GetAsync($"employees/{id}");
                        response.EnsureSuccessStatusCode();

                        string responseData = await response.Content.ReadAsStringAsync();
                        var Employee = JsonConvert.DeserializeObject<Employee>(responseData);

                        if (Employee == null)
                        {
                            return HttpNotFound();
                        }

                        return View(Employee);
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
        
    

        // GET: EmployeeMVC/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: EmployeeMVC/Create
        [HttpPost]
        public ActionResult Create(Employee obj)
        {
            using (var client = new HttpClient())
            {
                // Url of Webapi project
                client.BaseAddress = new Uri(baseURL);
                //HTTP POST
                var postTask = client.PostAsJsonAsync<Employee>("Employees",obj);
                // wait for task to complete
                postTask.Wait();
                // retrieve the result
                var result = postTask.Result;
                // check the status code for success
                if (result.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Employee details created successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Server Error. Please contact administrator.";
                }
            }
            // Add model error
            //ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");
            // send the view back with model error
            return View(obj);

        }

        // GET: PublisherMVC/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            Employee empOBJ = null;

            using (var client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri(baseURL); // Ensure baseURL is set correctly

                    HttpResponseMessage response = await client.GetAsync($"employees/{id}");
                    if (response.IsSuccessStatusCode)
                    {
                        empOBJ = await response.Content.ReadAsAsync<Employee>();
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

            if (empOBJ == null)
            {
                return HttpNotFound();
            }


            return View(empOBJ);

        }


        // POST: EmployeeMVC/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string id, Employee employee)
        {
            if (!ModelState.IsValid)
            {
                return View(employee);
            }

            using (var client = new HttpClient())
            {
                try
                {
                    employee.emp_id = id;
                    client.BaseAddress = new Uri(baseURL); // Ensure baseURL is set correctly

                    HttpResponseMessage response = await client.PutAsJsonAsync($"employees/" + employee.emp_id, employee);
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

       
        // GET: EmployeesMVC/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            Employee employee = null;

            using (var client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri(baseURL); // Ensure baseURL is set correctly

                    HttpResponseMessage response = await client.GetAsync($"employees/{id}");
                    if (response.IsSuccessStatusCode)
                    {
                        employee = await response.Content.ReadAsAsync<Employee>();
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

            if (employee == null)
            {
                return HttpNotFound();
            }

            return View(employee);
        }

        // POST: EmployeeMVC/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri(baseURL); // Ensure baseURL is set correctly

                    HttpResponseMessage response = await client.DeleteAsync($"employees/{id}");
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
