# Projekt Setup
Dieses Projekt wurde mit Unity 2021.3.0f1 erstellt.

<aside>
💡 Um mit der Entwicklung zu starten, müssen folgende initiale Schritte ausgeführt werden. Der Ordner `ImportedAssets` ist nicht in Git enthalten. Deswegen müssen verwendete Assets in Unity in den `ImportedAssets` Ordner importiert und dann selbstständig zwischen den Teammitgliedern synchronisiert werden. 
</aside>

1. Projekt pullen bzw. clonen
2. Projekt in Unity öffnen, auftretende Fehler ignorieren (diese entstehen durch fehlende Referenzen, die noch importiert werden)
3. Assets > Import Package > Custom Package.. > ImportedAssets.unitypackage auswählen und importieren
4. MRTK Project Configurator: Always Skip Setup auswählen

Wenn bei einem bestehenden Projekt Probleme / Fehler auftreten, kann es helfen, den Library Ordner des Projektes zu löschen.
    
# Git-Workflow

- Jegliches Arbeiten am Projekt geschieht auf gesonderten Branches. Z.B.: ZuständigerName_Feature: Emir_VRMovement
- Wenn die Funktionalität soweit fertig ist und alles fehlerfrei läuft, kann ein Pull-Request auf den Main-Branch gestellt werden. Alle Änderungen vom Main-Branch müssen in den eigenen Branch integriert werden. Die Änderungen werden zeitnah überprüft und der PR gemerged.


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
    - Resources (Prefabs, die von Photon instanziiert werden sollen)
    - Scenes - .scene
    - Scripts - .cs
    - Textures - .png, .jpg, .renderTexture
    - Video - .mp4, …
