---
applyTo: '**'
---
# Instruktion an ein Language Model – MudBlazor

## Kontext / Ziel
- **Framework**: MudBlazor (Material-Design-Komponenten für Blazor) :contentReference[oaicite:0]{index=0}  
- **Anwendungsfälle**:  
  - Installation und Projektinitialisierung  
  - Komponenten (Layouts, Dialoge, Snackbar, Formulare, Theming)  
- **Ziel**:  
  - Idiomatischen, syntaktisch korrekten C#- und Razor-Code generieren  
  - Formell, motivierend, professionell  
  - Verweise auf offizielle Dokumentation

---

## Prompt-Struktur

**Eingabe (User-Query)**  
```text
„Erkläre [Komponente/Konzept], zeige Beispielcode, erkläre typische Fehlerquellen und Best Practices.“
### Einführung  
(MudDialog ist … Funktion … Einsatz)

### Setup in Program.cs  
– NuGet-Paket „MudBlazor“ installieren  
– Service-Registrierung evtl. über `AddMudServices()` oder `AddMudBlazorDialog()`

### Layout – Voraussetzungen  
– `MudDialogProvider` im `MainLayout.razor` vor `MudLayout` einbinden :contentReference[oaicite:4]{index=4}

### Beispielcode  
```razor
@inject IDialogService DialogService

<MudButton OnClick="OpenDialog">Open Dialog</MudButton>

@code {
  void OpenDialog(){
    DialogService.Show<MyDialog>("Titel");
  }
}


Erstelle keine Kommentare