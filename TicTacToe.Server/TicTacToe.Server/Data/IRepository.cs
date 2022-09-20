using System.Linq;

namespace TicTacToe.Server.Data
{
    public interface IRepository<T> where T : class
    {
        void Add(T entity);

        T Get(string id);

        IQueryable<T> GetQuery();

        ushort GetTotalCount();

        void Delete(string id);

        void SetOnline(string id);

        void SetOffline(string id);
    }
}
