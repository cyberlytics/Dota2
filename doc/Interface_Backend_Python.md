# Interface Backend -> Python/KI


Für die Interaktion zwischen Backend und Python/KI sind folgende API-Calls
für den Jupyter-Notebooks-Server verfügbar: <br>
URL: http://localhost:8898

## Schnittstellen
* **/writematch**: schreibt übergebene Matchdaten in die MongoDB/KI
    * Parameter: Daten des zur übertragenden Matches im JSON-Format (siehe Abschnitt Parameter)
    * Rückgabe: None
* **/deletematch**: löscht ein Match aus der MongoDB/KI
    * Parameter: Match-ID des zu löschenden Match
    * Rückgabe: None
* **/trainmodel**: startet das Training des KI-Modells
    * Parameter: Name des zu trainierenden Modells (siehe Abschnitt Parameter)
    * Rückgabe: None
* **/predict**: startet die Vorhersage des Modells
    * Parameter: Name des zu trainierenden Modells (siehe Abschnitt Parameter)
    * Rückgabe: Beschreibung des Ergebnisses als String

## Parameter
### Aufbau Match
```
match = {
    'match_id': long int,
    'duration': int,
    'players': list [player],
    'player': {
        'pings': int,
        'assits': int,
        'deaths': int,
        'kills': int,
        'win': boolean,
    }
}
```
### Model Parameter
* model_name:
    * "no_KDA": Modell zu User Story 1
    * "KDA": Modell zu User Story 2 & 3


## Beispielaufrufe mit cURL:
* **/writematch**:
```
curl -i -X POST http://localhost:8898/writematch -H "Content-Type: application/json" --data "<match>"
```
* **/deletematch**:
```
curl -i -X POST http://localhost:8898/deletematch -H "Content-Type: application/json" --data "<match_id>"
```
* **/trainmodel**:
```
curl -i -X POST http://localhost:8898/trainmodel -H "Content-Type: application/json" --data "<model_name>"
```
* **/predict**:
```
curl -i -X POST http://localhost:8898/predict -H "Content-Type: application/json" --data "<model_name>"
```

## Return Codes:
```
200 OK: Keine Aktion ausgeführt
```
```
201 Created: Eintrag erfolgreich in Datenbank erstellt
```
```
204 No Content: Kein Eintrag in Datenbank gefunden
```
```
205 Reset Content: Eintrag erfolgreich aus Datenbank entfernt
```
```
409 Conflicted: Eintrag nicht in Datenbank erstellt, da schon vorhanden
```
```
501 Not Implemented: Funktion noch nicht implementiert
```