import pymongo
from os import environ
from typing import List, Dict

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
        self.client = pymongo.MongoClient(self.url, self.port)
        self.db = self.client[self.database]
        self.connection = self
        self.active_collection = self.connection.getCollection(self.collection)
        return self
    
    def disconnect(self):
        try:
            self.client.close()
            return self
        except AttributeError:
            raise ConnectionError("Client is not connected to any database.")
        
    
    def getCollectionNames(self):
        return self.db.list_collection_names()

    def getCollection(self, collection):
        return self.db[collection]
    
    def create(self, match):
        match = self.active_collection.insert_one(match)
        return match

    def get(self, id=None):
        if id == None:
            # Alle Einträge zurückgeben
            return self.active_collection.find()
        else:
            # Eintrag mit match_id <id> zurückgeben
            return self.active_collection.find({
                'match_id': {'$eq': id}
            })

    def update(self, id, match):
        self.active_collection.replace_one({
            'match_id': {'$eq': id}
        }, match)

    def remove(self, id):
        self.active_collection.delete_one({
            'match_id': {'$eq': id}
        })

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