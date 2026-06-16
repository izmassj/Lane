using UnityEngine;

[CreateAssetMenu(fileName = "RoadTile", menuName = "ScriptableObject/RoadTile")]
public class RoadTile : ScriptableObject
{
    [Header("Rotation")]
    public int meshRotation;

    [Header("Model")]
    public GameObject prefab;

    [Header("Directions")]
    public RoadTileConnectionType north;
    public RoadTileConnectionType south;
    public RoadTileConnectionType east;
    public RoadTileConnectionType west;

}
