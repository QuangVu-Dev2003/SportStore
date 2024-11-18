using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using SportStore.BusinessLogicLayer.Services.Base;
using SportStore.BusinessLogicLayer.Services.IService;
using SportStore.BusinessLogicLayer.ViewModels;
using SportStore.DataAccessLayer.Models;
using SportStore.DataAccessLayer.Repositories.IRepository;
using BrandStatus = SportStore.BusinessLogicLayer.ViewModels.BrandStatus;

namespace SportStore.BusinessLogicLayer.Services
{
    public class BrandService : BaseService<BrandModel>, IBrandService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public BrandService(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
            : base(unitOfWork)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IEnumerable<BrandVm>> GetAllBrandsAsync()
        {
            var brands = await _unitOfWork.BrandRepository.GetAllAsync();
            return brands.Select(b => new BrandVm
            {
                BrandId = b.BrandId,
                BrandName = b.BrandName,
                Description = b.Description,
                Image = b.Image,
                Status = b.Status.ToString()
            });
        }

        public async Task<BrandVm> GetBrandByIdAsync(Guid id)
        {
            var brand = await _unitOfWork.BrandRepository.GetByIdAsync(id);
            if (brand == null)
                return null;

            return new BrandVm
            {
                BrandId = brand.BrandId,
                BrandName = brand.BrandName,
                Description = brand.Description,
                Image = brand.Image,
                Status = brand.Status.ToString()
            };
        }

        public async Task<BrandVm> CreateBrandAsync(BrandVm brandVm)
        {
            var statusMapping = new Dictionary<string, BrandStatus>(StringComparer.OrdinalIgnoreCase)
            {
                { "Đợi xác nhận", BrandStatus.Pending },
                { "Mở", BrandStatus.Open },
                { "Đóng", BrandStatus.Closed }
            };

            if (string.IsNullOrWhiteSpace(brandVm.Status))
            {
                brandVm.Status = "Đợi xác nhận";
            }

            if (!statusMapping.TryGetValue(brandVm.Status, out BrandStatus brandStatus))
            {
                throw new ArgumentException("Status không hợp lệ.");
            }

            var brand = new BrandModel
            {
                BrandId = Guid.NewGuid(),
                BrandName = brandVm.BrandName,
                Description = brandVm.Description,
                Status = (DataAccessLayer.Models.BrandStatus)brandStatus
            };

            if (brandVm.ImageFile != null && brandVm.ImageFile.Length > 0)
            {
                brand.Image = await SaveImageAsync(brandVm.ImageFile);
            }

            await _unitOfWork.BrandRepository.AddAsync(brand);
            await _unitOfWork.SaveChangesAsync();

            brandVm.BrandId = brand.BrandId;
            brandVm.Image = brand.Image;

            return brandVm;
        }

        public async Task<bool> UpdateBrandAsync(Guid id, BrandVm brandVm)
        {
            var brand = await _unitOfWork.BrandRepository.GetByIdAsync(id);
            if (brand == null)
                return false;

            var statusMapping = new Dictionary<string, BrandStatus>(StringComparer.OrdinalIgnoreCase)
            {
                { "Đợi xác nhận", BrandStatus.Pending },
                { "Mở", BrandStatus.Open },
                { "Đóng", BrandStatus.Closed }
            };

            if (string.IsNullOrWhiteSpace(brandVm.Status))
            {
                brandVm.Status = "Đợi xác nhận";
            }

            if (!statusMapping.TryGetValue(brandVm.Status, out BrandStatus categoryStatus))
            {
                throw new ArgumentException("Status không hợp lệ.");
            }

            brand.BrandName = brandVm.BrandName;
            brand.Description = brandVm.Description;
            brand.Status = (DataAccessLayer.Models.BrandStatus)categoryStatus;

            if (brandVm.ImageFile != null && brandVm.ImageFile.Length > 0)
            {
                if (!string.IsNullOrEmpty(brand.Image))
                {
                    DeleteImage(brand.Image);
                }

                brand.Image = await SaveImageAsync(brandVm.ImageFile);
            }

            _unitOfWork.BrandRepository.Update(brand);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteBrandAsync(Guid id)
        {
            var brand = await _unitOfWork.BrandRepository.GetByIdAsync(id);
            if (brand == null)
                return false;

            if (!string.IsNullOrEmpty(brand.Image))
            {
                DeleteImage(brand.Image);
            }

            _unitOfWork.BrandRepository.RemoveAsync(brand.BrandId);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        private async Task<string> SaveImageAsync(IFormFile imageFile)
        {
            var uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img", "brands");
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
            var uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img", "brands");
            var imagePath = Path.Combine(uploadFolder, imageName);

            if (File.Exists(imagePath))
            {
                File.Delete(imagePath);
            }
        }
    }
}