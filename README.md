# EFCore.UtilExtensions
Data Annotations with extra attributes, Audit Info config, Generics

**FEATURES:**

1) **UnPluralize** method from DbContext for Table naming convention

2) ANNOTATIONS Extensions  
   (to avoid using FluentaPI and keep all Db configs on Entity - makes it more clear and simple / DRY principle)
    
-New Attributes from the library:  
`[Index()]` - enables configuring Index on one or several columns  
`[UniqueIndex()]` - enables configuring Unique Index on one or several columns  
`[DefaultValue(object)]`  
`[DefaultValueSql(("getdate()")]`  
`[ForeignKeyExtension(DeleteBehavior.)]` - extends ForeignKey attribute by adding option to set DeleteBehavior  
`                                   .NoAction` - sometimes needed to avoid fk cascade multiple paths and cycles)

-Native ones from EF are:  
`[Key]`  
`[DatabaseGenerated()]`  
`[Column()]`  
`[Required]`  
`[MaxLength(255)]`  
`[Precision(20, 4)]`  
`[ForeignKey(FkName)]`  
`[NotMapped]`  
