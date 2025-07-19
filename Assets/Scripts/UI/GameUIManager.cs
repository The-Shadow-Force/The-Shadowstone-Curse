using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUIManager : MonoBehaviour
{
    public GameObject panel;       // Panel thường
    public GameObject pausePanel;  // Panel tạm dừng

    public void ShowPausePanel()
    {
        panel.SetActive(false);         // Tắt panel thường
        pausePanel.SetActive(true);     // Bật pause panel
        Time.timeScale = 0f;            // Dừng game
    }

    public void HidePausePanel()
    {
        pausePanel.SetActive(false);    // Tắt pause panel
        panel.SetActive(true);          // Bật panel thường
        Time.timeScale = 1f;            // Tiếp tục game
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pausePanel.activeSelf)
                HidePausePanel();
            else
                ShowPausePanel();
        }
    }

    public void LoadSceneByName(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }
}
