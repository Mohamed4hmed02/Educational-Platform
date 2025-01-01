﻿using Educational_Platform.Domain.Abstractions;
using Educational_Platform.Domain.Abstractions.InfrastructureAbstractions.ReposInterfaces;
using Educational_Platform.Domain.Entities;
using Educational_Platform.Domain.Exceptions;
using Microsoft.AspNetCore.Http;

namespace Educational_Platform.Domain.Implementaions.Validators
{
    public class CartDetailValidator : IEntityValidator<CartDetail>
    {
        public void Validate(CartDetail detail)
        {
            if (detail.Quantity == 0)
                throw new InvalidDataException("Quantity Can't Be Zero")
                    .AddToExceptionData(ExceptionKeys.StatusCodeKey, StatusCodes.Status400BadRequest);

            if (detail.ProductType == Enums.ProductTypes.Course && detail.Quantity > 1)
                throw new InvalidOperationException("Course Quantity Can't Be More Than One")
                    .AddToExceptionData(ExceptionKeys.StatusCodeKey, StatusCodes.Status422UnprocessableEntity);
        }

        public void Validate(IEnumerable<CartDetail> details)
        {
            foreach (var detail in details)
            {
                Validate(detail);
            }
        }
    }
}
