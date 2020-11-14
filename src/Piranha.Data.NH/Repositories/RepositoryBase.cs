using System;
using System.Threading.Tasks;
using NHibernate;

namespace RimuTec.Piranha.Data.NH.Repositories
{
    public class RepositoryBase
    {
        protected RepositoryBase(ISessionFactory sessionFactory)
        {
            SessionFactory = sessionFactory;
        }
        private ISessionFactory SessionFactory { get; }

        protected async Task<T> InTx<T>(Func<ISession, Task<T>> function)
        {
            using (var session = SessionFactory.OpenSession())
            using (var txn = session.BeginTransaction())
            {
                T result;
                try
                {
                    result = await function.Invoke(session).ConfigureAwait(false);
                    await txn.CommitAsync().ConfigureAwait(false);
                    return result;
                }
                catch (Exception)
                {
                    // TODO: exception handling including retry logic
                    await txn.RollbackAsync().ConfigureAwait(false);
                    throw;
                }
            }
        }

        protected async Task InTx(Func<ISession, Task> action)
        {
            await InTx(async session =>
            {
                await action(session).ConfigureAwait(false);
                return 42; // need to return something, anything, so that the other InTxn() is inferred [Manfred, 14nov2020]
            }).ConfigureAwait(false);
        }
    }
}