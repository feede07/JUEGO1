using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PhotoCollectible : MonoBehaviour
{
    [SerializeField] private string photoId;

    public string PhotoId => photoId;
    public bool IsAvailable => isActiveAndEnabled && !isCollected;

    private PhotoCollectionManager collectionManager;
    private bool isCollected;

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

        if (!collectionManager.RegisterCollectible(photoId))
        {
            enabled = false;
        }
    }

    public bool Collect()
    {
        if (!IsAvailable || !collectionManager.TryUnlockPhoto(photoId))
        {
            return false;
        }

        isCollected = true;
        gameObject.SetActive(false);
        return true;
    }

    private void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
    }
}
