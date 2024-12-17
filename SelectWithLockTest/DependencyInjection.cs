using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Dialect;
using QS.Project.DB;
using System.Reflection;

namespace SelectWithLockTest
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDatabaseConnection(this IServiceCollection services)
        {
            services
                .AddDatabaseConnectionString()
                .AddSqlConfiguration()
                .AddNHibernateConfiguration()
                .AddSessionFactory();

            return services;
        }

        public static IServiceCollection AddDatabaseConnectionString(this IServiceCollection services)
        {
            services.AddSingleton<MySqlConnectionStringBuilder>((provider) => {
                var builder = new MySqlConnectionStringBuilder
                {
                    Server = "",
                    Port = 3307,
                    Database = "",
                    UserID = "",
                    Password = "",
                    SslMode = MySqlSslMode.None
                };

                return builder;
            });

            return services;
        }

        public static IServiceCollection AddSqlConfiguration(this IServiceCollection services)
        {

            services.AddSingleton<MySQLConfiguration>((provider) =>
            {
                var connectionStringBuilder = provider.GetRequiredService<MySqlConnectionStringBuilder>();
                var dbConfig = MySQLConfiguration.Standard
                    .Dialect< MySQL5Dialect>()
                    .ConnectionString(connectionStringBuilder.ConnectionString)
                    .AdoNetBatchSize(100)
                    .Driver<LoggedMySqlClientDriver>()
                ;
                return dbConfig;
            });

            return services;
        }

        public static IServiceCollection AddNHibernateConfiguration(this IServiceCollection services)
        {
            services.AddSingleton<Configuration>((provider) => {
                var sqlConfiguration = provider.GetRequiredService<MySQLConfiguration>();

                var fluentConfig = Fluently.Configure().Database(sqlConfiguration);
                fluentConfig.Mappings(m => {
                    m.FluentMappings.AddFromAssembly(Assembly.GetExecutingAssembly());
                });

                return fluentConfig.BuildConfiguration();
            });
            return services;
        }

        public static IServiceCollection AddSessionFactory(this IServiceCollection services)
        {
            services.AddSingleton<ISessionFactory>((provider) => {
                var databaseConfiguration = provider.GetRequiredService<Configuration>();
                var factory = databaseConfiguration.BuildSessionFactory();

                return factory;
            });
            return services;
        }
    }
}
