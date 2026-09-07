using System;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-90)]
public class PhotoCollectionManager : MonoBehaviour
{
    public static PhotoCollectionManager Instance { get; private set; }

    public event Action<string, PhotoState> PhotoStateChanged;
    public event Action<int, int> ProgressChanged;
    public event Action CollectionCompleted;

    public int TotalCount => photoStates.Count;
    public int UnlockedCount { get; private set; }
    public bool IsComplete => TotalCount > 0 && UnlockedCount == TotalCount;

    private readonly Dictionary<string, PhotoState> photoStates =
        new(StringComparer.Ordinal);
    private readonly HashSet<string> registeredCollectibleIds =
        new(StringComparer.Ordinal);
    private bool completionNotified;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError("Solo puede existir un PhotoCollectionManager activo en la escena.", this);
            enabled = false;
            return;
        }

        Instance = this;
        InitializeCatalog();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public bool TryUnlockPhoto(string photoId)
    {
        if (!photoStates.TryGetValue(photoId, out PhotoState currentState))
        {
            Debug.LogError($"La foto '{photoId}' no existe en el catálogo.", this);
            return false;
        }

        if (currentState != PhotoState.Locked)
        {
            return false;
        }

        photoStates[photoId] = PhotoState.New;
        UnlockedCount++;
        PhotoStateChanged?.Invoke(photoId, PhotoState.New);
        ProgressChanged?.Invoke(UnlockedCount, TotalCount);

        if (IsComplete && !completionNotified)
        {
            completionNotified = true;
            CollectionCompleted?.Invoke();
        }

        return true;
    }

    public bool RegisterCollectible(string photoId)
    {
        photoId = photoId?.Trim();

        if (string.IsNullOrEmpty(photoId))
        {
            Debug.LogError("Un coleccionable no tiene identificador de foto.", this);
            return false;
        }

        if (!registeredCollectibleIds.Add(photoId))
        {
            Debug.LogError($"Hay más de un coleccionable con el identificador '{photoId}'.", this);
            return false;
        }

        return RegisterPhoto(photoId);
    }

    public bool RegisterPhoto(string photoId)
    {
        photoId = photoId?.Trim();

        if (string.IsNullOrEmpty(photoId))
        {
            Debug.LogError("No se puede registrar una foto sin identificador.", this);
            return false;
        }

        if (!photoStates.ContainsKey(photoId))
        {
            Debug.LogError(
                $"La foto '{photoId}' no pertenece al catálogo photo_01-photo_86.", this);
            return false;
        }

        return true;
    }

    public bool TryMarkAsSeen(string photoId)
    {
        if (!photoStates.TryGetValue(photoId, out PhotoState currentState)
            || currentState != PhotoState.New)
        {
            return false;
        }

        photoStates[photoId] = PhotoState.Seen;
        PhotoStateChanged?.Invoke(photoId, PhotoState.Seen);
        return true;
    }

    public bool TryGetPhotoState(string photoId, out PhotoState state)
    {
        return photoStates.TryGetValue(photoId, out state);
    }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
    [ContextMenu("DEBUG/Desbloquear las 86 fotos")]
    private void DebugUnlockAllPhotos()
    {
        for (int photoNumber = 1; photoNumber <= AlbumCatalogLayout.TotalPhotos; photoNumber++)
        {
            string photoId = AlbumCatalogLayout.FormatPhotoId(photoNumber);
            photoStates[photoId] = PhotoState.Seen;
        }

        UnlockedCount = TotalCount;
        ProgressChanged?.Invoke(UnlockedCount, TotalCount);

        if (!completionNotified)
        {
            completionNotified = true;
            CollectionCompleted?.Invoke();
        }
    }
#endif

    private void InitializeCatalog()
    {
        photoStates.Clear();
        registeredCollectibleIds.Clear();
        UnlockedCount = 0;
        completionNotified = false;

        for (int photoNumber = 1; photoNumber <= AlbumCatalogLayout.TotalPhotos; photoNumber++)
        {
            string photoId = AlbumCatalogLayout.FormatPhotoId(photoNumber);
            photoStates.Add(photoId, PhotoState.Locked);
        }
    }
}
