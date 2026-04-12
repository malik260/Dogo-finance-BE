using DogoFinance.DataAccess.Layer.Interfaces.Base;
using DogoFinance.DataAccess.Layer.Repositories.Base;

namespace DogoFinance.DataAccess.Layer.Repositories.Base
{
    /// <summary>
    /// Base class for all domain repositories.
    /// Provides a lazily-instantiated <see cref="DbRepository"/> via <c>BaseRepository()</c>,
    /// matching Fintrak's DataRepository / RepositoryFactory pattern.
    /// </summary>
    public abstract class DataRepository
    {
        private IDbRepository? _repository;

        public void SetSharedRepository(IDbRepository repository)
        {
            _repository = repository;
        }

        protected IDbRepository BaseRepository()
            => _repository ??= new DbRepository(new DogoDbContext());
    }
}
