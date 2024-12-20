# EFCore.UtilExtensions
Several useful addons for EF (Entity Framework Core):  
1. UnPluralizing convention,
1. Data Annotations with extra attributes,
2. EntityId and Enum interface,
3. Audit Info config,
4. Generics.

Logo  
<img src="EFCoreUtilLogo.png" height=60>

[![NuGet](https://img.shields.io/npm/l/express.svg)](https://github.com/borisdj/EFCore.UtilExtensions/blob/master/LICENSE)  

*Note: *Still in progres...*

Also take a look into others packages:</br>
-Open source (MIT or cFOSS) authored [.Net libraries](https://infopedia.io/dot-net-libraries/) (@**Infopedia.io** personal blog post)
| №  | .Net library             | Description                                              |
| -  | ------------------------ | -------------------------------------------------------- |
| 1  | [EFCore.BulkExtensions](https://github.com/borisdj/EFCore.BulkExtensions) | EF Core Bulk CRUD Ops (**Flagship** Lib) |
| 2* | [EFCore.UtilExtensions](https://github.com/borisdj/EFCore.UtilExtensions) | EF Core Custom Annotations and AuditInfo |
| 3  | [EFCore.FluentApiToAnnotation](https://github.com/borisdj/EFCore.FluentApiToAnnotation) | Converting FluentApi configuration to Annotations |
| 4  | [ExcelIO.FastMapper](https://github.com/borisdj/ExcelIO.FastMapper) | Excel Input Output Mapper to-from Poco & .xlsx with attribute |
| 5  | [FixedWidthParserWriter](https://github.com/borisdj/FixedWidthParserWriter) | Reading & Writing fixed-width/flat data files |
| 6  | [CsCodeGenerator](https://github.com/borisdj/CsCodeGenerator) | C# code generation based on Classes and elements |
| 7  | [CsCodeExample](https://github.com/borisdj/CsCodeExample) | Examples of C# code in form of a simple tutorial |

## Support
If you find this project useful you can mark it by leaving a Github **Star** :star:  
And even with community license, if you want help development, you can make a DONATION:  
[<img src="https://www.buymeacoffee.com/assets/img/custom_images/yellow_img.png" alt="Buy Me A Coffee" height=28>](https://www.buymeacoffee.com/boris.dj) _ or _ 
[![Button](https://img.shields.io/badge/donate-Bitcoin-orange.svg?logo=bitcoin):zap:](https://borisdj.net/donation/donate-btc.html)

## Contributing
Please read [CONTRIBUTING](CONTRIBUTING.md) for details on code of conduct, and the process for submitting pull requests.  
When opening issues do write detailed explanation of the problem or feature with reproducible example.  
Want to **Contact** for Development & Consulting: [www.codis.tech](http://www.codis.tech) (*Quality Delivery*) 

**FEATURES:**

#### 1. **UnPluralize**
Keeps table names singular in DB like Entities classes are (also keeping PascalCase) while DbSets remains in plural.
To set it up, call `RemovePluralizingTableNameConvention` from `OnModelCreating`:
```C#
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.RemovePluralizingTableNameConvention();
    ...
}
```
NOTE: Useful for versions prior to .Net7, mainly for .Net6, since with .Net7+ it can be achieved directly by overriding a method in DbContext:
```C#
// In DbContext
protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
{
    configurationBuilder.Conventions.Remove(typeof(TableNameFromDbSetConvention));
}
```

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
| Attributes                 | Attributes                   |  Attributes             |  Attributes     |
| -------------------------- | ---------------------------- | ----------------------- | --------------- |
| `[Table(tblName)]`         | `[Key]`                      | `[DatabaseGenerated()]` | `[Owned]`       |
| `[Column(name, typeName)]` | `[ForeignKey(FkName)]`       | `[Timestamp]`           | `[ComplexType]` |
| `[Required]`               | `[Index(indName)]`           | `[ConcurrencyCheck]`    | `[NotMapped]`   |
| `[MaxLength(255)]`         | `[Index(indName, IsUnique)]` | `[Precision(20, 4)]`    |                 |

Notes:  
`[Precision]` is used for customizing Decimal type, default being (18, 2) meaning 18 significant digits of which 16 is for whole number and 2 decimal places.

