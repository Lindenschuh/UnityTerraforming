using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSettings : MonoBehaviour {

    [Range(0,1)]
    public float point;
    public bool circle;
    [Range(0, 1)]
    public float outerCircle;
    [Range(0, 1)]
    public float innerCircle;
    [Range(0, 20)]
    public int size;
    [Range(0, 20)]
    public float distanceBetweenObjects;
    [Range(0, 50)]
    public float distanceBetweenLocations;
    public int differentLocations;
    public int amount;
    public GameObject preFab;

    private int minDistance = 0;
    private int maxDistance = 1;
    public float center;


	// Use this for initialization
	void Start () {
        // set point to value between 0 and 100
        if (point < minDistance)
            point = minDistance;
        else if (point > maxDistance)
            point = maxDistance;

        // set center of spawning
        if (circle)
            center = point;
        else
            center = 0;

        // set width of cicle
        if (outerCircle < minDistance)
            outerCircle = minDistance;
        else if (outerCircle > maxDistance)
            outerCircle = maxDistance;

        if (innerCircle < minDistance)
            innerCircle = minDistance;
        else if (innerCircle > maxDistance)
            innerCircle = maxDistance;
        else if (innerCircle > outerCircle)
            innerCircle = outerCircle - 0.01f;


        // set maxDistance between spawning objects
        if (distanceBetweenObjects < 1)
            distanceBetweenObjects = 1;
        else if (distanceBetweenObjects > maxDistance)
            distanceBetweenObjects = maxDistance;

        if (distanceBetweenLocations < 1)
            distanceBetweenLocations = 1;
        else if (distanceBetweenLocations > maxDistance)
            distanceBetweenLocations = maxDistance;

        if (differentLocations < 0)
            differentLocations = 0;
	}


}
