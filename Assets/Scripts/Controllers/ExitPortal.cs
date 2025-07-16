using UnityEngine;

public class ExitPortal : MonoBehaviour
{
    private bool hasBeenTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasBeenTriggered)
        {
            hasBeenTriggered = true;
            
            // Tìm LevelManager trong scene và ra lệnh cho nó chuyển tầng
            FindObjectOfType<LevelManager>().GoToNextFloor();
        }
    }
}
