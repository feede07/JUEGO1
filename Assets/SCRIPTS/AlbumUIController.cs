using UnityEngine;

public class AlbumUIController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private AlbumPauseController pauseController;
    [SerializeField] private GameObject albumRoot;

    private void Awake()
    {
        pauseController ??= FindFirstObjectByType<AlbumPauseController>();

        if (pauseController == null)
        {
            Debug.LogError("No se ha encontrado AlbumPauseController en la escena.", this);
            enabled = false;
            return;
        }

        if (albumRoot == null)
        {
            Debug.LogError("No se ha asignado el panel raíz del álbum.", this);
            enabled = false;
            return;
        }

        albumRoot.SetActive(pauseController.IsAlbumOpen);
    }

    private void OnEnable()
    {
        if (pauseController != null)
        {
            pauseController.AlbumStateChanged += OnAlbumStateChanged;
        }
    }

    private void Start()
    {
        if (pauseController != null && albumRoot != null)
        {
            albumRoot.SetActive(pauseController.IsAlbumOpen);
        }
    }

    private void OnDisable()
    {
        if (pauseController != null)
        {
            pauseController.AlbumStateChanged -= OnAlbumStateChanged;
        }
    }

    private void OnAlbumStateChanged(bool isOpen)
    {
        albumRoot.SetActive(isOpen);
    }
}
