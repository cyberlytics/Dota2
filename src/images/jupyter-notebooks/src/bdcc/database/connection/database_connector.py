import pymongo
from os import environ

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
        self.client.close()
        return self
    
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