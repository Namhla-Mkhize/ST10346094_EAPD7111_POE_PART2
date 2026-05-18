# TechMove - Enterprise Contract Management System

## ST10346094 | EAPD7111 | POE Part 2

## Overview
TechMove is an enterprise contract management system built with ASP.NET Core MVC .NET 8. It allows businesses to manage clients, contracts, and service requests with live currency conversion.

## Features
- Client Management (CRUD)
- Contract Management with PDF upload and download
- Service Request Management with live USD to ZAR currency conversion
- Contract status workflow (blocks requests on Expired/OnHold contracts)
- Search and filter contracts by date range and status
- 16 Unit Tests passing with xUnit
- GitHub Actions automated testing

## Technologies Used
- ASP.NET Core MVC .NET 8
- Entity Framework Core with SQL Server
- Repository Pattern and Service Layer
- xUnit Unit Testing
- GitHub Actions CI/CD
- Bootstrap 5

## How to Run
1. Clone the repository
2. Open `TechMove.sln` in Visual Studio
3. Update the connection string in `appsettings.json`
4. Run `Update-Database` in Package Manager Console
5. Press F5 to run

## Unit Tests
Run tests in Visual Studio via Test → Run All Tests
All 16 tests should pass green.
