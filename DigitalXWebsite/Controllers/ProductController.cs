using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace DigitalXWebsite.Controllers
{
    public class ProductController : Controller
    {
        // GET: Product
        public ActionResult Index(string searchString, int? page, string currentFilter)
        {
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            int pageSize = 10;
            int pageNumber;

            pageNumber = (page ?? 1);

            ServiceReference.Service1Client product = new ServiceReference.Service1Client();
            try
            {
                var pro = product.GetAllProducts();
                var cat = product.GetAllCategories();
                var sub = product.GetAllSubCategories();


            if (!String.IsNullOrEmpty(searchString))
            {
                var products = new List<ServiceReference.Product>();
                var cid = cat.Where(c => c.Category.ToUpper() == searchString.ToUpper()).Select(c => c.CategoryID).FirstOrDefault();
                var sid = sub.Where(s => s.CategoryID == cid).Select(s => s.SubCategoryID).ToList();
                foreach (var item in pro)
                {
                    foreach (var i in sid)
                    {
                        if(item.SubCategoryID == i)
                        {
                            products.Add(item);
                        }                   
                    }
                }

                    return View(products.ToPagedList(pageNumber, pageSize));
                }

            return View(pro.ToPagedList(pageNumber, pageSize));
            }
            catch (System.ServiceModel.EndpointNotFoundException e)
            {
                return RedirectToAction("DatabaseError", "Error");
            }
        }

        public ActionResult Details(int id)
        {

            ServiceReference.Service1Client product = new ServiceReference.Service1Client();
            var pro = product.GetProduct(id);

            return View(pro);
        }

        public string SubCategory(int id)
        {
            ServiceReference.Service1Client product = new ServiceReference.Service1Client();
            var cat = product.GetAllSubCategories();
            var subCat = cat.Where(c => c.SubCategoryID == id).Select(c => c.SubCategory).FirstOrDefault();

            return subCat;
        }
        public string Category(int id)
        {
            ServiceReference.Service1Client product = new ServiceReference.Service1Client();
            var cat = product.GetAllCategories();
            var subCat = product.GetAllSubCategories();
            var categoryID = subCat.Where(c => c.SubCategoryID == id).Select(c => c.CategoryID).FirstOrDefault();
            var category = cat.Where(c => c.CategoryID == categoryID).Select(c => c.Category).FirstOrDefault();


            return category;
        }
        public string Retailer(int id)
        {
            ServiceReference.Service1Client product = new ServiceReference.Service1Client();
            var ret = product.GetAllRetailers();
            var retailer = ret.Where(r => r.RetailerID == id).Select(r => r.Retailer1).FirstOrDefault();

            return retailer;
        }

        public ContentResult AddOrderDetail(int id)
        {
            List<int> productsIDs = Session["ShoppingCart"] as List<int>;
            List<int> quantity = new List<int>();
            if (Session["Quantity"] != null) {
                quantity = Session["Quantity"] as List<int>;
            }

            if (productsIDs == null)
            {
                productsIDs = new List<int>();
            }

            productsIDs.Add(id);
            quantity.Add(1);
            Session["ShoppingCart"] = productsIDs;
            Session["Quantity"] = quantity;

            return Content("The product has been added to your cart", "text/plain", System.Text.Encoding.Default);
        }
    }

}