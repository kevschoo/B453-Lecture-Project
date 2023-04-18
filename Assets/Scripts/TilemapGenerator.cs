using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapGenerator : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Tilemap wallTilemap;
    [SerializeField] private TileBase wallTile;
    [SerializeField] private TileBase floorTile;
    [SerializeField] private BaseSpawner ObjectSpawner;

    [SerializeField] private Vector2Int startPosition;
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private int wallAreaWidth;
    [SerializeField] private int wallAreaHeight;

    [SerializeField] private int numLargeRooms;
    [SerializeField] private int numMediumRooms;
    [SerializeField] private int numSmallRooms;
    [SerializeField] private int maxConnections;
    [SerializeField] private List<Room> rooms;
    
    private void Start()
    {
        Generate();
        FillWithWalls();
        ObjectSpawner.GenerateObjects();
    }

    public Vector2Int GetStartPosition()
    {
        return startPosition;
    }

    public int GetWidth()
    {
        return width;
    }

    public int GetHeight()
    {
        return height;
    }

    public void Generate()
    {
        tilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
        rooms = GenerateRooms();
        SpawnAndConnectRooms(rooms);
        FillWithWalls();
    }
    
    public List<Room> GetRoomList()
    {
        return rooms;
    }

    private List<Room> GenerateRooms()
    {
        List<Room> rooms = new List<Room>();

        for (int i = 0; i < numLargeRooms; i++)
        {
            rooms.Add(new Room(Random.Range(4, 7), Room.Type.Large));
        }
        for (int i = 0; i < numMediumRooms; i++)
        {
            rooms.Add(new Room(Random.Range(3, 5), Room.Type.Medium));
        }
        for (int i = 0; i < numSmallRooms; i++)
        {
            rooms.Add(new Room(Random.Range(1, 4), Room.Type.Small));
        }

        return rooms;
    }

    private void FillWithWalls()
    {
        int extraWidth = (wallAreaWidth - width) / 2;
        int extraHeight = (wallAreaHeight - height) / 2;
        for (int x = startPosition.x - extraWidth ; x < startPosition.x + wallAreaWidth - extraWidth; x++)
        {
            for (int y = startPosition.y - extraHeight; y < startPosition.y + wallAreaHeight - extraHeight; y++)
            {
                Vector3Int tilePosition = new Vector3Int(x, y, 0);

                if (tilemap.GetTile(tilePosition) == null)
                {
                    wallTilemap.SetTile(tilePosition, wallTile);
                }
                else
                {
                    wallTilemap.SetTile(tilePosition, null);
                }
            }
        }
    }


    private void SpawnAndConnectRooms(List<Room> rooms)
    {
        // Place rooms
        foreach (var room in rooms)
        {
            bool placed = false;

            while (!placed)
            {
                room.position = new Vector2Int(Random.Range(startPosition.x, startPosition.x + width - room.radius * 2),
                                               Random.Range(startPosition.y, startPosition.y + height - room.radius * 2));
                placed = PlaceRoom(room);
            }
        }

        // Connect rooms
        for (int i = 1; i < rooms.Count; i++)
        {
            Room roomA = rooms[i - 1];
            Room roomB = rooms[i];
            int corridorWidth = GetCorridorWidth(roomA);
            ConnectRooms(roomA, roomB, corridorWidth);

            int additionalConnections = Random.Range(0, maxConnections + 1);
            for (int j = 0; j < additionalConnections; j++)
            {
                int randomRoomIndex = Random.Range(0, rooms.Count);
                Room randomRoom = rooms[randomRoomIndex];

                if (randomRoom != roomA && randomRoom != roomB)
                {
                    corridorWidth = GetCorridorWidth(randomRoom);
                    ConnectRooms(roomA, randomRoom, corridorWidth);
                }
            }
        }
    }

    private bool PlaceRoom(Room room)
    {
        for (int x = -room.radius; x < room.radius; x++)
        {
            for (int y = -room.radius; y < room.radius; y++)
            {
                if (Vector2.Distance(Vector2.zero, new Vector2(x, y)) > room.radius) continue;

                Vector3Int tilePosition = (Vector3Int)(room.position + new Vector2Int(x, y));

                if (tilemap.GetTile(tilePosition) != null)
                {
                    return false;
                }
            }
        }

        for (int x = -room.radius; x < room.radius; x++)
        {
            for (int y = -room.radius; y < room.radius; y++)
            {
                if (Vector2.Distance(Vector2.zero, new Vector2(x, y)) > room.radius) continue;

                Vector3Int tilePosition = (Vector3Int)(room.position + new Vector2Int(x, y));
                tilemap.SetTile(tilePosition, floorTile);
            }
        }

        return true;
    }

    private int GetCorridorWidth(Room room)
    {
        switch (room.roomType)
        {
            case Room.Type.Large:
                return Random.Range(3, 5);
            case Room.Type.Medium:
                return Random.Range(2, 4);
            case Room.Type.Small:
                return Random.Range(1, 3);
            default:
                return 1;
        }
    }

    private void ConnectRooms(Room roomA, Room roomB, int corridorWidth)
    {
        Vector2 currentPosition = roomA.position;
        Vector2 targetPosition = roomB.position;

        while (Vector2.Distance(currentPosition, targetPosition) > corridorWidth)
        {
            currentPosition = Vector2.MoveTowards(currentPosition, targetPosition, 1);
            Vector2Int currentPositionInt = new Vector2Int(Mathf.RoundToInt(currentPosition.x), Mathf.RoundToInt(currentPosition.y));

            for (int x = -corridorWidth / 2; x <= corridorWidth / 2; x++)
            {
                for (int y = -corridorWidth / 2; y <= corridorWidth / 2; y++)
                {
                    Vector3Int tilePosition = (Vector3Int)(currentPositionInt + new Vector2Int(x, y));
                    tilemap.SetTile(tilePosition, floorTile);
                }
            }
        }
    }

}

public class Room
{
    public enum Type { Large, Medium, Small }

    public int radius;
    public Type roomType;
    public Vector2Int position;

    public Room(int radius, Type roomType)
    {
        this.radius = radius;
        this.roomType = roomType;
    }
}