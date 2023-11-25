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

        // PauseMenu ������ Button ������Ʈ�� ��� ã��
        Button[] buttons = pauseMenuInstance.GetComponentsInChildren<Button>();

        foreach (Button button in buttons)
        {
            // ��ư�� �̸��� ���� ������ �Ҵ�
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

            // GameOver ������ Button ������Ʈ�� ��� ã��
            Button[] buttons = gameOverInstance.GetComponentsInChildren<Button>();

            // GameOver ������ TextMeshProUGUI ������Ʈ�� ��� ã��
            TextMeshProUGUI[] texts = gameOverInstance.GetComponentsInChildren<TextMeshProUGUI>();

            foreach (Button button in buttons)
            {
                // ��ư�� �̸��� ���� ������ �Ҵ�
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
                // �ؽ�Ʈ�� �̸��� ���� �ؽ�Ʈ ������ ����
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

            // GameOver ������ Button ������Ʈ�� ��� ã��
            Button[] buttons = gameClearInstance.GetComponentsInChildren<Button>();

            // GameOver ������ TextMeshProUGUI ������Ʈ�� ��� ã��
            TextMeshProUGUI[] texts = gameClearInstance.GetComponentsInChildren<TextMeshProUGUI>();

            foreach (Button button in buttons)
            {
                // ��ư�� �̸��� ���� ������ �Ҵ�
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
                // �ؽ�Ʈ�� �̸��� ���� �ؽ�Ʈ ������ ����
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
