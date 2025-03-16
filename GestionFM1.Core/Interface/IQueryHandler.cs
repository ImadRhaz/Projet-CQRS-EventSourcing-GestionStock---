using System.Threading.Tasks;

namespace GestionFM1.Core.Interfaces;

public interface IQueryHandler<TQuery, TResult>
{
    Task<TResult> Handle(TQuery query);
}