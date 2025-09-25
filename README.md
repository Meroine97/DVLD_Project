# Driving and Vehicle License Department (DVLD) Desktop Application

A full-featured desktop application built with the .NET Framework, implementing a Three-Tier Architecture.

## Architecture
- **Presentation Tier:** Windows Forms project for the user interface.
- **Business Logic Tier:** Class library containing application logic and rules.
- **Data Access Tier:** Class library handling all database operations using ADO.NET.

## Technologies Used
- **Language:** C#
- **Framework:** .NET Framework, Windows Forms
- **Database:** SQL Server with T-SQL
- **Data Access:** ADO.NET
- **Architecture:** Three-Tier, OOP

## Features
- 🛡️ Secure login for authorized system users.
- 📂 Simple, intuitive menu strip giving quick access to:
- 👤 People management.
- 👥 Users management.
- 🚗 Drivers management.
- ⚙️ Account settings.
- 📄 Driving License Services – issue new licenses (🏠 local & 🌍 international), renewals, replacements for lost/damaged licenses.
- 📋 Application Management – track & process local/international requests.
- 🚫 License Detainment & 🔓 Release.
- 🗂 Manage application types.
- 📝 Manage test types (Vesion, Written(theory), Pracitcal(street)).

## How to Run
1.  Clone this repository.
2.  Open the `DVLD_Project.sln` file in Visual Studio.
3.  Restore NuGet packages.
4.  Install SQL Server 'SSMS' then run the sql script to restore database
   ``` USE [master];
       GO
       RESTORE DATABASE [DVLD]
       FROM DISK = N'C:\Path\To\DVLD.bak' <!--Make sure about the path where you put the Database--> 
       GO```
5.  Update the connection string in the `DVLD_DataAccess -> clsDataAccessSettings` project.
6.  Set the `DVLD_PresentationLayer1` project as the startup project.
7.  Build and run the application.
8.  You can run the application without needing of `Remember Me` feature.
