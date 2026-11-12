using Unity.VisualScripting;
using UnityEngine;

public class RoomSpawner : MonoBehaviour
{
    [Header("Room Settings")]
    [SerializeField] private GameObject normalRoomPrefab;
    [SerializeField] private GameObject startingRoomPrefab;
    [SerializeField] private GameObject treasureRoomPrefab;
    [SerializeField] private GameObject bossRoomPrefab;

    [SerializeField] private Transform roomParent;

    public void Spawn(DungeonLayout layout, Vector2 roomSpacing)
    {
        // null checks for roomPrefab
        if (layout == null)
        {
            Debug.LogError("Layout is NULL.");
            return;
        }

        // null checks for roomParent
        if (roomParent == null)
        {
            Debug.LogError("Room parent is not assigned.");
            return;
        }

        // Clear existing rooms
        foreach (Transform child in roomParent)
        {
            Destroy(child.gameObject);
        }

        // Loop through the rooms in the layout and instantiate them
        foreach (Vector2Int position in layout.Rooms)
        {
            RoomType roomType = layout.GetRoomType(position);

            GameObject prefab = GetPrefabForType(roomType);

            if (prefab == null)
            {
                Debug.LogError($"No prefab assigned for room type: {roomType}");
                continue;
            }

            // Calculate the world position based on the room's grid position and spacing
            Vector3 worldPosition = new Vector3(position.x * roomSpacing.x, position.y * roomSpacing.y, 0f);

            // Instantiate the room prefab at the calculated world position
            GameObject roomObject = Instantiate(prefab, worldPosition, Quaternion.identity, roomParent);

            // Get the RoomController component from the instantiated room
            RoomController controller = roomObject.GetComponent<RoomController>();

            // null check for RoomController component
            if (controller == null)
            {
                Debug.LogError($"Rooom at {position} is missing room controller!");
                continue;
            }

            // Initialize the room controller with the position and layout
            controller.Initialize(position, layout);
        }
    }

    // Returns the appropriate prefab based on the room type
    private GameObject GetPrefabForType(RoomType type)
    {
        // Set the default prefab to null initially
        GameObject selectedPrefab = null;

        // Determine the prefab based on the room type
        switch (type)
        {
            case RoomType.Starting:
                selectedPrefab = startingRoomPrefab;
                break;

            case RoomType.Treasure:
                selectedPrefab = treasureRoomPrefab;
                break;

            case RoomType.Boss:
                selectedPrefab = bossRoomPrefab;
                break;

            case RoomType.Normal:
                selectedPrefab = normalRoomPrefab;
                break;
        }

        // Return the selected prefab, defaulting to the normal room prefab if none is selected
        return selectedPrefab != null ? selectedPrefab : normalRoomPrefab;
    }
}
