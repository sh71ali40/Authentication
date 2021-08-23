if  NOT  EXISTS ( SELECT * FROM ModuleDef WHERE ModuleDefId = 5001)
insert into ModuleDef(ModuleDefId,Name,HomeController,Enabled) values(5001,N'اعتبارسنجی','Authentication',1)
GO

IF  NOT  EXISTS ( SELECT * FROM PermissionRoleBase WHERE PermissionId = 500100)
insert into PermissionRoleBase (PermissionId,PermissionName,IsManager) values(500100,N'مشاهده',1)
GO

if  NOT  EXISTS ( SELECT * FROM PermissionRoleModuleDef WHERE PermissionId = 500100 and ModuleDefId=5001)
 insert into PermissionRoleModuleDef(ModuleDefId,PermissionId) values(5001,500100)
GO

if  not EXISTS (SELECT * FROM ModuleDefSetting WHERE SettingID =  500100)
insert into ModuleDefSetting ([SettingID],[ModuleDefID],[SettingName],[SettingValues],[DefaultValue],[SettingHelp],[NonModularValue])  values(500100,5001,N'شماره صفحه',null,null,N'پس از لاگین، کاربر به این صفحه ارسال می شود',null)
Go

if  not EXISTS (SELECT * FROM ModuleDefSetting WHERE SettingID =  500101)
insert into ModuleDefSetting ([SettingID],[ModuleDefID],[SettingName],[SettingValues],[DefaultValue],[SettingHelp],[NonModularValue])  values(500101,5001,N'شماره نقش کاربر جدید',null,null,N'این پراپرتی نشان دهنده نقش کاربر جدید در اپ است',null)
Go