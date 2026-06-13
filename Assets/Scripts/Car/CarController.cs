using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
    [Header("Car Speed")]
    [SerializeField] private float maxAcceleration = 30.0f;
    [SerializeField] private float brakeAcceleration = 50.0f;

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

        gameplayActions = playerInputAction.FindActionMap("Gameplay", true);

        movementAction = gameplayActions.FindAction("Move", true);
    }

    private void LateUpdate()
    {
        Move();
    }

    void Move()
    {
        foreach (var wheel in wheels)
        {
            wheel.wheelCollider.motorTorque = movementAction.ReadValue<Vector2>().y * maxAcceleration * Time.deltaTime;     
        }
    }


}
