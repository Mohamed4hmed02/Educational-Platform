using Educational_Platform.Application.Models.CommandModels;
using Educational_Platform.Domain.Abstractions;

namespace Educational_Platform.Application.Abstractions.UnitInterfaces
{
    public interface IUnitCommandServices : IEditable<CommandUnitModel>, IDeleteable<CommandUnitModel>, IDisposable
    {
        Task DeleteUnitsAsync(IEnumerable<int> unitsIds);
    }
}
