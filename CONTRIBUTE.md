# Projekt Setup
<aside>
💡 Um das Projekt zum laufen zu bekommen, müssen folgende initiale Schritte ausgeführt werden. Sobald sich Elemente in den `ImportedAssets` ändern, müssen die Schritte wiederholt werden.

</aside>

1. Projekt pullen bzw. clonen
2. alle Inhalte des Ordners`ImportedAssets` löschen
3. alle Inhalte des Ordners `Library` löschen
4. Projekt in Unity öffnen
5. **Unity-Package** importieren 
    
    [https://drive.google.com/file/d/1x5rHdCF8eu-ChTs9i_80rtx7WvA7Tsja/view?usp=sharing](https://drive.google.com/file/d/1x5rHdCF8eu-ChTs9i_80rtx7WvA7Tsja/view?usp=sharing)
    
6. Start und Endscene Video importieren
    
    [https://drive.google.com/file/d/1HGnyGWBqK896eOz__5YSctF4kY25UIPD/view?usp=sharing](https://drive.google.com/file/d/1HGnyGWBqK896eOz__5YSctF4kY25UIPD/view?usp=sharing)
    

# Git-Workflow
- Mainscene von der alle ihre Testscenes abzweigen
- Übersicht an Testscenes (welche Assets/Mechaniken/Packages werden jeweils neu gebraucht)
- Merging

## “Regeln”

- Jede/r erstellt sich aus dem Main Branch einen eigenen Branch, den er folgendermaßen bennent: ZuständigerName_Funktionalität → bsp: Emir_VRMovement
- Darin dupliziert er/sie die aktuelle Main-Scene (oder bei Bedarf eine andere Scene) und benennt sie wieder entsprechend um: AbgeleiteteScene_ZuständigerName_Funktionalität → bsp: Main_Emir_VRMovement
- Wenn die Funktionalität soweit fertig ist und alles fehlerfrei läuft, kann ein Pull-Request auf den Main-Branch gestellt werden. Der Zuständige überprüft dies zeitnah und merged den Branch auf die Main. Von dort aus kann der aktuelle Main-Stand gepullt und wiederum ein eigener Branch erstellt werden.

## Assets

- Eine zentrale Liste zu allen benötigten Assets
- Person A, die neue Assets hinzufügt
    1. Updated Liste (mit Asset-Name, Verlinkung und wie man es installiert)
    2. Packt Asset ins ./gitignore (bzw. Ordner “Imported Assets” mit Assets steht im gitignore)
- Person B
    1. Lädt Assets aus Liste via Link in Unity

## Unity Ordner-Struktur

- “Imported Assets” Ordner bei jedem lokal (im ./gitignore)
    - Hier werden alle eigenen und die Assets von anderen lokal abgespeichert und nicht ins git gepushed
- Andere Ordner
    - Animations - .anim, .controller (Animation Controller)
    - Audios - .mp3, .wav, .mixer (Sound Mixer), …
    - Materials - .mat
    - Models - .blend, .fbx
    - MRTK
    - Prefabs - .prefab
    - Paricles - .prefab von Particle Objekten
    - Resources
    - Scenes - .scene
    - Scripts - .cs
    - Textures - .png, .jpg, .renderTexture
    - Video - .mp4, …
