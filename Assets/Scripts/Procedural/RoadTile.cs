using UnityEngine;

[CreateAssetMenu(fileName = "RoadTile", menuName = "ScriptableObject/RoadTile")]
public class RoadTile : ScriptableObject
{
    public int meshRotation;
    public GameObject prefab;
    public RoadTileConnectionType north;
    public RoadTileConnectionType south;
    public RoadTileConnectionType east;
    public RoadTileConnectionType west;
}
