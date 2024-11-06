using ECommerseApp.Models;
using ECommerseApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerseApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _orderService;
        public OrderController(OrderService orderService)
        {
            _orderService = orderService;
        }
        [HttpPost]
        public IActionResult GetLatestOrderByCustomer([FromBody]OrderByCustomerRequestModel customerModel)
        {
           var orderDetails = _orderService.GetLatestOrderByCustomer(customerModel);
            if (orderDetails.Customer == null)
            {
                return BadRequest("Customer not found");
            }
            return Ok(orderDetails);
        }
    }
}
