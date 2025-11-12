Inlogg för Admin är "admin@friberg.com" och lösen: "Test1234!"
Som kund måste du registrera dig.

#  Friberg Car Rentals

Ett skolprojekt utvecklat i **ASP.NET Core MVC** med tillhörande **Web API**.  
Syftet är att skapa ett komplett biluthyrningssystem med stöd för både **kund**- och **administratörsroller**.

---

##  Projektbeskrivning

Friberg Car Rentals är en webbapplikation där:
- **Kunder** kan registrera sig, logga in, se tillgängliga bilar och boka dem under valda datum.
- **Administratörer** kan logga in för att hantera bilar, kunder och bokningar direkt i systemet.

Systemet är byggt med:
- ASP.NET Core MVC (frontend)
- ASP.NET Core Web API (backend)
- Entity Framework Core (databaslager)
- SQL Server (databas)
- Bootstrap 5 (UI-design)
- Session-baserad autentisering i MVC-klienten

---

##  Funktionalitet

###  För kunder
- Registrera nytt konto  
- Logga in / logga ut  
- Visa tillgängliga bilar  
- Se bildetaljer  
- Skapa bokningar  
- Se sina egna bokningar  
- Avboka kommande bokningar  

###  För administratörer
- Logga in / logga ut  
- Dashboard med översikt  
- Hantera bilar (skapa, redigera, ta bort)  
- Hantera kunder (visa, ta bort)  
- Hantera bokningar (visa, bekräfta, ta bort)  

---

##  Databasstruktur (förenklad)

| Tabell | Exempel på fält |
|--------|------------------|
| **Cars** | Id, Brand, Model, Year, Color, PriceRate, ImageUrls |
| **Customers** | Id, FullName, Email, ApiUserId |
| **Bookings** | Id, CarId, CustomerId, StartDate, EndDate, IsConfirmed |
| **AspNetUsers** | Hanteras av Identity (inloggningar) |

---

##  Roller & behörigheter

| Roll | Behörighet |
|------|-------------|
| **Admin** | Full tillgång till bil-, kund- och bokningshantering |
| **User (Kund)** | Kan se, boka och avboka bilar |
| **Anonym användare** | Kan endast se bilar och logga in / registrera sig |

---

## Teknisk översikt

**Frontend (MVC)**  
- Byggd i ASP.NET Core MVC  
- Använder `HttpClient` för att anropa API:t  
- Bootstrap 5 för layout och komponenter  
- Session används för att spara inloggningstillstånd

**Backend (API)**  
- ASP.NET Core Web API  
- Identity för användar- och rollhantering  
- Entity Framework Core + SQL Server  
- Automapper-liknande mapping via `DtoMapper`

---

