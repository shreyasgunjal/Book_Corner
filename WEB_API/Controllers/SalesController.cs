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
    public class SalesController : ApiController
    {
        private pubsEntities db = new pubsEntities();

        // GET: api/sales
        [HttpGet]
         public IQueryable<sale> Getsales()
         {
             return db.sales;
         }

        
        [HttpGet]
         [Route("api/sales/{id}")]
      
        public IHttpActionResult GetSale(string id)
        {
            // Find the sale record by order number (assuming `id` corresponds to `ord_num`)
            // var sale = db.sales.FirstOrDefault(s => s.ord_num == id);
            var sale = db.sales.Where(s => s.ord_num == id).ToList();
            //var sale = db.sales.Find(id);

            if (sale == null)
            {
                return NotFound();
            }

            return Ok(sale);
        }

        [HttpGet]
        [Route("api/sales/orderandtitle")]
        public IHttpActionResult GetSalesByTitleId(string id, string titleId)
        {
            // Query to filter sales by title_id
            var records = db.sales.FirstOrDefault(r => r.ord_num == id && r.title_id == titleId);
            if (records == null)
            {
                return NotFound();
            }

            return Ok(records);
        }


        [HttpGet]
        [Route("api/sales/titleid/{id}")]

        //public IQueryable<sale> GetSalesByTitleId(string id)
        public IHttpActionResult GetSalesByTitleId(string id)
        {
            // Query to filter sales by title_id
            var records = db.sales.Where(r => r.title_id == id);
            if (!records.Any())
            {
                return NotFound();
            }

            return Ok(records);
        }

        [HttpGet]
        [Route("api/sales/orderdate/{odate}")]
        public IHttpActionResult GetSalesByOrderDate(DateTime odate)
        {
            var startOfDay = odate.Date;
            var endOfDay = startOfDay.AddDays(1).AddTicks(-1);
            // Find records by OrderDate
            var records = db.sales.Where(r => r.ord_date >= startOfDay && r.ord_date <= endOfDay);
            if (!records.Any())
            {
                return NotFound();
            }

            return Ok(records);
        }

        [HttpGet]
        [Route("api/sales/storeid/{id}")]
        public IHttpActionResult GetSalesByStoreId(string id)
        {
            // Find records by StoreId
            var records = db.sales.Where(r => r.stor_id == id);
            if (!records.Any())
            {
                return NotFound();
            }

            return Ok(records);
        }


                         




        // POST: api/sales
        [HttpPost]
        [ResponseType(typeof(sale))]
        //job_id is Identity COloun so we dont gives job_id while posting the data because identity coloumn automatically updates the value of that coloumn
        public IHttpActionResult Postsale(sale sale)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); //original
            }
            db.sales.Add(sale);

            try
            {
                db.SaveChanges();
            }

            catch (DbUpdateException) 
            {
                var errorResponse = new
                {
                    timeStamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    message = "Validation Failed..." //"An error occurred while processing your request. The entity might already exist or there was a database issue."
                };

                // Check if the entity already exists or other specific constraints
                if (saleExists(sale.stor_id))  // Adjust condition based on your entity
                {
                    return Content(HttpStatusCode.Conflict, errorResponse); // HTTP 409 Conflict
                }
                
            }
            return Ok("Record Created Succssufully");

           

        }

        

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool saleExists(string id)
        {
            return db.sales.Count(e => e.stor_id == id) > 0;
        }

        // DELETE: api/sales/5
        #region Delete 
        [HttpDelete]
        [ResponseType(typeof(sale))]
        [Route("api/sales/Delete")]
        public IHttpActionResult Deletesale(string id, string titleId)
        {
            // Find the specific sale by order number and title id
            var sale = db.sales.FirstOrDefault(x => x.ord_num == id && x.title_id == titleId);

            if (sale == null)
            {
                return NotFound();
            }

            // Remove a single sale object
            db.sales.Remove(sale);
            db.SaveChanges();

            return Ok(sale);
        }

        #endregion




        // PUT: api/sales/5
        [HttpPut]
        [Route("api/sales/{id}")]
        [ResponseType(typeof(void))]
        public IHttpActionResult Putsale(string id, sale sale)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != sale.ord_num)
            {
                return BadRequest();
            }

            db.Entry(sale).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!saleExists(id))
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

        
    }
}