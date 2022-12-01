using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(WheelController))]
public class VehiclePhysics : MonoBehaviour, IBoostable
{
    private const float LOW_SPEED_THRESHOLD = 3f;
    private const float MS_TO_KMH = 3.6f;
    private const float DOWNFORCE = 8.0f;

    [Header("Physics")]
    [SerializeField] private Transform centerOfMass;

    [Header("Wheels")]
    [SerializeField] [Range(0f, 50.0f)] private float steerAngle = 30.0f;

    [Header("Behaviour")]
    [SerializeField] private AnimationCurve motorTorque;
    [SerializeField] private float brakeForce = 1500.0f;

    [Header("Boost")]
    [SerializeField] private float boostAmount = 0.15f;

    [Header("info")]
    [SerializeField] private float speed;
    [SerializeField] private bool _brake;

    private Rigidbody _rb;
    private WheelController _wheelController;

    private float BoostForce => _rb.mass * boostAmount * speed * 20;
    private float VerticalAxisInput => InputManager.Instance.inputAxis[InputManager.InputAxisType.Vertical];
    private float HorizontalAxisInput => InputManager.Instance.inputAxis[InputManager.InputAxisType.Horizontal];
    private bool Reset => InputManager.Instance.inputs[InputManager.InputKeyType.Reset];


    private Transform _transform;
    private Vector3 _spawnPosition;
    private Quaternion _spawnRotation;

    private float _gasValue;
    private float _steering;

    private bool _isGrounded;
    private bool _boosting;


    private void Awake()
    {
        _transform = transform;

        _rb = GetComponent<Rigidbody>();
        _wheelController = GetComponent<WheelController>();
    }

    private void Start()
    {
        _spawnPosition = _transform.position;
        _spawnRotation = _transform.rotation;
        _rb.centerOfMass = centerOfMass.localPosition;

        _wheelController.Init();
    }

    private void Update()
    {
        _gasValue = VerticalAxisInput;
        _steering = HorizontalAxisInput * steerAngle;

        _brake = VerticalAxisInput < 0 && speed > 3;

        if (Reset)
        {
            ResetPosition();
        }
    }

    private void FixedUpdate()
    {
        speed = _transform.InverseTransformDirection(_rb.velocity).z * MS_TO_KMH;

        _wheelController.Steer(_steering);
        _wheelController.UpdateTorquesInfo(0.0001f, 0);

        _rb.AddForce(speed * DOWNFORCE * -_transform.up);

        if (_brake)
        {
            _wheelController.UpdateTorquesInfo(0.0001f, brakeForce);
            return;
        }

        if (_gasValue == 0)
        {
            return;
        }

        if ((Mathf.Abs(speed) < LOW_SPEED_THRESHOLD || 
             Mathf.Sign(speed) == Mathf.Sign(_gasValue)))
        {
            _wheelController.Drive(_gasValue * motorTorque.Evaluate(speed) * 6f);
        }
        else
        {
            _wheelController.UpdateBrakeInfo(Mathf.Abs(_gasValue) * brakeForce);
        }
    }


    private void ResetPosition()
    {
        _transform.SetPositionAndRotation(_spawnPosition, _spawnRotation);

        _rb.velocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
    }

    public void Boost(Vector3 direction)
    {
        _rb.AddForce(BoostForce * direction);
    }
}