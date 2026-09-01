# Central-LMS
Central Learning Management System to simplify course management for the teachers and the communication between teachers and students.

To run this project you will need Entity Framework Core tools which can be installed with `dotnet tool install --global dotnet-ef`. Then you can run:
```
dotnet ef database update
dotnet run -lp https
```
## Database
Currently we're using sqlite. When making changes to the data model use `dotnet ef migrations add MyMigration` to create a new migration and then use `dotnet ef database update` to apply it.