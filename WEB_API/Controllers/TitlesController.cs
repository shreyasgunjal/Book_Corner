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
using WEB_API.Models;

namespace WEB_API.Controllers
{
    public class TitlesController : ApiController
    {
        private pubsEntities db = new pubsEntities();

        // GET: api/titles
        #region Get all titles
        [Route("api/titles")]
        public IQueryable<title> Gettitles()
        {
            return db.titles;
        }
        #endregion


        // GET: api/titles/5
        #region Search titles by title

        [ResponseType(typeof(title))]

        [HttpGet]
        [Route("api/titles/title/{title}")]

        public IHttpActionResult GetTitleByTitle(string title)
        {
            var title2 = db.titles.Where(t => t.title1.Contains(title)).ToList();

            if (title2 == null)
            {
                return NotFound();
            }

            return Ok(title2);
        }
        #endregion


        // GET: api/titles/type/{type}
        #region Search titles by type

        [HttpGet]
        [Route("api/titles/type/{type}")]
      
        public IHttpActionResult GetTitlesByType(string type)
        {
            var titlesByType = db.titles.Where(t => t.type == type).ToList();

            if (titlesByType == null)
            {
                return NotFound();
            }

            return Ok(titlesByType);
        }
        #endregion


        // GET: api/titles/pubid/{pubid}
        #region Search title by pub-id
        [HttpGet]
        [Route("api/titles/pubid/{pubid}")]
        public IHttpActionResult GetTitlesByPubId(string pubid)
        {
            var titlesByPubid = db.titles.Where(t => t.pub_id == pubid).ToList();

            if (titlesByPubid == null || !titlesByPubid.Any())
            {
                return NotFound();
            }
            return Ok(titlesByPubid);
        }
        #endregion


        // GET: api/titles/pubdate/{pubdate}
        #region Search title by pub-date

        [HttpGet]
        [Route("api/titles/pubdate/{pubdate}")]

        public IHttpActionResult GetTitlesByPubDate(DateTime pubdate)
        {
            var titlesByPubdate = db.titles.Where(t => t.pubdate == pubdate).ToList();

            if (titlesByPubdate == null || !titlesByPubdate.Any())
            {
                return NotFound();
            }
            return Ok(titlesByPubdate);
        }
        #endregion


        // GET: api/titles/authorname/{name}
        #region Search title by authorname

        [HttpGet]

        [Route("api/titles/authorname/{name}")]
        public IHttpActionResult GetTitlesByAuthorName(string name)
        {
            var titlesByAuthorName = db.titleauthors
                           .Where(ta => ta.author.au_fname.Contains(name) || ta.author.au_lname.Contains(name))
                           .Select(ta => ta.title)
                           .ToList();
       
            if (titlesByAuthorName == null || !titlesByAuthorName.Any())
            {
                return NotFound();
            }

            return Ok(titlesByAuthorName);
        }
        #endregion


        // GET: api/titles/top5titles
        #region Search top 5 title by ytd

        [HttpGet]
        [Route("api/titles/top5titles")]
        public IHttpActionResult GetTop5TitlesByYtdSales()
        {
            var top5Titles = db.titles
                               .OrderByDescending(t => t.ytd_sales)
                               .Take(5)
                               .ToList();

            if (top5Titles == null || !top5Titles.Any())
            {
                return NotFound();
            }

            return Ok(top5Titles);
        }
        #endregion


        // PUT: api/titles/5
        #region Update all details
        ///api/titles/{id}
        [ResponseType(typeof(void))]

        [HttpPut]
        [Route("api/titles/{id}")]
        public IHttpActionResult Puttitle(string id, title title)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != title.title_id)
            {
                var errorResponse = new
                {
                    timeStamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    message = "Validation failed…"
                };
                return Content(HttpStatusCode.Conflict, errorResponse); // HTTP 409 Conflict
            }

