using Microsoft.EntityFrameworkCore.Query;
using Streetcode.DAL.Contracts;
using Streetcode.DAL.Repositories.Realizations.Base;

namespace Streetcode.DAL.Repositories.Interfaces.Base;

public interface IEntityRepositoryBase<T> : IRepositoryBase<T>
    where T : class, IEntity
{
}
