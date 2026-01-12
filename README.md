# EventMemoria - Fotoportal

Eine moderne Blazor WebApp für das Teilen und Verwalten von Eventphotos (Hochzeiten etc.).

## Features

🎨 **Modern Design** - Material Design mit MudBlazor-Komponenten
📸 **Foto-Upload** - Mehrere Fotos gleichzeitig hochladen (bis zu 100 Dateien)
🖼️ **Fotogalerie** - Responsive Grid-Layout mit konfigurierbarer Spaltenanzahl (2-12 Spalten)
🔍 **Foto-Viewer** - Vollbild-Dialog zum Betrachten von Fotos mit Navigation
📱 **Responsive** - Funktioniert auf Desktop, Tablet und Smartphone
⚡ **Echtzeitaktualisierung** - Server-seitige Blazor-Interaktivität
🏷️ **Benutzer-Tagging** - Automatische Zuordnung von Fotos zu Autoren
🖼️ **Thumbnail-Generierung** - Automatische Erstellung optimierter Vorschaubilder
☁️ **Azure Storage Integration** - Skalierbare Cloud-Speicherung
🔒 **Data Protection** - Sichere Datenschlüssel-Verwaltung
📊 **QR-Code-Generator** - Einfaches Teilen der Galerie-URL
🎛️ **Anpassbare Konfiguration** - Themes, Titel und Einstellungen konfigurierbar

## Technologie-Stack

### Backend

- **.NET 9.0** - Moderne .NET-Plattform
- **Blazor WebApp** - Server-seitige Blazor-Anwendung mit interaktiven Komponenten
- **MudBlazor** - Material Design-Komponenten für Blazor
- **Azure Storage Blobs** - Cloud-basierte Dateispeicherung
- **ImageMagick.NET** - Bildverarbeitung und Thumbnail-Generierung
- **QRCoder** - QR-Code-Generierung
- **Blazored.LocalStorage** - Client-seitige Datenpersistierung

### Frontend

- **Bun** - Moderne JavaScript-Laufzeit und Package Manager
- **TypeScript** - Typisierte JavaScript-Entwicklung
- **Sass/SCSS** - Erweiterte CSS-Syntax
- **Responsive Design** - Mobile-first Ansatz

### Infrastructure

- **Docker** - Containerisierung mit Multi-Stage Builds
- **Docker Compose** - Lokale Entwicklungsumgebung
- **Azure Blob Storage** - Produktive Cloud-Speicherung
- **Health Checks** - Anwendungsüberwachung

## Installation und Setup

