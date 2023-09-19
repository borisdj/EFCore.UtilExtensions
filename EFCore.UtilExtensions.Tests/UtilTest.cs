using EFCore.UtilsExtensions.Tests;
using Xunit;

namespace EFCore.UtilExtensions.Tests
{
    public class EFCoreUtilTest
    {
        [Theory]
        [InlineData(SqlType.PostgreSql)]
        public void DemoTest(SqlType sqlType)
        {
            ContextUtil.DatabaseType = sqlType;

            using var context = new TestContext(ContextUtil.GetOptions());

            var entities = new List<Item>();
            for (int i = 1; i <= 2; i++)
            {
                var entity = new Item
                {
                    //ItemId = i,
                    Name = "Name " + i,
                    Price = 0.1m * i,
                };
                entities.Add(entity);
            }

            context.AddRange(entities);
            context.SaveChanges();
        }
    }
}