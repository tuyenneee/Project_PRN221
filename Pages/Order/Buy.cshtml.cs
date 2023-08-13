using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WebRazor.Hubs;

namespace WebRazor.Pages.Order
{
    public class BuyModel : PageModel
    {
        private readonly WebRazor.Models.PRN221DBContext _context;
        private readonly IHubContext<HubServer> hubContext;

        public BuyModel(WebRazor.Models.PRN221DBContext context, IHubContext<HubServer> hubContext)
        {
            _context = context;
            this.hubContext = hubContext;
        }

        [BindProperty]
        public Models.Product Product { get; set; } = default!;
  
        public int? ProductQuantity;


        public IActionResult OnGet(int? quantity, int? productid, int? orderid)
        {

            var Productss = _context.Products.SingleOrDefault(x => x.ProductId == productid);
            ProductQuantity = Productss.UnitsInStock;
            if (quantity != null)
            {
                string updateQuery = "UPDATE [dbo].[Products] SET  [UnitsInStock] = @quantity  WHERE ProductID = @productid";
                string updateQuery2 = "UPDATE [dbo].[Order Details] SET [Discount] = 1 WHERE OrderID = @orderid";
                SqlParameter parameter = new SqlParameter("@quantity", ProductQuantity - quantity);
                SqlParameter parameter1 = new SqlParameter("@productid", productid);
                SqlParameter parameter2 = new SqlParameter("@orderid", orderid);
                _context.Database.ExecuteSqlRaw(updateQuery, parameter, parameter1);
                _context.Database.ExecuteSqlRaw(updateQuery2, parameter2);
                _context.SaveChangesAsync();
            }
            return Redirect("/Order");
        }
    }
}
