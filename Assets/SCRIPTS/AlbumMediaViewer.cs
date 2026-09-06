using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class AlbumMediaViewer : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private AlbumPauseController pauseController;
    [SerializeField] private GameObject viewerRoot;
    [SerializeField] private RawImage mediaDisplay;
    [SerializeField] private AspectRatioFitter aspectRatioFitter;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private Button closeButton;
    [SerializeField] private VideoPlayer videoPlayer;

    [Header("Vídeo")]
    [SerializeField] private bool loopVideo;

    public bool IsOpen => viewerRoot != null && viewerRoot.activeSelf;

    private void Awake()
    {
        pauseController ??= FindFirstObjectByType<AlbumPauseController>();

        if (viewerRoot == null || mediaDisplay == null || videoPlayer == null)
        {
            Debug.LogError("El visor multimedia no tiene todas sus referencias obligatorias.", this);
            enabled = false;
            return;
        }

        videoPlayer.playOnAwake = false;
        videoPlayer.isLooping = loopVideo;
        videoPlayer.renderMode = VideoRenderMode.APIOnly;
        videoPlayer.prepareCompleted += OnVideoPrepared;
        videoPlayer.errorReceived += OnVideoError;

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(Close);
        }

        viewerRoot.SetActive(false);
    }

    private void OnEnable()
    {
        if (pauseController != null)
        {
            pauseController.AlbumStateChanged += OnAlbumStateChanged;
        }
    }

    private void OnDisable()
    {
        if (pauseController != null)
        {
            pauseController.AlbumStateChanged -= OnAlbumStateChanged;
        }
    }

    private void OnDestroy()
    {
        if (videoPlayer != null)
        {
            videoPlayer.prepareCompleted -= OnVideoPrepared;
            videoPlayer.errorReceived -= OnVideoError;
        }

        if (closeButton != null)
        {
            closeButton.onClick.RemoveListener(Close);
        }
    }

    public void ShowImage(Sprite image, string mediaTitle)
    {
        if (image == null)
        {
            return;
        }

        StopVideo();
        viewerRoot.SetActive(true);
        mediaDisplay.texture = image.texture;
        SetAspectRatio(image.rect.width, image.rect.height);
        SetTitle(mediaTitle);
    }

    public void ShowVideo(VideoClip clip, string mediaTitle)
    {
        if (clip == null)
        {
            return;
        }

        StopVideo();
        viewerRoot.SetActive(true);
        mediaDisplay.texture = null;
        SetTitle(mediaTitle);

        videoPlayer.clip = clip;
        videoPlayer.isLooping = loopVideo;
        videoPlayer.Prepare();
    }

    public void Close()
    {
        StopVideo();

        if (viewerRoot != null)
        {
            viewerRoot.SetActive(false);
        }
    }

    private void StopVideo()
    {
        if (videoPlayer != null)
        {
            videoPlayer.Stop();
            videoPlayer.clip = null;
        }
    }

    private void OnVideoPrepared(VideoPlayer source)
    {
        mediaDisplay.texture = source.texture;
        SetAspectRatio((float)source.width, (float)source.height);
        source.Play();
    }

    private void OnVideoError(VideoPlayer source, string message)
    {
        Debug.LogError($"No se ha podido reproducir el vídeo: {message}", source);
    }

    private void OnAlbumStateChanged(bool isOpen)
    {
        if (!isOpen)
        {
            Close();
        }
    }

    private void SetAspectRatio(float width, float height)
    {
        if (aspectRatioFitter != null && height > 0f)
        {
            aspectRatioFitter.aspectRatio = width / height;
        }
    }

    private void SetTitle(string mediaTitle)
    {
        if (titleText != null)
        {
            titleText.text = string.IsNullOrWhiteSpace(mediaTitle) ? string.Empty : mediaTitle;
        }
    }
}
