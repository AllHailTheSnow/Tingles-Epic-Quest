using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    [Header("Generation Settings")]
    [SerializeField, Min(1)] private int targetRoomCount = 15;
    [SerializeField] private int seed = 12345;
    [SerializeField] private bool randomizeSeed = true;

    [Header("Debug Visualtisation")]
    [SerializeField] private Vector2 roomSpacing = new Vector2(20f, 12f);

    private DungeonLayout layout;
    public DungeonLayout Layout => layout;
    public int CurrentSeed { get; private set; }
    private static readonly Vector2Int[] Directions =
    {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
    };

    [ContextMenu("Generate Dungeon")]
    public void GenerateDungeon()
    {
        CurrentSeed = randomizeSeed
            ? Environment.TickCount
            : seed;

        System.Random random = new System.Random(CurrentSeed);

        layout = new DungeonLayout();

        //Create new starting room
        Vector2Int startPosition = Vector2Int.zero;
        layout.TryAddRoom(startPosition);

        //Store all available positions
        List<Vector2Int> frontier = new List<Vector2Int>();
        HashSet<Vector2Int> frontierSet = new HashSet<Vector2Int>();

        AddNeighbours(startPosition, frontier, frontierSet);

        //Generate remaining rooms
        while (layout.RoomCount < targetRoomCount && frontier.Count > 0)
        {
            int randonIndex = random.Next(frontier.Count);

            Vector2Int newPosiion = frontier[randonIndex];

            frontier.RemoveAt(randonIndex);
            frontierSet.Add(newPosiion);

            if (layout.TryAddRoom(newPosiion))
            {
                AddNeighbours(newPosiion, frontier, frontierSet);
            }
        }

        Debug.Log($"Dungeon generated with {layout.RoomCount} rooms using seed {CurrentSeed}");

#if UNITY_EDITOR
        UnityEditor.SceneView.RepaintAll();
#endif

    }

    private void AddNeighbours(Vector2Int position, List<Vector2Int> frontier, HashSet<Vector2Int> frontierSet)
    {
        foreach (Vector2Int direction in Directions)
        {
            Vector2Int neighbour = position + direction;

            if (layout.ContainsRoom(neighbour))
                continue;

            if (frontierSet.Add(neighbour))
            {
                frontier.Add(neighbour);
            }
        }
    }

    private void OnValidate()
    {
        targetRoomCount = Mathf.Max(1, targetRoomCount);

        roomSpacing.x = Mathf.Max(1f, roomSpacing.x);
        roomSpacing.y = Mathf.Max(1f, roomSpacing.y);
    }

    private void OnDrawGizmos()
    {
        if (layout == null)
            return;

        foreach (Vector2Int room in layout.Rooms)
        {
            Vector3 position = new Vector3(room.x * roomSpacing.x, room.y * roomSpacing.y, 0);

            Gizmos.color = room == Vector2Int.zero ? Color.green : Color.cyan;

            Gizmos.DrawWireCube(position, new Vector3(roomSpacing.x * 0.9f, roomSpacing.y * 0.9f, 0.1f));

            Gizmos.DrawSphere(position, 0.25f);
        }
    }
}
