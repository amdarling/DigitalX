using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Web.Security;

namespace DigitalXService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service1 : IService1
    {
        private DigitalXDBEntities db = new DigitalXDBEntities();


        public List<Product> GetPopularProducts()
        {
            db.Configuration.LazyLoadingEnabled = false;
            db.Configuration.ProxyCreationEnabled = false;

            List<Product> ProductList = new List<Product>();
            List<OrderDetail> DetailList = new List<OrderDetail>();
            int quantity;
            using (var db2 = new DigitalXDBEntities())
            {
                    var products1 = (from od in db2.OrderDetails select od).ToList();
                for (int ind = 0; ind < products1.Count; ind++)
                {
                    for (int i = 0; i < products1.Count; i++)
                    {
                        if (ind != i)
                        {
                            if (products1[ind].ProductID == products1[i].ProductID)
                            {
                                quantity = products1[ind].Quantity + products1[i].Quantity;
                                var prod = products1.Where(p => p.ProductID == products1[ind].ProductID).Select(p => p).FirstOrDefault();
                                prod.Quantity = quantity;
                                DetailList.Add(prod);
                                products1.RemoveAt(i);
                            }
                            else
                            {
                                for (int ix = 0; ix < DetailList.Count && ind < products1.Count; ix++)
                                {
                                    if (products1[ind].ProductID != DetailList[ix].ProductID)
                                    {
                                        DetailList.Add(products1[ind]);
                                        products1.RemoveAt(ind);
                                    }
                                }
                                if (DetailList.Count == 0)
                                {
                                    DetailList.Add(products1[ind]);
                                    products1.RemoveAt(ind);
                                }
                            }
                        }
                    }
                }
            }

            //var products1 = db.OrderDetails.Where(od => od.Quantity != 0).Select(od => od).ToList();
            var products = (from p in /*db.OrderDetails*/DetailList orderby p.Quantity descending select p.ProductID).Take(5);

            //var products = (from p in db.OrderDetails orderby p.Quantity descending select p.ProductID).Take(5);
            foreach (var product in products)
            {

                var prod = (from p in db.Products where p.ProductID == (product) select p).FirstOrDefault();

                ProductList.Add(prod);
            }

            return ProductList;
        }

        public Product GetProduct(int id)
        {
            db.Configuration.LazyLoadingEnabled = false;
            db.Configuration.ProxyCreationEnabled = false;

            var product = db.Products.Where(p => p.ProductID == id).Select(p => p).FirstOrDefault();
            return product;
        }
        public List<Product> GetAllProducts()
        {
            db.Configuration.LazyLoadingEnabled = false;
            db.Configuration.ProxyCreationEnabled = false;

            var product = db.Products.Select(p => p).ToList();
            return product;
        }
        public List<ProductCategory> GetAllCategories()
        {
            db.Configuration.LazyLoadingEnabled = false;
            db.Configuration.ProxyCreationEnabled = false;

            var categories = db.ProductCategories.ToList();
            return categories;
        }
        public List<ProductSubCategory> GetAllSubCategories()
        {
            db.Configuration.LazyLoadingEnabled = false;
            db.Configuration.ProxyCreationEnabled = false;

            var categories = db.ProductSubCategories.ToList();
            return categories;
        }
        public List<Retailer> GetAllRetailers()
        {
            db.Configuration.LazyLoadingEnabled = false;
            db.Configuration.ProxyCreationEnabled = false;

            var retailers = db.Retailers.ToList();
            return retailers;
        }
        public List<Product> GetOrderProducts(List<int> productIds)
        {
            db.Configuration.LazyLoadingEnabled = false;
            db.Configuration.ProxyCreationEnabled = false;

            List<Product> ProductList = new List<Product>();
            if (productIds != null)
            {
                foreach (var id in productIds)
                {
                    var product = db.Products.Where(p => p.ProductID == id).Select(p => p).FirstOrDefault();
                    ProductList.Add(product);
                }
            }

            return ProductList;
        }
        public List<int> GetUnitsInStock(List<int> id)
        {
            db.Configuration.LazyLoadingEnabled = false;
            db.Configuration.ProxyCreationEnabled = false;

            List<int> unitsInStock = new List<int>();
            foreach (var item in id)
            {
                var units = db.Products.Where(p => p.ProductID == item).Select(p => p.UnitsInStock).FirstOrDefault();
                unitsInStock.Add(units);
            }
            return unitsInStock;

        }

        public Customer Login(string us, string p)
        {
            db.Configuration.LazyLoadingEnabled = false;
            db.Configuration.ProxyCreationEnabled = false;

            var cust = (from c in db.Customers where c.UserName == us && c.Password == p select c).FirstOrDefault();
            if (cust == null)
                return null;
            else
            {
                return cust;
            }
        }
        public Customer Register(Customer customer, Address address, Contact contact)
        {
            db.Configuration.LazyLoadingEnabled = false;
            db.Configuration.ProxyCreationEnabled = false;

            //db.insertCustomer(customer.FirstName, customer.LastName, cu)
            Customer c = new Customer();

            c.UserName = customer.UserName;
            c.FirstName = customer.FirstName;
            c.LastName = customer.LastName;
            c.Password = customer.Password;


            Contact con = new Contact();

            con.Contact1 = contact.Contact1;
            con.ContactType = contact.ContactType + 1;

            Address a = new Address();

            a.AddressType = address.AddressType + 1;
            a.Street = address.Street;
            a.Suburb = address.Suburb;
            a.City = address.City;
            a.Country = address.Country;
            a.PostalCode = address.PostalCode;

            db.Customers.Add(c);
            db.SaveChanges();
            db.insertCustomerContact(c.CustomerID, con.ContactType, contact.Contact1);
            db.insertCustomerAddress(c.CustomerID, a.AddressType, address.Street, address.Suburb, address.City, address.PostalCode, address.Country);

            return (customer);
        }
        public List<AddressType> GetAddressTypes()
        {
            db.Configuration.LazyLoadingEnabled = false;
            db.Configuration.ProxyCreationEnabled = false;

            var addr = db.AddressTypes.Select(a => a).ToList();
            return addr;
        }
        public List<ContactType> GetContactTypes()
        {
            db.Configuration.LazyLoadingEnabled = false;
            db.Configuration.ProxyCreationEnabled = false;

            var cont = db.ContactTypes.Select(c => c).ToList();
            return cont;
        }

        public List<Address> GetAddresses(int id)
        {
            db.Configuration.LazyLoadingEnabled = false;
            db.Configuration.ProxyCreationEnabled = false;

            var add = db.selectCustomerAddresses(id).Select(a => a.AddressID).ToList();
            List<Address> addresses = new List<Address>();
            foreach(var item in add)
            {
                var address = (from a in db.Addresses where a.AddressID == item select a).FirstOrDefault();
                addresses.Add(address);
            }
            //db.Addresses.Where(a => a.AddressID == id && a => a.A).Select(a => a).FirstOrDefault();

            return addresses;
        }
        public Address GetAddress(int id)
        {
            db.Configuration.LazyLoadingEnabled = false;
            db.Configuration.ProxyCreationEnabled = false;

            var add = db.Addresses.Where(a => a.AddressID == id).Select(a => a).FirstOrDefault();

            return add;
        }
        public string GetAddressType(int type)
        {
            db.Configuration.LazyLoadingEnabled = false;
            db.Configuration.ProxyCreationEnabled = false;

            var addressType = db.AddressTypes.Where(a => a.TypeID == type).Select(a => a.Type).FirstOrDefault();

            return addressType;
        }

        public Order CreateOrder(Order order)
        {
            db.Configuration.LazyLoadingEnabled = false;
            db.Configuration.ProxyCreationEnabled = false;

            //var aid = db.selectCustomerAddresses(cid).Select(ca => ca.AddressID).FirstOrDefault();

            //Order o = new Order();

            //o.CustomerID = cid;
            //o.AddressID = aid;
            //o.OrderDate = DateTime.Today;
            //o.Complete = false;


            //db.Orders.Add(o);
            db.insertOrder(order.CustomerID, order.AddressID, DateTime.Today, order.Complete);

            return order;
        }
        public Order DetailOrder(int id)
        {
            db.Configuration.LazyLoadingEnabled = false;
            db.Configuration.ProxyCreationEnabled = false;

            var aid = db.selectCustomerAddresses(id).Select(ca => ca.AddressID).FirstOrDefault();
            var oid = (from or in db.Orders orderby or.OrderID descending select or.OrderID).FirstOrDefault();

            Order o = new Order();

            o.OrderID = oid + 1;
            o.CustomerID = id;
            o.AddressID = aid;
            o.OrderDate = DateTime.Today;
            o.Complete = false;


            //db.Orders.Add(o);
            //db.insertOrder(cid, aid, DateTime.Today, false);

            return o;
        }
        public List<Order> GetOrder(int id)
        {
            db.Configuration.LazyLoadingEnabled = false;
            db.Configuration.ProxyCreationEnabled = false;

            var order = db.Orders.Where(o => o.CustomerID == id).Select(o => o).ToList();

            return order;
        }
        public List<OrderDetail> GetOrderDetails(int id)
        {
            db.Configuration.LazyLoadingEnabled = false;
            db.Configuration.ProxyCreationEnabled = false;

            var od = db.OrderDetails.Where(o => o.OrderID == id).Select(o => o).ToList();

            return od;
        }

        public List<OrderDetail> CreateOrderDetail(List<OrderDetail> orderDetails)
        {
            db.Configuration.LazyLoadingEnabled = false;
            db.Configuration.ProxyCreationEnabled = false;
            foreach(var orderDetail in orderDetails)
            {             
                var inStock = db.Products.Where(p=> p.ProductID == orderDetail.ProductID).Select(p => p.UnitsInStock).FirstOrDefault();
                if(orderDetail.Quantity <= inStock)
                {
                    var product = db.Products.Where(p => p.ProductID == orderDetail.ProductID).Select(p => p).FirstOrDefault();
                    inStock -= orderDetail.Quantity; 
                    product.UnitsInStock = inStock;
                }
                else
                {
                    var product = db.Products.Where(p => p.ProductID == orderDetail.ProductID).Select(p => p).FirstOrDefault();
                    orderDetail.Quantity -= inStock;
                    product.UnitsInStock = 0;
                    db.SaveChanges();

                }
                db.insertOrderDetail(orderDetail.OrderID, orderDetail.ProductID, orderDetail.Quantity, orderDetail.Packaged, orderDetail.PackagedBy);
            }

            return orderDetails;
        }
        public OrderDetail DetailOrderDetail(int oid, int pid, int q)
        {
            db.Configuration.LazyLoadingEnabled = false;
            db.Configuration.ProxyCreationEnabled = false;

            OrderDetail od = new OrderDetail
            {
                DetailID = db.OrderDetails.OrderByDescending(ord => ord.DetailID).Select(ord => ord.DetailID).FirstOrDefault() + 1,
                OrderID = oid,
                ProductID = pid,
                Quantity = q,
                Packaged = false,
                PackagedBy = null
            };

            //db.Orders.Add(o);
            //db.insertOrderDetail(oid, pid, q, false, null);

            return od;
        }


        public OrderDetail EditOrderDetail(int oid, int id)
        {
            throw new NotImplementedException();
        }
        public Address EditAddress(Address address)
        {
            db.Configuration.LazyLoadingEnabled = false;
            db.Configuration.ProxyCreationEnabled = false;
            int type = address.AddressType + 1;

            db.updateAddress(address.AddressID, type, address.Street, address.Suburb, address.City, address.PostalCode, address.Country);
            db.SaveChanges();
            return address;
        }
        public Address CreateAddress(Address address, int id)
        {
            db.Configuration.LazyLoadingEnabled = false;
            db.Configuration.ProxyCreationEnabled = false;

            db.insertCustomerAddress(id, address.AddressType, address.Street, address.Suburb, address.City, address.PostalCode, address.Country);
            db.SaveChanges();
            return address;
        }
    }
}
