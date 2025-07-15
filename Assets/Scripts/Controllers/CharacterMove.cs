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

    [Header("Footstep Settings")]
    public AudioClip footstepClip;
    public float footstepInterval = 0.4f;
    private float footstepTimer;
    private AudioSource audioSource;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        inputActions = new PlayerAction();
        animator = GetComponent<Animator>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
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
        Vector2 newPosition = rb.position + movementInput.normalized * (moveSpeed * Time.fixedDeltaTime);
        rb.MovePosition(newPosition);    }

    private void Update()
    {
        movementInput = inputActions.Newactionmap.Newaction.ReadValue<Vector2>();

        animator.SetFloat("MoveX", movementInput.x);
        animator.SetFloat("MoveY", movementInput.y);

        float speed = movementInput.sqrMagnitude;
        animator.SetFloat("Speed", speed);

        if (movementInput != Vector2.zero)
        {
            if (Mathf.Abs(movementInput.y) >= Mathf.Abs(movementInput.x))
            {
                currentDirection = new Vector2(0, Mathf.Sign(movementInput.y));
            }
            else
            {
                currentDirection = new Vector2(Mathf.Sign(movementInput.x), 0);
            }

            lastDirection = currentDirection;

            animator.SetFloat("LastMoveX", lastDirection.x);
            animator.SetFloat("LastMoveY", lastDirection.y);

            // ✅ Phát âm thanh bước chân
            footstepTimer += Time.deltaTime;
            if (footstepTimer >= footstepInterval && footstepClip != null)
            {
                audioSource.PlayOneShot(footstepClip);
                footstepTimer = 0f;
            }
        }
        else
        {
            animator.SetFloat("MoveX", 0);
            animator.SetFloat("MoveY", 0);

            animator.SetFloat("LastMoveX", lastDirection.x);
            animator.SetFloat("LastMoveY", lastDirection.y);

            footstepTimer = 0f; // reset nếu đứng yên
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
