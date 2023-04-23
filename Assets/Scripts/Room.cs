using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//room for generation
//All rooms have a size, number of entrances, radius
public class Room
{
    public Type roomType;
    public Vector2 center;
    public int radius;
    public List<Vector2> entrances;

    public Room(Vector2 center, int radius)
    {
        this.center = center;
        this.radius = radius;
    }

    public void GenerateEntrances(int entranceCount)
    {
        entrances = new List<Vector2>();

        for (int i = 0; i < entranceCount; i++)
        {
            float angle = i * (2 * Mathf.PI / entranceCount);
            Vector2 entrance = new Vector2(center.x + radius * Mathf.Cos(angle), center.y + radius * Mathf.Sin(angle));
            entrances.Add(entrance);
        }
    }

    public static float MinDistanceBetweenRooms(Vector2 center1, int radius1, Vector2 center2, int radius2)
    {
        return Vector2.Distance(center1, center2) - (radius1 + radius2);
    }

}

public enum Type { Large, Medium, Small }