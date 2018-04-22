using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class GameEntity : MonoBehaviour
{
    public float Mass { get { return _rigidbody.mass; } set { _rigidbody.mass = value; } }
    public float MaxVelocity;
    public float MaxForce;
    public float SlowingRadius;
    public GameEntity Destination;
    public Spawner Spawner;
    public float LookRadius;
    public float AvoidanceForce;

    protected Rigidbody _rigidbody;
    protected Vector3 _velocity;
    protected SteeringBehaviour _steeringBehaviour;
    protected Vector3 _avoidanceForce;

    public Vector3 Velocity { get { return _velocity; } private set { _velocity = value; } }

    public void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _steeringBehaviour = new SteeringBehaviour(this);
        _avoidanceForce = new Vector3();
    }

    private void FixedUpdate()
    {
        ObserveSourroundings();
        CalculatePath();
        var steering = _steeringBehaviour.UpdateSteering();
        _velocity += steering;
        _velocity = Vector3.ClampMagnitude(_velocity, MaxVelocity);
        _velocity = new Vector3(_velocity.x, 0, _velocity.z);
        _rigidbody.MovePosition(_rigidbody.position + _velocity * Time.fixedDeltaTime);
        if (_velocity != Vector3.zero)
        {
            _rigidbody.MoveRotation(Quaternion.LookRotation(_velocity));
        }

        //Debug.DrawRay(transform.position, _velocity.normalized * 2, Color.yellow);
        //Debug.DrawRay(transform.position, steering.normalized * 2, Color.blue);
    }

    public abstract void CalculatePath();

    private void ObserveSourroundings()
    {
        // Overlaping Sphere Cast

        Collider[] collider = Physics.OverlapSphere(transform.position, LookRadius);

        foreach (Collider col in collider)
        {
            if (col.tag == "Obsticle")
            {
                AvoidObsticle(col);
            }
        }

        // Wenn im SpereCast ein Obsticle
        // Sende Ray Casts

        // Mit Raycast Punkt führe Aktion aus.
    }

    private void AvoidObsticle(Collider collider)
    {
        RaycastHit hitCenter;
        RaycastHit hitLeft;
        RaycastHit hitRight;

        Ray rayCenter = new Ray(transform.position, transform.forward);
        Ray rayRight = new Ray(transform.position, (transform.right + transform.forward).normalized);
        Ray rayLeft = new Ray(transform.position, (-transform.right + transform.forward).normalized);

        //Debug.DrawRay(rayCenter.origin, rayCenter.direction * LookRadius, Color.magenta);
        //Debug.DrawRay(rayRight.origin, rayRight.direction * LookRadius, Color.magenta);
        //Debug.DrawRay(rayLeft.origin, rayLeft.direction * LookRadius, Color.magenta);

        Vector3 avoid = new Vector3();
        Vector3 ahead = transform.position + Velocity.normalized * LookRadius;
        Vector3 pointsTogether = new Vector3();
        //Debug.DrawLine(transform.position, ahead);

        if (Physics.Raycast(rayCenter, out hitCenter, LookRadius))
        {
            _steeringBehaviour.Avoid(ahead - hitCenter.point, AvoidanceForce);
        }
        if (Physics.Raycast(rayRight, out hitRight, LookRadius))
        {
            _steeringBehaviour.Avoid(ahead - hitRight.point, AvoidanceForce);
        }
        if (Physics.Raycast(rayLeft, out hitLeft, LookRadius))
        {
            _steeringBehaviour.Avoid(ahead - hitLeft.point, AvoidanceForce);
        }
    }
}