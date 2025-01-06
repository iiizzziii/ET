
```bash
git clone https://github.com/iiizzziii/ET.git

cd PROJECT-ROOT-DIR

update Api/appsettings.json

"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR-SERVER-NAME;Database=YOUR-DATABASE-NAME;User Id=YOUR-USERNAME;Password=YOUR-PASSWORD;"
}

dotnet ef migrations add InitialCreate

dotnet ef database update

cd .. / dotnet restore

dotnet build  

cd Api / dotnet run

cd .. / Web dotnet run  
