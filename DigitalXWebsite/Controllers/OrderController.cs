using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DigitalXWebsite.Controllers
{
    public class OrderController : Controller
    {

        // GET: Order
        public ActionResult Index()
        {
            List<int> productIDs = Session["ShoppingCart"] as List<int>;
            ServiceReference.Service1Client order = new ServiceReference.Service1Client();
            var orderList = order.GetOrderProducts(productIDs);
            if(orderList.Count == 0)
            {
                ViewBag.Message = "Your Cart is Empty";
            }
            return View(orderList);
        }

        [Authorize]
        // GET: Order/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        [Authorize]
        public ActionResult Checkout(List<int> quantity)
        {
            var customerId = Convert.ToInt32(Session["CustomerId"]);
            if (quantity.Count == 0)
            {
                quantity = Session["Quantity"] as List<int>;
            }
            ServiceReference.Service1Client order = new ServiceReference.Service1Client();
            var o = order.DetailOrder(customerId);
            var bo = order.DetailOrder(customerId);
            List<int> productIDs = Session["ShoppingCart"] as List<int>;
            var unitsInStock = order.GetUnitsInStock(productIDs);

            List<int> backOrderIDs = new List<int>();
            List<int> backOrderQ = new List<int>();

            for (int i = 0; i < quantity.Count; i++)
            {
                if (quantity[i] > unitsInStock[i])
                {
                    if (Session["BackOrder"] == null)
                    {
                        ViewBag.Message = "Some items in your order do not have enough units in stock.If you would like to create a back order continue to checkout otherwise change the quantity of items you are purchasing";
                        Session["BackOrder"] = "asked";
                        Session["Quantity"] = quantity;
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        backOrderIDs.Add(productIDs[i]);
                        productIDs.RemoveAt(i);

                        backOrderQ.Add(quantity[i]);
                        quantity.RemoveAt(i);

                        bo.OrderID = bo.OrderID + 1;
                    }
                }
            }
            List<ServiceReference.OrderDetail> orderList = new List<ServiceReference.OrderDetail>();
            List<ServiceReference.OrderDetail> backOrderList = new List<ServiceReference.OrderDetail>();

            for (int i = 0; i < quantity.Count; i++)
            {
                orderList.Add( order.DetailOrderDetail(o.OrderID, productIDs[i], quantity[i]) );
            }
            for (int i = 0; i < backOrderIDs.Count; i++)
            {
                var backOrder = order.DetailOrderDetail(bo.OrderID, backOrderIDs[i], backOrderQ[i]);
                backOrder.DetailID = backOrder.DetailID + 1;
                backOrderList.Add(backOrder);
                Session["BackOrderDetails"] = backOrderList;
            }
            Session["Quantity"] = quantity;
            Session["OrderDetails"] = orderList;
            Session["Order"] = o;
            Session["BOrder"] = bo;

            //if(o.AddressID == 0)
            //{
            //    RedirectToAction("CreateAddress");
            //}
            return View(o);
        }

        // GET: Order/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Order/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult PartialAddress()
        {
            ServiceReference.Service1Client address = new ServiceReference.Service1Client();
            int add = Convert.ToInt32(Session["AddressID"]);
            var a = new ServiceReference.Address();
            if (add == 0)
            {
                var customerId = Convert.ToInt32(Session["CustomerId"]);
                a = address.GetAddresses(customerId).FirstOrDefault();
                if (a != null)
                {
                    ViewBag.Type = address.GetAddressType(a.AddressType);
                    Session["AddressId"] = a.AddressID;
                }
            }
            else
            {
                a = address.GetAddress(add);
                ViewBag.Type = address.GetAddressType(a.AddressType);
            }
            return PartialView(a);
        }

        public ActionResult PartialOrderDetails(bool inStock)
        {
            //List<int> productIDs = Session["Order"] as List<int>;
            //ServiceReference.Service1Client orderDetail = new ServiceReference.Service1Client();
            List<ServiceReference.OrderDetail> orderDetails = Session["OrderDetails"] as List < ServiceReference.OrderDetail >;
            List<ServiceReference.OrderDetail> backOrderDetails = Session["BackOrderDetails"] as List<ServiceReference.OrderDetail>;
            if (inStock == false)
            {
                return PartialView(backOrderDetails);
            }


            return PartialView(orderDetails);
        }

        //public ActionResult EditAddress()
        //{
        //    ServiceReference.Service1Client address = new ServiceReference.Service1Client();
        //    var customerId = Convert.ToInt32(Session["CustomerId"]);
        //    ViewBag.AddressTypes = new SelectList(address.GetAddressTypes(), "TypeID", "Type");
        //    var a = address.GetAddresses(customerId).FirstOrDefault();
        //    //if(a == null)
        //    //{
        //    //    a = address.GetAddresses(id, 1);
        //    //}
        //    return View(a);
        //}

        //[HttpPost]
        //public ActionResult EditAddress([Bind(Include = "AddressType,City,Country,PostalCode,Street,Suburb")] ServiceReference.Address add)
        //{
        //    Debug.WriteLine("AddressType:" + add.AddressType);
        //    int a = add.AddressType;
        //    ServiceReference.Service1Client address = new ServiceReference.Service1Client();
        //    address.EditAddress(add);
        //    List<int> l = new List<int>();

        //    return RedirectToAction("Index", "Order", new { quantity = l });
        //}
        public ActionResult CreateAddress()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult CreateAddress([Bind(Include = "AddressType,City,Country,PostalCode,Street,Suburb")] ServiceReference.Address add, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                string AddressTypeValue = form["AddressTypes"];
                Debug.WriteLine("AddressType:" + add.AddressType);
                int a = add.AddressType;
                add.AddressType = Convert.ToInt32(AddressTypeValue);
                var id = Convert.ToInt32(Session["CustomerId"]);
                ServiceReference.Service1Client address = new ServiceReference.Service1Client();
                address.CreateAddress(add, id);

                var aid = address.GetAddresses(id).LastOrDefault();

                Session["AddressID"] = aid.AddressID;
                List<int> l = new List<int>();

                return RedirectToAction("Checkout", "Order", new { quantity = l });
            }
            return View();
        }
        public ActionResult ChangeAddress()
        {
            ServiceReference.Service1Client address = new ServiceReference.Service1Client();
            var customerId = Convert.ToInt32(Session["CustomerId"]);
            var a = address.GetAddresses(customerId);
            //ViewBag.AddressTypes = new SelectList(address.GetAddressTypes(), "TypeID", "Type");
            

            return View(a);
        }

        [HttpPost]
        public ActionResult ChangeAddress(int add)
        {
            Session["AddressID"] = add;
            List<int> l = new List<int>();

            return RedirectToAction("Checkout", "Order", new { quantity = l });

            //return View();
        }

        public ActionResult Create()
        {
            ServiceReference.Service1Client order = new ServiceReference.Service1Client();
            ServiceReference.Order detailOrder = Session["Order"] as ServiceReference.Order;
            ServiceReference.Order detailBackOrder = Session["BOrder"] as ServiceReference.Order;
            var orderList = Session["OrderDetails"] as List<ServiceReference.OrderDetail>;
            var backOrderList = Session["BackOrderDetails"] as List<ServiceReference.OrderDetail>;

            if (orderList.Count != 0)
            {
                order.CreateOrder(detailOrder);
                order.CreateOrderDetail(orderList);
            }
            if(backOrderList.Count != 0)
            {
                order.CreateOrder(detailBackOrder);
                order.CreateOrderDetail(backOrderList);
            }


            return RedirectToAction("Manage", "Account");
        }

        public ActionResult Remove(int id)
        {
            var productIDs = Session["ShoppingCart"] as List<int>;
            for (int i = 0;i <= productIDs.Count;i++)
            {
                if(productIDs[i] == id)
                {
                    productIDs.RemoveAt(i);
                }
            }

            return RedirectToAction("Index");
        }
    }
}
