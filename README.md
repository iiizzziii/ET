## Prerequisites

Before starting, ensure you have the following installed on your machine:

- [.NET SDK](https://dotnet.microsoft.com/download)  
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)  
- [SQL Server Studio](https://learn.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms)   

---

## Setup Instructions

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
