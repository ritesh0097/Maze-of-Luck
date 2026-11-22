using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameCanvasScript : MonoBehaviour
{
    [SerializeField] GameObject gamePausePanel;
    [SerializeField] GameObject pauseButton;
    [SerializeField] GameObject resumeButton;

    void Start()
    {
        EventSystem.current.SetSelectedGameObject(pauseButton);
    }

    public void GamePaused()
    {
        Time.timeScale = 0f;
        gamePausePanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(resumeButton);
    }

    public void GameContinue()
    {
        Time.timeScale = 1f;
        gamePausePanel.SetActive(false);
        EventSystem.current.SetSelectedGameObject(pauseButton);
    }

    public void GameRestart()
    {
        Time.timeScale = 1f;        
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }

    public void GotoMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

}
