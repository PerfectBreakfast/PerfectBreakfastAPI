using MapsterMapper;
using PerfectBreakfast.Application.Commons;
using PerfectBreakfast.Application.Interfaces;
using PerfectBreakfast.Application.Models.CategoryModels.Request;
using PerfectBreakfast.Application.Models.CategoryModels.Response;
using PerfectBreakfast.Application.Models.RoleModels.Response;
using PerfectBreakfast.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerfectBreakfast.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<OperationResult<CategoryResponse>> CreateCategory(CreateCategoryRequest requestModel)
        {
            var result = new OperationResult<CategoryResponse>();
            try
            {
                // map model to Entity
                var category = _mapper.Map<Category>(requestModel);
                // Add to DB
                var entity = await _unitOfWork.CategoryRepository.AddAsync(category);
                // save change 
                await _unitOfWork.SaveChangeAsync();
                // map model to response
                result.Payload = _mapper.Map<CategoryResponse>(entity);
            }
            catch (Exception e)
            {
                result.AddUnknownError(e.Message);
            }
            return result;
        }

        public async Task<OperationResult<List<CategoryResponse>>> GetAllCategorys()
        {
            var result = new OperationResult<List<CategoryResponse>>();
            try
            {
                var categories = await _unitOfWork.CategoryRepository.GetAllAsync();
                result.Payload = _mapper.Map<List<CategoryResponse>>(categories);
            }
            catch (Exception e)
            {
                result.AddUnknownError(e.Message);
            }
            return result;
        }

        public async Task<OperationResult<CategoryResponse>> RemoveCategory(Guid categoryId)
        {
            var result = new OperationResult<CategoryResponse>();
            try
            {
                // find supplier by ID
                var category = await _unitOfWork.CategoryRepository.GetByIdAsync(categoryId);
                // Remove
                var entity = _unitOfWork.CategoryRepository.Remove(category);
                // saveChange
                await _unitOfWork.SaveChangeAsync();
                // map entity to SupplierResponse
                result.Payload = _mapper.Map<CategoryResponse>(entity);
            }
            catch (Exception e)
            {
                result.AddUnknownError(e.Message);
            }
            return result;
        }

        public async Task<OperationResult<CategoryResponse>> UpdateCategory(Guid categoryId, UpdateCategoryRequest requestModel)
        {
            var result = new OperationResult<CategoryResponse>();
            try
            {
                // find supplier by ID
                var category = await _unitOfWork.CategoryRepository.GetByIdAsync(categoryId);
                // map from requestModel => supplier
                _mapper.Map(requestModel, category);
                // update
                _unitOfWork.CategoryRepository.Update(category);
                // saveChange
                await _unitOfWork.SaveChangeAsync();
                result.Payload = _mapper.Map<CategoryResponse>(category);
            }
            catch (Exception e)
            {
                result.AddUnknownError(e.Message);
            }
            return result;
        }
    }
}
