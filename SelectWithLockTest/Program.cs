using Microsoft.Extensions.DependencyInjection;
using NHibernate;
using NLog;
using System;

namespace SelectWithLockTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ServiceCollection services = new ServiceCollection();
            services.AddDatabaseConnection();

            var serviceProvider = services.BuildServiceProvider();
            

            var sessionFactory = serviceProvider.GetService<ISessionFactory>();
            using (var session = sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                // Получаем из бд сущность с блокировкой
                var entity = session.Get<Entity>(1, LockMode.UpgradeNoWait);
                //var entity = session.Get<Entity>(1, LockMode.Upgrade);

                // Не работает NO WAIT
                // Неизвестно как выполнить SKIP LOCKED
                // Неизвестно как регулировать время WAIT 


                // Редактируем ее
                entity.Name = entity.Name + "1";
                // Сохраняем
                session.SaveOrUpdate(entity);
                // Применяем изменения
                // И снимаем блокировку
                transaction.Commit();
            }
        }
    }

    
}
