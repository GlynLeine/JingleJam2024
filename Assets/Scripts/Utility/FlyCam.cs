using UnityEngine;
using UnityEngine.InputSystem;
using NaughtyAttributes;
public class FlyCam : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField]
    private float m_normalSpeed = 40.0f;
    [SerializeField]
    private float m_boostedSpeed = 85.0f;

    [Header("Mouse Settings")]
    [SerializeField]
    private float m_lookSensitivity = 0.5f;

    private Camera m_cam;
    private Vector3 m_moveInput;
    private Vector2 m_lookInput;
    private float xRotation = 0f;
    private float yRotation = 0f;
    private bool m_isBoosting = false;
    private bool m_rightClickDown = false;

    private InputSystem_Actions inputActions;

    private void Awake()
    {
        m_cam = GetComponent<Camera>();
    }

    private void Update()
    {
        if (!m_rightClickDown) return;

        var movementSpeed = m_normalSpeed;

        if (m_isBoosting)
            movementSpeed = m_boostedSpeed;

        movementSpeed *= Time.deltaTime;

        Vector3 forwardMovement = (transform.forward * m_moveInput.z).normalized * movementSpeed;
        Vector3 rightMovement = (transform.right * m_moveInput.x).normalized * movementSpeed;
        Vector3 upMovement = (transform.up * m_moveInput.y).normalized * movementSpeed;

        transform.position += forwardMovement + rightMovement + upMovement;

        LookAround();
    }

    private void LookAround()
    {
        float mouseX = m_lookInput.x * m_lookSensitivity;
        float mouseY = m_lookInput.y * m_lookSensitivity;

        yRotation += mouseX;
        yRotation %= 360.0f;
        xRotation = Mathf.Clamp(xRotation - mouseY, -90.0f, 90.0f);
        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }

    #region Input Boilerplate
    private void OnEnable()
    {
        inputActions = new InputSystem_Actions();
        inputActions.Enable();

        inputActions.FlyCam.Move.performed += OnMove;
        inputActions.FlyCam.Move.canceled += OnMove;

        inputActions.FlyCam.Look.performed += OnLook;
        inputActions.FlyCam.Look.canceled += OnLook;

        inputActions.FlyCam.MoveVertical.performed += OnMoveVertical;
        inputActions.FlyCam.MoveVertical.canceled += OnMoveVertical;

        inputActions.FlyCam.Boost.performed += OnBoost;
        inputActions.FlyCam.Boost.canceled += OnBoost;

        inputActions.FlyCam.RightClickHold.performed += RightClickHold;
        inputActions.FlyCam.RightClickHold.canceled += RightClickHold;
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }
    #endregion

    public void OnMove(InputAction.CallbackContext context)
    {
        var value = context.ReadValue<Vector2>();
        m_moveInput = new Vector3(value.x, m_moveInput.y, value.y);
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        m_lookInput = context.ReadValue<Vector2>();
    }

    public void OnMoveVertical(InputAction.CallbackContext context)
    {
        m_moveInput.y = context.ReadValue<float>();
    }

    public void OnBoost(InputAction.CallbackContext context)
    {
        m_isBoosting = context.performed;
    }

    public void RightClickHold(InputAction.CallbackContext context)
    {
        m_rightClickDown = context.performed;
    }
}
