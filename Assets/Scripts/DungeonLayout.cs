using System.Collections.Generic;
using UnityEngine;

public class DungeonLayout
{
    //Store all rooms in a hashset for fast lookup
    //Hashset used to prevent duplicate rooms being added
    private readonly HashSet<Vector2Int> rooms = new();

    //Store the type of each room in a dictionary for easy access
    private readonly Dictionary<Vector2Int, RoomType> roomTypes = new();

    //Exposes the rooms as a read-only collection to prevent external modification
    public IReadOnlyCollection<Vector2Int> Rooms => rooms;

    //Returns the number of rooms in the dungeon layout
    public int RoomCount => rooms.Count;

    // Checks if a room exists at the specified position
    public bool ContainsRoom(Vector2Int position)
    {
        return rooms.Contains(position);
    }

    // Attempts to add a room at the specified position returns true if the room was successfully added, false if a room already exists at that position
    public bool TryAddRoom(Vector2Int position)
    {
        return rooms.Add(position);
    }

    // Assigns a type to a room at the specified position if the room exists
    public void SetRoomType(Vector2Int position, RoomType type)
    {
        if (rooms.Contains(position))
        {
            roomTypes[position] = type;
        }
    }

    // Returns the type of a room at the specified position, defaulting to Normal if the room does not exist or has no assigned type
    public RoomType GetRoomType(Vector2Int position)
    {
        if (roomTypes.TryGetValue(position, out RoomType type))
        {
            return type;
        }

        return RoomType.Normal;
    }
}
