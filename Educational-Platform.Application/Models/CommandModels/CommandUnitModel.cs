using Educational_Platform.Application.Abstractions.Infrastructure;
using Educational_Platform.Domain.Entities;

namespace Educational_Platform.Application.Models.CommandModels
{
	public class CommandUnitModel
	{
		public int Id { get; set; }
		public int CourseId { get; set; }
		public string Name { get; set; } = string.Empty;
        public int UnitOrder { get; set; }

		public async ValueTask<Unit> GetUnitAsync(IUnitOfWork unitOfWork)
		{
			return new Unit
			{
				CourseId = CourseId,
				Name = Name,
				Id = Id,
				UnitOrder = UnitOrder,
				QuizFileName = await GetQuizFileAsync(Id, unitOfWork)
			};
		}

		public static explicit operator CommandUnitModel(Unit unit)
		{
			return new CommandUnitModel
			{
				CourseId = unit.CourseId,
				Name = unit.Name,
				Id = unit.Id,
				UnitOrder = unit.UnitOrder
			};
		}

		private async ValueTask<string?> GetQuizFileAsync(int unitId, IUnitOfWork unitOfWork)
		{
			try
			{
				return await unitOfWork.UnitsRepository
				.AsNoTracking()
				.ReadAsync(u => u.Id == unitId, u => u.QuizFileName);
			}
			catch
			{
				return null;
			}

		}
	}
}
