# 🛍️ E-Commerce Shopping Cart - ASP.NET MVC

A production-ready e-commerce platform built with **ASP.NET MVC Core**, implementing enterprise patterns and modern workflows.

## ✨ Key Features
- 🛒 **Full Shopping Cart** with session persistence
- 👔 **Role-Based UI** (Admin/Customer) using Microsoft Identity
- 💳 **Stripe Payment Gateway** integration
- 📊 **Admin Dashboard** for product/order management
- 🔒 **User Account Control** (Lock/Unlock functionality)
- 📝 **Order History & Tracking** system
- 📱 **Mobile-First Design** with Bootstrap 5
- 🛠️ **3-Tier Architecture** (Presentation/Business/Data)

## 🧰 Tech Stack
| Category       | Technologies |
|----------------|-------------|
| **Backend**    | ASP.NET Core MVC, Entity Framework Core, AutoMapper |
| **Frontend**   | Bootstrap 5, JavaScript, jQuery |
| **Patterns**   | Repository + Unit of Work, Dependency Injection |
| **Security**   | Microsoft Identity, Role Seeding, DbInitializer |
| **Payment**    | Stripe API Integration |
| **UX**         | Toaster Notifications, Pagination |

## 🏗️ Architecture
```plaintext
ECommerce/
├── Areas/
│   ├── Admin/        # Admin controllers/views
│   └── Customer/     # Customer-facing features
├── Core/             # Domain models & interfaces
├── Infrastructure/   # Data layer (EF Core + Repositories)
├── Utility/          # Helpers (Extensions, Stripe, etc.)
└── Web/              # Main MVC Controllers/Views
