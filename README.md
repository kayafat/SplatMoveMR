# SplatMoveMR (3D Gaussian Splatting in Virtual Reality: Entwicklung einer Anwendung zur interaktiven Raum- und Umzugsplanung)

**SplatMoveMR** ist ein Unity-Prototyp für die Darstellung und Interaktion mit 3D Gaussian Splats in einer Mixed-Reality-Umgebung auf der Meta Quest 3.

Die Anwendung ermöglicht es, reale Objekte wie Möbelstücke oder Pflanzen als Gaussian Splats in der Passthrough-Ansicht der Quest 3 anzuzeigen. Die Objekte können im realen Raum bewegt, gedreht, skaliert und miteinander verglichen werden. Das Projekt wurde im Rahmen eines Studienprojekts entwickelt.

## Projektidee

**Ziel** des Projekts ist es, reale Gegenstände mithilfe von 3D Gaussian Splatting in Mixed Reality nutzbar zu machen. Dadurch können beispielsweise Möbelstücke virtuell in einem realen Raum platziert werden, um eine bessere Vorstellung davon zu bekommen, wie sie im Raum wirken.

Der Prototyp kombiniert:

- Unity
- Meta Quest 3
- Passthrough / Mixed Reality
- OpenXR / Meta XR
- 3D Gaussian Splats
- Controllerbasierte Objektmanipulation

## Hauptfunktionen

- Darstellung von Gaussian Splats in Mixed Reality
- Nutzung der Passthrough-Ansicht der Meta Quest 3
- Wechsel zwischen mehreren Haupt-Splats
- Bewegung des aktiven Objekts über den linken Stick
- Rotation und Höhenanpassung über den rechten Stick
- Skalierung über die Trigger
- Ein- und Ausblenden eines zusätzlichen Single-Splats
- Umschalten der Steuerung zwischen Main-Splat und Single-Splat
- Hilfemenü mit Controllerbelegung
- Start-Hinweis beim Öffnen der App

## Wichtige Skripte

1. ```MoveSplatVR.cs```
   - Hauptskript für die Steuerung der Gaussian Splats. Es verarbeitet Controller-Eingaben für Bewegung, Rotation, Skalierung, Höhenanpassung, Splat-Wechsel, Recenter-Funktion und Floor-Reset.
2. ```SplatManagerSingle.cs```
   - Verwaltet ein zusätzliches einzelnes Splat-Objekt, zum Beispiel eine Pflanze. Dieses Objekt kann unabhängig von den Haupt-Splats ein- und ausgeblendet werden.
3. ```VRHelpMenu.cs```
   - Steuert das Hilfemenü. Das Menü kann über die X-Taste geöffnet und geschlossen werden und wird als World-Space-Canvas vor dem Nutzer platziert.
4. ```StartupHint.cs```
   - Zeigt beim Start der Anwendung kurz einen Hinweis an, dass das Hilfemenü über die X-Taste geöffnet werden kann.
---

# 1. Projekt in Unity öffnen

>Das Projekt wurde mit folgender Unity-Version erstellt: 
```text
https://unity.com/releases/editor/whats-new/2022.3.62f3
```
>Empfohlen wird, das Projekt mit derselben oder einer kompatiblen Unity-Version zu öffnen.

1. Unity Hub öffnen
2. Add from repository
```text
https://github.com/kayafat/SplatMoveMR.git 
``` 
3. Nach dem Herunterladen oder Klonen des Repositorys sollte das Projekt über Unity Hub geöffnet werden.

> [!NOTE]
> Nach dem Öffnen kann es sein, dass Unity zunächst eine leere Standardszene lädt. In diesem Fall muss manuell zur eigentlichen Projektszene gewechselt werden.

## Projektstruktur

```text
Assets/
├── Scenes/
│   └── SampleScene
├── Scripts/
├── Splats/
├── UI/
├── XR/
Packages/
ProjectSettings/
```

→ Dazu in Unity unten im Project-Fenster öffnen:
```text
Assets → Scenes
```
und anschließend die Szene öffnen:
```text
SampleScene
```
Erst danach ist die vollständige Projekt-Hierarchy sichtbar.

---

# 2. Build auf Meta Quest 3
> [!IMPORTANT]
> Um die Anwendung auf der Meta Quest 3 zu starten:

Oben links in Unity öffnen:
```text
File → Build Settings
```
Als Plattform auswählen:
```text
Android
```
- Auf Switch Platform klicken.
- Bei Run Device die angeschlossene Meta Quest 3 auswählen
- Sicherstellen, dass die Quest 3 per USB verbunden ist und USB-Debugging erlaubt wurde.
- Anschließend klicken:
```text
Build And Run
```

>[!NOTE]
>Wenn nach Build And Run mehrere Pop-ups auftreten, bestätigen Sie diese mit Yes.

Nach dem Build wird die Anwendung auf der Quest 3 gestartet. In der Passthrough-Ansicht sollten die Gaussian Splats in der realen Umgebung sichtbar sein.

---

# 3. Nutzung der Anwendung

> [!IMPORTANT]
> Bevor Sie die Anwendung auf der Meta Quest 3 starten, gehen Sie unter Einstellungen auf **Umgebung einrichten**, und wählen Sie die **Freiraum** Option, um den Raum Ihrer Wohnung darzustellen. Anschließend können Sie im Menü der Meta Quest 3, links unter Unbekannten Quellen die SplatMoveMR.apk starten.

Beim Start der App erscheint ein kurzer Hinweis, dass das Hilfemenü über die X-Taste geöffnet werden kann. Danach kann das aktive Objekt mit den Controllern im Raum bewegt, gedreht, skaliert und neu platziert werden.
Die reale Umgebung bleibt durch Passthrough sichtbar. Dadurch können die Gaussian Splats direkt im eigenen Raum betrachtet und positioniert werden.

## Steuerung

| Eingabe | Funktion |
|---|---|
| Linker Stick | Aktives Objekt auf der Bodenebene bewegen |
| Rechter Stick links/rechts | Aktives Objekt drehen |
| Rechter Stick hoch/runter | Höhe des aktiven Objekts ändern |
| Linker Trigger | Objekt verkleinern |
| Rechter Trigger | Objekt vergrößern |
| A-Taste | Zwischen Haupt-Splats wechseln |
| B-Taste | Aktives Objekt vor dem Nutzer platzieren |
| X-Taste | Hilfemenü öffnen oder schließen |
| Y-Taste | Objekt auf Floor-Level zurücksetzen |
| Linker Grip | Single-Splat ein- oder ausblenden |
| Rechter Grip | Steuerung zwischen Main-Splat und Single-Splat wechseln |

---

# Hinweise
Falls nach dem Öffnen des Projekts nur eine leere Szene mit Main Camera und Directional Light sichtbar ist, wurde vermutlich nicht die richtige Szene geöffnet. In diesem Fall unter Assets → Scenes die SampleScene öffnen.

Falls die Quest 3 in den Build Settings nicht angezeigt wird, sollten folgende Punkte geprüft werden:
- Quest 3 ist per USB verbunden
- Entwicklermodus ist aktiviert
- USB-Debugging wurde in der Quest bestätigt
- Android Build Support ist in Unity installiert
- Android ist als Build-Plattform ausgewählt

### Autor
**Fatih Kaya, Mason Schönherr**
>Studienprojekt: Gaussian Splatting / Mixed Reality (Hochschule Esslingen)
