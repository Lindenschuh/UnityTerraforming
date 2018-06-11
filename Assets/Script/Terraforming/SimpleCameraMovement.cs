using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCameraMovement : MonoBehaviour
{
    public float MovementSpeed;
    public float ScrollSpeed;

    public float HeightRadius;

    private float _startHeight;

    public GodStateManager GodState;

    private void Start()
    {
        _startHeight = transform.position.y;
        Cursor.lockState = CursorLockMode.Confined;
    }

    // Update is called once per frame
    private void Update()
    {
        Vector2 movement = new Vector2(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal")) * MovementSpeed * Time.deltaTime;
        float scroll = Input.mouseScrollDelta.y * ScrollSpeed * Time.deltaTime;

        Vector2 ClampedMovement = GodState.BoundCenter.position.xz() + Vector2.ClampMagnitude(new Vector2(transform.position.x - movement.x, transform.position.z + movement.y) - GodState.BoundCenter.position.xz(), GodState.BoundRadius);
        transform.position = new Vector3(ClampedMovement.x,
            Mathf.Clamp(transform.position.y - scroll, _startHeight - HeightRadius, _startHeight + HeightRadius),
            ClampedMovement.y
            );
    }
}