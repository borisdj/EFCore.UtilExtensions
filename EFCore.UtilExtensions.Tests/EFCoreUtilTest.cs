using EFCore.UtilExtensions.Tests.Entities;
using Xunit;

namespace EFCore.UtilExtensions.Tests
{
    public class EFCoreUtilTest
    {
        [Theory]
        [InlineData(SqlType.SqlServer)]
        public void DemoTest(SqlType sqlType)
        {
            ContextOptions.DatabaseType = sqlType;

            using var context = new TestContext(ContextOptions.GetOptions());

            context.ItemDetails.RemoveRange(context.ItemDetails);
            context.Items.RemoveRange(context.Items);
            context.ItemCategories.RemoveRange(context.ItemCategories);
            context.SaveChanges();

            var itemCategory = new ItemCategory
            {
                Name = "Cat1",
                Number = "0001"
            };
            context.ItemCategories.Add(itemCategory);
            context.SaveChanges();

            var entities = new List<Item>();
            for (int i = 1; i <= 10; i++)
            {
                var entity = new Item
                {
                    Id = Guid.NewGuid(),
                    Name = "Name " + i,
                    Code = "c " + i,
                    CustomDescription = i % 2 == 0 ? "" : "nn" + i,
                    Price = 10 * i,
                    ItemTypeId = (int)Enums.ItemType.Physical,
                    ItemCategoryId = itemCategory.Id,
                    ItemDetails = new List<ItemDetail> { 
                        new() { Id = Guid.NewGuid(), Price = 0 }, 
                        new() { Id = Guid.NewGuid(), Price = 10 * i, Remark = "Init" } }
                };
                entities.Add(entity);
            }
            context.AddRange(entities);
            context.SaveChanges();
        }
    }
}