# 🏛️ Topic Trips Guide

## 📑 Table of Contents

1. [Summary](#-summary)
2. [Project Context](#-project-context)
3. [Similar Applications](#-similar-applications)
4. [Key Differences Compared to Similar Apps](#-key-differences-compared-to-similar-apps)
5. [Motivation for Choosing the Topic](#-motivation-for-choosing-the-topic)
6. [Project Objectives](#-project-objectives)
7. [System Requirements](#-system-requirements)
8. [Requirements & Installation](#-requirements--installation)
9. [Database & Data Model](#-database--data-model)
10. [Architecture (MVVM)](#-architecture-mvvm)
11. [Pages / Modules Overview](#-pages--modules-overview)
   - [Login / Authentication](#login--authentication)
   - [Students / Users](#students--users)
   - [Courses / Specializations](#courses--specializations)
   - [Registrations / Enrollments](#registrations--enrollments)
   - [Reports / PDF Export](#reports--pdf-export)
12. [Conclusion](#-conclusion)
13. [License](#-license)


---

## 📄 Summary

This work proposes the development of a **web application** for planning and exploring tourist routes, focusing on significant historical and culinary locations in Europe. The application allows users to create personalized routes, either based on predefined itineraries or by selecting locations from the lives of European historical figures. Each location includes detailed historical information and relevant images, with a mini wiki component for additional context.  

Users can select locations to create customized routes that include points of interest such as castles and historical battle sites. The application provides route optimization options and features for saving routes and adding reviews.

---

## 🌍 Project Context

The application addresses the need for planning tourist routes based on historical and culinary locations. Many tourists and history enthusiasts want to discover remarkable sites, and Topic Trips Guide facilitates this through:

- Predefined or customizable routes  
- Interactive maps and optimized routing  
- Integration of local culinary attractions  
- Ability to save and download routes  

**Target audience:** tourists, history enthusiasts, and food lovers.

---

## 📱 Similar Applications

### Komoot
- Outdoor route planning (hiking, cycling)  
- Includes historical points of interest and route saving  

### Yelp
- Discover restaurants and accommodations  
- Personalized recommendations based on user reviews  

### TripAdvisor
- Reviews and information about destinations, restaurants, and hotels  
- Maps, distance calculation, and booking options  

---

## 🔑 Key Differences Compared to Similar Apps

1. **Personalized route planning**  
2. **Historical and cultural routes**  
3. **User interaction and personalized feedback**  
4. **Focus on local gastronomy**  
5. **Interactive maps and optimized navigation**

---

## 🎯 Motivation for Choosing the Topic

The topic was chosen due to a passion for **history and travel**, combining exploration of historical sites with gastronomy. The project allowed the development of an interactive web application that provides valuable information and facilitates efficient trip planning.

---

## 📝 Project Objectives

- User authentication system to save routes and preferences  
- Predefined routes for historical figures and culinary cities  
- Route customization by users  
- Location details: images, descriptions, historical information  
- Integration with OpenRouteService and OpenStreetMap APIs  
- Collection of user feedback and reviews  
- Route saving and PDF export  
- Route visualization with estimated distances  

---

## 💻 System Requirements

### Functional
- User authentication  
- Viewing and customizing routes  
- Interactive maps and routing API  
- Save and download routes  
- Feedback and reviews for locations

### Non-Functional
- Protection of user data  
- Performance and support for multiple simultaneous users  
- Intuitive and appealing interface  
- High availability

---

## ⚙️ Requirements & Installation

To run Topic Trips Guide, your system must meet the following requirements and follow the installation steps.

---

### 🖥️ System Requirements
- Windows 10 or higher (or any OS supporting .NET 8.0)  
- Modern web browser (Chrome, Edge, Firefox, Safari)

---

### 💻 Development Tools
- Visual Studio 2022 Community Edition  
- .NET SDK 8.0

---

### 🗄️ Database
- MySQL server installed and running  
- Optional: SQLite for local testing/development

---

### 📦 NuGet Packages / Dependencies
- Microsoft.AspNetCore.Identity.EntityFrameworkCore 8.0.11  
- Microsoft.AspNetCore.Identity.UI 8.0.11  
- Microsoft.EntityFrameworkCore.Sqlite 8.0.11  
- Microsoft.EntityFrameworkCore.SqlServer 8.0.11  
- Microsoft.EntityFrameworkCore.Tools 8.0.11  
- Microsoft.VisualStudio.Web.CodeGeneration.Design 8.0.7  
- Pomelo.EntityFrameworkCore.MySql 8.0.3  
- QuestPDF 2025.5.0  
- System.Text.Json 8.0.5

---

### 🌐 API / Services
- OpenRouteService API key for route calculations  
- Access to OpenStreetMap data for interactive maps

---

### 🛠️ Installation Instructions

1. **Clone the repository**  
```bash
git clone https://github.com/yourusername/Topic-Trips-Guide.git
cd Topic-Trips-Guide
```
2. **Configure database connection**

Copy ```appsettings.example.json``` to ```appsettings.json```

Update connection strings for MySQL / SQLite

3. **Apply database migrations**

First, create the migration (if not already created):

```bash
dotnet ef migrations add InitialCreate
```

Then apply the migration to the database:

```bash
dotnet ef database update
```

4. **Run the application**

```bash
dotnet run
```

5. **Access the app in your browser**

```http://localhost:5000```

## ⚠️ Notes

Ensure MySQL server is running before applying migrations.

```InitialCreate``` migration sets up all the required tables for the application.

---

# Database & Data Model

The application uses MySQL to store information about users, routes, locations, images, themes, and reviews.

## Entities

### User
- Attributes: `id_user`, `name`, `email`, `password`, `registration_date`
- Semantic Key: `email`

### UserRole
- Attributes: `id`, `user_id`, `role_id`
- Semantic Key: `user_id * role_id`

### Role
- Attributes: `id`, `name`
- Semantic Key: `name`

### Route
- Attributes: `id_route`, `route_name`, `description`, `estimated_duration`, `creation_date`, `user_id`, `theme_id`
- Semantic Key: `route_name`

### Theme
- Attributes: `id_theme`, `name`, `type`, `creation_date`
- Semantic Key: `name`

### Location
- Attributes: `id_location`, `name`, `location_type`, `description`, `estimated_time`, `geolocation`, `street`, `street_number`, `city_id`
- Semantic Key: `name * geolocation`

### City
- Attributes: `id_city`, `name`, `country_id`
- Semantic Key: `name * country_id`

### Country
- Attributes: `id_country`, `name`
- Semantic Key: `name`

### Image
- Attributes: `id_image`, `image_name`, `path`, `location_id`
- Semantic Key: `location_id * image_name`

### Review
- Attributes: `id_review`, `review_code`, `review_date`, `comment`, `rating`, `user_id`, `location_id`
- Semantic Keys: `review_code`, `user_id * location_id * review_date`

### LocationTheme
- Attributes: `id_location_theme`, `location_id`, `theme_id`
- Semantic Key: `location_id * theme_id`

### Itinerary
- Attributes: `id_itinerary`, `date`, `order_number`, `location_theme_id`, `route_id`
- Semantic Key: `route_id * location_theme_id * order_number`
