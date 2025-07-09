using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterBoundsController : MonoBehaviour
{
    public Tilemap tilemap; // Kéo tilemap bạn dùng vào đây
    public float padding = 0.3f;

    private Rigidbody2D rb;
    private Vector3 minWorldPos;
    private Vector3 maxWorldPos;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // Lấy cell bounds (tọa độ lưới) của Tilemap
        var cellBounds = tilemap.cellBounds;

        // Lấy tọa độ thế giới thực sự từ ô đầu tiên và ô cuối cùng
        minWorldPos = tilemap.CellToWorld(cellBounds.min); // góc dưới trái
        maxWorldPos = tilemap.CellToWorld(cellBounds.max); // góc trên phải

        Physics2D.IgnoreLayerCollision(
            LayerMask.NameToLayer("Player"),
            LayerMask.NameToLayer("Item"),
            true
        );
    }

    void LateUpdate()
    {
        Vector3 pos = transform.position;

        pos.x = Mathf.Clamp(pos.x, minWorldPos.x + padding, maxWorldPos.x - padding);
        pos.y = Mathf.Clamp(pos.y, minWorldPos.y + padding, maxWorldPos.y - padding);

        transform.position = pos;
    }
}
