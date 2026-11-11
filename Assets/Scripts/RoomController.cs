using UnityEngine;

public class RoomController : MonoBehaviour
{
    [Header("Room Settings")]
    [SerializeField] private Vector2Int gridPosition;

    [Header("Door References")]
    [SerializeField] private GameObject northDoor;
    [SerializeField] private GameObject southDoor;
    [SerializeField] private GameObject eastDoor;
    [SerializeField] private GameObject westDoor;

    private RoomType roomType;

    public RoomType Type => roomType;

    public Vector2Int GridPosition => gridPosition;

    public void Initialize(Vector2Int position, DungeonLayout layout)
    {
        // Set the grid position of the room
        gridPosition = position;

        // Get the room type from the layout
        roomType = layout.GetRoomType(position);

        // Check for neighboring rooms and activate/deactivate doors accordingly
        bool hasNorth = layout.ContainsRoom(position + Vector2Int.up);
        bool hasSouth = layout.ContainsRoom(position + Vector2Int.down);
        bool hasEast = layout.ContainsRoom(position + Vector2Int.right);
        bool hasWest = layout.ContainsRoom(position + Vector2Int.left);

        // Activate or deactivate doors based on the presence of neighboring rooms
        northDoor.SetActive(hasNorth);
        southDoor.SetActive(hasSouth);
        eastDoor.SetActive(hasEast);
        westDoor.SetActive(hasWest);
    }
}
