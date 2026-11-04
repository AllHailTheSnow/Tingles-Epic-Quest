using System.Collections.Generic;
using UnityEngine;

public class DungeonLayout
{
    private readonly HashSet<Vector2Int> rooms = new();

    public IReadOnlyCollection<Vector2Int> Rooms => rooms;

    public int RoomCount => rooms.Count;

    public bool ContainsRoom(Vector2Int position)
    {
        return rooms.Contains(position);
    }

    public bool TryAddRoom(Vector2Int position)
    {
        return rooms.Add(position);
    }
}
