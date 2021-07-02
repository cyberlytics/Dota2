import pymongo
import pickle
from os import environ
from typing import List, Dict

# Klasse zur Verwaltung der Verbindung zu einer MongoDB
class DatabaseConnector:
    url = ''
    port = -1
    database = ''
    collection = ''
    client = None
    connection = None
    active_collection = None

    def __init__(self, url = 'localhost', port = 27018, database = 'Data-KI', collection = "MatchesDto"):
        # Url aus docker-compose.yml verwenden, falls vorhanden
        if "MONGODB_HOST" in environ:
            self.url = environ['MONGODB_HOST']
        else:
            self.url = url
        # Port aus docker-compose.yml verwenden, falls vorhanden
        if "MONGODB_PORT" in environ:
            self.port = int(environ['MONGODB_PORT'])
        else:
            self.port = port
        
        self.database = database
        self.collection = collection
        pass
        
    def connect(self):
        """
        Baut eine Verbindung zur MongoDB auf.
        """
        self.client = pymongo.MongoClient(self.url, self.port)
        self.db = self.client[self.database]
        self.connection = self
        self.active_collection = self.connection.getCollection(self.collection)
        return self
    
    def disconnect(self):
        """
        Schließt die Verbindung zur MongoDB.
        """
        try:
            self.client.close()
            return self
        except AttributeError:
            raise ConnectionError("Client is not connected to any database.")
    
    def getCollectionNames(self):
        """
        Gibt alle in der MongoDB vorhandenen Collection-Names zurück.

        Returns:
        List: Liste mit den Collections
        """
        return self.db.list_collection_names()

    def getCollection(self, collection):
        """
        Gibt die gesuchte Collection zurück.

        Parameters:
        collection (string): Name der Collection.

        Returns:
        Collection: Gibt die Collection zurück.
        """
        return self.db[collection]
    
    def create(self, match):
        """
        Erstellt einen neuen Eintrag / neues Match in der MongoDB.

        Parameters:
        match (Match): Zu erstellendes Match

        Returns:
        Match: Erstelltes Match
        """
        match = self.active_collection.insert_one(match)
        return match

    def get(self, id=None):
        """
        Gibt alle hinterlegten Matches zurueck

        Parameters:
        id (int): Optional: Match-ID des gesuchten Matchs

        Returns:
        List<Match>: Liste mit den gesuchten Matches
        """
        if id == None:
            # Alle Einträge zurückgeben
            return self.active_collection.find()
        else:
            # Eintrag mit match_id <id> zurückgeben
            return self.active_collection.find({
                'match_id': {'$eq': id}
            })

    def update(self, id, match):
        """
        Aktualisiert ein Match

        Parameters:
        id (int): Match-ID des zu aktualisierende Matchs
        match (Match): Das neue Match
        """
        self.active_collection.replace_one({
            'match_id': {'$eq': id}
        }, match)

    def remove(self, id):
        """
        Löscht das Match.

        Parameters:
        id (int): Die id des zu löschende Matchs
        """
        self.active_collection.delete_one({
            'match_id': {'$eq': id}
        })
    
    def save_model(self, model, model_name):
        """
        Speichert das Model

        Parameters:
        model (Model): Zu speicherndes ML-Modell
        model_name (str): Name, unter dem das Modell gespeichert wird
            
        Returns:
        Model: Gespeicherte Model
        """
        model = self.active_collection.insert_one({
            'name': model_name,
            'model': model
        })
        return model
    
    def update_model(self, model, model_name):
        """
        Aktualisiert das übergebene Model

        Parameters:
        model (Model): Zu speicherndes ML-Modell
        model_name (str): Name, unter dem das Modell gespeichert wird

        Returns:
        Model: Aktualisierte Model
        """
        model = self.active_collection.replace_one({
            'name': {'$eq': model_name}
        }, {'name': model_name,
            'model': model})
        return model

def get_matches(start_id=0, num_matches=None) -> List[Dict]:
    """
    Funktion zum Holen von Matches aus der Datenbank.
    * params: 
        * start_id: Index für das erste Element relativ zur Datenbank
        * num_matches: Anzahl der zu holenden Matches
    * return: Liste mit Matches aus der Datenbank
    """
    retVal = []

    # Verbinden zu Datenbank
    db_con = DatabaseConnector()
    db_con.connect()
    
    # Alle Matches aus Datenbank holen
    db_matches = db_con.get()
    matches = [match for match in db_matches]

    db_con.disconnect()
    # Matches nach start_id und num_matches filtern
    if num_matches is None:
        # Ganze Liste zurückgeben
        retVal = matches
    else:
        # Liste abschneiden und zurückgeben
        retVal = matches[start_id:start_id+num_matches]

    return retVal

def save_model(model, model_name):
    """
    Funktion zum Speichern eines ML-Modells in der MongoDB.
    params: *model: zu speicherndes ML-Modell
            *model_name: Name, unter dem das Modell gespeichert wird
    """
    if model_name == "kda" or model_name == "no_kda":
        # Model in pickle laden
        pickled_model = pickle.dumps(model)
        db_con = DatabaseConnector(collection="Models")
        db_con.connect()
        # nach Model in Datenbank suchen und speichern/updaten
        models = list(db_con.get())
        if any(model['name'] == model_name for model in models):
            # Modell updaten
            db_con.update_model(pickled_model, model_name)
        else:
            # Modell speichern
            db_con.save_model(pickled_model, model_name)
        db_con.disconnect()
    else:
        raise NameError("Invalid model name {}.".format(model_name))

def load_model(model_name):
    """
    Funktion zum Laden eines ML-Modells aus der MongoDB.
    params: *model_name: Name des zu ladenden Models
    return: *pickled_model: Objekt des geladenen Modells
    """
    pickled_model = []
    db_con = DatabaseConnector(collection="Models")
    db_con.connect()
    models = list(db_con.get())
    try:
        # Model mit model_name aus Datenbank laden und zurückgeben
        pickled_model = [pickle.loads(model['model']) for model in models if model['name'] == model_name][0]
    except IndexError:
        raise NameError("Model with name {} not found in database".format(model_name))
    db_con.disconnect()
    return pickled_model