using Streetcode.DAL.Contracts;
using Streetcode.DAL.Persistence;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.DAL.Repositories.Realizations.Base;

public class EntityRepositoryBase<T> : RepositoryBase<T>, IEntityRepositoryBase<T>
    where T : class, IEntity
{
    public EntityRepositoryBase(StreetcodeDbContext context)
        : base(context)
    {
    }
}
