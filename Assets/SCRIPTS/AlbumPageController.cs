using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AlbumPageController : MonoBehaviour
{
    public event Action<int> PageChanged;

    [Header("Referencias")]
    [SerializeField] private AlbumPauseController pauseController;
    [SerializeField] private TMP_Text pageCounterText;

    [Header("Páginas")]
    [SerializeField] private List<GameObject> pages = new();

    public int CurrentPageIndex { get; private set; }
    public int PageCount => pages.Count;

    private GameInput gameInput;

    private void Awake()
    {
        gameInput = GameInput.Instance;
        pauseController ??= FindFirstObjectByType<AlbumPauseController>();

        if (gameInput == null || pauseController == null)
        {
            Debug.LogError("No se han encontrado los controladores necesarios para navegar por el álbum.", this);
            enabled = false;
        }
    }

    private void OnEnable()
    {
        if (gameInput == null)
        {
            return;
        }

        gameInput.PreviousPressed += ShowPreviousPage;
        gameInput.NextPressed += ShowNextPage;
    }

    private void Start()
    {
        if (pages.Count == 0)
        {
            Debug.LogWarning("El álbum no tiene páginas asignadas.", this);
            UpdatePageCounter();
            return;
        }

        SetPage(0, true);
    }

    private void OnDisable()
    {
        if (gameInput == null)
        {
            return;
        }

        gameInput.PreviousPressed -= ShowPreviousPage;
        gameInput.NextPressed -= ShowNextPage;
    }

    public void ShowPreviousPage()
    {
        if (pauseController.IsAlbumOpen)
        {
            SetPage(CurrentPageIndex - 1);
        }
    }

    public void ShowNextPage()
    {
        if (pauseController.IsAlbumOpen)
        {
            SetPage(CurrentPageIndex + 1);
        }
    }

    public void SetPage(int pageIndex)
    {
        SetPage(pageIndex, false);
    }

    private void SetPage(int pageIndex, bool forceRefresh)
    {
        if (pages.Count == 0)
        {
            return;
        }

        int clampedIndex = Mathf.Clamp(pageIndex, 0, pages.Count - 1);
        if (!forceRefresh && clampedIndex == CurrentPageIndex)
        {
            return;
        }

        CurrentPageIndex = clampedIndex;

        for (int i = 0; i < pages.Count; i++)
        {
            if (pages[i] != null)
            {
                pages[i].SetActive(i == CurrentPageIndex);
            }
        }

        UpdatePageCounter();
        PageChanged?.Invoke(CurrentPageIndex);
    }

    private void UpdatePageCounter()
    {
        if (pageCounterText != null)
        {
            pageCounterText.text = pages.Count == 0
                ? "0 / 0"
                : $"{CurrentPageIndex + 1} / {pages.Count}";
        }
    }
}
