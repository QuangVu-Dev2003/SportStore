﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SportStore.BusinessLogicLayer.Services.Base;
using SportStore.BusinessLogicLayer.Services.IService;
using SportStore.BusinessLogicLayer.ViewModels;
using SportStore.DataAccessLayer.Models;
using SportStore.DataAccessLayer.Repositories.IRepository;

namespace SportStore.BusinessLogicLayer.Services
{
    public class ProductService : BaseService<ProductModel>, IProductService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductService(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment) : base(unitOfWork)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IEnumerable<ProductVm>> GetAllProductsAsync()
        {
            var products = await _unitOfWork.ProductRepository
                .GetDbSet()
                .Include(p => p.Brand)
                .Include(p => p.ProductCategoryModels)
                    .ThenInclude(pc => pc.Category)
                .ToListAsync();

            return products.Select(p => MapToViewModel(p));
        }

        public async Task<ProductVm> GetProductByIdAsync(Guid productId)
        {
            var product = await _unitOfWork.ProductRepository
                .GetDbSet()
                .Include(p => p.Brand)
                .Include(p => p.ProductCategoryModels)
                    .ThenInclude(pc => pc.Category)
                .FirstOrDefaultAsync(p => p.ProductId == productId);

            return product == null ? null : MapToViewModel(product);
        }

        public async Task<ProductVm> CreateProductAsync(ProductVm productVm)
        {
            var brand = (await _unitOfWork.BrandRepository.FindAsync(b => b.BrandName == productVm.BrandName)).FirstOrDefault();
            if (brand == null)
            {
                throw new Exception($"Thương hiệu '{productVm.BrandName}' không tồn tại.");
            }

            var categoryIds = new List<Guid>();
            foreach (var categoryName in productVm.CategoryNames)
            {
                var category = (await _unitOfWork.CategoryRepository.FindAsync(c => c.CategoryName == categoryName)).FirstOrDefault();
                if (category == null)
                {
                    throw new Exception($"Danh mục '{categoryName}' không tồn tại.");
                }
                categoryIds.Add(category.CategoryId);
            }

            var product = new ProductModel
            {
                ProductId = Guid.NewGuid(),
                ProductName = productVm.ProductName,
                Description = productVm.Description,
                Detail = productVm.Detail,
                Price = productVm.Price,
                Instock = productVm.Instock,
                Status = (ProductStatus)productVm.Status,
                AddDate = productVm.AddDate,
                BrandId = brand.BrandId,
                ProductCategoryModels = categoryIds.Select(id => new ProductCategoryModel { CategoryId = id }).ToList()
            };

            if (productVm.ImageFiles != null && productVm.ImageFiles.Any())
            {
                product.Images = new List<string>();
                foreach (var imageFile in productVm.ImageFiles)
                {
                    product.Images.Add(await SaveImageAsync(imageFile));
                }
            }

            await _unitOfWork.ProductRepository.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();
            return MapToViewModel(product);
        }

        public async Task<bool> UpdateProductAsync(Guid productId, ProductVm productVm)
        {
            var product = await _unitOfWork.ProductRepository
                .GetDbSet()
                .Include(p => p.Brand)
                .Include(p => p.ProductCategoryModels)
                    .ThenInclude(pc => pc.Category)
                .FirstOrDefaultAsync(p => p.ProductId == productId);

            if (product == null) return false;

            var brand = (await _unitOfWork.BrandRepository.FindAsync(b => b.BrandName == productVm.BrandName)).FirstOrDefault();
            if (brand == null)
            {
                throw new Exception($"Thương hiệu '{productVm.BrandName}' không tồn tại.");
            }
            product.BrandId = brand.BrandId;

            if (productVm.CategoryNames != null && productVm.CategoryNames.Any())
            {
                product.ProductCategoryModels.Clear();

                foreach (var categoryName in productVm.CategoryNames)
                {
                    var category = (await _unitOfWork.CategoryRepository.FindAsync(c => c.CategoryName == categoryName)).FirstOrDefault();
                    if (category == null)
                    {
                        throw new Exception($"Danh mục '{categoryName}' không tồn tại.");
                    }

                    product.ProductCategoryModels.Add(new ProductCategoryModel
                    {
                        ProductId = product.ProductId,
                        CategoryId = category.CategoryId
                    });
                }
            }

            product.ProductName = productVm.ProductName;
            product.Description = productVm.Description;
            product.Detail = productVm.Detail;
            product.Price = productVm.Price;
            product.Instock = productVm.Instock;
            product.Status = (ProductStatus)productVm.Status;

            if (productVm.ImageFiles != null && productVm.ImageFiles.Any())
            {
                if (product.Images != null && product.Images.Any())
                {
                    foreach (var image in product.Images)
                    {
                        DeleteImage(image);
                    }
                }

                product.Images = new List<string>();
                foreach (var imageFile in productVm.ImageFiles)
                {
                    product.Images.Add(await SaveImageAsync(imageFile));
                }
            }

            _unitOfWork.ProductRepository.Update(product);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteProductAsync(Guid productId)
        {
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(productId);
            if (product == null) return false;

            if (product.Images != null)
            {
                foreach (var image in product.Images)
                {
                    DeleteImage(image);
                }
            }

            _unitOfWork.ProductRepository.RemoveAsync(productId);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ProductVm>> GetProductsByCategoryAsync(string categoryName)
        {
            var category = (await _unitOfWork.CategoryRepository.FindAsync(c => c.CategoryName == categoryName)).FirstOrDefault();
            if (category == null)
            {
                throw new Exception($"Danh mục '{categoryName}' không tồn tại.");
            }

            var products = await _unitOfWork.GenericRepository<ProductModel>()
                .FindAsync(p => p.ProductCategoryModels.Any(pc => pc.CategoryId == category.CategoryId));

            return products.Select(MapToViewModel);
        }

        public async Task<IEnumerable<ProductVm>> GetProductsByBrandAsync(string brandName)
        {
            var brand = (await _unitOfWork.BrandRepository.FindAsync(b => b.BrandName == brandName)).FirstOrDefault();
            if (brand == null)
            {
                throw new Exception($"Thương hiệu '{brandName}' không tồn tại.");
            }

            var products = await _unitOfWork.GenericRepository<ProductModel>()
                .FindAsync(p => p.BrandId == brand.BrandId);

            return products.Select(MapToViewModel);
        }

        private async Task<string> SaveImageAsync(IFormFile imageFile)
        {
            var uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img", "products");
            if (!Directory.Exists(uploadFolder)) Directory.CreateDirectory(uploadFolder);

            var imageName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imageFile.FileName);
            var filePath = Path.Combine(uploadFolder, imageName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            return imageName;
        }

        private void DeleteImage(string imageName)
        {
            var uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img", "products");
            var imagePath = Path.Combine(uploadFolder, imageName);

            if (File.Exists(imagePath)) File.Delete(imagePath);
        }

        private ProductVm MapToViewModel(ProductModel product)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product), "Product cannot be null.");
            }

            return new ProductVm
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                Description = product.Description,
                Detail = product.Detail,
                Price = product.Price,
                Instock = product.Instock,
                Status = (ProductViewStatus)product.Status,
                AddDate = product.AddDate,
                BrandName = product.Brand?.BrandName ?? "Unknown Brand",
                CategoryNames = product.ProductCategoryModels != null
                    ? product.ProductCategoryModels.Select(pc => pc.Category?.CategoryName ?? "Unknown Category").ToList()
                    : new List<string>(),
                Images = product.Images ?? new List<string>()
            };
        }
    }
}
