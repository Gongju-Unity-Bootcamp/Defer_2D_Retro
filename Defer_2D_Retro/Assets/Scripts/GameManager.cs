using UnityEngine;
using System.Diagnostics;
using System;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Keys")]
    public KeyCode esc = KeyCode.Escape;

    [Header("Manager")]
    public static GameManager instance;

    [Header("Score")]
    public int score = 0;
    public int totalScore = 0;

    [Header("StopWatch")]
    public Stopwatch stopwatch;
    public bool gameStarted = false;
    public TimeSpan elapsed;

    [Header("Bools")]
    public bool isPaused = false;

    // Start is called before the first frame update
    void Start()
    {
        if (instance != null && instance != this)
        {
            Destroy(instance.gameObject);
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        stopwatch = new Stopwatch();
    }

    // Update is called once per frame
    void Update()
    {
        // esc가 눌렸을때, 현재 씬이 메인메뉴가 아닐때
        if (Input.GetKeyDown(esc) && SceneManager.GetActiveScene().name != "MainMenu" && UIManager.instance.settingMenuInstance == null)
        {
            TogglePause();
        }
    }

    /// <summary>
    /// 점수를 add만큼 추가하는 함수
    /// </summary>
    /// <param name="add"></param>
    public void AddScore(int add)
    {
        score += add;
    }

    /// <summary>
    /// 게임 클리어 시 실행하는 함수. 점수와 시간 점수를 합산하여 totalScore로 저장 및 PlayerPrefs의 Score에 저장
    /// </summary>
    public void GameClear()
    {
        StopStopwatch();

        totalScore = score + CalculateTimeScore();

        PlayerPrefs.SetInt("Score", totalScore);
    }

    /// <summary>
    /// 게임 오버 시 실행하는 함수. 점수와 시간 점수를 합산하여 totalScore로 저장 및 PlayerPrefs의 Score에 저장
    /// </summary>
    public void GameOver()
    {
        StopStopwatch();

        totalScore = score + CalculateTimeScore();

        PlayerPrefs.SetInt("Score", totalScore);
    }

    /// <summary>
    /// 시간 당 점수를 계산하는 함수. 1초당 10점
    /// </summary>
    /// <returns></returns>
    public int CalculateTimeScore()
    {
        // 경과 시간을 초 단위로 계산
        float totalTimeInSeconds = (float)elapsed.TotalSeconds;

        // 특정 규칙에 따라 시간을 점수로 변환 (1초당 10점)
        int timeScore = Mathf.FloorToInt(totalTimeInSeconds) * 10;

        return timeScore;
    }

    /// <summary>
    /// 스톱워치를 시작하는 함수
    /// </summary>
    public void StartStopwatch()
    {
        stopwatch.Start();
        gameStarted = true;
    }

    /// <summary>
    /// 스톱워치를 중지하는 함수
    /// </summary>
    public void StopStopwatch()
    {
        stopwatch.Stop();
        gameStarted = false;

        // 스톱워치에서 경과된 시간 가져오기
        elapsed = stopwatch.Elapsed;
    }

    /// <summary>
    /// 게임을 시작할때 실행하는 함수. 첫 스테이지 씬을 불러오고 점수를 초기화.
    /// </summary>
    public void StartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("SampleScene");
        PlayerPrefs.SetInt("Score", 0);
        StartStopwatch();
    }

    /// <summary>
    /// 설정 기능 함수. UIManager의 ShowSettingMenu를 실행
    /// </summary>
    public void SettingFunc()
    {
        UIManager.instance.ShowSettingMenu();
    }

    /// <summary>
    /// 게임 종료 함수.
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }

    /// <summary>
    /// 게임 이어하기 함수. 일시정지된 상태에서 게임을 다시 재개하도록 한다.
    /// </summary>
    public void ResumeGame()
    {
        Time.timeScale = 1f;
        isPaused = false;
        UIManager.instance.HidePauseMenu();
    }

    /// <summary>
    /// 메인 메뉴로 돌아가는 함수. 메인메뉴 씬을 불러온다.
    /// </summary>
    public void QuitToMain()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// 일시정지 함수.
    /// </summary>
    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f;
            UIManager.instance.ShowPauseMenu();
        }
        else
        {
            Time.timeScale = 1f;
            UIManager.instance.HidePauseMenu();
        }
    }
}
