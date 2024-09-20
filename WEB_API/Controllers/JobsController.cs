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
//using System.Web.Mvc;
using WEB_API.Models;

namespace WEB_API.Controllers
{
    public class JobsController : ApiController
    {
        private pubsEntities db = new pubsEntities();

        // GET: api/jobs
        [HttpGet]

        public IQueryable<job> Getjobs()
        {
            return db.jobs;
        }

        // GET: api/jobs/5
        [HttpGet]
        [Route("api/jobs/{id}")]
        [ResponseType(typeof(job))]
        public IHttpActionResult Getjob(int id)
        {
            job job = db.jobs.Find(id);
            if (job == null)
            {
                return NotFound();
            }

            return Ok(job);
        }


        [HttpGet]
        [Route("api/jobs/minlvl/{minlvl}")]
        public IHttpActionResult GetJobsByMinLvl(byte minlvl)
        {
            var job = db.jobs.Where(j => j.min_lvl >= minlvl);
            return Ok(job);
        }

        [HttpGet]
        [Route("api/jobs/maxlvl/{maxlvl}")]
        public IHttpActionResult GetJobsByMaxLvl(byte maxlvl)
        {
            var job = db.jobs.Where(j => j.max_lvl <= maxlvl);
            return Ok(job);
        }

        

        // POST: api/jobs
        [HttpPost]
        [ResponseType(typeof(job))]
        public IHttpActionResult Postjob(job job)
        {
            if (db.jobs.Any(j => j.job_id == job.job_id))
            {
                return Content(HttpStatusCode.Conflict, new
                {
                    TimeStamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    Message = "Validation Failed..."
                });
            }

            db.jobs.Add(job);
            db.SaveChanges();

            return Ok(new
            {
                Message = "Record Created Successfully"
            });
        }

        

        

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool jobExists(short id)
        {
            return db.jobs.Count(e => e.job_id == id) > 0;
        }


        //PUT: api/jobs/5
        [ResponseType(typeof(void))]
        [Route("api/jobs/{id}")]
        [HttpPut]
        public IHttpActionResult Putjob(short id, job job)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != job.job_id)
            {
                return BadRequest();
            }

            db.Entry(job).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!jobExists(id))
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


        // DELETE: api/jobs/5
        [ResponseType(typeof(job))]
        [Route("api/jobs/Delete/{id}")]
        public IHttpActionResult Deletejob(short id)
        {
            job job = db.jobs.Find(id);
            if (job == null)
            {
                return NotFound();
            }

            db.jobs.Remove(job);
            db.SaveChanges();

            return Ok(job);
        }
    }
}