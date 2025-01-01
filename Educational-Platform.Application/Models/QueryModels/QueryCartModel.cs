using Educational_Platform.Domain.Entities;

namespace Educational_Platform.Application.Models.QueryModels
{
    public class QueryCartModel
    {
        public int Id { get; set; }
        public decimal Total { get; set; }

        public static QueryCartModel GetModel(Cart cart)
        {
            return new QueryCartModel
            {
                Id = cart.Id,
                Total = cart.Total
            };
        }
    }
}
