using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace DigitalXWebsite.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        [AllowAnonymous]
        public ActionResult Login(string ReturnUrl)
        {
            ViewBag.ReturnUrl = Request.UrlReferrer;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(string username, string password, string ReturnUrl)
        {
            ServiceReference.Customer cus = new ServiceReference.Customer();
            if (ModelState.IsValid)
            {
                ServiceReference.Service1Client customer = new ServiceReference.Service1Client();
                cus = customer.Login(username, password);
                if (cus == null)
                {
                    ModelState.AddModelError("", "The username or password is incorrect");
                }
                else
                {
                    FormsAuthentication.SetAuthCookie(cus.FirstName, false);
                    int c = cus.CustomerID;
                    Session["CustomerId"] = c;
                    if (ReturnUrl != null)
                    {
                        return Redirect(ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
            }

            return View(cus);
        }

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            Session["CustomerId"] = null;
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }
        [AllowAnonymous]
        public ActionResult PartialRegisterDetails()
        {
            return PartialView();
        }
        [AllowAnonymous]
        public ActionResult PartialRegisterAddress()
        {
            ServiceReference.Service1Client address = new ServiceReference.Service1Client();
            ViewBag.AddressTypes = new SelectList(address.GetAddressTypes(), "TypeID", "Type");

            return PartialView();
        }
        [AllowAnonymous]
        public ActionResult PartialRegisterContact()
        {
            ServiceReference.Service1Client contact = new ServiceReference.Service1Client();
            ViewBag.ContactTypes = new SelectList(contact.GetContactTypes(), "ContactTypeID", "ContactType1");
            return PartialView();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Register([Bind(Include = "FirstName,LastName,Username,Password")] ServiceReference.Customer cust, [Bind(Include = "AddressType,City,Country,PostalCode,Street,Suburb")] ServiceReference.Address add, [Bind(Include = "ContactType,Contact1")] ServiceReference.Contact cont)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ServiceReference.Service1Client cus = new ServiceReference.Service1Client();
                    cus.Register(cust, add, cont);

                    MembershipUser NewUser = Membership.CreateUser(cust.UserName, cust.Password);
                    FormsAuthentication.SetAuthCookie(cust.FirstName, false);
                    return RedirectToAction("Index", "Home");
                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError("Registration Error", "Registration error: " + e.StatusCode.ToString());
                }
            }

            return View();
        }

        [Authorize]
        public ActionResult Manage()
        {
            int c = Convert.ToInt32(Session["CustomerId"]);

            ServiceReference.Service1Client order = new ServiceReference.Service1Client();
            var o = order.GetOrder(c);
            var od = new List<ServiceReference.OrderDetail>();
            var p = new ServiceReference.Product();
            List<decimal> prices = new List<decimal>();
            foreach (var item in o)
            {
                od = order.GetOrderDetails(item.OrderID);
                ViewBag.OrderDtails = od;

                foreach (var detail in od)
                {
                    p = order.GetProduct(detail.ProductID);
                    if (p.UnitsInStock < detail.Quantity && p.UnitsInStock == 0)
                    {
                        ViewBag.IsBackOrder = true;
                    }
                    else
                    {
                        ViewBag.IsBackOrder = false;
                    }
                }
            }

            return View(o);
        }

        public decimal CalculateTotal()
        {
            int c = Convert.ToInt32(Session["CustomerId"]);

            ServiceReference.Service1Client order = new ServiceReference.Service1Client();
            var o = order.GetOrder(c);
            var ord = new List<ServiceReference.OrderDetail>();
            var p = new ServiceReference.Product();
            int i;

            decimal total = 1;
            if (Session["index"] != null) {
                i = Convert.ToInt32(Session["index"]);
            }
            else
            {
                i = 0;
            }

            if(i < o.Count)
            {
                foreach (var item in o)
                {
                    ord = order.GetOrderDetails(o[i].OrderID);

                    var od = ord[0];
                    p = order.GetProduct(od.ProductID);
                    total = od.Quantity * p.Price;
                    Session["index"] =+ 1;
                    return total;
                
                }
            }
        return total;
        }

        public ActionResult Details(int id)
        {
            ServiceReference.Service1Client order = new ServiceReference.Service1Client();
            var orderDetails = order.GetOrderDetails(id);

            return View(orderDetails);
        }
    }
}