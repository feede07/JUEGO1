using System.Collections;
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
    [SerializeField] private GameObject loadingIndicator;
    [SerializeField] private Button closeButton;
    [SerializeField] private VideoPlayer videoPlayer;

    [Header("Vídeo")]
    [SerializeField] private bool loopVideo;

    public bool IsOpen => viewerRoot != null && viewerRoot.activeSelf;

    private Object loadedResource;
    private int requestVersion;

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
        StopVideo();
        ReleaseLoadedResource();

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

        BeginOpen(mediaTitle);
        mediaDisplay.texture = image.texture;
        SetAspectRatio(image.rect.width, image.rect.height);
        SetLoading(false);
    }

    public void ShowVideo(VideoClip clip, string mediaTitle)
    {
        if (clip == null)
        {
            return;
        }

        BeginOpen(mediaTitle);
        SetLoading(false);

        videoPlayer.clip = clip;
        videoPlayer.isLooping = loopVideo;
        videoPlayer.Prepare();
    }

    public void ShowImageFromResources(string resourcePath, string mediaTitle)
    {
        BeginOpen(mediaTitle);
        StartCoroutine(LoadImage(resourcePath, requestVersion));
    }

    public void ShowVideoFromResources(string resourcePath, string mediaTitle)
    {
        BeginOpen(mediaTitle);
        StartCoroutine(LoadVideo(resourcePath, requestVersion));
    }

    public void Close()
    {
        requestVersion++;
        StopVideo();
        ReleaseLoadedResource();
        mediaDisplay.texture = null;
        SetLoading(false);

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

    private void BeginOpen(string mediaTitle)
    {
        requestVersion++;
        StopVideo();
        ReleaseLoadedResource();
        mediaDisplay.texture = null;
        SetTitle(mediaTitle);
        SetLoading(true);
        viewerRoot.SetActive(true);
    }

    private IEnumerator LoadImage(string resourcePath, int version)
    {
        ResourceRequest request = Resources.LoadAsync<Texture2D>(resourcePath);
        yield return request;

        Texture2D texture = request.asset as Texture2D;
        if (version != requestVersion)
        {
            if (texture != null)
            {
                Resources.UnloadAsset(texture);
            }
            yield break;
        }

        if (texture == null)
        {
            HandleMissingResource(resourcePath);
            yield break;
        }

        loadedResource = texture;
        mediaDisplay.texture = texture;
        SetAspectRatio(texture.width, texture.height);
        SetLoading(false);
    }

    private IEnumerator LoadVideo(string resourcePath, int version)
    {
        ResourceRequest request = Resources.LoadAsync<VideoClip>(resourcePath);
        yield return request;

        VideoClip clip = request.asset as VideoClip;
        if (version != requestVersion)
        {
            if (clip != null)
            {
                Resources.UnloadAsset(clip);
            }
            yield break;
        }

        if (clip == null)
        {
            HandleMissingResource(resourcePath);
            yield break;
        }

        loadedResource = clip;
        videoPlayer.clip = clip;
        videoPlayer.isLooping = loopVideo;
        videoPlayer.Prepare();
    }

    private void ReleaseLoadedResource()
    {
        if (loadedResource != null)
        {
            Resources.UnloadAsset(loadedResource);
            loadedResource = null;
        }
    }

    private void HandleMissingResource(string resourcePath)
    {
        SetLoading(false);
        Debug.LogError($"No se ha encontrado el contenido multimedia en Resources/{resourcePath}.", this);
    }

    private void OnVideoPrepared(VideoPlayer source)
    {
        if (!IsOpen || source.clip == null)
        {
            return;
        }

        mediaDisplay.texture = source.texture;
        SetAspectRatio((float)source.width, (float)source.height);
        SetLoading(false);
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

    private void SetLoading(bool isLoading)
    {
        if (loadingIndicator != null)
        {
            loadingIndicator.SetActive(isLoading);
        }
    }
}
