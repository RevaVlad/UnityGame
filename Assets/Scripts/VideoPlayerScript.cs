using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoPlayerScript : MonoBehaviour
{
    [SerializeField] private RawImage rawImage;
    private VideoPlayer videoPlayer;
    [SerializeField] private string sceneName;

    private void Start()
    {
        MainMenuManager.DestroyAllDontDestroyOnLoadObjects();
        rawImage.color = new Color(rawImage.color.r, rawImage.color.g, rawImage.color.b, 0f);

        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.loopPointReached += OnVideoEnd;
        StartCoroutine(PlayVideo());
    }

    private IEnumerator PlayVideo()
    {
        videoPlayer.Prepare();
        while (!videoPlayer.isPrepared)
        {
            yield return new WaitForSeconds(0.05f);
        }

        rawImage.texture = videoPlayer.texture;
        rawImage.color = new Color(rawImage.color.r, rawImage.color.g, rawImage.color.b, 1f);
        videoPlayer.Play();
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        SceneManager.SaveLevelNumber(0);
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }
}