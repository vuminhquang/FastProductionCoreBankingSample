﻿using CoreBanking.Application.Core.DTOs;
using CoreBanking.Application.Core.Queries;
using CoreBanking.Domain.Core.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CoreBanking.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CustomersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCustomerDto dto, CancellationToken cancellationToken = default)
        {
            if (null == dto)
                return BadRequest();
            var command = new CreateCustomer(Guid.NewGuid(), dto.FirstName, dto.LastName, dto.Email);
            await _mediator.Publish(command, cancellationToken);
            
            return CreatedAtAction("GetCustomer", new { id = command.CustomerId }, command);
        }

        [HttpGet, Route("{id:guid}", Name = "GetCustomer")]
        public async Task<IActionResult> GetCustomer(Guid id, CancellationToken cancellationToken= default)
        {
            var query = new CustomerById(id);
            var result = await _mediator.Send(query, cancellationToken);
            if (null == result) 
                return NotFound();
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken cancellationToken = default)
        {
            var query = new CustomersArchive();
            var results = await _mediator.Send(query, cancellationToken);
            if (null == results)
                return NotFound();
            return Ok(results);
        }
    }
}