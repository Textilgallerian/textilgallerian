using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Domain.Entities;
using Domain.Repositories;

namespace Api.Controllers
{
    public class CartController : Controller
    {
        private readonly CouponRepository couponRepository;

        public CartController(CouponRepository couponRepository)
        {
            this.couponRepository = couponRepository;
        }

        public IEnumerable<Coupon> Get()
        {
            return Post();
        }

        public void Test() {
            couponRepository.Store(new BuyProductXRecieveProductY
            {
                Code = "Hej :)",
                CustomersValidFor = new List<Customer> { new Customer { Email = "test@example.com" } }
            });
            couponRepository.SaveChanges();
        }

        public IEnumerable<Coupon> Post()
        {
            return couponRepository.FindByEmail("test@example.com");
        }
    }
}