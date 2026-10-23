# GymCore - Gym Management System

A web application for managing a gym: members, trainers, sessions, memberships, plans, and bookings. Built with ASP.NET Core MVC using a layered architecture.

## Overview

This project is part of my journey toward becoming a .NET Backend Developer. It focuses on building a structured web application using MVC, database relationships, and a layered architecture.

## Features

- Member management with photo upload and health records
- Trainer and session management, including trainer speciality validation
- Membership plans, with one active membership per member
- Session booking and attendance tracking
- Authentication and role-based access (Admin, SuperAdmin) with ASP.NET Core Identity
- Dashboard with live statistics

## Tech Stack

C#, ASP.NET Core MVC, Entity Framework Core, LINQ, SQL Server, ASP.NET Core Identity, AutoMapper, Bootstrap

## Architecture

- **GymManagement (PL):** Controllers, Views, presentation layer
- **GymManagement.BLL:** Services, ViewModels, business logic
- **GymManagement.DAL:** Entities, DbContext, Repositories, Unit of Work

Patterns used: Repository, Generic Repository, Unit of Work, Dependency Injection.

## Getting Started

1. Clone the repository
   `git clone https://github.com/yousefelfar46/GymManagementSolution.git`
2. Open `GymManagementSolution.slnx` in Visual Studio
3. Update the connection string in `appsettings.json`
4. Run the migrations: `Update-Database`
5. Run the project
