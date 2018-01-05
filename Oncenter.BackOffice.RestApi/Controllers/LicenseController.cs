using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Oncenter.BackOffice.RestApi.Controllers
{
    public class LicenseController : ApiController
    {
        // GET: api/License
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/License/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/License
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/License/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/License/5
        public void Delete(int id)
        {
        }
    }
}
