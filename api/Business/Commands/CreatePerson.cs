﻿using MediatR;
using MediatR.Pipeline;
using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Data;
using StargateAPI.Controllers;

namespace StargateAPI.Business.Commands
{
    public class CreatePerson : IRequest<CreatePersonResult>
    {
        public required string Name { get; set; } = string.Empty;
    }

    public class CreatePersonPreProcessor : IRequestPreProcessor<CreatePerson>
    {
        private readonly StargateContext _context;

        public CreatePersonPreProcessor(StargateContext context)
        {
            _context = context;
        }

        public Task Process(CreatePerson request, CancellationToken cancellationToken)
        {
            var person = _context.People.AsNoTracking().FirstOrDefault(z => z.Name == request.Name);

            if (person is not null) {
                _context.ExceptionLog.Add(new ExceptionLog { Process = "Create Person", Message = $"{person.Name} already exists" });
                _context.SaveChanges();
                throw new BadHttpRequestException("Person already exists"); 
            }

            return Task.CompletedTask;
        }    
    }

    public class CreatePersonHandler : IRequestHandler<CreatePerson, CreatePersonResult>
    {
        private readonly StargateContext _context;

        public CreatePersonHandler(StargateContext context)
        {
            _context = context;
        }

        public async Task<CreatePersonResult> Handle(CreatePerson request, CancellationToken cancellationToken)
        {
            var newPerson = new Person()
            {
                Name = request.Name
            };

            await _context.People.AddAsync(newPerson, cancellationToken);
            await _context.SuccessLog.AddAsync(new SuccessLog { Process = "Create Person", Message = $"Name: {request.Name}" }, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            return new CreatePersonResult()
            {
                Id = newPerson.Id
            };          
        }
    }

    public class CreatePersonResult : BaseResponse
    {
        public int Id { get; set; }
    }
}