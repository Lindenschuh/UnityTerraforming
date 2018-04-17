using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyWalking : MonoBehaviour
{
    public Transform destination;
    public float moveSpeed = 6;

    private Rigidbody rigidbody;
    private Vector3 velocity;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    private void Update()
    {
        // Calculate Direction
        var heading = destination.position - transform.position;
        var distance = heading.magnitude;
        var direction = heading / distance;
        Debug.DrawLine(transform.position, destination.position);
        // Walk Times Enemy Walking Speed
        // Walk
    }

    private void FixedUpdate()
    {
        var horiz = Input.GetAxis("Horizontal");
        var vert = Input.GetAxis("Vertical");

        velocity = new Vector3(horiz, 0, vert) * moveSpeed;

        rigidbody.MovePosition(rigidbody.position + velocity * Time.fixedDeltaTime);
    }
}