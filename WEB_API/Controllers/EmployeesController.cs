using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Policy;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.UI.WebControls;
using WEB_API.Models;

namespace WEB_API.Controllers
{
    public class EmployeesController : ApiController
    {
        private pubsEntities db = new pubsEntities();

        // GET: api/employees
        [HttpGet]
        [Route("api/employees")]
        public IQueryable<employee> Getemployees()
        {
            return db.employees;
        }

        // GET: api/employees/5
        [ResponseType(typeof(employee))]
        [Route("api/employees/{id}")]
        [HttpGet]
        public IHttpActionResult GetEmployeeById(string id)
        {
            var employee = db.employees.Find(id);
            if (employee == null)
            {
                return NotFound();
            }

            return Ok(employee);
        }
        

        [HttpGet]
        [Route("api/employees/pubid/{id}")]
        public IHttpActionResult Getemployeebypubid(string id)
        {
            var employee = db.employees.Where(a=>a.pub_id.Contains(id)).ToList();
            if (employee == null)
            {
                return NotFound();
            }

            return Ok(employee);
        }


        [HttpGet]
        [Route("api/employees/fname/{f_name}")]
        public IHttpActionResult Getemployeebyfname(string f_name)
        {
            var employee = db.employees.Where(a => a.fname.Contains(f_name)).ToList();
            if (employee == null)
            {
                return NotFound();
            }

            return Ok(employee);
        }

        [HttpGet]
        [Route("api/employees/lname/{l_name}")]
        public IHttpActionResult Getemployeebylname(string l_name)
        {
            var employee = db.employees.Where(a => a.lname.Contains(l_name)).ToList();
            if (employee == null)
            {
                return NotFound();
            }

            return Ok(employee);
        }
      
        [HttpGet]
        [Route("api/employees/hiredate/{hiredate}")]

        public IHttpActionResult GetemployeesByHiredate(DateTime hiredate)
        {
            var employeesByHiredate = db.employees.Where(t => t.hire_date == hiredate).ToList();

            if (employeesByHiredate == null || !employeesByHiredate.Any())
            {
                return NotFound();
            }
            return Ok(employeesByHiredate);
        }
        

        // PUT: api/employees/5
        [ResponseType(typeof(void))]
        [HttpPut]
        [Route("api/employees/{id}")]
        public IHttpActionResult Putemployee(string id, employee employee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != employee.emp_id)
            {
                var errorResponse = new
                {
                    timeStamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    message = "Validation failed..."
                };
                return Content(HttpStatusCode.Conflict, errorResponse); // HTTP 409 Conflict
            }

            db.Entry(employee).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!employeeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return StatusCode(HttpStatusCode.NoContent);          
        }

        //PATCH Method
        [HttpPatch]
        [ResponseType(typeof(void))]
        [Route("api/employees/{id}")]
        public IHttpActionResult Patchemployee(string id, [FromBody] Dictionary<string, object> updates)
        {
            if (string.IsNullOrEmpty(id) || updates == null || !updates.Any())
            {
                return BadRequest("Invalid request.");
            }

            var employee = db.employees.Find(id);
            if (employee == null)
            {
                return Content(HttpStatusCode.BadRequest, new
                {

                    timestamp = DateTime.Now.ToString("yyyy-MM-dd"),
                    message="Validation failed..."

                });
            }

            // Apply the updates
            foreach (var update in updates)
            {
                var property = typeof(employee).GetProperty(update.Key);
                if (property != null && property.CanWrite)
                {
                    // Ensure type conversion is safe
                    try
                    {
                        var value = Convert.ChangeType(update.Value, property.PropertyType);
                        property.SetValue(employee, value);
                    }
                    catch (InvalidCastException)
                    {
                        return BadRequest($"Invalid value for property {update.Key}.");
                    }
                }
                else
                {
                    return BadRequest($"Property {update.Key} does not exist on publisher.");
                }
            }

            try
            {
                db.Entry(employee).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!employeeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return StatusCode(HttpStatusCode.NoContent);
        }
       

        // POST: api/employees
        [ResponseType(typeof(employee))]
        [HttpPost]
        [Route("api/employees")]
        public IHttpActionResult Postemployee(employee employee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.employees.Add(employee);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                var errorResponse = new
                {
                    timeStamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    message = "Validation failed..."
                };

                // Check if the entity already exists or other specific constraints
                if (employeeExists(employee.emp_id))  // Adjust condition based on your entity
                {
                    return Content(HttpStatusCode.Conflict, errorResponse); // HTTP 409 Conflict
                }
               
            }
           

            //return CreatedAtRoute("DefaultApi", new { id = employee.emp_id }, employee);
            return Ok("Record Created Successfully");
        }

        //DELETE: api/employees/5
        [ResponseType(typeof(employee))]
        [Route("api/employees/{id}")]
        public IHttpActionResult Deleteemployee(string id)
        {
            employee employee = db.employees.Find(id);
            if (employee == null)
            {
                return NotFound();
            }

            db.employees.Remove(employee);
            db.SaveChanges();

            return Ok(employee);
        }

        // GET: api/employees/{id}
        #region Commented code
        //[HttpGet]
        //[Route("api/employees/{emp_id}")]
        //public IHttpActionResult Getemployeebyid(string emp_id)
        //{
        //    employee employee = db.employees.Find(emp_id);
        //    if (employee == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(employee);
        //}

        #endregion

        protected override void Dispose(bool disposing)   //release unmanaged packages
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool employeeExists(string id)  //checks whether an emp exists
        {
            return db.employees.Count(e => e.emp_id == id) > 0;
        }
    }
}