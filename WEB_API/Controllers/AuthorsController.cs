using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using WEB_API.Models;


namespace WEB_API.Controllers
{
    public class AuthorsController : ApiController
    {
        private pubsEntities db = new pubsEntities();


        // ADD NEW AUTHOR -- POST: api/authors/add  
        [HttpPost]
        [Route("api/authors/add")]
        public IHttpActionResult AddAuthor(author author)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.authors.Add(author);

            try
            {
                db.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                // Create a list to hold error details
                var errorMessages = ex.EntityValidationErrors
                    .SelectMany(x => x.ValidationErrors)
                    .Select(x => x.ErrorMessage);

                // Join the list to a single string and return it as a response
                var fullErrorMessage = string.Join("; ", errorMessages);
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                // Optionally log the exception or return the details in the response
                return Content(HttpStatusCode.BadRequest, new { message = exceptionMessage });
            }

            return Ok("Record Created Successfully");
        }


        // Get All authors      
        // GET: api/authors
        [HttpGet]
        [Route("api/authors")]
        public IQueryable<author> GetAllAuthors()
        {
            return db.authors;
        }

        // Search Films by lastname
        // Search authors by lastname 
        // /api/authors/lname/{ln} 
        [HttpGet]
        [Route("api/authors/lname/{ln}")]
        public IQueryable<author> GetAuthorsByLastName(string ln)
        {
            return db.authors.Where(p => p.au_lname == ln);
        }

        // Search Films by FirstName 
        // Search Films by LastName 
        // /api/authors/fname/{ln}  
        [HttpGet]
        [Route("api/authors/fname/{fn}")]
        public IQueryable<author> SearchAuthorsByFirstName(string fn)
        {
            return db.authors.Where(p => p.au_fname == fn);
        }

       

        //------- Method 2 using IQueryable --------------- 
        [HttpGet]
        [Route("api/authors/phone/{phno}")]
        public IQueryable<author> SearchAuthorsbyPhoneNumber(string phno)
        {
            return db.authors.Where(p => p.phone == phno).Take(1); // Returns as IQueryable
        }


       

        

        
        [HttpGet]
        [Route("api/authors/id/{id}")]
        public author GetAuthorById(string id)
        {
            return db.authors.Where(a => a.au_id == id).FirstOrDefault();
        }





        // /api/authors/zip/{zip} 
        // Search Films by zip 
        // Search Authors by ziP 
        [HttpGet]
        [Route ("api/authors/zip/{zip}")]
        public IQueryable<author> SearhAuthorsByZIP(string zip)
        {
            return db.authors.Where(p => p.zip == zip);
        }

        // Search Films by state 
        // /api/authors/state/{state} 
        [HttpGet]
        [Route ("api/authors/state/{state}")] 
        public IQueryable<author> SearchAuthorsByState(string state)
        {
            return db.authors.Where(p => p.state == state); 
        }

        // Search Authors by city 
        // /api/authors/city/{city} 
        [HttpGet]
        [Route ("api/authors/city/{city}")]
        //public IHttpActionResult SearchAuthorsByCity(string city)
        public IQueryable<author> SearchAuthorsByCity(string city)
        {
            return db.authors.Where(p => p.city == city);
        }





        // PATCH: api/authors/update/{id}
        //[HttpPatch]
        //[Route("api/authors/update/{id}")]
        //public IHttpActionResult PatchAuthor(string id, [FromBody] author author)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return Content(HttpStatusCode.BadRequest, new
        //        {
        //            timeStamp = DateTime.Now.ToString("yyyy-MM-dd"),
        //            message = "Validation failed"
        //        });
        //    }

        //    if (id != author.au_id)
        //    {
        //        return BadRequest();
        //    }

        //    db.Entry(author).State = EntityState.Modified;

        //    try
        //    {
        //        db.SaveChanges();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!authorExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return StatusCode(HttpStatusCode.NoContent);
        //}

        // PUT: api/authors/5
        //[Route("api/authors/update/{id}")]
        //[ResponseType(typeof(void))]
        //public IHttpActionResult Putauthor(string id, author author)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        var errorResponse = new
        //        {
        //            timeStamp = DateTime.UtcNow.ToString("yyyy-MM-dd"),
        //            message = "Validation failed. Please check the data provided.",
        //            errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
        //        };
        //        return Content(HttpStatusCode.BadRequest, errorResponse);
        //    }

        //    if (id != author.au_id)
        //    {
        //        var errorResponse = new
        //        {
        //            timeStamp = DateTime.UtcNow.ToString("yyyy-MM-dd"),
        //            message = "Author ID mismatch."
        //        };
        //        return Content(HttpStatusCode.BadRequest, errorResponse);
        //    }

