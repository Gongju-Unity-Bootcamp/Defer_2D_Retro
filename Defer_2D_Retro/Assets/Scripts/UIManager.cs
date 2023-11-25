using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Managers")]
    public static UIManager instance;

    [Header("PauseMenu")]
    public GameObject pauseMenu;
    public GameObject pauseMenuInstance;

    [Header("GameOver")]
    public GameObject gameOver;
    public GameObject gameOverInstance;

    [Header("GameClear")]
    public GameObject gameClear;
    public GameObject gameClearInstance;

    // Start is called before the first frame update
    void Start()
    {
        if (instance != null && instance != this)
        {
            Destroy(instance.gameObject);
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartButton()
    {
        GameManager.instance.StartGame();
    }

    public void Setting()
    {
        GameManager.instance.SettingFunc();
    }

    public void QuitGame()
    {
        GameManager.instance.QuitGame();
    }

    public void ResumeGame()
    {
        GameManager.instance.ResumeGame();
    }

    public void QuitToMain()
    {
        GameManager.instance.QuitToMain();
    }

    public void ShowPauseMenu()
    {
        pauseMenuInstance = Instantiate(pauseMenu);

        // PauseMenu 내에서 Button 컴포넌트를 모두 찾음
        Button[] buttons = pauseMenuInstance.GetComponentsInChildren<Button>();

        foreach (Button button in buttons)
        {
            // 버튼의 이름에 따라 동작을 할당
            switch (button.name)
            {
                case "ResumeButton":
                    button.onClick.AddListener(ResumeGame);
                    break;
                case "SettingButton":
                    button.onClick.AddListener(Setting);
                    break;
                case "QuitToMain":
                    button.onClick.AddListener(QuitToMain);
                    break;
            }
        }
    }

    public void HidePauseMenu()
    {
        pauseMenuInstance.SetActive(false);
        Destroy(pauseMenuInstance);
    }

    public void ShowGameOver()
    {
        if(gameOverInstance == null)
        {
            GameManager.instance.GameOver();

            gameOverInstance = Instantiate(gameOver);

            // GameOver 내에서 Button 컴포넌트를 모두 찾음
            Button[] buttons = gameOverInstance.GetComponentsInChildren<Button>();

            // GameOver 내에서 TextMeshProUGUI 컴포넌트를 모두 찾음
            TextMeshProUGUI[] texts = gameOverInstance.GetComponentsInChildren<TextMeshProUGUI>();

            foreach (Button button in buttons)
            {
                // 버튼의 이름에 따라 동작을 할당
                switch (button.name)
                {
                    case "RestartButton":
                        button.onClick.AddListener(StartButton);
                        break;
                    case "QuitToMain":
                        button.onClick.AddListener(QuitToMain);
                        break;
                }
            }

            foreach (TextMeshProUGUI text in texts)
            {
                // 텍스트의 이름에 따라 텍스트 내용을 변경
                switch (text.name)
                {
                    case "Score":
                        text.text = $"Score: {GameManager.instance.totalScore:D4}";
                        break;
                    case "Time":
                        text.text = $"Time: {GameManager.instance.elapsed.Hours:D2}:{GameManager.instance.elapsed.Minutes:D2}:{GameManager.instance.elapsed.Seconds:D2}";
                        break;
                    case "Monster":
                        text.text = $"Monster: {GameManager.instance.score}";
                        break;
                }
            }
        }
    }

    public void ShowGameClear()
    {
        if (gameClearInstance == null)
        {
            GameManager.instance.GameClear();

            gameClearInstance = Instantiate(gameClear);

            // GameOver 내에서 Button 컴포넌트를 모두 찾음
            Button[] buttons = gameClearInstance.GetComponentsInChildren<Button>();

            // GameOver 내에서 TextMeshProUGUI 컴포넌트를 모두 찾음
            TextMeshProUGUI[] texts = gameClearInstance.GetComponentsInChildren<TextMeshProUGUI>();

            foreach (Button button in buttons)
            {
                // 버튼의 이름에 따라 동작을 할당
                switch (button.name)
                {
                    case "RestartButton":
                        button.onClick.AddListener(StartButton);
                        break;
                    case "QuitToMain":
                        button.onClick.AddListener(QuitToMain);
                        break;
                }
            }

            foreach (TextMeshProUGUI text in texts)
            {
                // 텍스트의 이름에 따라 텍스트 내용을 변경
                switch (text.name)
                {
                    case "Score":
                        text.text = $"Score: {GameManager.instance.totalScore:D4}";
                        break;
                    case "Time":
                        text.text = $"Time: {GameManager.instance.elapsed.Hours:D2}:{GameManager.instance.elapsed.Minutes:D2}:{GameManager.instance.elapsed.Seconds:D2}";
                        break;
                    case "Monster":
                        text.text = $"Monster: {GameManager.instance.score}";
                        break;
                }
            }
        }
    }
}
