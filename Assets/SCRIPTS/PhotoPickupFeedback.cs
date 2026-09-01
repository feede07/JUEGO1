using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(AudioSource))]
public class PhotoPickupFeedback : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private CanvasGroup notificationGroup;
    [SerializeField] private TMP_Text notificationText;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip pickupClip;

    [Header("Presentación")]
    [SerializeField] private string messageFormat = "Foto recogida: {0}";
    [SerializeField, Min(0f)] private float visibleDuration = 1.5f;
    [SerializeField, Min(0.01f)] private float fadeDuration = 0.35f;

    private Coroutine hideRoutine;

    private void Awake()
    {
        notificationGroup ??= GetComponent<CanvasGroup>();
        notificationText ??= GetComponentInChildren<TMP_Text>(true);
        audioSource ??= GetComponent<AudioSource>();

        notificationGroup.alpha = 0f;
        notificationGroup.interactable = false;
        notificationGroup.blocksRaycasts = false;
        audioSource.playOnAwake = false;
    }

    private void OnEnable()
    {
        if (PhotoCollectionManager.Instance == null)
        {
            Debug.LogError("No se ha encontrado PhotoCollectionManager en la escena.", this);
            enabled = false;
            return;
        }

        PhotoCollectionManager.Instance.PhotoStateChanged += OnPhotoStateChanged;
    }

    private void OnDisable()
    {
        if (PhotoCollectionManager.Instance != null)
        {
            PhotoCollectionManager.Instance.PhotoStateChanged -= OnPhotoStateChanged;
        }
    }

    private void OnPhotoStateChanged(string photoId, PhotoState state)
    {
        if (state != PhotoState.New)
        {
            return;
        }

        ShowPickup(photoId);
    }

    private void ShowPickup(string photoId)
    {
        if (notificationText != null)
        {
            notificationText.text = string.Format(messageFormat, photoId);
        }

        notificationGroup.alpha = 1f;

        if (pickupClip != null)
        {
            audioSource.PlayOneShot(pickupClip);
        }

        if (hideRoutine != null)
        {
            StopCoroutine(hideRoutine);
        }

        hideRoutine = StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSecondsRealtime(visibleDuration);

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            notificationGroup.alpha = 1f - Mathf.Clamp01(elapsed / fadeDuration);
            yield return null;
        }

        notificationGroup.alpha = 0f;
        hideRoutine = null;
    }

    private void Reset()
    {
        notificationGroup = GetComponent<CanvasGroup>();
        notificationText = GetComponentInChildren<TMP_Text>(true);
        audioSource = GetComponent<AudioSource>();
    }
}
