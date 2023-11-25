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
    /// 시작버튼. GameManager의 StartGame을 실행
    /// </summary>
    public void StartButton()
    {
        GameManager.instance.StartGame();
    }

    /// <summary>
    /// 세팅(옵션)버튼. GameManager의 SettingFunc를 실행
    /// </summary>
    public void Setting()
    {
        GameManager.instance.SettingFunc();
    }

    /// <summary>
    /// 게임 종료 버튼. GameManager의 QuitGame을 실행
    /// </summary>
    public void QuitGame()
    {
        GameManager.instance.QuitGame();
    }

    /// <summary>
    /// 이어하기 버튼. GameManager의 ResumeGame을 실행
    /// </summary>
    public void ResumeGame()
    {
        GameManager.instance.ResumeGame();
    }

    /// <summary>
    /// 메인 메뉴로 돌아가기 버튼. GameManager의 QuitToMain을 실행
    /// </summary>
    public void QuitToMain()
    {
        GameManager.instance.QuitToMain();
    }

    /// <summary>
    /// 일시정지 버튼. 일시정지 메뉴를 표시한 뒤 모든 버튼을 찾아 각 버튼의 이름에 맞는 동작(기능)을 할당
    /// </summary>
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

    /// <summary>
    /// 설정 버튼. 설정 메뉴를 표시한 뒤 모든 버튼과 슬라이더를 찾아 각 슬라이더와 버튼의 이름에 맞는 동작(기능)을 할당. 또한 해당 UI 이외의 다른 UI를 모두 비활성화 시킴
    /// </summary>
    public void ShowSettingMenu()
    {
        allUI = new List<Canvas>(FindObjectsOfType<Canvas>());

        settingMenuInstance = Instantiate(settingMenu);

        // settingMenu 내에서 Slider 컴포넌트를 모두 찾음
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
            // 슬라이더의 이름에 따라 동작을 할당
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

        // settingMenu 내에서 Button 컴포넌트를 모두 찾음
        Button[] buttons = settingMenuInstance.GetComponentsInChildren<Button>();

        foreach (Button button in buttons)
        {
            // 버튼의 이름에 따라 동작을 할당
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
    /// 설정 메뉴 제거(뒤로가기 버튼). ShowSettingMenu에서 비활성화된 오브젝트를 다시 활성화하고 설정 UI 제거
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
    /// 일시정지 제거
    /// </summary>
    public void HidePauseMenu()
    {
        pauseMenuInstance.SetActive(false);
        Destroy(pauseMenuInstance);
    }

    /// <summary>
    /// 게임 오버 UI. 게임 오버 UI를 표시하고 각 버튼과 텍스트를 찾아 기능 및 텍스트 값을 할당
    /// </summary>
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

    /// <summary>
    /// 게임 클리어 UI. 게임 클리어 UI를 표시하고 각 버튼과 텍스트를 찾아 기능 및 텍스트 값을 할당
    /// </summary>
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
