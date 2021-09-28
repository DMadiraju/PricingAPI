using System;
using System.ComponentModel.DataAnnotations;

namespace Pricing
{


    public class PricingObj
    {
        //Business Object
        public int billTo { get; set; }
        public int shipTo { get; set; }
        public int prodErPNo { get; set; }
        public int unitprice { get; set; }
        public string currencycode { get; set; }
        public string unitofmeasure { get; set; }
    }
}
