using TMPro;
using UnityEngine;

public class AlbumProgressUI : MonoBehaviour
{
    [SerializeField] private TMP_Text progressText;
    [SerializeField] private string format = "{0} / {1} fotos";

    private PhotoCollectionManager collectionManager;

    private void Awake()
    {
        progressText ??= GetComponent<TMP_Text>();
        collectionManager = PhotoCollectionManager.Instance;

        if (progressText == null || collectionManager == null)
        {
            Debug.LogError("No se puede inicializar el contador de progreso del álbum.", this);
            enabled = false;
        }
    }

    private void OnEnable()
    {
        if (collectionManager != null)
        {
            collectionManager.ProgressChanged += UpdateProgress;
            UpdateProgress(collectionManager.UnlockedCount, collectionManager.TotalCount);
        }
    }

    private void OnDisable()
    {
        if (collectionManager != null)
        {
            collectionManager.ProgressChanged -= UpdateProgress;
        }
    }

    private void UpdateProgress(int unlocked, int total)
    {
        progressText.text = string.Format(format, unlocked, total);
    }
}
