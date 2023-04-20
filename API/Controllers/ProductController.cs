using Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Core.Interfaces;
using Core.Specifications;
using API.Dtos;
using AutoMapper;
using API.Errors;

namespace API.Controllers
{
    public class ProductController : BaseApiController
    {
        private readonly IGenericRepo<Product> _productRepo;
        private readonly IGenericRepo<ProductType> _typeRepo;
        private readonly IGenericRepo<ProductBrand> _brandRepo;
        private readonly IMapper _mapper;

        public ProductController(IGenericRepo<Product> productRepo, 
        IGenericRepo<ProductType> typeRepo, IGenericRepo<ProductBrand> brandRepo, IMapper mapper)
        {
            _productRepo = productRepo;
            _brandRepo = brandRepo;
            _typeRepo = typeRepo;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<ProductToReturnDto>>> GetProducts()
        {
            var spec = new ProductsWithTypesAndBrandsSpec();

            var products = await _productRepo.ListAsync(spec);

            return 
            Ok(_mapper.Map<IReadOnlyList<Product>,
            IReadOnlyList<ProductToReturnDto>>(products));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse),StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductToReturnDto>> GetProduct(int id)
        {
            var spec = new ProductsWithTypesAndBrandsSpec(id);
            var product = await _productRepo.GetEntityWithSpec(spec);
            if(product == null) return NotFound(new ApiResponse(404));
            return _mapper.Map<Product, ProductToReturnDto>(product);
        }

        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetProductBrands()
        {
            var brands = await _brandRepo.ListAllAsync();
            return Ok(brands);
        }

        [HttpGet("types")]

        public async Task<ActionResult<IReadOnlyList<ProductType>>> GetProductTypes()
        {
            var types = await _typeRepo.ListAllAsync();
            return Ok(types);
        }
    }
}
