using ECommerseApp.Models;
using System.Data.SqlClient;

namespace ECommerseApp.Services
{
    public class OrderService
    {
        private readonly string _connectionString;
        public OrderService( IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public OrderByCustomerResponseModel GetLatestOrderByCustomer(OrderByCustomerRequestModel orderByCustomerRequestModel)
        {
            OrderByCustomerResponseModel responseModel = new OrderByCustomerResponseModel();
            string queryString = $"SELECT C.FIRSTNAME, C.LASTNAME FROM CUSTOMERS C WHERE C.CUSTOMERID = '{orderByCustomerRequestModel.CustomerId}' AND C.EMAIL = '{orderByCustomerRequestModel.User}'";

            using(SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(queryString, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            responseModel.Customer = new()
                            {
                                FirstName = reader.GetString(0),
                                LastName = reader.GetString(1)
                            };
                        }
                    }
                }
                if (responseModel.Customer != null)
                {
                    queryString = $"SELECT O.ORDERID, O.ORDERDATE, " +
                      $"C.HOUSENO + ' ' + C.STREET + ', ' + C.TOWN + ', ' + C.POSTCODE AS DELIVERYADDRESS, " +
                      $"P.PRODUCTNAME, OI.QUANTITY, OI.PRICE, O.DELIVERYEXPECTED, O.CONTAINSGIFT " +
                      $"FROM CUSTOMERS C " +
                      $"JOIN ORDERS O ON O.CUSTOMERID = C.CUSTOMERID " +
                      $"JOIN ORDERITEMS OI ON O.ORDERID = OI.ORDERID " +
                      $"JOIN PRODUCTS P ON OI.PRODUCTID = P.PRODUCTID " +
                      $"WHERE C.CUSTOMERID = '{orderByCustomerRequestModel.CustomerId}' " +
                      $"AND C.EMAIL = '{orderByCustomerRequestModel.User}' " +
                      $"AND O.ORDERDATE = (SELECT MAX(ORDERDATE) FROM ORDERS WHERE CUSTOMERID = '{orderByCustomerRequestModel.CustomerId}');";


                    using (SqlCommand command = new SqlCommand(queryString, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if(responseModel.Order == null)
                                {
                                    responseModel.Order = new()
                                    {
                                        OrderNumber = reader.GetInt32(reader.GetOrdinal("ORDERID")),
                                        OrderDate = reader.GetDateTime(reader.GetOrdinal("ORDERDATE")),
                                        DeliveryAdress = reader.GetString(reader.GetOrdinal("DELIVERYADDRESS")),
                                        DeliveryExpected = reader.GetDateTime(reader.GetOrdinal("DELIVERYEXPECTED"))
                                    };
                                    responseModel.Order.OrderItems = new();
                                }
                                responseModel.Order.OrderItems.Add(new OrderItemModel()
                                {
                                    Product = (reader.GetBoolean(reader.GetOrdinal("CONTAINSGIFT")) == true) ? "Gift":reader.GetString(reader.GetOrdinal("PRODUCTNAME")),
                                    Quantity = reader.GetInt32(reader.GetOrdinal("QUANTITY")),
                                    PriceEach = reader.GetDecimal(reader.GetOrdinal("PRICE"))
                                });
                            }
                        }
                    }
                }
            }
            return responseModel;
        }
    }
}
