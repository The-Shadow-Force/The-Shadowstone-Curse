using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance; // Singleton để dễ truy cập
    public GameObject rewardCanvas; // Kéo Canvas phần thưởng vào đây

    void Awake()
    {
        instance = this;
    }

    public void ShowRewardUI()
    {
        if (rewardCanvas != null) rewardCanvas.SetActive(true);
    }
}
