# EFCore.UtilExtensions
Several useful addons for EF:  
1. UnPluralizing convention config
1. Data Annotations with extra attributes,
2. Audit Info config,
3. Generics.

**FEATURES:**

#### 1. **UnPluralize** - `RemovePluralizingTableNameConvention`
Keeps table names singular in DB like Entities classes are while DbSets remains in plural.

#### 2. ANNOTATIONS Extensions  
Are made to avoid using FluentAPI and keep all Db configs in a single place on Entity  
(makes it more clear and simple - [DRY](https://en.wikipedia.org/wiki/Don%27t_repeat_yourself) principle)  
Implemented with method `ConfigureExtendedAnnotations` on ***modelBuilder*** called from *OnModelCreating*.
    
-New Attributes from the library:  
| Attributes                              | Description                                                    |
| --------------------------------------- | -------------------------------------------------------------- |
| `[Index()]`                             | enables configuring Index on one or several columns            |
| `[UniqueIndex()]`                       | enables configuring Unique Index on one or several columns     |
| `[DefaultValue(object)]  `              | sets Db defualt value                                          |
| `[DefaultValueSql("getdate()")]`        | sets Db defualt value with Sql                                 |
| `[ForeignKeyExtension(DeleteBehavior.)]`| extends FK attribute, adds option to set DeleteBehavior        |
| `             DeleteBehavior.NoAction`  | sometimes needed to avoid fk cascade multiple paths and cycles |

-Native ones from EF are:  
| Attributes    | Attributes              |
| ------------- | ----------------------- |
| `[Key]`       | `[DatabaseGenerated()]` |
| `[Column()]`  | `[MaxLength(255)]`      |
| `[Required]`  | `[Precision(20, 4)]`    |
| `[NotMapped]` | `[ForeignKey(FkName)]`  |
| `[Timestamp]` | `[ConcurrencyCheck]`    |
