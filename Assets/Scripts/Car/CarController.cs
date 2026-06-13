using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using Unity.VisualScripting.Dependencies.NCalc;

public class CarController : MonoBehaviour
{
    [Header("Car Speed")]
    [SerializeField] private float acceleration;
    [SerializeField] private float brakeForce;

    [Header("Car Turning")]
    [SerializeField] private float turnSensitivity;
    [SerializeField] private float maxSteerAngle;
    [SerializeField] private Vector3 centerOfMass;

    [Header("Wheels")]
    public List<Wheel> wheels;

    [Header("Input")]
    [SerializeField] InputActionAsset playerInputAction;

    // Inputs

    private InputAction movementAction;

    private InputActionMap gameplayActions;

    // Physics

    private Rigidbody carRb;

    private void Start()
    {
        carRb = GetComponent<Rigidbody>();

        carRb.centerOfMass = centerOfMass;

        gameplayActions = playerInputAction.FindActionMap("Gameplay", true);

        movementAction = gameplayActions.FindAction("Move", true);
    }

    private void LateUpdate()
    {
        Move();
        Steer();
    }

    void Move()
    {
        foreach (var wheel in wheels)
        {
            wheel.wheelCollider.motorTorque = movementAction.ReadValue<Vector2>().y * acceleration;     
        }
    }

    void Steer()
    {
        foreach (var wheel in wheels)
        {
            if (wheel.axel == Axel.Front)
            {
                var steerAngle = movementAction.ReadValue<Vector2>().x * maxSteerAngle * turnSensitivity;
                wheel.wheelCollider.steerAngle = Mathf.Lerp(wheel.wheelCollider.steerAngle, steerAngle, 0.6f);
            }
        }
    }

    
}
