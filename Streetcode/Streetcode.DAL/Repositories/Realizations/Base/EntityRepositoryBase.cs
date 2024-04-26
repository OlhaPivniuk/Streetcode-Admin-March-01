namespace Streetcode.DAL.Repositories.Realizations.Base;
using Contracts;
using Persistence;
using Repositories.Interfaces.Base;

public class EntityRepositoryBase<T> : RepositoryBase<T>, IEntityRepositoryBase<T>
    where T : class, IEntity
{
    public EntityRepositoryBase(StreetcodeDbContext context)
        : base(context)
    {
    }
}