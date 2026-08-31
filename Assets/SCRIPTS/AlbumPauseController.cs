using System;
using UnityEngine;

[RequireComponent(typeof(GameInput))]
public class AlbumPauseController : MonoBehaviour
{
    public event Action<bool> AlbumStateChanged;

    public bool IsAlbumOpen { get; private set; }

    private GameInput gameInput;
    private float previousTimeScale = 1f;

    private void Awake()
    {
        gameInput = GetComponent<GameInput>();
    }

    private void OnEnable()
    {
        gameInput.AlbumPressed += ToggleAlbum;
    }

    private void OnDisable()
    {
        gameInput.AlbumPressed -= ToggleAlbum;

        if (IsAlbumOpen)
        {
            SetAlbumOpen(false);
        }
    }

    public void ToggleAlbum()
    {
        SetAlbumOpen(!IsAlbumOpen);
    }

    public void SetAlbumOpen(bool open)
    {
        if (IsAlbumOpen == open)
        {
            return;
        }

        IsAlbumOpen = open;

        if (open)
        {
            previousTimeScale = Time.timeScale;
            Time.timeScale = 0f;
            gameInput.SetGameplayInputEnabled(false);
        }
        else
        {
            Time.timeScale = previousTimeScale;
            gameInput.SetGameplayInputEnabled(true);
        }

        AlbumStateChanged?.Invoke(IsAlbumOpen);
    }
}
