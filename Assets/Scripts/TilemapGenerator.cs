using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using System;

//Bruhg using system just to make a unity event makes my random need unityengine.random :(
    
public class TileMapGenerator : MonoBehaviour
{
    [SerializeField] private Tilemap floorTilemap;
    [SerializeField] private Tilemap wallTilemap;
    [SerializeField] private TileBase wallTile;
    [SerializeField] private TileBase floorTile;
    [SerializeField] private TileBase hardWallTile;

    [SerializeField] private int largeRoomCount;
    [SerializeField] private int mediumRoomCount;
    [SerializeField] private int smallRoomCount;

    [SerializeField] private int largeRadiusMin = 7;
    [SerializeField] private int largeRadiusMax = 8;
    [SerializeField] private int mediumRadiusMin = 5;
    [SerializeField] private int mediumRadiusMax = 6;
    [SerializeField] private int smallRadiusMin = 3;
    [SerializeField] private int smallRadiusMax = 4;

    [SerializeField] private int entranceMin = 1;
    [SerializeField] private int entranceMax = 4;

    [SerializeField] private int minPathWidth = 1;
    [SerializeField] private int maxPathWidth = 3;

    public int areaWidth = 100;
    public int areaHeight = 100;
    [SerializeField] private int borderSize = 5;
    public Vector2 areaCenter = Vector2.zero;

    [SerializeField] private float minimumRoomDistance;
    public int seed;
    public bool UseSeed = true;
    public bool generationSuccessful = true;
    public event Action OnGenerationCompleted;

    public List<Room> rooms;

    void Start()
    {
        StartGeneration();
        
    }

    void StartGeneration()
    {
        if(UseSeed){UnityEngine.Random.InitState(seed);}
        else
        {
            int rSeed = UnityEngine.Random.Range(0, 1000000);
            Debug.Log("RS: " + rSeed );
            UnityEngine.Random.InitState(rSeed);
        }
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();

        GenerateRooms();
        //check generation function
        //id make one and put it here but im being really bad at finding simple solutions to solve this
        //
        if(generationSuccessful)
        {
            GenerateWalls();
            FillRemainingAreaWithWalls();

            OnGenerationCompleted?.Invoke();
            Debug.Log("Spawn objects now"); 
        }
        else
        {
            Debug.Log("Gen had an error! Unconnected rooms!");
        }
    }

    void GenerateRooms()
    {
        rooms = new List<Room>();
        GenerateRoomsOfType(Type.Large, largeRoomCount);
        GenerateRoomsOfType(Type.Medium, mediumRoomCount);
        GenerateRoomsOfType(Type.Small, smallRoomCount);
        ConnectRooms();
        
    }

    void GenerateRoomsOfType(Type type, int count)
    {
        int radiusMin, radiusMax;
        switch (type)
        {
            case Type.Large:
                radiusMin = largeRadiusMin;
                radiusMax = largeRadiusMax;
                break;

            case Type.Medium:
                radiusMin = mediumRadiusMin;
                radiusMax = mediumRadiusMax;
                break;

            default:
                radiusMin = smallRadiusMin;
                radiusMax = smallRadiusMax;
                break;
        }

        for (int i = 0; i < count; i++)
        {
            Vector2 center = GetRandomRoomCenter();
            int radius = UnityEngine.Random.Range(radiusMin, radiusMax + 1);
            Room room = new Room(center, radius);
            room.roomType = type;

            if (!RoomOverlapsOtherRooms(room))
            {
                int entranceCount = UnityEngine.Random.Range(entranceMin, entranceMax);
                room.GenerateEntrances(entranceCount);
                rooms.Add(room);
                FillRoom(room);
            }
        }
    }

    Vector2 GetRandomRoomCenter()
    {
        float x = UnityEngine.Random.Range(areaCenter.x - areaWidth / 2, areaCenter.x + areaWidth / 2);
        float y = UnityEngine.Random.Range(areaCenter.y - areaHeight / 2, areaCenter.y + areaHeight / 2);
        return new Vector2(x, y);
    }

    bool RoomOverlapsOtherRooms(Room room)
    {
        foreach (Room otherRoom in rooms)
        {
            if (Room.MinDistanceBetweenRooms(room.center, room.radius, otherRoom.center, otherRoom.radius) < minimumRoomDistance)
            {
                return true;
            }
        }
        return false;
    }


    void FillPath(Vector2Int center, int width)
    {
        int halfWidth = width / 2;

        for (int x = center.x - halfWidth; x <= center.x + halfWidth; x++)
        {
            for (int y = center.y - halfWidth; y <= center.y + halfWidth; y++)
            {
                floorTilemap.SetTile(new Vector3Int(x, y, 0), floorTile);
            }
        }
    }

