using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-80)]
public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject menuRoot;

    private static bool startImmediatelyAfterReload;

    private GameInput gameInput;
    private float previousTimeScale = 1f;
    private bool isMenuOpen;

    private void Awake()
    {
        menuRoot ??= gameObject;
        gameInput = GameInput.Instance;

        if (gameInput == null)
        {
            Debug.LogError("No se ha encontrado GameInput para controlar el menú inicial.", this);
            enabled = false;
            return;
        }

        if (startImmediatelyAfterReload)
        {
            startImmediatelyAfterReload = false;
            StartGameplay();
            return;
        }

        ShowMenu();
    }

    public void ContinueGame()
    {
        StartGameplay();
    }

    public void StartNewGame()
    {
        PhotoCollectionManager collectionManager = PhotoCollectionManager.Instance;

        if (collectionManager == null)
        {
            Debug.LogError("No se ha encontrado PhotoCollectionManager para borrar el progreso.", this);
            return;
        }

        if (!collectionManager.DeleteSavedProgress())
        {
            return;
        }

        startImmediatelyAfterReload = true;
        Time.timeScale = 1f;

        Scene activeScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(activeScene.name);
    }

    private void ShowMenu()
    {
        isMenuOpen = true;
        previousTimeScale = Time.timeScale;
        Time.timeScale = 0f;

        menuRoot.SetActive(true);
        gameInput.SetGameplayInputEnabled(false);
        gameInput.SetAlbumNavigationEnabled(false);
        gameInput.SetAlbumInputEnabled(false);
    }

    private void StartGameplay()
    {
        isMenuOpen = false;
        menuRoot.SetActive(false);

        Time.timeScale = previousTimeScale > 0f ? previousTimeScale : 1f;
        gameInput.SetAlbumNavigationEnabled(false);
        gameInput.SetGameplayInputEnabled(true);
        gameInput.SetAlbumInputEnabled(true);
    }

    private void OnDestroy()
    {
        if (isMenuOpen)
        {
            Time.timeScale = 1f;
        }
    }
}
