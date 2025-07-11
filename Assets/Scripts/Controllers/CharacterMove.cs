using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMove : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 movementInput;

    private PlayerAction inputActions;
    private Animator animator;

    private Vector2 currentDirection;
    private Vector2 lastDirection = Vector2.right;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        inputActions = new PlayerAction();
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        inputActions.Enable();
        inputActions.Newactionmap.Newaction.performed += OnMove;
        inputActions.Newactionmap.Newaction.canceled += OnMove;
    }

    private void OnDisable()
    {
        inputActions.Newactionmap.Newaction.performed -= OnMove;
        inputActions.Newactionmap.Newaction.canceled -= OnMove;
        inputActions.Disable();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        // Vẫn xử lý vật lý ở đây
        rb.linearVelocity = movementInput * moveSpeed;
    }

    private void Update()
    {
        movementInput = inputActions.Newactionmap.Newaction.ReadValue<Vector2>();

        animator.SetFloat("MoveX", movementInput.x);
        animator.SetFloat("MoveY", movementInput.y);

        float speed = movementInput.sqrMagnitude;
        animator.SetFloat("Speed", speed);

        if (movementInput != Vector2.zero)
        {
            // ✅ Cập nhật hướng hiện tại
            if (Mathf.Abs(movementInput.y) >= Mathf.Abs(movementInput.x))
            {
                currentDirection = new Vector2(0, Mathf.Sign(movementInput.y));
            }
            else
            {
                currentDirection = new Vector2(Mathf.Sign(movementInput.x), 0);
            }

            lastDirection = currentDirection;

            // Cập nhật ngay lập tức hướng mới khi đang di chuyển
            animator.SetFloat("LastMoveX", lastDirection.x);
            animator.SetFloat("LastMoveY", lastDirection.y);
        }
        else
        {
            animator.SetFloat("MoveX", 0);
            animator.SetFloat("MoveY", 0);
            // Khi đứng yên, tiếp tục giữ hướng cuối cùng
            animator.SetFloat("LastMoveX", lastDirection.x);
            animator.SetFloat("LastMoveY", lastDirection.y);
        }

    }

    public void DisableInputTemporarily(float duration)
    {
        inputActions.Disable();
        Invoke(nameof(ReenableInput), duration);
    }

    private void ReenableInput()
    {
        inputActions.Enable();
    }

}
