﻿using System;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using AdminView.Annotations;
using AdminView.ViewModel;
using Domain;
using Domain.Entities;
using Domain.Repositories;

namespace AdminView.Controllers
{
   [LoggedIn]
    public class CouponController : Controller
    {

        private readonly CouponRepository _couponRepository;

        public CouponController(CouponRepository couponRepository)
        {
            _couponRepository = couponRepository;
        }


        // GET: Coupon
        public ActionResult Index(int page = 0)
        {
            CouponViewModel cvm = new CouponViewModel();
            cvm.Coupons = _couponRepository.FindActiveCoupons().ToList();

            // TestData for now
            //var tempCoupons = Testdata.RandomCoupon();

            //_couponRepository.Store(tempCoupons);

            //_couponRepository.SaveChanges();

            cvm.CurrentPage = page;

            cvm.Coupons = _couponRepository.FindActiveCoupons().ToList();

            return View("Coupons", cvm);
        }

        // GET: Coupon/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        #region NEEDS_FIXES
        // GET: Coupon/SelectType
        public ActionResult SelectType()
        {
            var model = new CouponViewModel();
            return View(model);
        }

        // POST: Coupon/SelectType
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SelectType(CouponViewModel model)
        {
            return RedirectToAction("Create", "Coupon", new { id = model.Type});
        }

        // GET: Coupon/Create
        public ActionResult Create(Types id)
        {
            //var model = new CouponViewModel {Coupon = new BuyProductXRecieveProductY()};

            return View(String.Format("{0}", id));
        }


        // POST: Coupon/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BuyProductXRecieveProductY coupon)
        {


            try
            {
                if (ModelState.IsValid)
                {
                //    switch (model.Type)
                //    {
                //        case Types.BuyProductXRecieveProductY:
                //            model.Coupon = new BuyProductXRecieveProductY();
                //            break;
                //        case Types.BuyXProductsPayForYProducts:
                //            model.Coupon = new BuyXProductsPayForYProducts();
                //            model.Coupon.Code = model.code
                //            break;
                //    }


                    //CouponViewModel.Coupon = coupon;
                    coupon.IsActive = true;
                    _couponRepository.Store(coupon);
                    _couponRepository.SaveChanges();
                    TempData["success"] = "Rabatt sparad!";
                    return RedirectToAction("Index");
                }
            }
            catch
            {
                TempData["error"] = "Misslyckades att spara rabatten!";
            }
            return View();
        }
        #endregion

        // GET: Coupon/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Coupon/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: /Coupon/Delete/:code
        public ActionResult Delete(string code)
        {
            var coupon = _couponRepository.FindByCode(code);

            return View(coupon);
        }


        // POST: /Coupon/Delete/42
        /// <summary>
        ///     Not really removes the coupon
        ///     (because of statitics) https://github.com/Textilgallerian/textilgallerian/issues/53
        ///     We only set it to unactive
        /// </summary>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string code)
        {
            try
            {
                var coupon = _couponRepository.FindByCode(code);
                // Sets the given coupon to unactive
                coupon.IsActive = false;
                _couponRepository.SaveChanges();
                TempData["success"] = "Rabatten togs bort.";
            }
            catch (DataException)
            {
                TempData["error"] = "Misslyckades att ta bort rabatten!";
                return RedirectToAction("Delete", new { id = code });
            }

            return RedirectToAction("Index");
        }
    }
}
