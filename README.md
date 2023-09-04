# Product CRUD ASP.NET Core Web API

This is a sample ASP.NET Core Web API for managing products. It allows you to CRUD products

## Getting Started

Follow these steps to set up and run the project on your local machine.

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download/dotnet) - Make sure you have .NET SDK installed.
- [Git](https://git-scm.com/) - You need Git for cloning the project repository.

### Clone the Repository

Clone this repository to your local machine using Git:

```bash
git clone git@github.com:Ador-25/product-crud-asp-net-core-web-api.git
```

### Replace Connection String

Open appsettings.json and put a connection string(Postgresql) either from local machine or use the connection string I have sent you in email
If you are running it on local machine make sure to migrate first.
```bash
add-migration YourMigrationName
update-database
```

### Run the Project

After creating connection string Run the project

```bash
dotnet run
```


### Test

After Running , Open Postman Doc folder and open the json file using Postman.
Call the apis to test.
If you are using terminal to run the project make sure you change it to correct port

