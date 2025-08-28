# EventMemoria - Fotoportal

Eine moderne Blazor WebApp für das Teilen und Verwalten von Hochzeitsfotos. Die Anwendung ermöglicht es Hochzeitsgästen, Fotos hochzuladen und gemeinsam eine Fotogalerie zu erstellen.

## Features

🎨 **Modern Design** - Material Design mit MudBlazor-Komponenten
📸 **Foto-Upload** - Mehrere Fotos gleichzeitig hochladen
🖼️ **Fotogalerie** - Responsive Grid-Layout für Fotos
🔍 **Foto-Viewer** - Vollbild-Dialog zum Betrachten von Fotos
📱 **Responsive** - Funktioniert auf Desktop, Tablet und Smartphone
⚡ **Echtzeitaktualisierung** - Server-seitige Blazor-Interaktivität

## Technologie-Stack

- **.NET 9.0** - Moderne .NET-Plattform
- **Blazor WebApp** - Blazor-Anwendung
- **MudBlazor** - Material Design-Komponenten
- **C#** - Programmiersprache
- **HTML/CSS** - Frontend-Styling

## Installation und Setup

### Voraussetzungen

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) oder höher
- Ein Code-Editor (Visual Studio, Visual Studio Code, Rider)

### Schritt-für-Schritt Installation

1. **Repository klonen**

   ```bash
   git clone https://github.com/philippmos/EventMemoria.git
   cd EventMemoria
   ```

2. **Abhängigkeiten installieren**

   ```bash
   dotnet restore
   ```

3. **Anwendung kompilieren**

   ```bash
   dotnet build
   ```

4. **Anwendung starten**

   ```bash
   dotnet run
   ```

5. **Browser öffnen**
   
   Die Anwendung ist unter `https://localhost:7087` oder `http://localhost:5111` erreichbar.

## Verwendung

### Fotos hochladen

1. Klicken auf das **"Hochladen"**-Feld in der Fotogalerie
2. Wählen eine oder mehrere Bilddateien aus
3. Die Fotos werden automatisch verarbeitet und der Galerie hinzugefügt

### Fotos betrachten

- Klicke auf ein beliebiges Foto in der Galerie
- Das Foto öffnet sich in einem Vollbild-Dialog
- Navigieren mit den Pfeiltasten oder Schaltflächen durch die Galerie

### Unterstützte Dateiformate

- JPEG (.jpg, .jpeg)
- PNG (.png)
- GIF (.gif)
- WebP (.webp)
- BMP (.bmp)

**Maximale Dateigröße:** 150 MB pro Datei

## Konfiguration

### Styling anpassen

Individuelle Styles können in `wwwroot/css/styles.css` hinzugefügt werden.

## Development

### Projekt erweitern

Das Projekt nutzt eine modulare Struktur. Neue Komponenten können im `Components`-Ordner hinzugefügt werden.

### MudBlazor Dokumentation

Die Anwendung nutzt [MudBlazor](https://mudblazor.com/) für die UI-Komponenten. Weitere Komponenten und Styling-Optionen finden sich in der offiziellen Dokumentation.

### Hot Reload

Für die Entwicklung unterstützt die Anwendung Hot Reload:

```bash
dotnet watch run
```
