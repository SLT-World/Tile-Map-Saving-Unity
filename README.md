# Tile-Map-Saving-Unity
A project that might benefit someone who wants a tile based small-scale custom map system with the ability to save and load templates online.

It works like this, whenever a client places a road tile, the game will save the type (I use index as type) of the road tile, position and rotation to three lists.
When the client clicks on the Save button, the game will send a post request with the data of the three list and map name to an API that will save data received into a template in an json file.
When the clients loads a template, the game sends a get request to the API with the template name, the API will then search for a template with that name, Once the API finds the template, it will return the json data of the template back to the game.
Once the game receives the data, it will destroy the existing road tiles and replace the current template with the new template and instantiate all the road tile indexes.

Should be able to get a grasp of the functions of the API if you understand the explanation.
