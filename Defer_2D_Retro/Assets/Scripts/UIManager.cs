using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Manager")]
    public static UIManager instance;

    [Header("PauseMenu")]
    public GameObject pauseMenu;
    public GameObject pauseMenuInstance;

    [Header("SettingMenu")]
    public GameObject settingMenu;
    public GameObject settingMenuInstance;

    [Header("GameOver")]
    public GameObject gameOver;
    public GameObject gameOverInstance;

    [Header("GameClear")]
    public GameObject gameClear;
    public GameObject gameClearInstance;

    [Header("Canvas")]
    public List<Canvas> allUI;

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

    /// <summary>
    /// ���۹�ư. GameManager�� StartGame�� ����
    /// </summary>
    public void StartButton()
    {
        GameManager.instance.StartGame();
    }

    /// <summary>
    /// ����(�ɼ�)��ư. GameManager�� SettingFunc�� ����
    /// </summary>
    public void Setting()
    {
        GameManager.instance.SettingFunc();
    }

    /// <summary>
    /// ���� ���� ��ư. GameManager�� QuitGame�� ����
    /// </summary>
    public void QuitGame()
    {
        GameManager.instance.QuitGame();
    }

    /// <summary>
    /// �̾��ϱ� ��ư. GameManager�� ResumeGame�� ����
    /// </summary>
    public void ResumeGame()
    {
        GameManager.instance.ResumeGame();
    }

    /// <summary>
    /// ���� �޴��� ���ư��� ��ư. GameManager�� QuitToMain�� ����
    /// </summary>
    public void QuitToMain()
    {
        GameManager.instance.QuitToMain();
    }

    /// <summary>
    /// �Ͻ����� ��ư. �Ͻ����� �޴��� ǥ���� �� ��� ��ư�� ã�� �� ��ư�� �̸��� �´� ����(���)�� �Ҵ�
    /// </summary>
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

    /// <summary>
    /// ���� ��ư. ���� �޴��� ǥ���� �� ��� ��ư�� �����̴��� ã�� �� �����̴��� ��ư�� �̸��� �´� ����(���)�� �Ҵ�. ���� �ش� UI �̿��� �ٸ� UI�� ��� ��Ȱ��ȭ ��Ŵ
    /// </summary>
    public void ShowSettingMenu()
    {
        allUI = new List<Canvas>(FindObjectsOfType<Canvas>());

        settingMenuInstance = Instantiate(settingMenu);

        // settingMenu ������ Slider ������Ʈ�� ��� ã��
        Slider[] sliders = settingMenuInstance.GetComponentsInChildren<Slider>();

        foreach(Canvas canvas in allUI)
        {
            if (canvas.transform.parent != null && canvas.transform.parent.name == "SettingMenu")
            {
                continue;
            }
            canvas.gameObject.SetActive(false);
        }

        foreach (Slider slider in sliders)
        {
            // �����̴��� �̸��� ���� ������ �Ҵ�
            switch (slider.name)
            {
                case "Master":
                    SoundManager.instance.masterSlider = slider;
                    slider.onValueChanged.AddListener(SoundManager.instance.SetMasterVolume);
                    break;
                case "BGM":
                    SoundManager.instance.bgmSlider = slider;
                    slider.onValueChanged.AddListener(SoundManager.instance.SetBGMVolume);
                    break;
                case "SFX":
                    SoundManager.instance.sfxSlider = slider;
                    slider.onValueChanged.AddListener(SoundManager.instance.SetSFXVolume);
                    break;
            }
        }

        // settingMenu ������ Button ������Ʈ�� ��� ã��
        Button[] buttons = settingMenuInstance.GetComponentsInChildren<Button>();

        foreach (Button button in buttons)
        {
            // ��ư�� �̸��� ���� ������ �Ҵ�
            switch (button.name)
            {
                case "BackButton":
                    button.onClick.AddListener(HideSettingMenu);
                    break;
            }
        }

        SoundManager.instance.SetSliderValue();
    }

    /// <summary>
    /// ���� �޴� ����(�ڷΰ��� ��ư). ShowSettingMenu���� ��Ȱ��ȭ�� ������Ʈ�� �ٽ� Ȱ��ȭ�ϰ� ���� UI ����
    /// </summary>
    public void HideSettingMenu()
    {
        foreach (Canvas canvas in allUI)
        {
            if (canvas.transform.parent != null && canvas.transform.parent.name == "SettingMenu")
            {
                continue;
            }
            canvas.gameObject.SetActive(true);
        }

        settingMenuInstance.SetActive(false);
        Destroy(settingMenuInstance);
    }

    /// <summary>
    /// �Ͻ����� ����
    /// </summary>
    public void HidePauseMenu()
    {
        pauseMenuInstance.SetActive(false);
        Destroy(pauseMenuInstance);
    }

    /// <summary>
    /// ���� ���� UI. ���� ���� UI�� ǥ���ϰ� �� ��ư�� �ؽ�Ʈ�� ã�� ��� �� �ؽ�Ʈ ���� �Ҵ�
    /// </summary>
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

    /// <summary>
    /// ���� Ŭ���� UI. ���� Ŭ���� UI�� ǥ���ϰ� �� ��ư�� �ؽ�Ʈ�� ã�� ��� �� �ؽ�Ʈ ���� �Ҵ�
    /// </summary>
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
