using SportStore.BusinessLogicLayer.Services.Base;
using SportStore.BusinessLogicLayer.ViewModels;
using SportStore.DataAccessLayer.Models;

namespace SportStore.BusinessLogicLayer.Services.IService
{
    public interface ICategoryService : IBaseService<CategoryModel>
    {
        Task<IEnumerable<CategoryVm>> GetAllCategoriesAsync();
        Task<CategoryVm> GetCategoryByIdAsync(Guid id);
        Task<CategoryVm> CreateCategoryAsync(CategoryVm categoryVm);
        Task<bool> UpdateCategoryAsync(Guid id, CategoryVm categoryVm);
        Task<bool> DeleteCategoryAsync(Guid id);
    }
}