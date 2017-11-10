using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oncenter.BackOffice.Entities.Zuora
{
    public class ZuoraContext<T>
    {
        public ZuoraContext()
        {

        }

        public string UserName { get; set; }

        public string AccessKey { get; set; }

        public T Response { get; set; }

        public string Url { get; set; }

        public byte[] File { get; set; }

        public string FileName { get; set; }

        public string ContentType { get; set; }

        public string Description { get; set; }
       
        public string AssociatedObject { get; set; }

        public string AssociatedObjectKey { get; set; }

        //https://docs.google.com/spreadsheets/d/15mR5qyMDz6l_be1fSJwbtLTl_wn35p142ePZuTQ8OJE/edit#gid=1516451691
    }
}
