using iShop.Data;
using iShop.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace iShop.Controllers
{
    public class AdminController : Controller
    {
        private readonly iShopContext _context;
        private readonly IWebHostEnvironment _env;

        public AdminController(iShopContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Products = _context.Product.ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Picture picture, List<IFormFile> PictureUrl)
        {
            if(PictureUrl != null)
            {
                foreach(IFormFile item in PictureUrl)
                {
                    long timeNow = DateTimeOffset.Now.ToUnixTimeSeconds();
                    string timeStamp = timeNow.ToString();
                    string fileName = timeStamp + "_" + item.FileName;
                    string filePath = Path.Combine(_env.WebRootPath, "images", fileName);
                    using (FileStream stream = new FileStream(filePath, FileMode.Create))
                    {
                        await item.CopyToAsync(stream);
                    }
                    Picture pic = new Picture() { 
                        ProductId = picture.ProductId,
                        PictureUrl = "/images/"+ fileName
                    };
                    _context.Picture.Add(pic);
                }
                _context.SaveChanges();
            }
            return RedirectToAction("Create");
        }

        public void ReadJsonFile()
        {
            using (StreamReader reader = new StreamReader("products.txt") )
            {
                string json = reader.ReadToEnd();
                List<Product> products = JsonSerializer.Deserialize<List<Product>>(json);
                foreach(Product item in products)
                {
                    _context.Product.Add(item);
                }
                _context.SaveChanges();
            }
        }
    }
}
