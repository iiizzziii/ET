1 - git clone https://github.com/iiizzziii/ET.git
2 - cd <PROJECT-ROOT-DIR>
3 - update Api/appsettings.json ConnectionStrings DefaultConnection: <YOUR-LOCAL-SQL-SERVER-INSTANCE>
4 - dotnet ef migrations add InitialCreate
5 - dotnet ef database update
6 - cd .. / dotnet restore
7 - dotnet build
8 - cd Api / dotnet run
9 - cd .. / Web dotnet run
