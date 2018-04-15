using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCameraMovement : MonoBehaviour
{
    public float MovementSpeed;
    public float ScrollSpeed;

    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        Vector2 movement = new Vector2(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal")) * MovementSpeed * Time.deltaTime;
        float scroll = Input.mouseScrollDelta.y * ScrollSpeed * Time.deltaTime;
        transform.position = new Vector3(transform.position.x - movement.x, transform.position.y - scroll, transform.position.z + movement.y);
    }
}