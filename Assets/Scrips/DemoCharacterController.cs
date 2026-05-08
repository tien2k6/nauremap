using UnityEngine;
using UnityEngine.InputSystem;

public class DemoCharacterController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;

    [Header("Jump")]
    public float jumpForce = 5f;
    public float groundCheckDistance = 0.2f; // Tăng nhẹ để check đất nhạy hơn

    [Header("References")]
    public Animator animator;

    private Rigidbody rb;
    private Vector2 moveInput;
    private bool isGrounded = false;
    void Start()
{
    // Giấu con trỏ chuột và khóa nó vào giữa màn hình
    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;
}

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
            Debug.LogError("Rigidbody not found on this GameObject.");

        if (animator == null)
            animator = GetComponent<Animator>();
    }

    void Update()
    {
        ReadInput();

        bool jumpPressed = false;
        if (Keyboard.current != null)
            jumpPressed |= Keyboard.current.spaceKey.wasPressedThisFrame;
        if (Gamepad.current != null)
            jumpPressed |= Gamepad.current.buttonSouth.wasPressedThisFrame;

        if (jumpPressed)
            DoJump();
        if (Input.GetKeyDown(KeyCode.Escape))
{
    Cursor.lockState = CursorLockMode.None;
    Cursor.visible = true;
}
    }

    void FixedUpdate()
    {
        HandleMovement();
        CheckGrounded();
    }

    void ReadInput()
    {
        if (Gamepad.current != null)
        {
            moveInput = Gamepad.current.leftStick.ReadValue();
        }
        else if (Keyboard.current != null)
        {
            float x = (Keyboard.current.dKey.isPressed ? 1f : 0f) - (Keyboard.current.aKey.isPressed ? 1f : 0f);
            float y = (Keyboard.current.wKey.isPressed ? 1f : 0f) - (Keyboard.current.sKey.isPressed ? 1f : 0f);
            moveInput = new Vector2(x, y);
        }
        else
        {
            moveInput = Vector2.zero;
        }
    }

    void HandleMovement()
{
    if (rb == null) return;

    // 1. Lấy hướng phía trước và bên phải của Camera
    Vector3 forward = Camera.main.transform.forward;
    Vector3 right = Camera.main.transform.right;

    // 2. Triệt tiêu thành phần Y (độ cao) để nhân vật không bị chúi xuống đất hoặc bay lên
    forward.y = 0f;
    right.y = 0f;
    forward.Normalize();
    right.Normalize();

    // 3. Tính toán hướng di chuyển dựa trên Camera và Input (A/D và W/S)
    // Với logic mới của bạn: Nhấn A/D thì moveInput.y đã được ép bằng 1 ở hàm ReadInput
    Vector3 movement = (forward * moveInput.y + right * moveInput.x).normalized;

    if (movement.magnitude > 0.1f)
    {
        // Di chuyển nhân vật
        Vector3 targetPos = transform.position + movement * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(targetPos);

        // Xoay nhân vật nhìn theo hướng di chuyển mượt mà
        Quaternion targetRot = Quaternion.LookRotation(movement);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.fixedDeltaTime);
        
        animator.SetBool("isWalking", true);
    }
    else
    {
        animator.SetBool("isWalking", false);
    }
}
    void DoJump()
    {
        // Chỉ cho phép nhảy khi đang chạm đất
        if (rb == null || !isGrounded) return;

        // Reset vận tốc dọc để lực nhảy luôn ổn định
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        
        // Cập nhật Animation ngay khi bấm nút nhảy
        if (animator != null)
        {
            animator.SetTrigger("jumpTrigger"); 
            animator.SetBool("isGrounded", false);
        }
        
        isGrounded = false; // Tạm thời set bằng false để tránh nhảy liên tục (double jump)
    }

    void CheckGrounded()
    {
        if (rb == null) return;

        // Raycast từ vị trí nhân vật rọi xuống dưới
        // RayStart cao hơn chân một chút (0.1f) để tránh việc tia bắt đầu từ dưới sàn
        Vector3 rayStart = transform.position + Vector3.up * 0.1f;
        bool currentlyGrounded = Physics.Raycast(rayStart, Vector3.down, groundCheckDistance + 0.1f);

        // Chỉ cập nhật animator khi trạng thái chạm đất thay đổi để tối ưu hiệu năng
        if (currentlyGrounded != isGrounded)
        {
            isGrounded = currentlyGrounded;
            if (animator != null)
            {
                animator.SetBool("isGrounded", isGrounded);
            }
        }
    }

    // Vẽ tia Raycast trong Scene để bạn dễ dàng căn chỉnh groundCheckDistance
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 rayStart = transform.position + Vector3.up * 0.1f;
        Gizmos.DrawRay(rayStart, Vector3.down * (groundCheckDistance + 0.1f));
    }
    // Hàm này tự chạy khi nhân vật chạm vào vật thể có tích "Is Trigger"
private void OnTriggerEnter(Collider other)
{
    // Nếu vật chạm phải có Tag là "Coin"
    if (other.CompareTag("Coin"))
    {
        // Xóa đồng coin khỏi màn hình
        Destroy(other.gameObject);
        
        // Bạn có thể cộng điểm ở đây, ví dụ: score += 10;
        Debug.Log("Đã ăn coin!");
    }
}
}