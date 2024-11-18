using SportStore.BusinessLogicLayer.Services.Base;
using SportStore.BusinessLogicLayer.ViewModels;
using SportStore.DataAccessLayer.Models;

namespace SportStore.BusinessLogicLayer.Services.IService
{
    public interface IProductService : IBaseService<ProductModel>
    {
        Task<IEnumerable<ProductVm>> GetAllProductsAsync();
        Task<ProductVm> GetProductByIdAsync(Guid productId);
        Task<ProductVm> CreateProductAsync(ProductVm productVm);
        Task<bool> UpdateProductAsync(Guid productId, ProductVm productVm);
        Task<bool> DeleteProductAsync(Guid productId);
        Task<IEnumerable<ProductVm>> GetProductsByCategoryAsync(string categoryName);
        Task<IEnumerable<ProductVm>> GetProductsByBrandAsync(string brandName);
    }
}
