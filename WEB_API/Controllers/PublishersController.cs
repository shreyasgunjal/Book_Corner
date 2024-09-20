using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Policy;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Results;
using System.Web.UI.WebControls;
//using System.Windows.Forms;
using WEB_API.Models;

namespace WEB_API.Controllers
{
    public class PublishersController : ApiController
    {
        private pubsEntities db = new pubsEntities();

        // POST: api/publishers

        //Allowed Primary Key : ([pub_id]='1756' OR [pub_id]='1622' OR [pub_id]='0877' OR [pub_id]='0736' OR [pub_id]='1389' OR [pub_id] like '99[0-9][0-9]')
        #region Add new publisher object to DB

        ///api/titles/post 
        [ResponseType(typeof(publisher))]
        [HttpPost]
        [Route("api/publishers/post")]

        public IHttpActionResult Posttitle(publisher publisher)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.publishers.Add(publisher);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                var errorResponse = new
                {
                    timeStamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    message = "Validation failed…"
                };

                // Check if the entity already exists or other specific constraints
                if (publisherExists(publisher.pub_id))  // Adjust condition based on your entity
                {
                    return Content(HttpStatusCode.Conflict, errorResponse); // HTTP 409 Conflict
                }
            }

            return Ok("Record Created Successfully");
        }

        #endregion

        // GET: api/publishers
        #region Collection of Publishers
        [Route("api/publishers")]
        public IQueryable<publisher> GetPublishers()
        {
            return db.publishers;
        }
        #endregion

        //GET: api/publishers/pubname/{name}
        #region Search Publisher by Pub Name
        [HttpGet]
        [Route("api/publishers/pubname/{name}")]
        [ResponseType(typeof(IEnumerable<publisher>))]
        public IHttpActionResult GetPublisherByName(string name)
        {
            // Search for publishers whose name contains the provided string (case-insensitive)
            var publishers = db.publishers
                               .Where(p => p.pub_name.Contains(name))
                               .ToList();

            if (publishers == null || !publishers.Any())
            {
                return NotFound();
            }

            return Ok(publishers);
        }
        #endregion

        // GET : api/publishers/city/{city}
        #region Search publisher by pub City
        [HttpGet]
        [Route("api/publishers/city/{city}")]
        [ResponseType(typeof(IEnumerable<publisher>))]
        public IHttpActionResult GetPublisherByCity(string city)
        {
            // Search for publishers whose city matches the provided string (case-insensitive)
            var publishers = db.publishers
                               .Where(p => p.city.Equals(city, StringComparison.OrdinalIgnoreCase))
                               .ToList();

            if (publishers == null || !publishers.Any())
            {
                return NotFound();
            }

            return Ok(publishers);
        }
        #endregion

        //GET: api/publishers/state/{stat}
        #region Search Publisher by pub State

        [HttpGet]
        [Route("api/publishers/state/{stat}")]
        [ResponseType(typeof(IEnumerable<publisher>))]
        public IHttpActionResult GetPublisherByState(string stat)
        {
            // Search for publishers whose state matches the provided string (case-insensitive)
            var publishers = db.publishers
                               .Where(p => p.state.Equals(stat, StringComparison.OrdinalIgnoreCase))
                               .ToList();

            if (publishers == null || !publishers.Any())
            {
                return NotFound();
            }

            return Ok(publishers);
        }
        #endregion

        // GET: api/publishers/country/{country}
        #region Search Publisher by pub Country
        [HttpGet]
        [Route("api/publishers/country/{country}")]
        [ResponseType(typeof(IEnumerable<publisher>))]
        public IHttpActionResult GetPublisherByCountry(string country)
        {
            // Search for publishers whose country matches the provided string (case-insensitive)
            var publishers = db.publishers
                               .Where(p => p.country.Equals(country, StringComparison.OrdinalIgnoreCase))
                               .ToList();

            if (publishers == null || !publishers.Any())
            {
                return NotFound();
            }

            return Ok(publishers);
        }
        #endregion

        // PUT: api/publishers/5
        #region Update Publishers Info By PUT
        [ResponseType(typeof(void))]
        [Route("api/publishers/{id}")]
        public IHttpActionResult Putpublisher(string id, publisher publisher)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != publisher.pub_id)
            {
                var errorResponse = new
                {
                    timeStamp = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                    message = "Author ID mismatch."
                };
                return Content(HttpStatusCode.BadRequest, errorResponse);
            }

            db.Entry(publisher).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!publisherExists(id))
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
        #endregion

        // PATCH: api/publishers/id
        #region Update Publishers Info By Patch
        //[HttpPatch]
        //[ResponseType(typeof(void))]
        //[Route("api/publishers/{id}")]
        //public IHttpActionResult Patchpublisher(string id, [FromBody] Dictionary<string, object> updates)
        //{
        //    if (string.IsNullOrEmpty(id) || updates == null || !updates.Any())
        //    {
        //        return BadRequest("Invalid request.");
        //    }

        //    var publisher = db.publishers.Find(id);
        //    if (publisher == null)
        //    {
        //        return NotFound();
        //    }

        //    // Apply the updates
        //    foreach (var update in updates)
        //    {
        //        var property = typeof(publisher).GetProperty(update.Key);
        //        if (property != null && property.CanWrite)
        //        {
        //            // Ensure type conversion is safe
        //            try
        //            {
        //                var value = Convert.ChangeType(update.Value, property.PropertyType);
        //                property.SetValue(publisher, value);
        //            }
        //            catch (InvalidCastException)
        //            {
        //                return BadRequest($"Invalid value for property {update.Key}.");
        //            }
        //        }
        //        else
        //        {
        //            return BadRequest($"Property {update.Key} does not exist on publisher.");
        //        }
        //    }

        //    try
        //    {
        //        db.Entry(publisher).State = EntityState.Modified;
        //        db.SaveChanges();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!publisherExists(id))
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

        [HttpPatch]
        [Route("api/publishers/{id}")]
        [ResponseType(typeof(void))]
        public IHttpActionResult Patchpublisher(string id, [FromBody] Dictionary<string, object> updates)
        {
            if (string.IsNullOrEmpty(id) || updates == null || !updates.Any())
            {
                return BadRequest("Invalid request.");
            }

            var publisher = db.publishers.Find(id);
            if (publisher == null)
            {
                return Content(HttpStatusCode.InternalServerError, new
                {
                    timeStamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    message = "Validation failed…"
                });
            }

            //try
            //{
            //    //ApplyUpdates(publisher, updates);
            //    db.SaveChanges();
            //}
            //catch (ArgumentException ex)
            //{
            //    return BadRequest(ex.Message);
            //}

            try
            {
                db.Entry(publisher).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!publisherExists(id))
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

        #endregion

        //PUT: api/pubinfo/id
        #region Update Pub into Object by PUT
        [HttpPut]
        [ResponseType(typeof(void))]
        [Route("api/pubinfo/{id}")]
        public IHttpActionResult UpdatePublisherInfo(string id, publisher publisher)
        {


            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != publisher.pub_id)
            {
                var errorResponse = new
                {
                    timeStamp = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                    message = "Validation Failed."
                };
                return Content(HttpStatusCode.BadRequest, errorResponse);
            }

            db.Entry(publisher).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {

                if (!publisherExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, new
                {
                    timeStamp = DateTime.Now.ToString("yyyy-MM-dd"),
                    message = $"Validation failed: {ex.Message}"
                });
            }

            return StatusCode(HttpStatusCode.NoContent);
        }
        #endregion

        //PATCH: api/pubinfo/id
        #region Update Pub Info Object By Patch
        //[Route("api/pubinfo/{id}")]
        //[ResponseType(typeof(void))]
        //[HttpPatch]
        //public IHttpActionResult PatchPubInfo(string id, Dictionary<string, object> patchData)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var publisher = db.publishers.Find(id);
        //    if (publisher == null)
        //    {
        //        return NotFound();
        //    }

        //    // Get all properties of the publisher object
        //    var publisherProperties = typeof(publisher).GetProperties();

        //    // Apply the patch by setting values from the patchData dictionary to the publisher object
        //    foreach (var key in patchData.Keys)
        //    {
        //        var propInfo = publisherProperties.FirstOrDefault(p => p.Name.Equals(key, StringComparison.OrdinalIgnoreCase));
        //        if (propInfo != null)
        //        {
        //            var value = Convert.ChangeType(patchData[key], propInfo.PropertyType);
        //            propInfo.SetValue(publisher, value);
        //        }
        //    }

        //    // Mark the entity as modified
        //    db.Entry(publisher).State = EntityState.Modified;

        //    try
        //    {
        //        db.SaveChanges();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        return Content(HttpStatusCode.InternalServerError, new { timeStamp = DateTime.UtcNow, message = "Validation failed…" });
        //    }

        //    return StatusCode(HttpStatusCode.NoContent);
        //}

        [HttpPatch]
        [Route("api/pubinfo/{id}")]
        [ResponseType(typeof(void))]
        public IHttpActionResult PatchPubInfo(string id, publisher updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var publisher = db.publishers.Find(id);
            if (publisher == null)
            {
                return Content(HttpStatusCode.InternalServerError, new
                {
                    timeStamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    message = "Validation failed…"
                });
            }

            // Apply updates directly
            if (updateDto.pub_name != null)
            {
                publisher.pub_name = updateDto.pub_name;
            }

            // Mark the entity as modified
            db.Entry(publisher).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Content(HttpStatusCode.InternalServerError, new
                {
                    timeStamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    message = "Validation failed…"
                });
            }

            return StatusCode(HttpStatusCode.NoContent);
        }



        #endregion

        #region Get Publishers By Id
        [Route("api/publishers/{id}")]
        [HttpGet]
        public IHttpActionResult GetPublisherById(string id)
        {
            var publisher = db.publishers.Find(id);
            if (publisher == null)
            {
                return NotFound();
            }

            return Ok(publisher);
        }
        #endregion

        // DELETE: api/titles/5
        #region Delete 
        [HttpDelete]

        [Route("api/publishers/{id}")]
        [ResponseType(typeof(publisher))]
        public IHttpActionResult Deletetitle(string id)
        {
            publisher publishers = db.publishers.Find(id);
            if (publishers == null)
            {
                return NotFound();
            }

            db.publishers.Remove(publishers);
            db.SaveChanges();

            return Ok(publishers);
        }
        #endregion


        [HttpPost]
        [Route("api/StoreRequests/{id}/accept")]
        [ResponseType(typeof(StoreRequest))]
        public IHttpActionResult AcceptRequest(int id)
        {
            var storeRequest = db.StoreRequests.Find(id);
            if (storeRequest == null)
            {
                return NotFound();
            }

            // Update status to accepted
            storeRequest.Status = "Accepted";
            db.Entry(storeRequest).State = EntityState.Modified;
            db.SaveChanges();

            return Ok(storeRequest);
        }

        [HttpPost]
        [Route("api/StoreRequests/{id}/delete")]
        [ResponseType(typeof(StoreRequest))]
        public IHttpActionResult DeleteRequest(int id)
        {
            var storeRequest = db.StoreRequests.Find(id);
            if (storeRequest == null)
            {
                return NotFound();
            }

            db.StoreRequests.Remove(storeRequest);
            db.SaveChanges();

            return Ok(storeRequest);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool publisherExists(string id)
        {
            return db.publishers.Count(e => e.pub_id == id) > 0;
        }
    }
}