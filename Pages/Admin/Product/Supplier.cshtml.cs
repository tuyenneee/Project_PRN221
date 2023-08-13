using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OfficeOpenXml;
using System.Diagnostics;

namespace WebRazor.Pages.Admin.Product
{
    public class SupplierModel : PageModel
    {
        private readonly WebRazor.Models.PRN221DBContext _context;

        public SupplierModel(WebRazor.Models.PRN221DBContext context)
        {
            _context = context;
        }
        public List<Models.Product> Product = new List<Models.Product>();
        public async Task OnGetAsync()
        {
        }

        private int getTotalProducts()
        {
            var list = (from p in _context.Products select p).ToList();

            if (!String.IsNullOrEmpty(search))
            {
                return list.Where(p => p.ProductName.ToLower().Contains(search.ToLower())).ToList().Count;
            }
            else if (categoryChoose > 0)
            {
                return list.Where(p => p.CategoryId == categoryChoose).ToList().Count;
            }
            else
            {
                return list.Count;
            }
        }

        [HttpPost]
        public async Task OnPostImportAsync(IFormFile file)
        {
            //List<Models.Product> lstImportProduct = new List<Models.Product>();
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                try
                {
                    using (var package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                        var rowcount = worksheet.Dimension.Rows;
                        for (int row = 2; row <= rowcount; row++)
                        {
                            Models.Product x = new Models.Product();
                            x.ProductName = worksheet.Cells[row, 2].Value.ToString().Trim();
                            x.QuantityPerUnit = worksheet.Cells[row, 3].Value.ToString().Trim();
                            x.UnitPrice = int.Parse(worksheet.Cells[row, 4].Value.ToString().Trim());
                            x.UnitsInStock = short.Parse(worksheet.Cells[row, 5].Value.ToString().Trim());
                            x.UnitsOnOrder = short.Parse(worksheet.Cells[row, 6].Value.ToString().Trim());
                            x.ReorderLevel = short.Parse(worksheet.Cells[row, 7].Value.ToString().Trim());
                            x.CategoryId = 1;
                            Product.Add(x);
                            _context.Products.Add(x);
                            _context.SaveChanges();
                            //lstImportProduct.Add(x);
                        }
                        //foreach (var p in Product)
                        //{
                        //    Models.Request x = new Models.Request();
                        //    x.ProductName = p.ProductName.Trim();
                        //    x.QuantityPerUnit = p.QuantityPerUnit.Trim();
                        //    x.UnitPrice = p.UnitPrice;
                        //    x.UnitsInStock = p.UnitsInStock;
                        //    x.UnitsOnOrder = p.UnitsOnOrder;
                        //    x.ReorderLevel = p.ReorderLevel;
                        //    x.Status = 2;
                        //    _context.Requests.Add(x);
                        //    _context.SaveChanges();
                        //}

                    }
                }
                catch (Exception e) { Debug.WriteLine(e.ToString()); }
            }
            //   HttpContext.Session.Set("request", lstImportProduct);
        }

        //public async Task OnPostRequestAsync()
        //{
        //    List<Models.Request> lstImportProduct = new List<Models.Request>();
        //    foreach (var p in Product)
        //    {
        //        Models.Request x = new Models.Request();
        //        x.ProductName = p.ProductName.Trim();
        //        x.QuantityPerUnit = p.QuantityPerUnit.Trim();
        //        x.UnitPrice = p.UnitPrice;
        //        x.UnitsInStock = p.UnitsInStock;
        //        x.UnitsOnOrder = p.UnitsOnOrder;
        //        x.ReorderLevel = p.ReorderLevel;
        //        x.Status = 2;
        //        _context.Requests.Add(x);
        //        _context.SaveChanges();
        //    }

        //}
        public const int pageSize = 10;
        [BindProperty(SupportsGet = true, Name = "currentPage")]
        public int currentPage { get; set; }
        [BindProperty(SupportsGet = true)]
        public string search { get; set; }
        [BindProperty(SupportsGet = true)]
        public int categoryChoose { get; set; }
        public int countPages { get; set; }
    }
}

