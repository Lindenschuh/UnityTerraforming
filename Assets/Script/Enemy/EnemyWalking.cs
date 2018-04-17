using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyWalking : MonoBehaviour
{
    public Transform Destination;
    public float moveSpeed = 6;

    private Rigidbody _rigidbody;
    private Vector3 _velocity;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    private void Update()
    {
        // Calculate Direction
        var heading = (Destination.position - transform.position).normalized;
        Debug.DrawLine(transform.position, transform.position + heading * 10);
        // Walk Times Enemy Walking Speed
        // Walk
    }

    private void FixedUpdate()
    {
        var horiz = Input.GetAxis("Horizontal");
        var vert = Input.GetAxis("Vertical");

        _velocity = new Vector3(horiz, 0, vert) * moveSpeed;

        _rigidbody.MovePosition(_rigidbody.position + _velocity * Time.fixedDeltaTime);
    }
}