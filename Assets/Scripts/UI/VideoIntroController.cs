using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class VideoIntroController : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public Button skipButton;
    public string nextSceneName = "";

    private bool hasFinished = false;

    void Start()
    {
        skipButton.onClick.AddListener(SkipVideo);
        videoPlayer.loopPointReached += OnVideoFinished;
    }

    void SkipVideo()
    {
        if (hasFinished) return;

        hasFinished = true;
        SceneManager.LoadScene(nextSceneName);
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        if (hasFinished) return;

        hasFinished = true;
        SceneManager.LoadScene(nextSceneName);
    }
}