            db.Entry(title).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)

            {

                if (!titleExists(id))

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


        // POST: api/titles
        #region Add new title Object in DB

        ///api/titles/post 
        [ResponseType(typeof(title))]
        [HttpPost]
        [Route("api/titles/post")]

        public IHttpActionResult Posttitle(title title)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.titles.Add(title);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException )
            {
                var errorResponse = new
                {
                    timeStamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    message = "Validation failed…"
                };

                // Check if the entity already exists or other specific constraints
                if (titleExists(title.title_id))  // Adjust condition based on your entity
                {
                    return Content(HttpStatusCode.Conflict, errorResponse); // HTTP 409 Conflict
                }
            }

            return Ok("Record Created Successfully");
        }
        #endregion


        // POST: api/authortitles/post
        #region Add new AuthortitleObject in DB

        [HttpPost]
        [Route("api/authortitles/post")]

        //Rquired Data for authortitles
        //{
        //"au_id": "238-95-7766",
        //"title_id": "BU7832",
        //"au_ord": 2,
        //"royaltyper": 40
        //}

        ///api/authortitles/post 

        public IHttpActionResult PostAuthorTitle([FromBody] titleauthor newAuthorTitle)
        {
            //// Step 2: Check if the provided model is valid
            //if (!ModelState.IsValid)
            //{
            //    return Content(HttpStatusCode.BadRequest, new
            //    {
            //        timeStamp = DateTime.Now.ToString("yyyy-MM-dd"),
            //        message = "Validation failed. Please ensure all required fields are correct."
            //    });
            //}

            // Step 3: Check if the referenced title_id exists in the titles table
            var existingTitle = db.titles.SingleOrDefault(t => t.title_id == newAuthorTitle.title_id);
            if (existingTitle == null)
            {
                return NotFound();  // Return 404 if the title isn't found
            }

            // Step 4: Add the new titleauthor object to the database
            try
            {
                db.titleauthors.Add(newAuthorTitle);
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
                if (titleExists(newAuthorTitle.title_id))  // Adjust condition based on your entit
                {
                    return Content(HttpStatusCode.Conflict, errorResponse); // HTTP 409 Conflict
                }

                //This comment default check from above post mehtod
                //if (!titleExists(newAuthorTitle.title_id))
                //{
                //    return NotFound();  // Return 404 if the title isn't found
                //}
                else
                {
                    // Log the exception
                    // For demonstration purposes, we include the original exception details in the response
                    var detailedErrorResponse = new
                    {
                        timeStamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                        message = "Validation Failed",
                    };

                    return Content(HttpStatusCode.InternalServerError, detailedErrorResponse); // HTTP 500 Internal Server Error
                }
            }

            return Ok("Record Created Successfully");
        }
        //    catch (Exception ex)
        //    {
        //        return InternalServerError(ex);  // Handle any errors that occur during saving
        //    }

        //    // Step 5: Return success message
        //    return Ok("Record Created Successfully");
        //}
        #endregion


        // PATCH: /api/titles/{id}
        #region Update specific details

        [HttpPatch]

        [Route("api/titles/{id}")]
        //[Route("api/titles/{id}")]
        ///api/titles/{id}
        ///
        public IHttpActionResult PatchTitle(string id, [FromBody] title updatedFields)
        {
            // Step 2: Check if the provided model is valid

            //Added this TimeStamp in CATCH Block

            if (!ModelState.IsValid)
            {
                return Content(HttpStatusCode.BadRequest, new
                {
                    timeStamp = DateTime.Now.ToString("yyyy-MM-dd"),
                    message = "Validation failed."
                });
            }

            // Step 3: Find the existing title by id
            var existingTitle = db.titles.SingleOrDefault(t => t.title_id == id);
            if (existingTitle == null)
            {
                return Content(HttpStatusCode.BadRequest, new
                {
                    timeStamp = DateTime.Now.ToString("yyyy-MM-dd"),
                    message = "Validation failed."
                }); ;  // Return 404 if the title isn't found
            }

            // Step 4: Update specific fields of the existing title
            if (updatedFields.title1 != null)
            {
                existingTitle.title1 = updatedFields.title1;
            }
            if (updatedFields.type != null)
            {
                existingTitle.type = updatedFields.type;
            }
            if (updatedFields.pub_id != null)
            {
                existingTitle.pub_id = updatedFields.pub_id;
            }
            if (updatedFields.price.HasValue)
            {
                existingTitle.price = updatedFields.price;
            }
            if (updatedFields.advance.HasValue)
            {
                existingTitle.advance = updatedFields.advance;
            }
            if (updatedFields.royalty.HasValue)
            {
                existingTitle.royalty = updatedFields.royalty;
            }
            if (updatedFields.ytd_sales.HasValue)
            {
                existingTitle.ytd_sales = updatedFields.ytd_sales;
            }
            if (updatedFields.notes != null)
            {
                existingTitle.notes = updatedFields.notes;
            }
            if (updatedFields.pubdate != default(DateTime))
            {
                existingTitle.pubdate = updatedFields.pubdate;
            }

            // Step 5: Save changes to the database
            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);  // Handle any errors that occur during saving
            }

            // Step 6: Return No Content (204) if the update was successful
            return StatusCode(HttpStatusCode.NoContent);
        }
        #endregion


        // DELETE: api/titles/5
        #region Delete 
        [HttpDelete]

        [Route("api/titles/{id}")]
        [ResponseType(typeof(title))]
        public IHttpActionResult Deletetitle(string id)
        {
            title title = db.titles.Find(id);
            if (title == null)
            {
                return NotFound();
            }

            db.titles.Remove(title);
            db.SaveChanges();

            return Ok(title);
        }
        #endregion


        // GET: api/titles/{id}
        #region Search Titles By id.... By Me
        [Route("api/titles/{id}")]
        [HttpGet]
        public IHttpActionResult GetTitleById(string id)
        {
            var title = db.titles.Find(id);
            if (title == null)
            {
                return NotFound();
            }

            return Ok(title);
        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool titleExists(string id)
        {
            return db.titles.Count(e => e.title_id == id) > 0;
        }
    }
}