        //    db.Entry(author).State = EntityState.Modified;

        //    try
        //    {
        //        db.SaveChanges();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!authorExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return StatusCode(HttpStatusCode.NoContent);
        //}
        [Route("api/authors/update/{id}")]
        [ResponseType(typeof(void))]
        public IHttpActionResult Putauthor(string id, author author)
        {
            if (!ModelState.IsValid)
            {
                var errorResponse = new
                {
                    timeStamp = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                    message = "Validation failed. Please check the data provided.",
                    errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
                };
                return Content(HttpStatusCode.BadRequest, errorResponse);
            }

            // Ensure that the ID in the URL and the ID in the author object match
            if (id != author.au_id)
            {
                // Instead of returning an error, we can set the ID in the author object
                author.au_id = id;
            }

            db.Entry(author).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!authorExists(id))
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


        // POST: api/authors
        //[ResponseType(typeof(author))]
        //public IHttpActionResult Postauthor(author author)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    db.authors.Add(author);

        //    try
        //    {
        //        db.SaveChanges();
        //    }
        //    catch (DbUpdateException)
        //    {
        //        if (authorExists(author.au_id))
        //        {
        //            return Conflict();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return CreatedAtRoute("DefaultApi", new { id = author.au_id }, author);
        //}

        // Patch Update the author details
        //[HttpPatch] 
        //[Route("api/authors/update/{id}")]
        //[ResponseType(typeof(author))]
        //public IHttpActionResult Patchauthor(string id, [FromBody] author author)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return Content(HttpStatusCode.BadRequest, new
        //        {
        //            timeStamp = DateTime.Now.ToString("yyyy-MM-dd"),
        //            message = "Validation failed",
        //            errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
        //        });
        //    }

        //    if (id != author.au_id)
        //    {
        //        return Content(HttpStatusCode.BadRequest, new
        //        {
        //            timeStamp = DateTime.Now.ToString("yyyy-MM-dd"),
        //            message = "ID in URL does not match ID in the author data"
        //        });
        //    }

        //    db.Entry(author).State = EntityState.Modified;

        //    try
        //    {
        //        db.SaveChanges();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!authorExists(id))
        //        {
        //            return Content(HttpStatusCode.NotFound, new
        //            {
        //                timeStamp = DateTime.Now.ToString("yyyy-MM-dd"),
        //                message = "Author not found"
        //            });
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return StatusCode(HttpStatusCode.NoContent);
        //}

        // PATCH: api/authors/update/{id} 
        
        [HttpPatch]
        [ResponseType(typeof(void))]
        [Route("api/authors/update/{id}")]
        public IHttpActionResult PatchAuthor(string id, [FromBody] Dictionary<string, object> updates)
        {
            try
            {
                if (string.IsNullOrEmpty(id) || updates == null || !updates.Any()) 
                {
                    throw new Exception("Validation failed.");
                }

                var author = db.authors.Find(id);
                if (author == null)
                {
                    throw new Exception("Validation failed.");
                }

                // Apply the updates
                foreach (var update in updates)
                {
                    var property = typeof(author).GetProperty(update.Key);
                    if (property != null && property.CanWrite)
                    {
                        // Ensure type conversion is safe
                        try
                        {
                            var value = Convert.ChangeType(update.Value, property.PropertyType);
                            property.SetValue(author, value);
                        }
                        catch (InvalidCastException)
                        {
                            throw new Exception("Validation failed.");
                        }
                    }
                    else
                    {
                        throw new Exception("Validation failed.");
                    }
                }

                db.Entry(author).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception)
            {
                var errorResponse = new
                {
                    timeStamp = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                    message = "Validation failed."
                };
                return Content(HttpStatusCode.BadRequest, errorResponse);
            }

            return StatusCode(HttpStatusCode.NoContent);
        }


        // --------------------------------------EXTRAS -----------------------------------

        // DELETE: api/authors/5
        [HttpDelete]
        [ResponseType(typeof(author))]
        [Route("api/authors/{id}")]
        public IHttpActionResult Deleteauthor(string id)
        {
            author del = db.authors.Find(id);
            if (del == null)
            {
                return NotFound();
            }
            else
            {
                db.authors.Remove(del);
                db.SaveChanges();
                return Ok(del);
            } 

        } 
            

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool authorExists(string id)
        {
            return db.authors.Count(e => e.au_id == id) > 0;
        }

    }
}

