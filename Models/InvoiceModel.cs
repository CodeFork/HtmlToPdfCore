using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HtmlToPdf.Models
{
    public class InvoiceModel
    {
        public string InvoiceNo { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string CompanyName { get; set; }
        public string OfficeAddress { get; set; }
        public string Address { get; set; }
        public DateTime DueDate { get; set; }
        public string PhoneNo { get; set; }
    }
}