# Interface Backend -> Python/KI


Für die Interaktion zwischen Backend und Python/KI sind folgende API-Calls
für den Jupyter-Notebooks-Server verfügbar: <br>
URL: http://localhost:8898

## Schnittstellen
* **/writematch**: schreibt übergebene Matchdaten in die MongoDB/KI-Datenbank
    * params: Matchdaten als String oder Dateiinhalt
* **/deletematch**: löscht ein Match aus der MongoDB/KI-Datenbank
    * params: Match-ID des zu löschenden Match
* **/trainmodel**: startet das Training des KI-Modells
* **/predict**: startet die Vorhersage des Modells

## Aufbau der zu übertragenden Matchdaten

```
{
    'match_id': long int,
    'radiant_win': boolean,
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

## Beispielaufrufe mit cURL:
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

## Return Codes:

* 200: Call war erfolgreich
* 501: Funktion noch nicht implementiert