    void FillRoom(Room room)
    {
        for (int x = (int)(room.center.x - room.radius); x <= room.center.x + room.radius; x++)
        {
            for (int y = (int)(room.center.y - room.radius); y <= room.center.y + room.radius; y++)
            {
                Vector2Int position2D = new Vector2Int(x, y);
                Vector3Int position3D = new Vector3Int(position2D.x, position2D.y, 0);

                if (Vector2.Distance(position2D, room.center) <= room.radius)
                {
                    floorTilemap.SetTile(position3D, floorTile);
                }
            }
        }
    }


    void ConnectRooms()
    {
        for (int i = 0; i < rooms.Count; i++)
        {
            Room room = rooms[i];
            ConnectEntrancesToNearbyRooms(room);
        }
    }


    void ConnectEntrancesToNearbyRooms(Room room)
    {
        List<Room> connectedRooms = new List<Room>();

        foreach (Vector2 entrance in room.entrances)
        {
            Room nearestRoom = GetNearestRoom(room, connectedRooms);

            if (nearestRoom != null)
            {
                Vector2 nearestEntrance = nearestRoom.entrances[UnityEngine.Random.Range(0, nearestRoom.entrances.Count)];
                ConnectEntrances(entrance, nearestEntrance);
                connectedRooms.Add(nearestRoom);
            }
        }
    }


    void ConnectEntrances(Vector2 entrance1, Vector2 entrance2)
    {
        float distanceFactor = Vector2.Distance(entrance1, entrance2) / (areaWidth + areaHeight);
        int pathWidth = Mathf.RoundToInt(Mathf.Lerp(minPathWidth, maxPathWidth, distanceFactor));
        Vector2Int startPos = new Vector2Int(Mathf.RoundToInt(entrance1.x), Mathf.RoundToInt(entrance1.y));
        Vector2Int endPos = new Vector2Int(Mathf.RoundToInt(entrance2.x), Mathf.RoundToInt(entrance2.y));

        Vector2Int currentPos = startPos;

        while (currentPos != endPos)
        {
            if (Mathf.Abs(currentPos.x - endPos.x) > Mathf.Abs(currentPos.y - endPos.y))
            {
                currentPos.x += (currentPos.x < endPos.x) ? 1 : -1;
            }
            else
            {
                currentPos.y += (currentPos.y < endPos.y) ? 1 : -1;
            }

            FillPath(currentPos, pathWidth);
        }
    }


    Room GetNearestRoom(Room currentRoom, List<Room> excludeRooms)
    {
        Room nearestRoom = null;
        float minDistance = float.MaxValue;

        foreach (Room room in rooms)
        {
            if (room == currentRoom || excludeRooms.Contains(room)) 
            {continue;}

            float distance = Vector2.Distance(currentRoom.center, room.center);

            if (distance < minDistance)
            {
                minDistance = distance;
                nearestRoom = room;
            }
        }

        return nearestRoom;
    }



    void GenerateWalls()
    {
        BoundsInt bounds = floorTilemap.cellBounds;

        //fix bounding box issue with addition of border to stop wall cropping
        for (int x = bounds.xMin-borderSize; x <= bounds.xMax+borderSize; x++)
        {
            for (int y = bounds.yMin-borderSize; y <= bounds.yMax+borderSize; y++)
            {
                Vector3Int position = new Vector3Int(x, y, 0);
                TileBase tile = floorTilemap.GetTile(position);

                if (tile == null)
                {
                    if (HasAdjacentFloorTile(position))
                    {
                        wallTilemap.SetTile(position, wallTile);
                    }
                }
            }
        }
    }

    bool HasAdjacentFloorTile(Vector3Int position)
    {
        Vector3Int[] neighbors = 
        {
            new Vector3Int(position.x + 1, position.y, position.z),
            new Vector3Int(position.x - 1, position.y, position.z),
            new Vector3Int(position.x, position.y + 1, position.z),
            new Vector3Int(position.x, position.y - 1, position.z)
        };

        foreach (Vector3Int neighbor in neighbors)
        {
            if (floorTilemap.GetTile(neighbor) != null)
            {
                return true;
            }
        }

        return false;
    }

    void FillRemainingAreaWithWalls()
    {
        int extendedAreaWidth = areaWidth + borderSize * 2;
        int extendedAreaHeight = areaHeight + borderSize * 2;
        Vector3Int topLeftCorner = new Vector3Int((int)areaCenter.x - areaWidth / 2 - borderSize, (int)areaCenter.y - areaHeight / 2 - borderSize, 0);

        for (int x = 0; x < extendedAreaWidth; x++)
        {
            for (int y = 0; y < extendedAreaHeight; y++)
            {
                Vector3Int position = new Vector3Int(topLeftCorner.x + x, topLeftCorner.y + y, 0);
                TileBase floorTileAtPosition = floorTilemap.GetTile(position);
                TileBase wallTileAtPosition = wallTilemap.GetTile(position);

                if (floorTileAtPosition == null && wallTileAtPosition == null)
                {
                    wallTilemap.SetTile(position, hardWallTile);
                }
            }
        }
    }



}