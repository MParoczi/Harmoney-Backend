# HarMoney project backend  - ASP.NET Core with C#

## The project
This repository contains the WebAPI for the website called HarMoney. It is a three sprint long project which aims to 
create a RESTful API that serves the HarMoney web application. The main technologies behind are the following:
 * C# targeting .NET Core 3.1
 * ASP.NET Core
 * Entity Framework with PostgreSQL relational database management system
 * Identity Framework

HarMoney is a personal financial management web application. Users are able to register their incomes and expenditures so
they can monitor and analyze their financial decisions.

## How to run
To be able to run the WebAPI you have to follow these simple steps:
 1. Create an empty database
 2. Populate your newly created database with the ```create-tables-or-reset-database.sql``` file.  
 ```(~/HarMoney/Contexts/SQL/create-tables-or-reset-database.sql)```
 3. Setup your environment variables with the following key-value pairs:
    * ```HARMONEY_CONNECTION : Host=DBHost; Database=DBName; Username=YourUsername; Password=YourPassword;```
    * ```HARMONEY_FRONTEND : https://harmoney.netlify.com```
 4. Run the application
 
This way you will be able to run the WebAPI, however the EmailService will not be available because the credentials of
this service are not public.

## Contributors
The contributors of this project are all students of Codecool Ltd.

 * [Paróczi Márk](https://github.com/MParoczi)
 * [Sipos Zoltán](https://github.com/siposzoltan03)
 * [Szűcs Nikolett](https://github.com/szucsnikolett)
 * [Vörös Eszter](https://github.com/wory04)