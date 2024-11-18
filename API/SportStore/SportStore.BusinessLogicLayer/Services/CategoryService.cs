using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using SportStore.BusinessLogicLayer.Services.Base;
using SportStore.BusinessLogicLayer.Services.IService;
using SportStore.BusinessLogicLayer.ViewModels;
using SportStore.DataAccessLayer.Models;
using SportStore.DataAccessLayer.Repositories.IRepository;
using CategoryStatus = SportStore.BusinessLogicLayer.ViewModels.CategoryStatus;

namespace SportStore.BusinessLogicLayer.Services
{
    public class CategoryService : BaseService<CategoryModel>, ICategoryService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CategoryService(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
            : base(unitOfWork)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IEnumerable<CategoryVm>> GetAllCategoriesAsync()
        {
            var categories = await _unitOfWork.CategoryRepository.GetAllAsync();
            return categories.Select(c => new CategoryVm
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName,
                Description = c.Description,
                Image = c.Image,
                Status = c.Status.ToString()
            });
        }

        public async Task<CategoryVm> GetCategoryByIdAsync(Guid id)
        {
            var category = await _unitOfWork.CategoryRepository.GetByIdAsync(id);
            if (category == null)
                return null;

            return new CategoryVm
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
                Description = category.Description,
                Image = category.Image,
                Status = category.Status.ToString()
            };
        }

        public async Task<CategoryVm> CreateCategoryAsync(CategoryVm categoryVm)
        {
            var statusMapping = new Dictionary<string, CategoryStatus>(StringComparer.OrdinalIgnoreCase)
            {
                { "Đợi xác nhận", CategoryStatus.Pending },
                { "Mở", CategoryStatus.Open },
                { "Đóng", CategoryStatus.Closed }
            };

            if (string.IsNullOrWhiteSpace(categoryVm.Status))
            {
                categoryVm.Status = "Đợi xác nhận";
            }

            if (!statusMapping.TryGetValue(categoryVm.Status, out CategoryStatus categoryStatus))
            {
                throw new ArgumentException("Status không hợp lệ.");
            }

            var category = new CategoryModel
            {
                CategoryId = Guid.NewGuid(),
                CategoryName = categoryVm.CategoryName,
                Description = categoryVm.Description,
                Status = (DataAccessLayer.Models.CategoryStatus)categoryStatus
            };

            if (categoryVm.ImageFile != null && categoryVm.ImageFile.Length > 0)
            {
                category.Image = await SaveImageAsync(categoryVm.ImageFile);
            }

            await _unitOfWork.CategoryRepository.AddAsync(category);
            await _unitOfWork.SaveChangesAsync();

            categoryVm.CategoryId = category.CategoryId;
            categoryVm.Image = category.Image;

            return categoryVm;
        }

        public async Task<bool> UpdateCategoryAsync(Guid id, CategoryVm categoryVm)
        {
            var category = await _unitOfWork.CategoryRepository.GetByIdAsync(id);
            if (category == null)
                return false;

            var statusMapping = new Dictionary<string, CategoryStatus>(StringComparer.OrdinalIgnoreCase)
            {
                { "Đợi xác nhận", CategoryStatus.Pending },
                { "Mở", CategoryStatus.Open },
                { "Đóng", CategoryStatus.Closed }
            };

            if (string.IsNullOrWhiteSpace(categoryVm.Status))
            {
                categoryVm.Status = "Đợi xác nhận";
            }

            if (!statusMapping.TryGetValue(categoryVm.Status, out CategoryStatus categoryStatus))
            {
                throw new ArgumentException("Status không hợp lệ.");
            }

            category.CategoryName = categoryVm.CategoryName;
            category.Description = categoryVm.Description;
            category.Status = (DataAccessLayer.Models.CategoryStatus)categoryStatus;

            if (categoryVm.ImageFile != null && categoryVm.ImageFile.Length > 0)
            {
                if (!string.IsNullOrEmpty(category.Image))
                {
                    DeleteImage(category.Image);
                }

                category.Image = await SaveImageAsync(categoryVm.ImageFile);
            }

            _unitOfWork.CategoryRepository.Update(category);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteCategoryAsync(Guid id)
        {
            var category = await _unitOfWork.CategoryRepository.GetByIdAsync(id);
            if (category == null)
                return false;

            if (!string.IsNullOrEmpty(category.Image))
            {
                DeleteImage(category.Image);
            }

            _unitOfWork.CategoryRepository.RemoveAsync(category.CategoryId);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        private async Task<string> SaveImageAsync(IFormFile imageFile)
        {
            var uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img", "categories");
            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }

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
            var uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img", "categories");
            var imagePath = Path.Combine(uploadFolder, imageName);

            if (File.Exists(imagePath))
            {
                File.Delete(imagePath);
            }
        }
    }
}