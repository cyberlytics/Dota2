# Interface Backend -> Python/KI
Für die Interaktion zwischen Backend und Python/KI sind folgende API-Calls
für den Jupyter-Notebooks-Server verfügbar: <br>
URL: http://localhost:8898

* **/writematch**: schreibt übergebene Matchdaten in die MongoDB/KI-Datenbank
    * params: Matchdaten als String oder Dateiinhalt
* **/deletematch**: löscht ein Match aus der MongoDB/KI-Datenbank
    * params: Match-ID des zu löschenden Match
* **/trainmodel**: startet das Training des KI-Modells
* **/predict**: startet die Vorhersage des Modells

Beispielaufrufe mit cURL:
* Daten als String
```
curl -X POST http://localhost:8898/writematch -H "Content-Type: application/json" --data "{'match_id': 0, 'radiant_win': True, "players": [...]}"
```
* Daten aus Datei:
```
curl -X POST http://localhost:8898/writematch -H "Content-Type: application/json" --data @FILEPATH
```
Anmerkungen: 
1. Die Daten als Strings müssen in **doppelten** Anführungszeichen übertragen werden. Attribute innerhalb des JSON-Objekts müssen in **einfachen** Anführungszeichen angegeben werden.
2. Bei der Übertragung von Inhalten aus Dateien muss das "@" vor dem Dateipfad stehen.
