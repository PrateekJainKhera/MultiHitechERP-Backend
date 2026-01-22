using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MultiHitechERP.API.DTOs.Request;
using MultiHitechERP.API.Services.Interfaces;

namespace MultiHitechERP.API.Controllers.Masters
{
    /// <summary>
    /// Controller for Customer master data operations
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        /// <summary>
        /// Get all customers
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _customerService.GetAllAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Get customer by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _customerService.GetByIdAsync(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Get customer by customer code
        /// </summary>
        [HttpGet("by-code/{customerCode}")]
        public async Task<IActionResult> GetByCustomerCode(string customerCode)
        {
            var result = await _customerService.GetByCustomerCodeAsync(customerCode);
            return result.Success ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Get all active customers
        /// </summary>
        [HttpGet("active")]
        public async Task<IActionResult> GetActiveCustomers()
        {
            var result = await _customerService.GetActiveCustomersAsync();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Search customers by name
        /// </summary>
        [HttpGet("search")]
        public async Task<IActionResult> SearchByName([FromQuery] string searchTerm)
        {
            var result = await _customerService.SearchByNameAsync(searchTerm);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Get customers by type
        /// </summary>
        [HttpGet("by-type/{customerType}")]
        public async Task<IActionResult> GetByCustomerType(string customerType)
        {
            var result = await _customerService.GetByCustomerTypeAsync(customerType);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Get customers by city
        /// </summary>
        [HttpGet("by-city/{city}")]
        public async Task<IActionResult> GetByCity(string city)
        {
            var result = await _customerService.GetByCityAsync(city);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Get customers by state
        /// </summary>
        [HttpGet("by-state/{state}")]
        public async Task<IActionResult> GetByState(string state)
        {
            var result = await _customerService.GetByStateAsync(state);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Create new customer
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCustomerRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _customerService.CreateCustomerAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Update existing customer
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCustomerRequest request)
        {
            if (id != request.Id)
            {
                return BadRequest("ID mismatch");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _customerService.UpdateCustomerAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Delete customer
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _customerService.DeleteCustomerAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Activate customer
        /// </summary>
        [HttpPost("{id}/activate")]
        public async Task<IActionResult> Activate(int id)
        {
            var result = await _customerService.ActivateCustomerAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Deactivate customer
        /// </summary>
        [HttpPost("{id}/deactivate")]
        public async Task<IActionResult> Deactivate(int id)
        {
            var result = await _customerService.DeactivateCustomerAsync(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
