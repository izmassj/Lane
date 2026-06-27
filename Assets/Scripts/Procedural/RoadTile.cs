using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "RoadTile", menuName = "ScriptableObject/RoadTile")]
public class RoadTile : ScriptableObject
{
    [Header("Rotation")]
    public int meshRotation;

    [Header("Model")]
    public GameObject prefab;

    [Header("Directions")]
    public RoadTileConnectionType _north;
    public RoadTileConnectionType _south;
    public RoadTileConnectionType _east;
    public RoadTileConnectionType _west;

    private Pair<RoadTileDirection ,RoadTileConnectionType> north = new Pair<RoadTileDirection, RoadTileConnectionType>(RoadTileDirection.North, RoadTileConnectionType.NonConnectable);
    private Pair<RoadTileDirection, RoadTileConnectionType> south = new Pair<RoadTileDirection, RoadTileConnectionType>(RoadTileDirection.South, RoadTileConnectionType.NonConnectable);
    private Pair<RoadTileDirection, RoadTileConnectionType> east = new Pair<RoadTileDirection, RoadTileConnectionType>(RoadTileDirection.East, RoadTileConnectionType.NonConnectable);
    private Pair<RoadTileDirection, RoadTileConnectionType> west = new Pair<RoadTileDirection, RoadTileConnectionType>(RoadTileDirection.West, RoadTileConnectionType.NonConnectable);

    private void OnValidate()
    {
        if (_north != north.Second)
        {
            north.Second = _north;
        }
        else if (_south != south.Second)
        {
            south.Second = _south;
        }
        else if (_east != east.Second)
        {
            east.Second = _east;
        }
        else if (_west != west.Second)
        {
            west.Second = _west;
        }
    }

    public Pair<RoadTileDirection, RoadTileConnectionType> GetDirection(RoadTileDirection rtd)
    {
        switch (rtd)
        {
            case RoadTileDirection.North:
                return north;

            case RoadTileDirection.South:
                return south;
                
            case RoadTileDirection.East:
                return east;

            case RoadTileDirection.West:
                return west;

        }

        return null;
    }
}
