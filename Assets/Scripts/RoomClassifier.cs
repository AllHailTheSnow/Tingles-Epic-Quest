using System.Collections.Generic;
using UnityEngine;

public static class RoomClassifier
{
    private static readonly Vector2Int[] Directions =
    {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
    };

    public static void AssignRoomTypes(DungeonLayout layout, int seed)
    {
        if (layout == null || layout.RoomCount == 0)
        {
            Debug.LogWarning("DungeonLayout is null or has no rooms. Cannot assign room types.");
            return;
        }

        // Set start position to (0, 0)
        Vector2Int startPosition = Vector2Int.zero;

        // Assign every room as Normal initially
        foreach (Vector2Int room in layout.Rooms)
        {
            layout.SetRoomType(room, RoomType.Normal);
        }

        // Identify the start room
        layout.SetRoomType(startPosition, RoomType.Starting);

        // Calculate distances from the start room to all other rooms
        Dictionary<Vector2Int, int> distances = CalculateDistances(layout, startPosition);

        // Sort rooms to make selection deterministic based on distance and then position
        List<Vector2Int> orderedRooms = new List<Vector2Int>(layout.Rooms);

        // Sort rooms first by distance from the start room, then by their coordinates for deterministic selection
        orderedRooms.Sort((a, b) =>
        {
            int xComparison = a.x.CompareTo(b.x);

            return xComparison != 0 ? xComparison : a.y.CompareTo(b.y); 
        });

        Vector2Int bossPosition = startPosition;
        int furthestDistance = -1;

        // Find the furthest room from the start position to assign as the boss room
        foreach (Vector2Int room in orderedRooms)
        {
            // Skip the start position
            if (room == startPosition)
            {
                continue;
            }

            // Check if the room has a calculated distance
            if (distances.TryGetValue(room, out int distance))
            {
                // Update the furthest room if this room is further away
                if (distance > furthestDistance)
                {
                    furthestDistance = distance;
                    bossPosition = room;
                }
            }
        }

        // Assign the boss room type if it's not the same as the start position
        if (bossPosition != startPosition)
        {
            layout.SetRoomType(bossPosition, RoomType.Boss);
        }

        // Identify potential treasure room candidates (rooms that are not the start or boss room)
        List<Vector2Int> treasureCandidates = new List<Vector2Int>();

        foreach (Vector2Int room in orderedRooms)
        {
            // Skip the start and boss positions
            if (room == startPosition || room == bossPosition)
            {
                continue;
            }

            // Check if the room has a calculated distance
            if (!distances.TryGetValue(room, out int distance))
            {
                continue;
            }

            // Only consider rooms that are at least 2 units away from the start room for treasure room candidates
            if (distance < 2)
            {
                continue;
            }

            // Check if the room has exactly one neighbor to be a candidate for a treasure room
            if (CountNeighbours(layout, room) == 1)
            {
                // Add the room to the list of treasure candidates
                treasureCandidates.Add(room);
            }
        }

        // If no treasure candidates were found, consider all rooms except the start and boss rooms
        if (treasureCandidates.Count == 0)
        {
            foreach (Vector2Int room in orderedRooms)
            {
                if (room != startPosition && room != bossPosition)
                {
                    treasureCandidates.Add(room);
                }
            }
        }

        // Randomly select a treasure room from the candidates if any exist
        if (treasureCandidates.Count > 0)
        {
            System.Random random = new System.Random(seed);

            int index = random.Next(treasureCandidates.Count);

            Vector2Int treasurePosition = treasureCandidates[index];

            layout.SetRoomType(treasurePosition, RoomType.Treasure);
        }
    }

    // Calculates the distance from the start position to all other rooms using a breadth-first search approach
    private static Dictionary<Vector2Int, int> CalculateDistances(DungeonLayout layout, Vector2Int startPosition)
    {
        // Dictionary to store the distance of each room from the start position
        Dictionary<Vector2Int, int> distances = new Dictionary<Vector2Int, int>();

        // Queue to facilitate the breadth-first search
        Queue<Vector2Int> queue = new Queue<Vector2Int>();

        // Initialize the distance for the start position and enqueue it for processing
        distances[startPosition] = 0;
        queue.Enqueue(startPosition);

        // Perform a breadth-first search to calculate distances to all reachable rooms
        while (queue.Count > 0)
        {
            // Dequeue the current room position for processing
            Vector2Int current = queue.Dequeue();

            // Explore all four cardinal directions to find neighboring rooms
            foreach (Vector2Int direction in Directions)
            {
                // Set the neighbor position based on the current position and direction
                Vector2Int neighbour = current + direction;

                /// Check if the neighbor room exists in the layout; if not, skip to the next direction
                if (!layout.ContainsRoom(neighbour))
                {
                    continue;
                }

                // If the neighbor has already been visited (i.e., has a recorded distance), skip it
                if (distances.ContainsKey(neighbour))
                {
                    continue; ;
                }

                // Record the distance for the neighbor room as one more than the current room's distance
                distances[neighbour] = distances[current] + 1;

                // Enqueue the neighbor room for further processing in the breadth-first search
                queue.Enqueue(neighbour);
            }
        }

        // Return the dictionary containing distances from the start position to all reachable rooms
        return distances;
    }

    // Counts the number of neighboring rooms for a given room position in the layout
    private static int CountNeighbours(DungeonLayout layout, Vector2Int position)
    {
        // Initialize a counter to keep track of the number of neighboring rooms
        int count = 0;

        // Iterate through all four cardinal directions to check for neighboring rooms
        foreach ( Vector2Int direction in Directions)
        {
            // If a neighboring room exists in the specified direction, increment the count
            if (layout.ContainsRoom(position + direction))
            {
                count++;
            }
        }

        // Return the total count of neighboring rooms
        return count;
    }
}
