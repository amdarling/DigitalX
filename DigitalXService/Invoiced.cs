//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DigitalXService
{
    using System;
    using System.Collections.Generic;
    
    public partial class Invoiced
    {
        public int InvoiceID { get; set; }
        public int OrderID { get; set; }
        public int EmployeeID { get; set; }
        public int ShippingOption { get; set; }
        public System.DateTime InvoiceDate { get; set; }
    
        public virtual Employee Employee { get; set; }
        public virtual Order Order { get; set; }
        public virtual ShipperOption ShipperOption { get; set; }
    }
}
