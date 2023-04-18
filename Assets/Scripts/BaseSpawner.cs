using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BaseSpawner : MonoBehaviour
{
    [SerializeField] private TilemapGenerator tilemapGenerator;
    [SerializeField] private Tilemap floorTilemap;
    [SerializeField] private List<GameObject> largeObjects;
    [SerializeField] private List<GameObject> mediumObjects;
    [SerializeField] private List<GameObject> smallObjects;
    [SerializeField] private List<GameObject> randomSmallObjects;
    [SerializeField] private int randomSpawnCount;

    private HashSet<Vector3Int> occupiedPositions = new HashSet<Vector3Int>();

    public void GenerateObjects()
    {
        List<Room> rooms = tilemapGenerator.GetRoomList();
        SpawnObjectsInRooms(rooms);
        SpawnRandomObjects();
    }

    private void SpawnObjectsInRooms(List<Room> rooms)
{
    foreach (Room room in rooms)
    {
        Vector3Int roomCenter = new Vector3Int(room.position.x + room.radius / 2, room.position.y + room.radius / 2, 0);

        if (!occupiedPositions.Contains(roomCenter))
        {
            GameObject prefabToSpawn = null;

            if (room.radius >= 4)
            {
                prefabToSpawn = GetRandomObjectFromList(largeObjects);
            }
            else if (room.radius >= 3)
            {
                prefabToSpawn = GetRandomObjectFromList(mediumObjects);
            }
            else
            {
                prefabToSpawn = GetRandomObjectFromList(smallObjects);
            }

            if (prefabToSpawn != null)
            {
                Instantiate(prefabToSpawn, roomCenter, Quaternion.identity);
                occupiedPositions.Add(roomCenter);
            }
        }
    }
}

    private GameObject GetRandomObjectFromList(List<GameObject> objects)
    {
        if (objects.Count == 0)
        {
            return null;
        }

        int randomIndex = Random.Range(0, objects.Count);
        GameObject selectedObject = objects[randomIndex];

        objects.RemoveAt(randomIndex); // Remove the selected object from the list

        return selectedObject;
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
            int x = Random.Range(tilemapGenerator.GetStartPosition().x, tilemapGenerator.GetStartPosition().x + tilemapGenerator.GetWidth());
            int y = Random.Range(tilemapGenerator.GetStartPosition().y, tilemapGenerator.GetStartPosition().y + tilemapGenerator.GetHeight());
            randomPosition = new Vector3Int(x, y, 0);

            if (floorTilemap.GetTile(randomPosition) != null)
            {
                return randomPosition;
            }

            attempts++;
        }

        return Vector3Int.zero;
    }
}