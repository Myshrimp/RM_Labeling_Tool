using Flower;
using UnityEngine;

namespace Robo.Cam
{
    public class FPCamera : MonoBehaviour
{
    [Header("移动设置")]
    public float normalSpeed = 5f;
    public float fastSpeed = 15f;
    public float slowSpeed = 2f;
    public KeyCode fastKey = KeyCode.LeftShift;
    public KeyCode slowKey = KeyCode.LeftControl;

    [Header("鼠标设置")]
    public float mouseSensitivity = 100f;
    public bool invertY = false;
    public bool lockCursor = true;

    [Header("视角限制")]
    public float minVerticalAngle = -90f;
    public float maxVerticalAngle = 90f;

    private float _xRotation = 0f;
    private Camera _camera;

    void Start()
    {
        _camera = GetComponent<Camera>();
        if (_camera == null)
            _camera = Camera.main;

        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        MyGameEntry.ScreenShot.targetCamera = _camera;
    }

    void Update()
    {
        HandleMouseLook();
        HandleMovement();
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleCursorLock();
        }
    }

    void HandleMouseLook()
    {
        if (Cursor.lockState != CursorLockMode.Locked) return;

        Vector2 input = MyGameEntry.Input.GetVec2("CameraRotate");
        float mouseX = input.x * mouseSensitivity * Time.deltaTime;
        float mouseY = input.y * mouseSensitivity * Time.deltaTime;

        // 垂直视角
        float verticalInput = mouseY * (invertY ? 1f : -1f);
        _xRotation += verticalInput;
        _xRotation = Mathf.Clamp(_xRotation, minVerticalAngle, maxVerticalAngle);

        // 应用旋转
        transform.localRotation = Quaternion.Euler(_xRotation, transform.localEulerAngles.y, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleMovement()
    {
        // 确定移动速度
        float currentSpeed = normalSpeed;
        if (Input.GetKey(fastKey))
            currentSpeed = fastSpeed;
        else if (Input.GetKey(slowKey))
            currentSpeed = slowSpeed;

        // 获取输入
        Vector2 moveInput = MyGameEntry.Input.GetVec2("CameraMove");
        float x = moveInput.x; // A/D 左右移动
        float z = moveInput.y;   // W/S 前后移动
        float y = 0f;
        
        // Q/E 上下移动
        if (MyGameEntry.Input.GetBool("CameraAscend"))
            y = -1f;
        if (MyGameEntry.Input.GetBool("CameraDescend"))
            y = 1f;

        // 计算移动方向
        Vector3 move = transform.right * x + transform.forward * z + transform.up * y;
        
        // 应用移动
        transform.Translate(move * currentSpeed * Time.deltaTime, Space.World);
    }

    void ToggleCursorLock()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
}
