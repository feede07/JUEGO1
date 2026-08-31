using System;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-100)]
public class GameInput : MonoBehaviour
{
    public static GameInput Instance { get; private set; }

    public event Action InteractPressed;
    public event Action AlbumPressed;

    public Vector2 Move => gameplayInputEnabled && moveAction != null
        ? moveAction.ReadValue<Vector2>()
        : Vector2.zero;

    public Vector2 Look => gameplayInputEnabled && lookAction != null
        ? lookAction.ReadValue<Vector2>()
        : Vector2.zero;

    public bool GameplayInputEnabled => gameplayInputEnabled;

    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction interactAction;
    private InputAction albumAction;
    private bool gameplayInputEnabled = true;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError("Solo puede existir un GameInput activo en la escena.", this);
            enabled = false;
            return;
        }

        Instance = this;
        moveAction = FindRequiredAction("Player/Move");
        lookAction = FindRequiredAction("Player/Look");
        interactAction = FindRequiredAction("Player/Interact");
        albumAction = FindRequiredAction("Player/Album");
    }

    private void OnEnable()
    {
        if (Instance != this)
        {
            return;
        }

        interactAction.performed += OnInteractPerformed;
        albumAction.performed += OnAlbumPerformed;

        SetGameplayInputEnabled(true);
        albumAction.Enable();
    }

    private void OnDisable()
    {
        if (Instance != this)
        {
            return;
        }

        interactAction.performed -= OnInteractPerformed;
        albumAction.performed -= OnAlbumPerformed;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void SetGameplayInputEnabled(bool enabled)
    {
        gameplayInputEnabled = enabled;
        SetActionEnabled(moveAction, enabled);
        SetActionEnabled(lookAction, enabled);
        SetActionEnabled(interactAction, enabled);
    }

    private InputAction FindRequiredAction(string actionPath)
    {
        InputAction action = InputSystem.actions.FindAction(actionPath);

        if (action == null)
        {
            throw new InvalidOperationException(
                $"No se ha encontrado la acción obligatoria '{actionPath}' en las acciones globales del proyecto.");
        }

        return action;
    }

    private static void SetActionEnabled(InputAction action, bool enabled)
    {
        if (enabled)
        {
            action.Enable();
        }
        else
        {
            action.Disable();
        }
    }

    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        if (gameplayInputEnabled)
        {
            InteractPressed?.Invoke();
        }
    }

    private void OnAlbumPerformed(InputAction.CallbackContext context)
    {
        AlbumPressed?.Invoke();
    }
}
