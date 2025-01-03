using Educational_Platform.Application.Abstractions.CartDetailInterfaces;
using Educational_Platform.Application.Abstractions.Infrastructure;
using Educational_Platform.Application.Models.CommandModels;
using Educational_Platform.Domain.Abstractions;
using Educational_Platform.Domain.Entities;
using Educational_Platform.Domain.Enums;
using Educational_Platform.Domain.Exceptions;
using Educational_Platform.Domain.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Educational_Platform.Application.Services.PaymentDetailServices
{
    public partial class CartDetailCommandService(
        IUnitOfWork unitOfWork,
        IEntityValidator<CartDetail> detailValidation,
        ILogger<CartDetailCommandService> log) : ICartDetailCommandService
    {
        public async ValueTask AddAsync(CommandCartDetailModel model)
        {
            var cart = await unitOfWork.CartsRepository
                .AsNoTracking()
                .ReadAsync(c => c.Id == model.CartId);

            await CheckIfUserAlreadyOwnCourse(model, cart.UserId);

            try
            {
                var cartDetail = await unitOfWork.CartDetailsRepository
                    .ReadAsync(cd =>
                    cd.CartId == model.CartId &&
                    cd.ProductId == model.ProductId &&
                    cd.ProductType == model.ProductType);

                cartDetail.Quantity += model.Quantity;

                detailValidation.ValidateEntity(cartDetail);

                log.LogInformation("A Cart Detail Is Updated To {@CartDetail}", CommandCartDetailModel.GetCartDetailModel(cartDetail));
            }
            catch
            {
                CartDetail detail = model.GetCartDetail();

                detailValidation.ValidateEntity(detail);

                await unitOfWork.CartDetailsRepository.CreateAsync(detail);

                log.LogInformation("A Cart Detail {@CartDetail} Is Added", model);
            }
            await unitOfWork.SaveChangesAsync();
        }

        public async ValueTask RemoveAsync(CommandCartDetailModel model)
        {
            var cart = await unitOfWork.CartsRepository
                .AsNoTracking()
                .ReadAsync(c => c.Id == model.CartId);

            var cartDetail = await unitOfWork.CartDetailsRepository
                .ReadAsync(cd =>
                cd.CartId == cart.Id &&
                cd.ProductType == model.ProductType &&
                cd.ProductId == model.ProductId);

            if (cartDetail.Quantity < model.Quantity)
                throw new InvalidDataException("Invalid Quantity")
                    .AddToExceptionData(ExceptionKeys.StatusCodeKey, StatusCodes.Status400BadRequest);

            else if (cartDetail.Quantity == model.Quantity)
            {
                await unitOfWork.CartDetailsRepository.DeleteAsync(cd =>
                cd.CartId == cart.Id &&
                cd.ProductType == model.ProductType &&
                cd.ProductId == model.ProductId);

                log.LogInformation("A Cart Detail Was Deleted {@CartDetail}", model);
            }
            else
            {
                cartDetail.Quantity -= model.Quantity;
                log.LogInformation("A Cart Detail Was Updated To {@CartDetail}", CommandCartDetailModel.GetCartDetailModel(cartDetail));
            }

            await unitOfWork.SaveChangesAsync();
        }
    }

    public partial class CartDetailCommandService
    {
        private async ValueTask CheckIfUserAlreadyOwnCourse(CommandCartDetailModel model, object? userId)
        {
            if (model.ProductType != ProductTypes.Course)
                return;

            if (await unitOfWork.UsersCoursesRepository.IsExistAsync(uc => uc.CourseId == model.ProductId && uc.UserId.Equals(userId)))
                throw new InvalidDataException("You Can't Add This Course To Cart Beacuase This User Already Has Access To It")
                    .AddToExceptionData(ExceptionKeys.StatusCodeKey, StatusCodes.Status409Conflict);
        }
    }
}
