using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(WheelController))]
public class VehiclePhysics : MonoBehaviour, IBoostable
{
    private const float LOW_SPEED_THRESHOLD = 3f;
    private const float MS_TO_KMH = 3.6f;

    private const float DOWNFORCE_GAIN = 8.0f;
    private const float BRAKE_GAIN = 0.03f;
    private const float BOOST_GAIN = 30f;
    private const float GAS_GAIN = 6f;

    [Header("Camera")]
    [SerializeField] private CinemachineFreeLook cmCamera;
    
    [Header("Physics")]
    [SerializeField] private Transform centerOfMass;

    [Header("Wheels")]
    [SerializeField] [Range(0f, 50.0f)] private float steerAngle = 30.0f;

    [Header("Behaviour")]
    [SerializeField] private AnimationCurve towardsMotorTorque;
    [SerializeField] private AnimationCurve backwardsMotorTorque;
    [SerializeField] private float timeToStop = 3f;

    [Header("Boost")]
    [SerializeField] private float boostAmount = 0.15f;

    [Header("info")]
    [SerializeField] private float speed;

    private float GasValue => InputManager.Instance.inputAxis[InputManager.InputAxisType.Vertical];
    private float SteerValue => InputManager.Instance.inputAxis[InputManager.InputAxisType.Horizontal] * steerAngle;
    private bool ResetPressed => InputManager.Instance.inputs[InputManager.InputKeyType.Reset];

    private bool BrakeActivated => !IsLowSpeed && !IsDirectionAccordsToInput;
    private bool IsLowSpeed => AbsSpeed < LOW_SPEED_THRESHOLD;
    private bool IsDirectionAccordsToInput => GasValue.Sign() == speed.Sign();
    private float AbsSpeed => Mathf.Abs(speed);

    private float BoostForce => _rb.mass * AbsSpeed * BOOST_GAIN * boostAmount;
    private float BrakeTorque => _rb.mass * AbsSpeed * BRAKE_GAIN / timeToStop;

    private float MotorTorque =>
        speed > 0 ? towardsMotorTorque.Evaluate(AbsSpeed) : backwardsMotorTorque.Evaluate(AbsSpeed);


    private Rigidbody _rb;
    private WheelController _wheelController;

    private Transform _transform;
    private Vector3 _spawnPosition;
    private Quaternion _spawnRotation;


    private void Awake()
    {
        GetComponents();
    }

    private void Start()
    {
        Init();
    }

    private void FixedUpdate()
    {
        UpdatePhysics();
        UpdateWheels();
        
        UpdateCameraSettings();
        if (ResetPressed)
        {
            ResetPosition();
        }
    }


    private void GetComponents()
    {
        _transform = transform;

        _rb = GetComponent<Rigidbody>();
        _wheelController = GetComponent<WheelController>();
    }

    private void Init()
    {
        _spawnPosition = _transform.position;
        _spawnRotation = _transform.rotation;
        _rb.centerOfMass = centerOfMass.localPosition;

        _wheelController.Init();
    }

    private void UpdateCameraSettings()
    {
        cmCamera.m_RecenterToTargetHeading.m_enabled = !IsLowSpeed;
    }

    private void UpdatePhysics()
    {
        speed = _transform.InverseTransformDirection(_rb.velocity).z * MS_TO_KMH;
        _rb.AddForce(AbsSpeed * DOWNFORCE_GAIN * -_transform.up);
    }

    private void UpdateWheels()
    {
        _wheelController.Steer(SteerValue);
        _wheelController.UpdateTorquesInfo(0.0001f, 0);

        if (BrakeActivated)
        {
            _wheelController.UpdateBrakeInfo(BrakeTorque);
            return;
        }

        if (GasValue == 0)
        {
            return;
        }

        _wheelController.Drive(GasValue * GAS_GAIN * MotorTorque);
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