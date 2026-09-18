using TMPro;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class PhotoInteractionPrompt : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private PhotoInteractor photoInteractor;
    [SerializeField] private CanvasGroup promptGroup;
    [SerializeField] private TMP_Text promptText;

    [Header("Texto")]
    [SerializeField] private string message = "Pulsa E para recoger";

    private void Awake()
    {
        promptGroup ??= GetComponent<CanvasGroup>();
        promptText ??= GetComponentInChildren<TMP_Text>(true);

        if (photoInteractor == null)
        {
            photoInteractor = FindFirstObjectByType<PhotoInteractor>();
        }

        if (promptText != null)
        {
            promptText.text = message;
        }

        SetVisible(false);
    }

    private void Update()
    {
        bool gameplayInputEnabled = GameInput.Instance != null &&
                                    GameInput.Instance.GameplayInputEnabled;
        bool shouldBeVisible = gameplayInputEnabled &&
                               photoInteractor != null &&
                               photoInteractor.HasAvailablePhoto;

        SetVisible(shouldBeVisible);
    }

    private void OnDisable()
    {
        SetVisible(false);
    }

    private void SetVisible(bool visible)
    {
        promptGroup.alpha = visible ? 1f : 0f;
        promptGroup.interactable = false;
        promptGroup.blocksRaycasts = false;
    }

    private void Reset()
    {
        promptGroup = GetComponent<CanvasGroup>();
        promptText = GetComponentInChildren<TMP_Text>(true);
        photoInteractor = FindFirstObjectByType<PhotoInteractor>();
    }
}
