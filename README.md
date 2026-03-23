# The Middleman Simulator 🏢📊

<div align="center">

**[🇩🇪 Deutsche Version](#deutsche-dokumentation) | [🇬🇧 English Version](#english-documentation)**

---

### Business Simulation Game | Educational Software | C# .NET 8.0

<img src="img/screenshot2.png" alt="Game Title Screen" width="600"/>

</div>

---

# Deutsche Dokumentation

## 📋 Inhaltsverzeichnis

- [Überblick](#überblick)
- [Technologien](#technologien)
- [Architektur](#architektur)
- [Spielmechanik](#spielmechanik)
- [Features](#features)
- [Screenshots](#screenshots)
- [Installation & Ausführung](#installation--ausführung)
- [Projektstruktur](#projektstruktur)
- [Testing](#testing)
- [Entwickler-Highlights](#entwickler-highlights)

---

## 🎯 Überblick

**The Middleman Simulator** ist eine umfassende Business-Simulationsanwendung, die in C# mit .NET 8.0 entwickelt wurde. Das Projekt demonstriert fortgeschrittene Softwareentwicklungspraktiken und objektorientiertes Design im Kontext einer Echtzeit-Marktsimulation.

### Projektziel

Eine vollständige Wirtschaftssimulation, in der mehrere Spieler (Zwischenhändler) auf einem virtuellen Agrarmarkt konkurrieren. Spieler müssen Ressourcen verwalten, strategische Entscheidungen treffen und ihre finanzielle Position über eine festgelegte Zeitperiode optimieren.

### Kernkonzept

Spieler übernehmen die Rolle von Zwischenhändlern, die:
- 📦 Agrarprodukte kaufen, verkaufen und lagern
- 💰 Finanzkonten und Budgets verwalten
- 🏭 Lagerkapazitäten und Lagerkosten optimieren
- 💳 Kredite mit Zinssätzen managen
- 📈 Marktdynamik und Produktverfügbarkeit analysieren
- 🤖 Gegen KI-gesteuerte Konkurrenten antreten

---

## 🛠️ Technologien

### Kern-Framework
- **.NET 8.0** - Neueste .NET Runtime-Version
- **C# 11+** - Moderne Sprachfeatures (implicit usings, nullable reference types)
- **Spectre.Console v0.48.0** - Rich Console UI mit formatierten Tabellen, Panels und farbiger Ausgabe

### Abhängigkeiten & Tools
```
├── Spectre.Console v0.48.0      → Rich Terminal UI
├── NUnit v4.0.1                 → Unit Testing Framework
├── Moq v4.20.70                 → Mocking Library
├── Microsoft.NET.Test.SDK v17.8.0 → Test Infrastructure
```

### Entwicklungs-Features
- **Strict Null Checking** aktiviert
- **YAML-Konfiguration** für Produktdaten
- **Dependency Injection** Pattern
- **Repository Pattern** für Datenzugriff
- **Custom Exception Handling**

---

## 🏗️ Architektur

Das Projekt folgt einer sauberen, mehrschichtigen Architektur mit klarer Trennung der Verantwortlichkeiten:

### Architektur-Patterns

#### 1. **Repository Pattern**
Abstraktion der Datenzugriffsschicht für lose Kopplung
```csharp
IProductRepository → ProductRepository
IMiddlemanRepository → MiddlemanRepository
```

#### 2. **Service Layer Pattern**
Geschäftslogik in dedizierten Services
```csharp
MarketService    → Tägliche Simulation & Marktmechanik
MiddlemanService → Händler-Operationen (Kauf/Verkauf/Kredite)
ProductService   → Produktverwaltung & Updates
```

#### 3. **Dependency Injection**
Services werden über Konstruktor-Injektion bereitgestellt

#### 4. **Custom Exception Framework**
Spezifische Ausnahmen für Fehlerbehandlung:
- `InsufficientFundsException`
- `WarehouseCapacityExceededException`
- `ProductNotAvailableException`
- `ProductNotFoundException`
- `LoanAlreadyExistsException`
- `UserInputException`

#### 5. **Observer/Event Pattern**
Aktions-Delegates für UI-Updates und Event-Handling

---

## 🎮 Spielmechanik

### Spielinitialisierung

<img src="img/screenshot2.png" alt="Game Setup" width="600"/>

1. **Konfiguration der Simulationsdauer** (Anzahl der Tage)
2. **Erstellung mehrerer Zwischenhändler** mit:
   - Name und Firmenname
   - Startkapital (initiales Kontostand)
   - Standard-Lagerkapazität: 100 Einheiten

### Täglicher Simulationszyklus

<img src="img/screenshot1.png" alt="Daily Report" width="600"/>

Jeder Spieltag umfasst:

1. **Produkt-Updates** - Verfügbarkeit und Preise schwanken basierend auf Produktionsraten
2. **Lagerkostenabzug** - Spieler zahlen für Lagerraum (belegt + leer)
3. **Kreditzins-Verarbeitung** - Fällige Kredite werden zwangsweise zurückgezahlt
4. **Spieler-Aktionen** - Jeder Spieler kann:
   - 🛒 **Einkaufen** - Produkte vom Markt kaufen
   - 💵 **Verkaufen** - Produkte an Kunden verkaufen
   - 📦 **Lager vergrößern** - Lagerkapazität erweitern
   - 💳 **Kredit aufnehmen** - Finanzielle Mittel leihen
   - ⏭️ **Runde beenden** - Zum nächsten Tag übergehen

### Verfügbare Produkte

10 Agrarprodukte mit unterschiedlichen Eigenschaften (aus `produkte.yml`):

| Produkt | Haltbarkeit | Basispreis |
|---------|-------------|------------|
| 🥒 Gurke | 21 Tage | €5.00 |
| 🍑 Aprikose | 4 Tage | €6.00 |
| 🍅 Tomate | 10 Tage | €7.00 |
| 🥔 Kartoffel | 30 Tage | €3.00 |
| 🧅 Zwiebel | 40 Tage | €2.00 |
| 🥕 Karotte | 20 Tage | €4.00 |
| 🍎 Apfel | 14 Tage | €6.00 |
| 🍌 Banane | 7 Tage | €8.00 |
| 🧃 Orangensaft | 5 Tage | €12.00 |
| 🥛 Milch | 3 Tage | €8.00 |

### Preissystem

- **Basispreis** - Festgelegt für jedes Produkt
- **Einkaufspreis** - Berechnet mit Marktschwankungen
- **Verkaufspreis** - 80% des Basispreises (Gewinnmarge)
- **Rabattstufen** - Mengenbasierte Rabatte beim Einkauf:
  - 25+ Einheiten: 2% Rabatt
  - 50+ Einheiten: 5% Rabatt
  - 75+ Einheiten: 10% Rabatt

### Finanzverwaltung

- **Lagerkosten** - 5€ pro belegter Einheit + 1€ pro leerer Einheit täglich
- **Lagererweiterung** - 50€ pro Einheit Kapazitätserweiterung
- **Kredite** - 7-tägige Rückzahlungsfrist mit konfigurierbaren Zinssätzen
- **Konkurs** - Spiel endet für Spieler bei Kontostand ≤ 0

### Spielende-Bedingungen

- ⏰ Simulationsdauer abgelaufen
- 💸 Alle Spieler bankrott
- 🏆 Gewinner nach finalem Kontostand gerankt

<img src="img/screenshot11.png" alt="Final Rankings" width="400"/>

---

## ✨ Features

### 1. **Rich Console UI mit Spectre.Console**

<img src="img/screenshot8.png" alt="Warehouse Visualization" width="600"/>

- 📊 Formatierte Tabellen für Produktlisten
- 📈 Visuelle Balkendiagramme für Lagerverteilung
- 🎨 Farbcodierte Ausgaben für bessere Lesbarkeit
- 📋 Strukturierte Panels für Tagesberichte
- ✅ Interaktive Menüs und Eingabeaufforderungen

### 2. **Dynamische Marktsimulation**
- Produkte mit variabler Verfügbarkeit
- Preisschwankungen basierend auf Angebot
- Verderbliche Waren mit Haltbarkeitsdatum
- Mengenrabatte für Großeinkäufe

### 3. **Umfassendes Finanzsystem**
- Detaillierte Ausgabenverfolgung
- Kreditsystem mit Zinsen
- Automatische Lagerkostenberechnung
- Konkursmechanik

### 4. **KI-Konkurrenten**
- Multiple KI-gesteuerte Händler
- Automatisierte Handelsentscheidungen
- Wettbewerbsbasiertes Ranking-System

### 5. **YAML-basierte Konfiguration**
- Externe Produktdefinitionen
- Einfache Erweiterbarkeit
- Datentrennung vom Code

---

## 📸 Screenshots

### Spielsetup & Konfiguration
<img src="img/screenshot2.png" alt="Setup" width="600"/>

*Konfiguration der Simulation mit mehreren Spielern und Firmen*

### Tagesbericht
<img src="img/screenshot1.png" alt="Daily Report" width="600"/>

*Detaillierte Übersicht über Finanzen und Ausgaben*

### Verkaufen & Lagerverwaltung
<img src="img/screenshot8.png" alt="Selling Interface" width="600"/>

*Produktverkauf mit visueller Lagerverteilung*

### Finale Rangliste
<img src="img/screenshot11.png" alt="Leaderboard" width="400"/>

*Endgültige Platzierung nach Kontostand*

---

## 🚀 Installation & Ausführung

### Voraussetzungen
- .NET 8.0 SDK oder höher
- Windows, macOS oder Linux
- Terminal/Konsole mit Unicode-Unterstützung

### Installation

```bash
# Repository klonen
git clone https://gitlab.myfesko.com/docker/middlemen-simulator.git
cd middlemen-simulator

# Dependencies wiederherstellen
dotnet restore

# Projekt bauen
dotnet build

# Anwendung ausführen
dotnet run --project TheMiddleman/TheMiddleman.csproj
```

### Alternativer Start (aus Build-Verzeichnis)
```bash
cd TheMiddleman/bin/Debug/net8.0
./TheMiddleman
```

---

## 📁 Projektstruktur

```
middlemen-simulator/
├── TheMiddleman/                    # Hauptanwendung
│   ├── Entities/                    # Datenmodelle
│   │   ├── Middleman.cs            # Händler-Entity
│   │   ├── Product.cs              # Produkt-Entity
│   │   └── Loan.cs                 # Kredit-Entity
│   │
│   ├── business_logic/             # Geschäftslogik
│   │   ├── MarketService.cs        # Marktsimulation
│   │   ├── MiddlemanService.cs     # Händler-Operationen
│   │   └── ProductService.cs       # Produktverwaltung
│   │
│   ├── data_access/                # Repository Pattern
│   │   ├── IProductRepository.cs   # Produkt-Interface
│   │   ├── ProductRepository.cs    # Produkt-Implementierung
│   │   ├── IMiddlemanRepository.cs # Händler-Interface
│   │   └── MiddlemanRepository.cs  # Händler-Implementierung
│   │
│   ├── exceptions/                 # Custom Exceptions
│   │   ├── InsufficientFundsException.cs
│   │   ├── WarehouseCapacityExceededException.cs
│   │   ├── ProductNotAvailableException.cs
│   │   ├── ProductNotFoundException.cs
│   │   ├── LoanAlreadyExistsException.cs
│   │   └── UserInputException.cs
│   │
│   ├── user_interface/             # UI-Layer
│   │   └── ConsoleUI.cs            # Spectre.Console UI
│   │
│   ├── Utils/                      # Hilfsfunktionen
│   │   └── CurrencyFormatter.cs    # Währungsformatierung
│   │
│   ├── tests/                      # Unit Tests
│   │   ├── ProductServiceTests.cs
│   │   └── MiddlemanServiceTests.cs
│   │
│   ├── Game.cs                     # Spiel-Orchestrator
│   ├── MainRunner.cs               # Entry Point
│   ├── produkte.yml                # Produktkonfiguration
│   └── TheMiddleman.csproj         # Projektdatei
│
├── img/                            # Dokumentation & Screenshots
│   ├── screenshot1.png - screenshot11.png
│
├── TheMiddleman.sln                # Visual Studio Solution
├── README.md                       # Diese Datei
└── .gitignore                      # Git-Konfiguration
```

---

## 🧪 Testing

Das Projekt verwendet **NUnit 4.0.1** und **Moq 4.20.70** für umfassendes Testing:

```bash
# Alle Tests ausführen
dotnet test

# Tests mit detaillierter Ausgabe
dotnet test --verbosity detailed
```

### Test-Coverage
- ✅ ProductService Unit Tests
- ✅ MiddlemanService Unit Tests
- ✅ Mock-basierte Repository Tests
- ✅ Exception Handling Tests

---

## 💼 Entwickler-Highlights

### Warum dieses Projekt beeindruckend ist:

#### 1. **Saubere Architektur**
- Strikte Trennung von Concerns (UI, Business Logic, Data Access)
- SOLID-Prinzipien konsequent angewandt
- Dependency Injection für testbare und wartbare Codebase

#### 2. **Moderne C# Features**
- Nullable Reference Types für erhöhte Typsicherheit
- Implicit Usings für reduzierten Boilerplate
- Pattern Matching und moderne Sprachkonstrukte

#### 3. **Professional UI/UX**
- Rich Terminal UI mit Spectre.Console
- Informative Visualisierungen (Tabellen, Charts)
- Benutzerfreundliche Menüführung

#### 4. **Umfassendes Exception Handling**
- Custom Exception-Framework
- Spezifische Fehlerbehandlung für jeden Geschäftsfall
- Benutzerfreundliche Fehlermeldungen

#### 5. **Konfigurierbarkeit**
- YAML-basierte Produktkonfiguration
- Externe Datenverwaltung
- Leicht erweiterbar ohne Code-Änderungen

#### 6. **Testing & Quality**
- Unit Tests mit NUnit
- Mocking mit Moq
- Testbare Architektur durch DI

#### 7. **Komplexe Geschäftslogik**
- Multi-Spieler Simulation
- Finanzielle Berechnungen
- Marktdynamik und Preisfindung
- Zeitbasierte Mechaniken (Haltbarkeit, Kreditzinsen)

---

## 🎓 Gelerntes & Demonstrierte Fähigkeiten

- ✅ .NET 8.0 / C# 11+ Entwicklung
- ✅ Objektorientierte Programmierung (OOP)
- ✅ Design Patterns (Repository, Service Layer, DI)
- ✅ SOLID-Prinzipien
- ✅ Exception Handling
- ✅ Unit Testing (NUnit, Moq)
- ✅ YAML-Konfiguration
- ✅ Console Application Development
- ✅ Algorithmisches Denken
- ✅ Software-Architektur
- ✅ Code-Organisation und -Struktur
- ✅ Third-Party Library Integration (Spectre.Console)

---

## 📧 Kontakt & Weitere Informationen

Dieses Projekt wurde als Demonstrationsprojekt für Portfolio-Zwecke entwickelt und zeigt fortgeschrittene C#/.NET-Entwicklungsfähigkeiten.

**Repository:** https://gitlab.myfesko.com/docker/middlemen-simulator

---

<div align="center">

**[⬆️ Zurück nach oben](#the-middleman-simulator-)**

---

# English Documentation

</div>

## 📋 Table of Contents

- [Overview](#overview)
- [Technologies](#technologies)
- [Architecture](#architecture)
- [Game Mechanics](#game-mechanics)
- [Features](#features-1)
- [Screenshots](#screenshots-1)
- [Installation & Running](#installation--running)
- [Project Structure](#project-structure)
- [Testing](#testing-1)
- [Developer Highlights](#developer-highlights)

---

## 🎯 Overview

**The Middleman Simulator** is a comprehensive business simulation application developed in C# with .NET 8.0. The project demonstrates advanced software development practices and object-oriented design in the context of a real-time market simulation.

### Project Goal

A complete economic simulation where multiple players (middlemen/traders) compete in a virtual agricultural market. Players must manage resources, make strategic decisions, and optimize their financial position over a defined time period.

### Core Concept

Players take on the role of middlemen who:
- 📦 Buy, sell, and store agricultural products
- 💰 Manage financial accounts and budgets
- 🏭 Optimize warehouse capacity and storage costs
- 💳 Handle loans with interest rates
- 📈 Analyze market dynamics and product availability
- 🤖 Compete against AI-driven opponents

---

## 🛠️ Technologies

### Core Framework
- **.NET 8.0** - Latest .NET Runtime version
- **C# 11+** - Modern language features (implicit usings, nullable reference types)
- **Spectre.Console v0.48.0** - Rich Console UI with formatted tables, panels, and colored output

### Dependencies & Tools
```
├── Spectre.Console v0.48.0      → Rich Terminal UI
├── NUnit v4.0.1                 → Unit Testing Framework
├── Moq v4.20.70                 → Mocking Library
├── Microsoft.NET.Test.SDK v17.8.0 → Test Infrastructure
```

### Development Features
- **Strict Null Checking** enabled
- **YAML Configuration** for product data
- **Dependency Injection** pattern
- **Repository Pattern** for data access
- **Custom Exception Handling**

---

## 🏗️ Architecture

The project follows a clean, multi-layered architecture with clear separation of concerns:

### Architecture Patterns

#### 1. **Repository Pattern**
Data access layer abstraction for loose coupling
```csharp
IProductRepository → ProductRepository
IMiddlemanRepository → MiddlemanRepository
```

#### 2. **Service Layer Pattern**
Business logic in dedicated services
```csharp
MarketService    → Daily simulation & market mechanics
MiddlemanService → Trader operations (buy/sell/loans)
ProductService   → Product management & updates
```

#### 3. **Dependency Injection**
Services provided via constructor injection

#### 4. **Custom Exception Framework**
Specific exceptions for error handling:
- `InsufficientFundsException`
- `WarehouseCapacityExceededException`
- `ProductNotAvailableException`
- `ProductNotFoundException`
- `LoanAlreadyExistsException`
- `UserInputException`

#### 5. **Observer/Event Pattern**
Action delegates for UI updates and event handling

---

## 🎮 Game Mechanics

### Game Initialization

<img src="img/screenshot2.png" alt="Game Setup" width="600"/>

1. **Configure simulation duration** (number of days)
2. **Create multiple middlemen** with:
   - Name and company name
   - Starting capital (initial account balance)
   - Default warehouse capacity: 100 units

### Daily Simulation Cycle

<img src="img/screenshot1.png" alt="Daily Report" width="600"/>

Each game day includes:

1. **Product Updates** - Availability and pricing fluctuate based on production rates
2. **Storage Cost Deduction** - Players pay for warehouse space (occupied + empty)
3. **Loan Interest Processing** - Due loans are enforced for repayment
4. **Player Actions** - Each player can:
   - 🛒 **Buy** - Purchase products from the market
   - 💵 **Sell** - Sell products to customers
   - 📦 **Expand Warehouse** - Increase storage capacity
   - 💳 **Take Loan** - Borrow financial resources
   - ⏭️ **End Round** - Proceed to the next day

### Available Products

10 agricultural products with varying characteristics (from `produkte.yml`):

| Product | Durability | Base Price |
|---------|------------|------------|
| 🥒 Cucumber | 21 days | €5.00 |
| 🍑 Apricot | 4 days | €6.00 |
| 🍅 Tomato | 10 days | €7.00 |
| 🥔 Potato | 30 days | €3.00 |
| 🧅 Onion | 40 days | €2.00 |
| 🥕 Carrot | 20 days | €4.00 |
| 🍎 Apple | 14 days | €6.00 |
| 🍌 Banana | 7 days | €8.00 |
| 🧃 Orange Juice | 5 days | €12.00 |
| 🥛 Milk | 3 days | €8.00 |

### Pricing System

- **Base Price** - Set for each product
- **Purchase Price** - Calculated with market fluctuations
- **Selling Price** - 80% of base price (profit margin)
- **Discount Tiers** - Volume-based discounts on purchases:
  - 25+ units: 2% discount
  - 50+ units: 5% discount
  - 75+ units: 10% discount

### Financial Management

- **Storage Costs** - €5 per occupied unit + €1 per empty unit daily
- **Warehouse Expansion** - €50 per unit capacity increase
- **Loans** - 7-day repayment period with configurable interest rates
- **Bankruptcy** - Game ends for player when balance ≤ 0

### End Conditions

- ⏰ Simulation duration expires
- 💸 All players bankrupt
- 🏆 Winners ranked by final account balance

<img src="img/screenshot11.png" alt="Final Rankings" width="400"/>

---

## ✨ Features

### 1. **Rich Console UI with Spectre.Console**

<img src="img/screenshot8.png" alt="Warehouse Visualization" width="600"/>

- 📊 Formatted tables for product listings
- 📈 Visual bar charts for warehouse distribution
- 🎨 Color-coded outputs for better readability
- 📋 Structured panels for daily reports
- ✅ Interactive menus and prompts

### 2. **Dynamic Market Simulation**
- Products with variable availability
- Price fluctuations based on supply
- Perishable goods with expiration dates
- Volume discounts for bulk purchases

### 3. **Comprehensive Financial System**
- Detailed expense tracking
- Loan system with interest
- Automatic storage cost calculation
- Bankruptcy mechanics

### 4. **AI Competitors**
- Multiple AI-driven traders
- Automated trading decisions
- Competition-based ranking system

### 5. **YAML-based Configuration**
- External product definitions
- Easy extensibility
- Data separation from code

---

## 📸 Screenshots

### Game Setup & Configuration
<img src="img/screenshot2.png" alt="Setup" width="600"/>

*Simulation configuration with multiple players and companies*

### Daily Report
<img src="img/screenshot1.png" alt="Daily Report" width="600"/>

*Detailed overview of finances and expenses*

### Selling & Warehouse Management
<img src="img/screenshot8.png" alt="Selling Interface" width="600"/>

*Product sales with visual warehouse distribution*

### Final Leaderboard
<img src="img/screenshot11.png" alt="Leaderboard" width="400"/>

*Final rankings by account balance*

---

## 🚀 Installation & Running

### Prerequisites
- .NET 8.0 SDK or higher
- Windows, macOS, or Linux
- Terminal/Console with Unicode support

### Installation

```bash
# Clone repository
git clone https://gitlab.myfesko.com/docker/middlemen-simulator.git
cd middlemen-simulator

# Restore dependencies
dotnet restore

# Build project
dotnet build

# Run application
dotnet run --project TheMiddleman/TheMiddleman.csproj
```

### Alternative Start (from build directory)
```bash
cd TheMiddleman/bin/Debug/net8.0
./TheMiddleman
```

---

## 📁 Project Structure

```
middlemen-simulator/
├── TheMiddleman/                    # Main application
│   ├── Entities/                    # Data models
│   │   ├── Middleman.cs            # Trader entity
│   │   ├── Product.cs              # Product entity
│   │   └── Loan.cs                 # Loan entity
│   │
│   ├── business_logic/             # Business logic
│   │   ├── MarketService.cs        # Market simulation
│   │   ├── MiddlemanService.cs     # Trader operations
│   │   └── ProductService.cs       # Product management
│   │
│   ├── data_access/                # Repository pattern
│   │   ├── IProductRepository.cs   # Product interface
│   │   ├── ProductRepository.cs    # Product implementation
│   │   ├── IMiddlemanRepository.cs # Trader interface
│   │   └── MiddlemanRepository.cs  # Trader implementation
│   │
│   ├── exceptions/                 # Custom exceptions
│   │   ├── InsufficientFundsException.cs
│   │   ├── WarehouseCapacityExceededException.cs
│   │   ├── ProductNotAvailableException.cs
│   │   ├── ProductNotFoundException.cs
│   │   ├── LoanAlreadyExistsException.cs
│   │   └── UserInputException.cs
│   │
│   ├── user_interface/             # UI layer
│   │   └── ConsoleUI.cs            # Spectre.Console UI
│   │
│   ├── Utils/                      # Utility functions
│   │   └── CurrencyFormatter.cs    # Currency formatting
│   │
│   ├── tests/                      # Unit tests
│   │   ├── ProductServiceTests.cs
│   │   └── MiddlemanServiceTests.cs
│   │
│   ├── Game.cs                     # Game orchestrator
│   ├── MainRunner.cs               # Entry point
│   ├── produkte.yml                # Product configuration
│   └── TheMiddleman.csproj         # Project file
│
├── img/                            # Documentation & screenshots
│   ├── screenshot1.png - screenshot11.png
│
├── TheMiddleman.sln                # Visual Studio solution
├── README.md                       # This file
└── .gitignore                      # Git configuration
```

---

## 🧪 Testing

The project uses **NUnit 4.0.1** and **Moq 4.20.70** for comprehensive testing:

```bash
# Run all tests
dotnet test

# Run tests with detailed output
dotnet test --verbosity detailed
```

### Test Coverage
- ✅ ProductService Unit Tests
- ✅ MiddlemanService Unit Tests
- ✅ Mock-based Repository Tests
- ✅ Exception Handling Tests

---

## 💼 Developer Highlights

### Why This Project is Impressive:

#### 1. **Clean Architecture**
- Strict separation of concerns (UI, Business Logic, Data Access)
- SOLID principles consistently applied
- Dependency Injection for testable and maintainable codebase

#### 2. **Modern C# Features**
- Nullable Reference Types for increased type safety
- Implicit Usings for reduced boilerplate
- Pattern Matching and modern language constructs

#### 3. **Professional UI/UX**
- Rich Terminal UI with Spectre.Console
- Informative visualizations (tables, charts)
- User-friendly menu navigation

#### 4. **Comprehensive Exception Handling**
- Custom exception framework
- Specific error handling for each business case
- User-friendly error messages

#### 5. **Configurability**
- YAML-based product configuration
- External data management
- Easily extensible without code changes

#### 6. **Testing & Quality**
- Unit tests with NUnit
- Mocking with Moq
- Testable architecture through DI

#### 7. **Complex Business Logic**
- Multi-player simulation
- Financial calculations
- Market dynamics and price discovery
- Time-based mechanics (expiration, loan interest)

---

## 🎓 Skills Learned & Demonstrated

- ✅ .NET 8.0 / C# 11+ Development
- ✅ Object-Oriented Programming (OOP)
- ✅ Design Patterns (Repository, Service Layer, DI)
- ✅ SOLID Principles
- ✅ Exception Handling
- ✅ Unit Testing (NUnit, Moq)
- ✅ YAML Configuration
- ✅ Console Application Development
- ✅ Algorithmic Thinking
- ✅ Software Architecture
- ✅ Code Organization and Structure
- ✅ Third-Party Library Integration (Spectre.Console)

---

## 📧 Contact & Further Information

This project was developed as a demonstration project for portfolio purposes and showcases advanced C#/.NET development skills.

**Repository:** https://gitlab.myfesko.com/docker/middlemen-simulator

---

<div align="center">

**[⬆️ Back to Top](#the-middleman-simulator-)**

---

*Developed with ❤️ using .NET 8.0 and C#*

</div>
