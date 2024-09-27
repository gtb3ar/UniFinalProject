using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class TileController : MonoBehaviour {

    [SerializeField] private TileData[] tileData; // Getting the tile data for each of the tiles it will place
    [SerializeField] private TileData firstTile; // Making sure to place the first tile manually 
    [SerializeField] private TileData finalTile; // Making sure to place the first tile manually 
    [SerializeField] private GameObject Enviroment; //Sotring the envirmoneti ts placed in
    [SerializeField] private GameObject Encapsule;
    [SerializeField] private int MaxTiles; // The maximuum  number of tiles to be placed
    [SerializeField] private int MinTiles;// The minimum number of tiles to be placed
    private TileData lastTile; // The last tile to be placed
    private Vector3 spawnOrigin ; // The place where they should spawn from
    private Vector3 spawnlocalPosition; 
    private int tilesToSpawn; // An old value to have a specific number of tiles spawn
    private int tilesHaveSpawned = 0; // A track to know how many tiles have spwaned
    private GameObject[] tiles; // Holds the tiles

    private float x;
    private float y;

    private void Awake() { // Settings up origin
        float x = Encapsule.transform.position.x;
        Debug.Log(x);
        float y = Encapsule.transform.position.z;
        Debug.Log(y);
        spawnlocalPosition.x = x;
        spawnlocalPosition.y = y;
        
    
    }

    public void generateNewMap() { // Generate a new tile map 
        tilesToSpawn = Random.Range(MinTiles,MaxTiles); // randomises how many tiles are going to spawn between min and max

        // Finds all tiles that are currently on the map and deletes themn
        tiles = GameObject.FindGameObjectsWithTag("Tile"); 
        for (int i = 0; i < tiles.Length; i++) {
            Destroy(tiles[i]);
        }

        // Places the starting tile
        PlaceTile(firstTile);
        tilesHaveSpawned = tilesHaveSpawned + 1; // Incrementing value
        // Rnadomly selects the rest of the tiles
        for (int i = 0; i < tilesToSpawn; i++) {
            PickAndPlaceTile();
            tilesHaveSpawned++;
        }
        //PickAndPlaceTile();
        PlaceFinalTile();
        tilesHaveSpawned = -1;
        spawnlocalPosition = new Vector3(x, 0, y); // resets the spawn point
        Debug.Log(spawnlocalPosition);

        
    }

    TileData PickTile() { // Pick a tile corresponding to the previous tile
        List<TileData> permitedTiles = new List<TileData>();

        TileData.Cardinal nextRequiredCardinal = TileData.Cardinal.North;

        switch (lastTile.exitCardinal) {  // Reading the previous tiles exit cardinal, and inverts it to the new entry cardinal
            case TileData.Cardinal.North:
                nextRequiredCardinal = TileData.Cardinal.South;
                spawnlocalPosition = spawnlocalPosition + new Vector3(0, 0, lastTile.tileSize.y);
                break;
            case TileData.Cardinal.South:
                nextRequiredCardinal = TileData.Cardinal.North;
                spawnlocalPosition = spawnlocalPosition + new Vector3(0, 0, -lastTile.tileSize.y);
                break;
            case TileData.Cardinal.West:
                nextRequiredCardinal = TileData.Cardinal.East;
                spawnlocalPosition = spawnlocalPosition + new Vector3(-lastTile.tileSize.x, 0, 0);
                break;
            case TileData.Cardinal.East:
                nextRequiredCardinal = TileData.Cardinal.West;
                spawnlocalPosition = spawnlocalPosition + new Vector3(lastTile.tileSize.x, 0, 0);
                break;
            default: break;
        }

        // Collects the tiles that have the correct entry cardinal
        for (int i = 0; i < tileData.Length; i++) {
            if (tileData[i].entryCardinal == nextRequiredCardinal) {
                permitedTiles.Add(tileData[i]);
            }
        }
        // If its time to place the last tile, remove all tiles from the list that don;t have the final quality
        for (int i = 0; i < permitedTiles.Count; i++) {

            bool final = permitedTiles[i].getFinal();

            if (final != CheckFinal()) {
                permitedTiles.RemoveAt(i);
            }
        }
        //Radomly pick the tile form the list
        TileData pickedTile = permitedTiles[Random.Range(0, permitedTiles.Count)];
        // A bug fix as the tile preserved the last quality despite not having it
        /*if ((CheckFinal() && (pickedTile == tileData[5])) | (CheckFinal() && (pickedTile == tileData[4])) | (CheckFinal() && (pickedTile == tileData[6])) | (CheckFinal() && (pickedTile == tileData[8]))) {
            pickedTile = tileData[2];
        }*/
        // Return the tile to place
        return pickedTile;
    }

    void PickAndPlaceTile() { // Calls pick tile and then places it in the relevant space
        TileData tileToPlace = PickTile();
        GameObject objectFromTile = tileToPlace.tiles[Random.Range(0, tileToPlace.tiles.Length)];
        lastTile = tileToPlace;
        Instantiate(objectFromTile, spawnlocalPosition, Quaternion.identity, Enviroment.transform);
    }

    void PlaceTile(TileData selectedTile) { // Just places a tile
        GameObject objectFromTile = selectedTile.tiles[0];
        lastTile = selectedTile;
        Instantiate(objectFromTile, spawnlocalPosition, Quaternion.identity, Enviroment.transform);
    }

    void PlaceFinalTile() { // Just places a tile
        GameObject objectFromTile = finalTile.tiles[0];
        lastTile = finalTile;
        spawnlocalPosition = spawnlocalPosition + new Vector3(0, 0, lastTile.tileSize.y);
        Instantiate(objectFromTile, spawnlocalPosition, Quaternion.identity, Enviroment.transform);
    }

    public void UpdateSpawnOrigin(Vector3 originDelta) { 
        spawnOrigin = spawnlocalPosition + originDelta;
    }

    private bool CheckFinal() { // Checks if tile has the final quality
        if (tilesHaveSpawned-2 == tilesToSpawn) {
            return true;
        } else {
            return false;
        }

    }



}
