using System;
using UnityEngine;

[Serializable]
public struct Wheel
{
    public GameObject wheelModel;
    public WheelCollider wheelCollider;
    public Axel axel;
}