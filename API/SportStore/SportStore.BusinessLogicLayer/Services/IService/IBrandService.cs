using SportStore.BusinessLogicLayer.Services.Base;
using SportStore.BusinessLogicLayer.ViewModels;
using SportStore.DataAccessLayer.Models;

namespace SportStore.BusinessLogicLayer.Services.IService
{
    public interface IBrandService : IBaseService<BrandModel>
    {
        Task<IEnumerable<BrandVm>> GetAllBrandsAsync();
        Task<BrandVm> GetBrandByIdAsync(Guid id);
        Task<BrandVm> CreateBrandAsync(BrandVm brandVm);
        Task<bool> UpdateBrandAsync(Guid id, BrandVm brandVm);
        Task<bool> DeleteBrandAsync(Guid id);
    }
}