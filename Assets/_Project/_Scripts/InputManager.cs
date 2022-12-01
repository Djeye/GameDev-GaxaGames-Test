using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public readonly Dictionary<InputAxisType, float> inputAxis = new Dictionary<InputAxisType, float>()
    {
        {InputAxisType.Vertical, 0f},
        {InputAxisType.Horizontal, 0f}
    };
    
    public readonly Dictionary<InputKeyType, bool> inputs = new Dictionary<InputKeyType, bool>()
    {
        {InputKeyType.Reset, false},
        {InputKeyType.Brake, false}
    };
    
    public static InputManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        UpdateAxis();
        UpdateKeys();
    }

    
    private void UpdateAxis()
    {
        inputAxis[InputAxisType.Vertical] = Input.GetAxis("Vertical");
        inputAxis[InputAxisType.Horizontal] = Input.GetAxis("Horizontal");
    }

    private void UpdateKeys()
    {
        inputs[InputKeyType.Reset] = Input.GetKeyDown(KeyCode.R);
        inputs[InputKeyType.Brake] = Input.GetKey(KeyCode.Space);
    }

    public enum InputAxisType : byte
    {
        Vertical = 0,
        Horizontal = 1
    }

    public enum InputKeyType : byte
    {
        Reset = 0,
        Brake = 1
    }
}