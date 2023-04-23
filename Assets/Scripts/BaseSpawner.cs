using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BaseSpawner : MonoBehaviour
{
    [SerializeField] private TileMapGenerator tilemapGenerator;
    [SerializeField] private Tilemap floorTilemap;
    [SerializeField] private List<GameObject> largeObjects;
    [SerializeField] private List<GameObject> mediumObjects;
    [SerializeField] private List<GameObject> smallObjects;
    [SerializeField] private List<GameObject> randomSmallObjects;
    [SerializeField] private int randomSpawnCount;

    private HashSet<Vector3Int> occupiedPositions = new HashSet<Vector3Int>();

    
    void OnEnable()
    {
        tilemapGenerator.OnGenerationCompleted += SpawnObjects;
    }

    //I was gonna extract the spawning code out and clean it up but now im too lazy.
    //was having spawning issues with extracted functions and large objects needing to always be spawned
    //simplier to just force check if theres any rooms and downsizing bases into smaller rooms
    private void SpawnObjects()
    {
        Debug.Log("Spawning large objects now");
        foreach (Room room in tilemapGenerator.rooms)
        {
            if(room.roomType != Type.Large)
            {continue;}

            GameObject objToSpawn = null;
            if (largeObjects.Count > 0)
            {
                objToSpawn = largeObjects[0];
                largeObjects.RemoveAt(0);
            }
            if (objToSpawn != null)
            {
                Vector3Int spawnPosition = new Vector3Int(Mathf.RoundToInt(room.center.x), Mathf.RoundToInt(room.center.y), 0);
                if (!occupiedPositions.Contains(spawnPosition))
                {
                    Instantiate(objToSpawn, floorTilemap.CellToWorld(spawnPosition), Quaternion.identity);
                    occupiedPositions.Add(spawnPosition);
                }
            }
        }
        Debug.Log("Spawning med objects now");
        foreach (Room room in tilemapGenerator.rooms)
        {
            if(room.roomType != Type.Medium)
            {continue;}

            GameObject objToSpawn = null;
            if (largeObjects.Count > 0)
            {
                objToSpawn = largeObjects[0];
                largeObjects.RemoveAt(0);
            }
            else if (mediumObjects.Count > 0)
            {
                objToSpawn = mediumObjects[0];
                mediumObjects.RemoveAt(0);
            }
            if (objToSpawn != null)
            {
                Vector3Int spawnPosition = new Vector3Int(Mathf.RoundToInt(room.center.x), Mathf.RoundToInt(room.center.y), 0);
                if (!occupiedPositions.Contains(spawnPosition))
                {
                    Instantiate(objToSpawn, floorTilemap.CellToWorld(spawnPosition), Quaternion.identity);
                    occupiedPositions.Add(spawnPosition);
                }
            }
        }
        
        Debug.Log("Spawning small objects now");
        foreach (Room room in tilemapGenerator.rooms)
        {
            if(room.roomType != Type.Small)
            {continue;}

            GameObject objToSpawn = null;
            if (largeObjects.Count > 0)
            {
                objToSpawn = largeObjects[0];
                largeObjects.RemoveAt(0);
            }
            else if (mediumObjects.Count > 0)
            {
                objToSpawn = mediumObjects[0];
                mediumObjects.RemoveAt(0);
            }
            else if (smallObjects.Count > 0)
            {
                objToSpawn = smallObjects[0];
                smallObjects.RemoveAt(0);
            }
            
            if (objToSpawn != null)
            {
                Vector3Int spawnPosition = new Vector3Int(Mathf.RoundToInt(room.center.x), Mathf.RoundToInt(room.center.y), 0);
                if (!occupiedPositions.Contains(spawnPosition))
                {
                    Instantiate(objToSpawn, floorTilemap.CellToWorld(spawnPosition), Quaternion.identity);
                    occupiedPositions.Add(spawnPosition);
                }
            }
        }
        
        Debug.Log("Spawning random objects now");
        SpawnRandomObjects();
    }

    void OnDestroy()
    {
        tilemapGenerator.OnGenerationCompleted -= SpawnObjects;
    }

    private GameObject GetRandomSmallObjectFromList(List<GameObject> objects)
    {
        if (objects.Count == 0)
        {
            return null;
        }

        int randomIndex = Random.Range(0, objects.Count);
        return objects[randomIndex];
    }

    private void SpawnRandomObjects()
    {
        for (int i = 0; i < randomSpawnCount; i++)
        {
            Vector3Int randomPosition = GetRandomFloorPosition();
            if (randomPosition != Vector3Int.zero && !occupiedPositions.Contains(randomPosition))
            {
                GameObject prefabToSpawn = GetRandomSmallObjectFromList(randomSmallObjects);
                if (prefabToSpawn != null)
                {
                    Instantiate(prefabToSpawn, randomPosition, Quaternion.identity);
                    occupiedPositions.Add(randomPosition);
                }
            }
        }
    }

    private Vector3Int GetRandomFloorPosition()
    {
        int attempts = 0;
        Vector3Int randomPosition = Vector3Int.zero;

        while (attempts < 1000)
        {
            int x = Random.Range((int)tilemapGenerator.areaCenter.x - tilemapGenerator.areaWidth, (int)tilemapGenerator.areaCenter.x + tilemapGenerator.areaWidth);
            int y = Random.Range((int)tilemapGenerator.areaCenter.y - tilemapGenerator.areaHeight, (int)tilemapGenerator.areaCenter.y + tilemapGenerator.areaHeight);
            randomPosition = new Vector3Int(x, y, 0);

            if (floorTilemap.GetTile(randomPosition) != null && !occupiedPositions.Contains(randomPosition))
            {
                return randomPosition;
            }


            attempts++;
        }

        return Vector3Int.zero;
    }
}