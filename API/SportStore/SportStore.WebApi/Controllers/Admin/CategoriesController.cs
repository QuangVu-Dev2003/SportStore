using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportStore.BusinessLogicLayer.Services.IService;
using SportStore.BusinessLogicLayer.ViewModels;

namespace SportStore.WebApi.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService, IMapper mapper)
        {
            _categoryService = categoryService;
            _mapper = mapper;
        }

        [HttpGet("get-all-categories")]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return Ok(categories);
        }

        [HttpGet("get-category-by-id/{id}")]
        public async Task<IActionResult> GetCategoryById(Guid id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            return category == null ? NotFound("Không tìm thấy danh mục.") : Ok(category);
        }

        [HttpPost("create-category")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCategory([FromForm] CategoryVm categoryVm)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var newCategory = await _categoryService.CreateCategoryAsync(categoryVm);
            return CreatedAtAction(nameof(GetCategoryById), new { id = newCategory.CategoryId }, newCategory);
        }

        [HttpPut("update-category/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCategory(Guid id, [FromForm] CategoryVm categoryVm)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updated = await _categoryService.UpdateCategoryAsync(id, categoryVm);
            return updated ? Ok("Cập nhật thành công") : NotFound("Không tìm thấy danh mục.");
        }

        [HttpDelete("delete-category/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            var deleted = await _categoryService.DeleteCategoryAsync(id);
            return deleted ? Ok("Xóa thành công") : NotFound("Không tìm thấy danh mục.");
        }
    }
}