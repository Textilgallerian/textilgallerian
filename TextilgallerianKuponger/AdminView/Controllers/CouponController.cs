﻿﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
﻿using System.Text.RegularExpressions;
﻿using System.Web.Mvc;
using AdminView.Annotations;
using AdminView.ViewModel;
using Domain.Entities;
using Domain.Repositories;
using Domain.Tests.Helpers;
using Domain.ExtensionMethods;
using AdminView.Controllers.Helpers;

namespace AdminView.Controllers
{
    [LoggedIn]
    public class CouponController : Controller
    {
        private readonly CouponRepository _couponRepository;
        private readonly CouponHelper _couponHelper;

        private const int PageSize = 15;

        public CouponController(CouponRepository couponRepository, CouponHelper couponHelper)

        {
            _couponRepository = couponRepository;
            _couponHelper = couponHelper;
        }

        // GET: Coupon
        [RequiredPermission(Permission.CanListCoupons)]
        public ActionResult Index(int page = 1)
        {
            var model = new PagedViewModel<Coupon>
            {
                PagedObjects = _couponRepository.FindAllCoupons().OrderBy(c => c.CreatedAt).Page(page - 1, PageSize),
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(_couponRepository.FindAllCoupons().Count() / (double) PageSize)
            };

            // TestData for now
            //var tempCoupons = Testdata.RandomAmount(() => Testdata.RandomCoupon());
            //tempCoupons.ForEach(_couponRepository.Store);
            //_couponRepository.SaveChanges();

            return View(model);
        }

        // GET: Coupon/Details/5
        [RequiredPermission(Permission.CanListCoupons)]
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Coupon/Create
        [RequiredPermission(Permission.CanAddCoupons)]
        public ActionResult Create()
        {
            return View(new CouponViewModel());
        }

        // POST: Coupon/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequiredPermission(Permission.CanAddCoupons)]
        public ActionResult Create(CouponViewModel model)
        {
            var type = Assembly.GetAssembly(typeof(Coupon)).GetType(model.Type);

            List<Customer> customers = null;
            List<Product> products = null;
            
            if (model.DisposableCodes)
            {
                // There can't be both a campaign and disposable codes
                model.Parameters["Code"] = null;

                customers = _couponHelper.GenerateDispoableCodes(model.NumberOfCodes);
            }
            else if (model.CustomerString != null)
            {
                var customerLines = model.CustomerString.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                customers = _couponHelper.GetCustomers(customerLines);
            }
            
            if(model.ProductsString != null)
            {
                var productLines =  model.ProductsString.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                products = _couponHelper.GetProducts(productLines);
            }
           

            if (!ModelState.IsValid) return View();
            try
            {
                // Magic super perfect code, do not touch!
                var constructor = type.GetConstructor(new[] { typeof(IReadOnlyDictionary<String, String>) });
                var coupon = constructor.Invoke(new object[] { model.Parameters }) as Coupon;

                var user = (User) Session["user"];
                coupon.CreatedBy = user.Email;

                coupon.CanBeCombined = model.CanBeCombined;
                coupon.CustomersValidFor = customers;
                coupon.Products = products;
                coupon.IsActive = true;
                coupon.CreatedAt = DateTime.Now;

                _couponRepository.Store(coupon);
                _couponRepository.SaveChanges();

                TempData["success"] = "Rabatt sparad!";
                if (model.DisposableCodes)
                {
                    return View("Codes", customers);
                }
                return RedirectToAction("Index");
            }
            catch (NullReferenceException)
            {
                throw new ArgumentException("Invalid coupon type");
            }
            catch
            {
                TempData["error"] = "Misslyckades att spara rabatten!";
            }

            return View(model);
        }

        // GET: Coupon/Edit/5
        [RequiredPermission(Permission.CanChangeCoupons)]

        public ActionResult Edit(string code)

        {
            var coupon = _couponRepository.FindByCode(code);
            var dictionary = coupon.GetProperties();
            var cvm = new CouponViewModel();
            cvm.Parameters = dictionary;

            return View( cvm);
        }

        // POST: Coupon/Edit/5
        [HttpPost]
        [RequiredPermission(Permission.CanChangeCoupons)]
        public ActionResult Edit(CouponViewModel model)
        {
            var coupon = _couponRepository.FindByCode(model.Parameters["Code"]);

            List<Customer> customers = null;
            List<Product> products = null;

            if (model.CustomerString != null)
            {
                var customerLines = model.CustomerString.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                customers = _couponHelper.GetCustomers(customerLines);
            }

            if (model.ProductsString != null)
            {
                var productLines = model.ProductsString.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                products = _couponHelper.GetProducts(productLines);
            }


            if (!ModelState.IsValid) return View();
            try
            {
                coupon.SetProperties(model.Parameters);
                var user = (User)Session["user"];
                coupon.CreatedBy = user.Email;
                coupon.CanBeCombined = model.CanBeCombined;
                coupon.CustomersValidFor = customers;
                coupon.Products = products;
                coupon.IsActive = true;
                coupon.CreatedAt = DateTime.Now;
                _couponRepository.Store(coupon);
                _couponRepository.SaveChanges();
                TempData["success"] = "Rabatt sparad!";
                return RedirectToAction("index");
            }
            catch (NullReferenceException)
            {
                throw new ArgumentException("Invalid coupon type");
            }
            catch
            {
                TempData["error"] = "Misslyckades att spara rabatten!";
            }

            return View();
        }

        // GET: /Coupon/Delete/:code
       [RequiredPermission(Permission.CanDeleteCoupons)]
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
        [RequiredPermission(Permission.CanDeleteCoupons)]
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
                return RedirectToAction("delete", new { id = code });
            }

            return RedirectToAction("index");
        }
    }
}