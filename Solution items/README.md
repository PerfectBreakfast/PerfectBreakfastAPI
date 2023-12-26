# Perfect Breakfast#

### run with VSCode
cd to the ```PerfectBreakfast``` folder run command below
```
dotnet run --project .\src\PerfectBreakfast.API\
```
### run with VisualStudio
open file ```PerfectBreakfast.sln``` and run

# EF migration
0. install global tool to make migration(do only 1 time & your machine is good to go for the next)
```
dotnet tool install --global dotnet-ef
```
1. create migrations & the dbcontext snapshot will rendered.
   Open CLI at folder & run command
   -s is startup project(create dbcontext instance at design time)
   -p is migrations assembly project
```
dotnet ef migrations add NewMigration -s .\src\PerfectBreakfast.API\ -p .\src\PerfectBreakfast.Infrastructure\ 
```

2. apply the change
```
dotnet ef database update -s .\src\PerfectBreakfast.API\ -p .\src\PerfectBreakfast.Infrastructure\
```
