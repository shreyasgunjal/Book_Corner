
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.UI.WebControls;
using WEB_API.Models;



namespace WEB_API.Controllers
{
    public class StoresController : ApiController
    {
        private pubsEntities db = new pubsEntities();

        // GET: api/stores
        [Route("api/Stores")]
        public IQueryable<store> Getstores()
        {
            return db.stores;
        }

        // GET: api/stores/5
        [ResponseType(typeof(store))]
        [HttpGet]
        [Route("api/Stores/{id}")]
        public IHttpActionResult GetStoresByID(string id)
        {
            var store = db.stores.Where(a => a.stor_id == id);
            if (store == null)
            {
                return NotFound();
            }

            return Ok(store);
        }

        // GET: api/stores/5
        [ResponseType(typeof(store))]
        [HttpGet]
        [Route("api/Stores/name/{name}")]
        public IHttpActionResult Getstore(string name)
        {
            var store = db.stores.Where(a => a.stor_name.Contains(name)).ToList();
            if (store == null)
            {
                return NotFound();
            }

            return Ok(store);
        }

        [ResponseType(typeof(store))]
        [HttpGet]
        [Route("api/Stores/city/{cityname}")]
        public IHttpActionResult GetstoresByCityName(string cityname)
        {
            var store = db.stores.Where(a => a.city.Contains(cityname)).ToList();
            if (store == null)
            {
                return NotFound();
            }

            return Ok(store);
        }

        [ResponseType(typeof(store))]
        [HttpGet]
        [Route("api/Stores/zip/{zipcode}")]
        public IHttpActionResult GetCitiesByZip(string zipcode)
        {
            var store = db.stores.Where(a => a.zip == zipcode);
            if (store == null)
            {
                return NotFound();
            }

            return Ok(store);
        }

        [HttpGet]
        [Route("api/Stores/id/{id}")]
        public IHttpActionResult GetCitiesByID(string id)
        {
            var store = db.stores.Where(a => a.stor_id == id);
            if (store == null)
            {
                return NotFound();
            }

            return Ok(store);
        }

        // PUT: api/stores/5
        [ResponseType(typeof(store))]
        [HttpPut]
        [Route("api/Stores/update/{id}")]
        public IHttpActionResult Putstore(string id, store store)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != store.stor_id)
            {
                var errorResponse = new
                {
                    timeStamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    message = "Validation Failed..."
                };
                return Content(HttpStatusCode.Conflict, errorResponse);
            }

            db.Entry(store).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!storeExists(id))
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


            // POST: api/stores
        [ResponseType(typeof(store))]
        [Route("api/Stores")]
        [HttpPost]
        public IHttpActionResult Poststore(store store)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.stores.Add(store);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                var errorResponse = new
                {
                    timeStamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    message = "Validation Failed..."
                };

                // Check if the entity already exists or other specific constraints
                if (storeExists(store.stor_id))  // Adjust condition based on your entity
                {
                    return Content(HttpStatusCode.Conflict, errorResponse); // HTTP 409 Conflict
                }
                
            }

            return Ok("Record Created Successfully");
        }


        //PATCH Method

        [HttpPatch]
        [ResponseType(typeof(void))]
        [Route("api/Stores/update/{id}")]
        public IHttpActionResult Patch(string id, [FromBody] Dictionary<string, object> updates)
        {
            if (string.IsNullOrEmpty(id) || updates == null || !updates.Any())
            {
                return BadRequest("Invalid request.");
            }


            var st = db.stores.Find(id);
            if (st == null)
            {
                var errorResponse = new
                {
                    timeStamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    message = "Validation Failed..."
                };
                return Content(HttpStatusCode.Conflict, errorResponse);
            }

            // Apply the updates
            foreach (var update in updates)
            {
                var property = typeof(store).GetProperty(update.Key);
                if (property != null && property.CanWrite)
                {
                    // Ensure type conversion is safe
                    try
                    {
                        var value = Convert.ChangeType(update.Value, property.PropertyType);
                        property.SetValue(st, value);
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
                db.Entry(st).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!storeExists(id))
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





        // DELETE: api/stores/5

        [ResponseType(typeof(store))]
        [HttpDelete]
        [Route("api/Stores/Delete/{id}")]

        public IHttpActionResult Deletestore(string id)
        {
            store store = db.stores.Find(id);
            if (store == null)
            {
                return NotFound();
            }

            db.stores.Remove(store);
            db.SaveChanges();

            return Ok(store);
        }







        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool storeExists(string id)
        {
            return db.stores.Count(e => e.stor_id == id) > 0;
        }
    }
}