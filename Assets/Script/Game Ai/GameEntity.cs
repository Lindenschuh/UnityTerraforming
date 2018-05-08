using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class GameEntity : Photon.PunBehaviour
{
    public const float MAX_RANGE = 10f;

    public float Mass { get { return _rigidbody.mass; } set { _rigidbody.mass = value; } }
    public float MaxVelocity;
    public float MaxForce;
    public float SlowingRadius;

    public Spawner Spawner;
    public float LookRadius;
    public float AvoidanceForce;

    public LayerMask ObsticleLayers;

    [Range(0, MAX_RANGE)]
    public float AvoidanceWeight;

    protected Rigidbody _rigidbody;
    protected Vector3 _velocity;
    protected SteeringManager _steeringBehaviour;
    protected Vector3 _avoidanceForce;

    public Vector3 Velocity { get { return _velocity; } private set { _velocity = value; } }

    public void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _steeringBehaviour = new SteeringManager(this);
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
        if (_velocity != Vector3.zero && PhotonNetwork.isMasterClient)
        {
            _rigidbody.MoveRotation(Quaternion.LookRotation(_velocity));
        }

        //Debug.DrawRay(transform.position, _velocity.normalized * 2, Color.yellow);
        //Debug.DrawRay(transform.position, steering.normalized * 2, Color.blue);
    }

    public abstract void CalculatePath();

    private void ObserveSourroundings()
    {
        RaycastHit hit;
        Ray obsticleCheck = new Ray(transform.position, transform.forward);
        Debug.DrawRay(obsticleCheck.origin, obsticleCheck.direction * LookRadius, Color.cyan);

        if (Physics.Raycast(obsticleCheck, out hit, LookRadius))
        {
            var layer = hit.collider.gameObject.layer;

            if (ObsticleLayers == (ObsticleLayers | (1 << layer)))
            {
                var go = new GameObject();
                go.transform.position = hit.point;
                var sphere = go.AddComponent<SphereCollider>();
                sphere.radius = AvoidanceForce;
                sphere.isTrigger = true;
            }
        }
    }

    private void AvoidObsticle(Collider collider)
    {
        //RaycastHit hitCenter;
        RaycastHit hitLeft;
        RaycastHit hitRight;

        //Ray rayCenter = new Ray(transform.position, transform.forward);
        Ray rayRight = new Ray(transform.position, (transform.right / 2 + transform.forward).normalized);
        Ray rayLeft = new Ray(transform.position, (-transform.right / 2 + transform.forward).normalized);

        //Debug.DrawRay(rayCenter.origin, rayCenter.direction * LookRadius, Color.magenta);
        Debug.DrawRay(rayRight.origin, rayRight.direction * LookRadius, Color.magenta);
        Debug.DrawRay(rayLeft.origin, rayLeft.direction * LookRadius, Color.magenta);

        Vector3 avoid = new Vector3();
        Vector3 ahead = transform.position + Velocity.normalized * LookRadius;
        Vector3 pointsTogether = new Vector3();
        //Debug.DrawLine(transform.position, ahead);

        //if (Physics.Raycast(rayCenter, out hitCenter, LookRadius))
        //{
        //    _steeringBehaviour.Avoid(ahead - hitCenter.point, AvoidanceForce, weight);
        //}
        var rightHited = false;
        if (Physics.Raycast(rayRight, out hitRight, LookRadius))
        {
            _steeringBehaviour.Avoid(ahead - hitRight.point, AvoidanceForce, AvoidanceWeight);
            rightHited = true;
        }
        if (Physics.Raycast(rayLeft, out hitLeft, LookRadius) && !rightHited)
        {
            _steeringBehaviour.Avoid(ahead - hitLeft.point, AvoidanceForce, AvoidanceWeight);
        }
    }
}