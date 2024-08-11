using Microsoft.EntityFrameworkCore.Storage;

namespace Globe.Core.SqlExecution
{
    public class DynamicSqlRelationalCommandBuilderFactory : RelationalCommandBuilderFactory
    {
        public DynamicSqlRelationalCommandBuilderFactory(RelationalCommandBuilderDependencies dependencies)
            : base(dependencies) { }

        public override IRelationalCommandBuilder Create() => new DynamicSqlRelationalCommandBuilder(Dependencies);
    }
}
