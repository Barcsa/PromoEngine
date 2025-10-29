#PromoEngine

A simple web application built for Billy Bonka Chocolate Factory to let users submit promo codes found inside chocolate packaging and automatically enter daily or weekly prize draws.


#Tech Stack

Backend: .NET 8 (C#)
Frontend: React with Tailwind CSS
Database: MySQL (Entity Framework Core)
Security: SHA256 hashing for personal data
Architecture: Layered structure (Repository - Service - Controller) with interfaces

#How It Works

The user fills out the submission form and enters their promo code.
The backend checks if the code is valid and safely stores the data.
The promo code is marked as used.
The system finds which predefined winning timestamp is closest to the submission time and assigns the prize if applicable.

#Data Security

No personal information (name, email, phone) is ever stored in plain text.
All sensitive data is securely hashed using SHA256 before saving.
Each promo code can be used only once.

#Architecture Overview

Frontend: Handles user input and communicates with the backend via REST API (JSON)
Backend: Validates, hashes, and processes data through a service-repository structure.
Database: Stores promo codes, submissions, and winning timestamps through EF Core.

#Testing
Unit tests cover core business logic (promo code validation, winner selection).

Project structure

- PromoEngine – Backend (.NET 8 + EF Core)
- promoengine-frontend – Frontend (React + Tailwind)
- PromoEngine.Tests – Unit tests (xUnit)

Backend 
- Controllers – API controllers  
- Data - Database context and EF Core setup  
- Dtos – Data transfer objects  
- Migrations – EF Core migrations  
- Models – Entity models  
- Repositories – Data access layer  
- Services – Business logic layer  
- Utils – Utility classes (hashing, code generation)  
- PromoEngine-diagram.svg – Architecture diagram  
- appsettings.json – Configuration  
- Program.cs – Entry point  

Frontend
- src – React components, pages, and API logic  
- public – Static assets  
- package.json – Dependencies  
- vite.config.ts – Vite configuration  