### Voraussetzungen

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) oder höher
- [Bun](https://bun.sh/) für Frontend-Asset-Building
- Ein Code-Editor (Visual Studio, Visual Studio Code, Rider)
- **Optional für Produktion**: Docker und Docker Compose
- **Optional für Cloud-Deployment**: Azure Storage Account

### Schritt-für-Schritt Installation

#### Option 1: Lokale Development-Installation

1. **Repository klonen**

   ```bash
   git clone https://github.com/philippmos/EventMemoria.git
   cd EventMemoria/src
   ```

2. **Frontend-Assets bauen**

   ```bash
   cd EventMemoria.Web/client
   npm run build:install
   npm run build:dev
   ```

3. **Backend-Abhängigkeiten installieren**

   ```bash
   dotnet restore
   ```

4. **Secrets als UserSecrets aufsetzen**
Struktur aus secrets.example.json

5. **Anwendung starten**

   ```bash
   dotnet run --project EventMemoria.Web
   ```

#### Option 2: Docker-Installation

1. **Repository klonen**

   ```bash
   git clone https://github.com/philippmos/EventMemoria.git
   cd EventMemoria/src
   ```

2. **Environment-Variablen konfigurieren**

   ```bash
   cp .env.example .env
   # Bearbeite .env-Datei mit deinen Azure Storage-Einstellungen
   ```

3. **Mit Docker Compose starten**

   ```bash
   docker-compose up -d
   ```

4. **Anwendung ist erreichbar unter**

   - **Development**: `https://localhost:7087` oder `http://localhost:5111`
   - **Docker**: `http://localhost:8080`

#### Option 3: Docker Image von Docker Hub verwenden

Das fertige Docker Image ist auf Docker Hub verfügbar und kann direkt verwendet werden:

1. **Docker Image pullen**

   ```bash
   docker pull philippmos/eventmemoria:latest
   ```

2. **Container starten**

   ```bash
   docker run -d \
     -p 8080:8080 \
     -e ConnectionStrings__AzureStorage="CONNECTIONSTRING" \
     --name eventmemoria \
     philippmos/eventmemoria:latest
   ```

3. **Anwendung aufrufen**

   Öffne `http://localhost:8080` in deinem Browser

**Docker Hub Repository**: [https://hub.docker.com/repository/docker/philippmos/eventmemoria](https://hub.docker.com/repository/docker/philippmos/eventmemoria)

## Verwendung

### Fotos hochladen

1. Klicken auf das **"Hochladen"**-Feld in der Fotogalerie
2. Wählen eine oder mehrere Bilddateien aus
3. Die Fotos werden automatisch verarbeitet und der Galerie hinzugefügt

### Fotos betrachten

- Klicke auf ein beliebiges Foto in der Galerie
- Das Foto öffnet sich in einem Vollbild-Dialog
- Navigieren mit den Pfeiltasten oder Schaltflächen durch die Galerie

### Download-Anfordern-Funktionalität

Die Anwendung bietet eine integrierte Funktion, über die Benutzer alle Event-Fotos als Download anfordern können:

#### Funktionsweise

1. **E-Mail-Registrierung**: Benutzer können ihre E-Mail-Adresse auf der `/download`-Seite hinterlegen
2. **Validierung**: Die E-Mail-Adresse wird validiert und in Azure Table Storage gespeichert
3. **Benachrichtigung**: Nach Abschluss des Events können registrierte Benutzer einen Download-Link per E-Mail erhalten
4. **Duplikatschutz**: Mehrfache Registrierungen mit derselben E-Mail-Adresse werden verhindert

#### Technische Details

- **E-Mail-Template**: Vorgefertigte HTML-E-Mail-Vorlage im `mailing/`-Ordner
- **Storage**: Subscriber werden in Azure Table Storage (`DownloadSubscribers`) gespeichert


### Unterstützte Dateiformate

- JPEG (.jpg, .jpeg)
- PNG (.png)
- GIF (.gif)
- WebP (.webp)
- BMP (.bmp)
- DNG (.dng) - Raw-Format

**Maximale Dateigröße:** 100 MB pro Datei  
**Maximale Anzahl gleichzeitiger Uploads:** 100 Dateien

## Konfiguration

### Anwendungseinstellungen

Die Anwendung kann über die `appsettings.json` konfiguriert werden:

#### PhotoOptions

```json
{
  "PhotoOptions": {
    "StorageContainer": {
      "FullSize": "fullsize-photos",
      "Thumbnails": "thumbnails"
    },
    "AllowedFileTypes": [".jpg", ".jpeg", ".png", ".gif", ".webp", ".bmp", ".dng"],
    "DefaultPhotosPerRow": 2
  }
}
```

#### CustomizationOptions

```json
{
  "CustomizationOptions": {
    "PageTitle": "Meine Hochzeit - Fotoportal",
    "Title": "Hochzeitsgalerie",
    "AvatarPath": "https://example.com/avatar.jpg",
    "Names": "Max & Maria",
    "WelcomeMessage": "Willkommen zu unserer Hochzeit!",
    "QrCodeTargetUrl": "https://photos.example.com"
  }
}
```

### Azure Storage-Konfiguration

Für die Produktion benötigst du einen Azure Storage Account:

1. **Azure Storage Account erstellen**
2. **Connection String kopieren**
3. **In der Konfiguration setzen**:
   - Als User Secret: `dotnet user-secrets set "ConnectionStrings:AzureStorage" "deine-connection-string"`
   - Als Umgebungsvariable: `AZURE_STORAGE_CONNECTION_STRING=deine-connection-string`
   - In Docker: über `.env`-Datei

## Development

### Projekt erweitern

Das Projekt nutzt eine modulare Struktur. Neue Komponenten können im `Components`-Ordner hinzugefügt werden.

### MudBlazor Dokumentation

Die Anwendung nutzt [MudBlazor](https://mudblazor.com/) für die UI-Komponenten. Weitere Komponenten und Styling-Optionen finden sich in der offiziellen Dokumentation.

### Hot Reload

Für die Entwicklung unterstützt die Anwendung Hot Reload:

```bash
dotnet watch run --project EventMemoria.Web
```

## Architektur

Die Anwendung folgt einer sauberen Architektur mit klarer Trennung:

- **Components**: Razor-Komponenten und UI-Logic
- **Services**: Business Logic und externe Integrationen
- **Models**: Datenstrukturen und DTOs
- **Helpers**: Utility-Funktionen und Extensions
- **Common**: Konstanten, Settings und gemeinsame Klassen

