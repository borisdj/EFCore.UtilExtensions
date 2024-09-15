# EFCore.UtilExtensions
Several useful addons for EF (Entity Framework Core):  
1. UnPluralizing convention,
1. Data Annotations with extra attributes,
2. EntityId and Enum interface,
3. Audit Info config,
4. Generics.

*Note: *Still in progres...*

Also take a look into others packages:</br>
-Open source (MIT or cFOSS) authored [.Net libraries](https://infopedia.io/dot-net-libraries/) (@**Infopedia.io** personal blog post)
| №  | .Net library             | Description                                              |
| -  | ------------------------ | -------------------------------------------------------- |
| 1  | [EFCore.BulkExtensions](https://github.com/borisdj/EFCore.BulkExtensions) | EF Core Bulk CRUD Ops (Flagship Lib) |
| 2  | [EFCore.UtilExtensions](https://github.com/borisdj/EFCore.UtilExtensions) | EF Core Custom Annotations and AuditInfo |
| 3  | [EFCore.FluentApiToAnnotation](https://github.com/borisdj/EFCore.FluentApiToAnnotation) | Converting FluentApi configuration to Annotations |
| 4  | [FixedWidthParserWriter](https://github.com/borisdj/FixedWidthParserWriter) | Reading & Writing fixed-width/flat data files |
| 5  | [CsCodeGenerator](https://github.com/borisdj/CsCodeGenerator) | C# code generation based on Classes and elements |
| 6  | [CsCodeExample](https://github.com/borisdj/CsCodeExample) | Examples of C# code in form of a simple tutorial |

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
| `[DefaultValue(object)]  `              | sets Db default value                                          |
| `[DefaultValueSql("getdate()")]`        | sets Db default value with Sql                                 |
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
