using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace DigitalXService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IService1
    {

        [OperationContract]
        List<Product> GetPopularProducts();
        [OperationContract]
        Product GetProduct(int id);
        [OperationContract]
        List<Product> GetAllProducts();
        [OperationContract]
        List<ProductCategory> GetAllCategories();
        [OperationContract]
        List<ProductSubCategory> GetAllSubCategories();
        [OperationContract]
        List<Retailer> GetAllRetailers();
        [OperationContract]
        List<Product> GetOrderProducts(List<int> productIds);
        [OperationContract]
        List<int> GetUnitsInStock(List<int> id);

        [OperationContract]
        Customer Login(string us, string p);
        [OperationContract]
        Customer Register(Customer customer, Address address, Contact contact);
        [OperationContract]
        List<AddressType> GetAddressTypes();
        [OperationContract]
        List<ContactType> GetContactTypes();

        [OperationContract]
        List<Address> GetAddresses(int id);
        [OperationContract]
        Address GetAddress(int id);
        [OperationContract]
        string GetAddressType(int type);

        [OperationContract]
        Order CreateOrder(Order order);
        [OperationContract]
        Order DetailOrder(int id);
        [OperationContract]
        List<Order> GetOrder(int id);
        [OperationContract]
        List<OrderDetail> GetOrderDetails(int id);

        [OperationContract]
        List<OrderDetail> CreateOrderDetail(List<OrderDetail> orderDetails);
        [OperationContract]
        OrderDetail DetailOrderDetail(int oid, int id, int q);
        [OperationContract]
        OrderDetail EditOrderDetail(int oid, int id);
        [OperationContract]
        Address EditAddress(Address address);
        [OperationContract]
        Address CreateAddress(Address address, int id);
    }
}
