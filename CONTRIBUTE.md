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
    
# Deployment
Die Applikation besteht aus zwei Teilen, VR und PC. Deswegen müssen für beide Plattformen Builds gemacht werden. Die Anwendung kann zu Testzwecken auch im Editor sowohl für PC als auch VR gestartet werden. Hierfür müssen in den Managern der einzelnen Szenen die Compile-Time Variablen eingestellt werden. Hier gibt es VR_IN_EDITOR und ON_OCULUS_LINK (StartSceneManager, EnvironmentSceneGameManager und AssemblySceneManager). Diese Variablen bestimmen, in welcher Konfiguration das Spiel im Editor gestartet wird, sind aber unentscheidend für einen fertigen Build.
Es ist nicht möglich, auf dem selben Computer den PC-Build mit dem Unity Editor als VR-Spieler zu testen, weil sowohl die gebaute Applikation als auch der Editor die angeschlossene VR-Brille erkennen und verwenden.

(In der Vergangenheit gab es Probleme, wenn Builds von verschiedenen PCs zusammenspielen. Zum Beispiel: PC-Build wurde auf PC X gebaut, VR-Build wurde auf PC Y gebaut.)
    
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
