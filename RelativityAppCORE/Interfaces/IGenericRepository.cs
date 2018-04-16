using System.Collections.Generic;

namespace RelativityAppCore.Repositories
{
    public interface IGenericRepository<T> where T:class
    {
        IEnumerable<T> GetAll();

        T Get(int artifactId);

        int Create(T artifact);

        bool Update(int artifactId, T artifact);

        bool Delete(int artifactId);

    }
}
