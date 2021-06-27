using ApplicationFoundation.Interfaces;
using ConfigManager.Lib;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Products.Core.Entities;
using Products.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Products.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {

        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("isAlive")]
        [AllowAnonymous]
        public IActionResult IsAlive()
        {
            var sdsd = Helper.ConfigExtension.Instance.GetValue<string>("Databases:Product:ConnectionString");
            var sdsdd =  _productService.GetAll();

            var model = new Product()
            {
                Id = "546c776b3e23f5f2ebdd3b03",
                Name = "Test",
                Description = "Desc 1"
            };

            _productService.InsertOne(model);
            
            return Ok(sdsd);
        }
    }
}
