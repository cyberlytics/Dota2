"""
    Interface between python/ai and backend for data exchange.
"""
import os
import requests

def request_match_data(request_type: str, id=0) -> str:
    """ Function to request match data from backend. 
        * Params:
            * request_type: type of request to be done.
                - "all": request all matches from database
                - "parsed": request all parsed matches from database
                - "id": request match with specific id
            * id: match id
        * Return: string of the requested data
    """
    data = ""
    # Data request from backend
    if request_type == "all": # Request all available matches from database
        data = requests.get("http://localhost/api/Matches/all")
    elif request_type == "parsed": # Request all parsed matches from database
        data = requests.get("http://localhost/api/Matches/parsed")
    elif request_type == "id": # Request a match with a certain ID
        data = requests.get("http://localhost/api/Matches/request?id={}".format(id))
    # Convert bytes object to string
    data = data.content.decode("utf-8")
    return data


def start_training():
    """ Function to start the training process of the ai model. """
    pass