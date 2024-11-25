using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportStore.BusinessLogicLayer.Services.IService;
using SportStore.BusinessLogicLayer.ViewModels;

namespace SportStore.WebApi.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IBrandService _brandService;

        public BrandsController(IBrandService brandService, IMapper mapper)
        {
            _brandService = brandService;
            _mapper = mapper;
        }

        [HttpGet("get-all-brands")]
        public async Task<IActionResult> GetAllBrands()
        {
            var brands = await _brandService.GetAllBrandsAsync();
            return Ok(brands);
        }

        [HttpGet("get-brand-by-id/{id}")]
        public async Task<IActionResult> GetBrandById(Guid id)
        {
            var brand = await _brandService.GetBrandByIdAsync(id);
            return brand == null ? NotFound("Không tìm thấy thương hiệu.") : Ok(brand);
        }

        [HttpPost("create-brand")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateBrand([FromForm] BrandVm brandVm)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var newBrand = await _brandService.CreateBrandAsync(brandVm);
            return CreatedAtAction(nameof(GetBrandById), new { id = newBrand.BrandId }, newBrand);
        }

        [HttpPut("update-brand/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateBrand(Guid id, [FromForm] BrandVm brandVm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid data provided." });
            }

            var updated = await _brandService.UpdateBrandAsync(id, brandVm);
            if (updated)
            {
                return Ok(new { message = "Cập nhật thành công" });
            }
            else
            {
                return NotFound(new { message = "Không tìm thấy thương hiệu." });
            }
        }

        [HttpDelete("delete-brand/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteBrand(Guid id)
        {
            var deleted = await _brandService.DeleteBrandAsync(id);

            if (deleted)
            {
                return Ok(new { message = "Xóa thành công" });
            }
            else
            {
                return NotFound(new { message = "Không tìm thấy thương hiệu." });
            }
        }
    }
}