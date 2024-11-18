using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportStore.DataAccessLayer.Models
{
    public class ProductBrandModel
    {
        public Guid ProductId { get; set; }
        public ProductModel Product { get; set; }

        public Guid BrandId { get; set; }
        public BrandModel Brand { get; set; }
    }
}
