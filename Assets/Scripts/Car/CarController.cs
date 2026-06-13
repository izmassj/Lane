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
    private InputAction brakeAction;
    private InputAction lookAction;

    private InputActionMap gameplayActions;

    // Physics

    private Rigidbody carRb;

    private void Start()
    {
        carRb = GetComponent<Rigidbody>();

        carRb.centerOfMass = centerOfMass;

        gameplayActions = playerInputAction.FindActionMap("Gameplay", true);

        movementAction = gameplayActions.FindAction("Move", true);

        brakeAction = gameplayActions.FindAction("Brake", true);

        lookAction = gameplayActions.FindAction("Look", true);
    }

    private void LateUpdate()
    {
        Move();
        Steer();
        Brake();
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

    void Brake()
    {
        if (brakeAction.ReadValue<float>() > 0.0f)
        {
            foreach (var wheel in wheels)
            {
                wheel.wheelCollider.brakeTorque = 300 * brakeForce * Time.deltaTime;
            }
        }
        else
        {
            foreach (var wheel in wheels)
            {
                wheel.wheelCollider.brakeTorque = 0;
            }
        }
    }
}
