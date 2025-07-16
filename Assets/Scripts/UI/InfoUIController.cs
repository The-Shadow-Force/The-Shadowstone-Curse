using UnityEngine;
using UnityEngine.SceneManagement;  // Thêm namespace để load scene

public class InfoUIController : MonoBehaviour
{
    public GameObject infoPanel;
    public GameObject instructionPanel;
    public GameObject mainGroup;

    public void ShowInfo()
    {
        infoPanel.SetActive(true);
        mainGroup.SetActive(false);
    }

    public void HideInfo()
    {
        infoPanel.SetActive(false);
        mainGroup.SetActive(true);
    }

    public void ShowInstruction()
    {
        instructionPanel.SetActive(true);
        mainGroup.SetActive(false);
    }

    public void HideInstruction()
    {
        instructionPanel.SetActive(false);
        mainGroup.SetActive(true);
    }

    public void QuitGame()
    {
        Debug.Log("Game is quitting...");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void LoadSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
