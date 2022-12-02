using System;
using UnityEngine;

public class WheelController : MonoBehaviour
{
    [SerializeField] private WheelInfo[] wheels;
    
    private int _driveWheelsCount;


    public void Init()
    {
        _driveWheelsCount = 0;
        
        foreach (WheelInfo wheel in wheels)
        {
            wheel.Init(out bool isDrives);
            
            if (isDrives)
            {
                _driveWheelsCount++;
            }
        }
    }

    private void FixedUpdate()
    {
        foreach (WheelInfo wheel in wheels)
        {
            wheel.UpdateRotation();
        }
    }


    public void UpdateTorquesInfo(float motorTorque, float brakeTorque)
    {
        foreach (WheelInfo wheel in wheels)
        {
            wheel.SetTorque(motorTorque, brakeTorque);
        }
    }

    public void UpdateMotorInfo(float motorTorque)
    {
        foreach (WheelInfo wheel in wheels)
        {
            wheel.SetMotorTorque(motorTorque);
        }
    }

    public void UpdateBrakeInfo(float brakeTorque)
    {
        foreach (WheelInfo wheel in wheels)
        {
            wheel.SetBrakeTorque(brakeTorque);
        }
    }

    public void Steer(float steering)
    {
        foreach (WheelInfo wheel in wheels)
        {
            wheel.Steer(steering);
        }
    }

    public void Drive(float throttle)
    {
        foreach (WheelInfo wheel in wheels)
        {
            wheel.Drive(throttle, _driveWheelsCount);
        }
    }


    [Serializable]
    public struct WheelInfo
    {
        private const int RPM_TO_ANGLE = 6;
        
        private const float DEFAULT_MOTOR_TORQUE = 0.0001f;
        private const float DEFAULT_BRAKE_TORQUE = 0.0001f;

        [SerializeField] private WheelCollider wheelCollider;
        [SerializeField] private Transform wheelModel;
        [SerializeField] private bool isTurns;
        [SerializeField] private bool isDrives;

        
        public void Init(out bool isDrives)
        {
            isDrives = this.isDrives;
            wheelCollider.motorTorque = DEFAULT_MOTOR_TORQUE;
        }

        
        public void UpdateRotation()
        {
            wheelModel.localEulerAngles = new Vector3(
                wheelModel.localEulerAngles.x,
                wheelCollider.steerAngle - wheelModel.localEulerAngles.z,
                wheelModel.localEulerAngles.z);

            wheelModel.Rotate(wheelCollider.rpm * RPM_TO_ANGLE * Time.deltaTime, 0, 0);
        }

        public void SetTorque(float motorTorque=DEFAULT_MOTOR_TORQUE, float brakeTorque=DEFAULT_BRAKE_TORQUE)
        {
            SetMotorTorque(motorTorque);
            SetBrakeTorque(brakeTorque);
        }

        public void SetMotorTorque(float motorTorque)
        {
            wheelCollider.motorTorque = motorTorque;
        }

        public void SetBrakeTorque(float brakeTorque)
        {
            wheelCollider.brakeTorque = brakeTorque;
        }

        public void Steer(float steering)
        {
            if (!isTurns)
            {
                return;
            }

            wheelCollider.steerAngle = steering;
        }

        public void Drive(float torque, int driveWheelsCount=1)
        {
            if (!isDrives)
            {
                return;
            }

            wheelCollider.motorTorque = torque / driveWheelsCount;
        }
    }
}