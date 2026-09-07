using TMPro;
using UnityEngine;

public class FinalAlbumPage : MonoBehaviour
{
    [Header("Estados visuales")]
    [SerializeField] private GameObject lockedContent;
    [SerializeField] private GameObject completedContent;
    [SerializeField] private TMP_Text lockedProgressText;
    [SerializeField] private string lockedFormat = "Completa el álbum para desbloquear esta página\n{0} / {1} fotos";

    private PhotoCollectionManager collectionManager;

    private void Awake()
    {
        collectionManager = PhotoCollectionManager.Instance;

        if (collectionManager == null)
        {
            Debug.LogError("No se ha encontrado PhotoCollectionManager en la escena.", this);
            enabled = false;
        }
    }

    private void OnEnable()
    {
        if (collectionManager == null)
        {
            return;
        }

        collectionManager.ProgressChanged += OnProgressChanged;
        collectionManager.CollectionCompleted += Refresh;
        Refresh();
    }

    private void OnDisable()
    {
        if (collectionManager == null)
        {
            return;
        }

        collectionManager.ProgressChanged -= OnProgressChanged;
        collectionManager.CollectionCompleted -= Refresh;
    }

    private void OnProgressChanged(int unlocked, int total)
    {
        Refresh();
    }

    private void Refresh()
    {
        bool isComplete = collectionManager.IsComplete;

        if (lockedContent != null)
        {
            lockedContent.SetActive(!isComplete);
        }

        if (completedContent != null)
        {
            completedContent.SetActive(isComplete);
        }

        if (lockedProgressText != null)
        {
            lockedProgressText.text = string.Format(
                lockedFormat,
                collectionManager.UnlockedCount,
                collectionManager.TotalCount);
        }
    }
}
