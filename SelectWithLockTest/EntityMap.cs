using FluentNHibernate.Mapping;

namespace SelectWithLockTest
{
    public class EntityMap : ClassMap<Entity>
    {
        public EntityMap()
        {
            Table("test_entity");

            //HibernateMapping.DefaultAccess.CamelCaseField(Prefix.Underscore);

            Id(x => x.Id)
                .Column("id")
                .GeneratedBy.Native();

            Map(x => x.Name)
                .Column("name");
        }
    }
}
