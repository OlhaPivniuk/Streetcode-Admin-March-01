namespace Streetcode.DAL.Repositories.Interfaces.Base;
using Contracts;

public interface IEntityRepositoryBase<T> : IRepositoryBase<T>
    where T : class, IEntity
{
}