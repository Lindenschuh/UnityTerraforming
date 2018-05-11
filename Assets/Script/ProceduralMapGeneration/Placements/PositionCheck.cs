using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionCheck : MonoBehaviour {

    public bool isPositionAvailable;
    public bool isColliding { get; set; }
    public Vector3 randomPosition { get; set; }
    public Quaternion normalizedRotation { get; set; }

    public PositionCheck() { }

    public void ObjectIsColliding()
    {
        this.isColliding = true;
        this.isPositionAvailable = false;
    }

    public void PossitionAllowed()
    {
        this.isColliding = false;
        this.isPositionAvailable = true;
    }

    public void Reset()
    {
        isColliding = false;

        randomPosition = Vector3.zero;
        normalizedRotation = Quaternion.identity;
    }
}
