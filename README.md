# EFCore.UtilExtensions
Data Annotations with extra attributes, Audit Info config, Generics

**FEATURES:**

#### 1. **UnPluralize** - `RemovePluralizingTableNameConvention`
Keeps table names singular in DB like Entities classes are while DbSets remains in plural.

#### 2. ANNOTATIONS Extensions  
are made to avoid using FluentAPI and keep all Db configs in a single place on Entity - makes it more clear and simple / DRY principle)
    
-New Attributes from the library:  
| Attributes                              | Description                                                    |
| --------------------------------------- | -------------------------------------------------------------- |
| `[Index()]`                             | enables configuring Index on one or several columns            |
| `[UniqueIndex()]`                       | enables configuring Unique Index on one or several columns     |
| `[DefaultValue(object)]  `              |                                                                |
| `[DefaultValueSql(("getdate()")]`       |                                                                |
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
