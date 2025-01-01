using Educational_Platform.Domain.Entities;
using Educational_Platform.Domain.Enums;

namespace Educational_Platform.Domain.Abstractions
{
	public interface IPaymentValidator
	{
		/// <summary>
		/// Throws Exception If Payment Is Paid,Return The Validation Result
		/// </summary>
		/// <param name="entity"></param>
		OrderStatus Validate(Cart entity);
	}
}
