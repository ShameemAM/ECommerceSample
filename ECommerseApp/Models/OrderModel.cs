namespace ECommerseApp.Models
{
    public class OrderModel
    {
        public int OrderNumber {  get; set; }
        public DateTime OrderDate {  get; set; }
        public string DeliveryAdress { get; set; }
        public List<OrderItemModel> OrderItems { get; set; }
        public DateTime DeliveryExpected {  get; set; } 
    }
}
