using UnityEngine;
using System.Diagnostics;
using System;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Keys")]
    public KeyCode esc = KeyCode.Escape;

    [Header("GameManager")]
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
        if (Input.GetKeyDown(esc) && SceneManager.GetActiveScene().name != "MainMenu")
        {
            TogglePause();
        }
    }

    public void AddScore(int add)
    {
        score += add;
    }

    public void GameClear()
    {
        StopStopwatch();

        totalScore = score + CalculateTimeScore();

        PlayerPrefs.SetInt("Score", totalScore);
    }

    public void GameOver()
    {
        StopStopwatch();

        totalScore = score + CalculateTimeScore();

        PlayerPrefs.SetInt("Score", totalScore);
    }

    public int CalculateTimeScore()
    {
        // 경과 시간을 초 단위로 계산
        float totalTimeInSeconds = (float)elapsed.TotalSeconds;

        // 특정 규칙에 따라 시간을 점수로 변환 (1초당 10점)
        int timeScore = Mathf.FloorToInt(totalTimeInSeconds) * 10;

        return timeScore;
    }

    public void StartStopwatch()
    {
        stopwatch.Start();
        gameStarted = true;
    }

    public void StopStopwatch()
    {
        stopwatch.Stop();
        gameStarted = false;

        // 스톱워치에서 경과된 시간 가져오기
        elapsed = stopwatch.Elapsed;
    }

    public void StartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("SampleScene");
        PlayerPrefs.SetInt("Score", 0);
        StartStopwatch();
    }

    public void SettingFunc()
    {
        UnityEngine.Debug.Log("Setting");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        isPaused = false;
        UIManager.instance.HidePauseMenu();
    }

    public void QuitToMain()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

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
