using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    /// <summary>
    /// Base class for discount coupons
    /// </summary>
    public abstract class Coupon
    {
        public String Code { get; set; }

        public String Type
        {
            get { return GetType().Name; }
        }

        public DateTime Start { get; set; }
        // End is set to null if Unlimited time
        public DateTime End { get; set; }

        // If Discount if for all customers, we set to null
        public List<Customer> CustomersValidFor { get; set; }
        public List<Customer> CustomersUsedBy { get; set; }

        /// <summary>
        /// Max amount of times that a customer is allowed to use the coupon
        /// </summary>
        public int UseLimit { get; set; }

        public bool CanBeCombined { get; set; }
    }
}
