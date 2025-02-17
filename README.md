# Passwort Manager

## Überblick

Dieses Projekt ist ein Passwort-Manager, der auf moderne Software-Architekturprinzipien setzt. Dabei wird besonderer Wert auf **Inversion of Control (IoC)** gelegt. 

## Architektur

- **Lose Kopplung** durch den Einsatz von **Interfaces**
- **Dependency Injection (DI)** zur Verwaltung der Services
- **Service-Komposition** in einer zentralen Klasse
- **Persistenz** mit **SQLite** und **Entity Framework Core**

## Features

- Speicherung und Verwaltung von Passwörtern
- Nutzung von **Entity Framework Core** für die Datenverwaltung 
- **IoC**-Prinzipien für bessere Wartbarkeit und Erweiterbarkeit
- **Avalonia UI** für eine plattformübergreifende Desktop-Anwendung

## In Entwicklung

- **Benutzeroberfläche mit Avalonia UI**
- **Unit Tests für Services**

## Geplante Features

- **Webinterface mit Blazor**
- **Web-API für zentrale Passwortverwaltung**

## Technologie-Stack

- **Programmiersprache:** C#
- **Frameworks & Bibliotheken:**
  - Entity Framework Core
  - Avalonia UI
  - Blazor (geplant)
  - SQLite
  - Dependency Injection
