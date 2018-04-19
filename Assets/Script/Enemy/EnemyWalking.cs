using System;
using System.Collections;
using UnityEngine;

namespace Script.Enemy
{
    [RequireComponent(typeof(Rigidbody))]
    public class EnemyWalking : MonoBehaviour
    {
        public Transform Destination;
        public float MoveSpeed = 6;

        private Rigidbody _rigidbody;
        private Vector3 _velocity;
        private bool _playerInSight;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _playerInSight = false;
        }

        private void Update()
        {
            // TODO: Player Interaction,
            // TODO: Think of logic if enemy reaches Destination (Game Master Script?)
            // TODO: Obsticle avoidence
            // TODO: Methods for checking (Coroutines)
            // TODO: What happens if the Enemy comes to anoter height?

            var destination = (Destination.position - transform.position).normalized;

            if (_playerInSight)
            {
                // Set the destination from the Player to the Destination of the Enemy
            }

            Debug.DrawLine(transform.position, transform.position + destination * 5);
            _velocity = new Vector3(destination.x * MoveSpeed, 0, destination.z * MoveSpeed);
        }

        private void FixedUpdate()
        {
            _rigidbody.MovePosition(_rigidbody.position + _velocity * Time.fixedDeltaTime);
        }
    }
}