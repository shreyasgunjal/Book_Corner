using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using WEB_API.Models;

namespace WEB_API.Controllers
{
    public class StoreRequestsController : ApiController
    {
        private pubsEntities db = new pubsEntities();



        // GET: api/StoreRequests/5
        [HttpGet]
        [Route("api/StoreRequests")]
        [ResponseType(typeof(IEnumerable<StoreRequest>))]
        public IHttpActionResult GetStoreRequests()
        {
            try
            {
                var requests = db.StoreRequests.ToList();
                return Ok(requests);
            }
            catch (Exception ex)
            {
                // Log the exception
                return InternalServerError(ex);
            }
        }


        [HttpPost]
        [Route("api/StoreRequests")]
        [ResponseType(typeof(StoreRequest))]
        public IHttpActionResult PostStoreRequest(StoreRequest storeRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Example to set default values if needed
            storeRequest.Status = "Pending";
            storeRequest.RequestDate = DateTime.Now;

            db.StoreRequests.Add(storeRequest);
            db.SaveChanges();

            return Ok(storeRequest);
        }





        // PUT: api/StoreRequests/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutStoreRequest(int id, StoreRequest storeRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != storeRequest.RequestId)
            {
                return BadRequest();
            }

            db.Entry(storeRequest).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StoreRequestExists(id))
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

        //DELETE: api/StoreRequests/5
        [HttpDelete]
        [ResponseType(typeof(object))]  // Specify the response type as object or a custom DTO
        public IHttpActionResult DeleteStoreRequest(int id)
        {
            StoreRequest storeRequest = db.StoreRequests.Find(id);
            string Id = storeRequest.TitleId;
            if (storeRequest == null)
            {
                return NotFound();
            }

            db.StoreRequests.Remove(storeRequest);
            db.SaveChanges();

            // Return only the fields you want to expose, including TitleId
            return Ok(Id);
            
        }


        [HttpPost]
        [Route("api/StoreRequests/Reject/{id}")]
        public IHttpActionResult RejectRequest(int id)
        {
            // Find the store request by id
            var storeRequest = db.StoreRequests.Find(id);
            if (storeRequest == null)
            {
                return NotFound(); // Return not found if the store request doesn't exist
            }

            // Find the title using the title_id from the storeRequest
            var title = db.titles.Find(storeRequest.TitleId); // Assuming Titles table has the TitleId column as primary key
            if (title == null)
            {
                return NotFound(); // Return not found if the title doesn't exist
            }

            // Remove the store request as well
            db.StoreRequests.Remove(storeRequest);
            db.titles.Remove(title);
            db.SaveChanges();

            // Return the titleId so that the client can use it to delete the title
            return Ok(new { TitleId = storeRequest.TitleId });
        }


        private bool StoreRequestExists(int id)
        {
            return db.StoreRequests.Count(e => e.RequestId == id) > 0;
        }
    }
}
