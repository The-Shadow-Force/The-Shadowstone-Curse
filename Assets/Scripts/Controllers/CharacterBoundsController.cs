using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterBoundsController : MonoBehaviour
{
    private Camera mainCamera;
    private Vector2 screenBounds;
    private float playerWidth;
    private float playerHeight;

    [Header("Giới Hạn Tùy Chỉnh")]
    [Tooltip("Tick vào đây để dùng giới hạn trên tùy chỉnh thay vì cạnh camera.")]
    public bool useCustomTopBoundary = false;

    [Tooltip("Nhập tọa độ Y của bức tường vào đây.")]
    public float topBoundaryValue = 5f;


    void Awake()
    {
        mainCamera = Camera.main;
    }

    void Start()
    {
        // Tính toán ranh giới của màn hình theo đơn vị của thế giới game
        float cameraHeight = mainCamera.orthographicSize * 2;
        float cameraWidth = cameraHeight * mainCamera.aspect;
        screenBounds = new Vector2(cameraWidth / 2, cameraHeight / 2);

        // Lấy kích thước của sprite
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            playerWidth = spriteRenderer.bounds.size.x / 2;
            playerHeight = spriteRenderer.bounds.size.y / 2;
        }
    }

    void LateUpdate()
    {
        Vector3 currentPosition = transform.position;

        // --- PHẦN LOGIC ĐÃ ĐƯỢC NÂNG CẤP ---

        // Giới hạn trái, phải, và dưới vẫn giữ nguyên theo camera
        float minX = -screenBounds.x + playerWidth;
        float maxX = screenBounds.x - playerWidth;
        float minY = -screenBounds.y + playerHeight;
        float maxY; // Giới hạn trên sẽ được quyết định bởi tùy chọn

        // Nếu bạn chọn dùng giới hạn tùy chỉnh
        if (useCustomTopBoundary)
        {
            // Giới hạn trên sẽ là giá trị bạn nhập vào
            maxY = topBoundaryValue;
        }
        else
        {
            // Nếu không, giới hạn trên vẫn là cạnh của camera
            maxY = screenBounds.y - playerHeight;
        }

        // "Kẹp" vị trí của người chơi trong các giới hạn đã được tính toán
        currentPosition.x = Mathf.Clamp(currentPosition.x, minX, maxX);
        currentPosition.y = Mathf.Clamp(currentPosition.y, minY, maxY);
        
        // Gán lại vị trí đã được "kẹp"
        transform.position = currentPosition;
    }
}
