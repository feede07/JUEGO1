using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Video;

public class AlbumPhotoSlot : MonoBehaviour, IPointerClickHandler
{
    [Header("Identidad")]
    [SerializeField] private string photoId;

    [Header("Estados visuales")]
    [SerializeField] private Image unlockedPhoto;
    [SerializeField] private GameObject lockedOverlay;
    [SerializeField] private GameObject newIndicator;

    [Header("Contenido ampliado")]
    [SerializeField] private string mediaTitle;
    [Tooltip("Ruta dentro de una carpeta Resources, sin extensión. Ejemplo: AlbumFull/photo_01")]
    [SerializeField] private string fullMediaResourcePath;
    [SerializeField] private bool resourceIsVideo;
    [Tooltip("Compatibilidad temporal. Déjalo vacío cuando uses la ruta de Resources.")]
    [SerializeField] private VideoClip videoClip;
    [SerializeField] private AlbumMediaViewer mediaViewer;

    public string PhotoId => photoId;

    private PhotoCollectionManager collectionManager;
    private bool isUnlocked;

    private void Awake()
    {
        photoId = photoId?.Trim();
        collectionManager = PhotoCollectionManager.Instance;

        if (collectionManager == null)
        {
            Debug.LogError("No se ha encontrado PhotoCollectionManager en la escena.", this);
            enabled = false;
            return;
        }

        if (!collectionManager.RegisterPhoto(photoId))
        {
            enabled = false;
        }

        mediaViewer ??= FindFirstObjectByType<AlbumMediaViewer>();
    }

    private void OnEnable()
    {
        if (collectionManager == null)
        {
            return;
        }

        collectionManager.PhotoStateChanged += OnPhotoStateChanged;
        RefreshState();
    }

    private void OnDisable()
    {
        if (collectionManager == null)
        {
            return;
        }

        collectionManager.PhotoStateChanged -= OnPhotoStateChanged;
        collectionManager.TryMarkAsSeen(photoId);
    }

    private void OnPhotoStateChanged(string changedPhotoId, PhotoState state)
    {
        if (changedPhotoId == photoId)
        {
            ApplyState(state);
        }
    }

    private void RefreshState()
    {
        if (collectionManager.TryGetPhotoState(photoId, out PhotoState state))
        {
            ApplyState(state);
        }
    }

    private void ApplyState(PhotoState state)
    {
        isUnlocked = state != PhotoState.Locked;

        if (unlockedPhoto != null)
        {
            unlockedPhoto.enabled = isUnlocked;
        }

        if (lockedOverlay != null)
        {
            lockedOverlay.SetActive(!isUnlocked);
        }

        if (newIndicator != null)
        {
            newIndicator.SetActive(state == PhotoState.New);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isUnlocked || mediaViewer == null)
        {
            return;
        }

        collectionManager.TryMarkAsSeen(photoId);

        if (!string.IsNullOrWhiteSpace(fullMediaResourcePath))
        {
            if (resourceIsVideo)
            {
                mediaViewer.ShowVideoFromResources(fullMediaResourcePath, mediaTitle);
            }
            else
            {
                mediaViewer.ShowImageFromResources(fullMediaResourcePath, mediaTitle);
            }
        }
        else if (videoClip != null)
        {
            mediaViewer.ShowVideo(videoClip, mediaTitle);
        }
        else if (unlockedPhoto != null && unlockedPhoto.sprite != null)
        {
            mediaViewer.ShowImage(unlockedPhoto.sprite, mediaTitle);
        }
    }
}
