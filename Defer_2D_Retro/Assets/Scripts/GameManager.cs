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
        // esc�� ��������, ���� ���� ���θ޴��� �ƴҶ�
        if (Input.GetKeyDown(esc) && SceneManager.GetActiveScene().name != "MainMenu" && UIManager.instance.settingMenuInstance == null)
        {
            TogglePause();
        }
    }

    /// <summary>
    /// ������ add��ŭ �߰��ϴ� �Լ�
    /// </summary>
    /// <param name="add"></param>
    public void AddScore(int add)
    {
        score += add;
    }

    /// <summary>
    /// ���� Ŭ���� �� �����ϴ� �Լ�. ������ �ð� ������ �ջ��Ͽ� totalScore�� ���� �� PlayerPrefs�� Score�� ����
    /// </summary>
    public void GameClear()
    {
        StopStopwatch();

        totalScore = score + CalculateTimeScore();

        PlayerPrefs.SetInt("Score", totalScore);
    }

    /// <summary>
    /// ���� ���� �� �����ϴ� �Լ�. ������ �ð� ������ �ջ��Ͽ� totalScore�� ���� �� PlayerPrefs�� Score�� ����
    /// </summary>
    public void GameOver()
    {
        StopStopwatch();

        totalScore = score + CalculateTimeScore();

        PlayerPrefs.SetInt("Score", totalScore);
    }

    /// <summary>
    /// �ð� �� ������ ����ϴ� �Լ�. 1�ʴ� 10��
    /// </summary>
    /// <returns></returns>
    public int CalculateTimeScore()
    {
        // ��� �ð��� �� ������ ���
        float totalTimeInSeconds = (float)elapsed.TotalSeconds;

        // Ư�� ��Ģ�� ���� �ð��� ������ ��ȯ (1�ʴ� 10��)
        int timeScore = Mathf.FloorToInt(totalTimeInSeconds) * 10;

        return timeScore;
    }

    /// <summary>
    /// �����ġ�� �����ϴ� �Լ�
    /// </summary>
    public void StartStopwatch()
    {
        stopwatch.Start();
        gameStarted = true;
    }

    /// <summary>
    /// �����ġ�� �����ϴ� �Լ�
    /// </summary>
    public void StopStopwatch()
    {
        stopwatch.Stop();
        gameStarted = false;

        // �����ġ���� ����� �ð� ��������
        elapsed = stopwatch.Elapsed;
    }

    /// <summary>
    /// ������ �����Ҷ� �����ϴ� �Լ�. ù �������� ���� �ҷ����� ������ �ʱ�ȭ.
    /// </summary>
    public void StartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("SampleScene");
        PlayerPrefs.SetInt("Score", 0);
        StartStopwatch();
    }

    /// <summary>
    /// ���� ��� �Լ�. UIManager�� ShowSettingMenu�� ����
    /// </summary>
    public void SettingFunc()
    {
        UIManager.instance.ShowSettingMenu();
    }

    /// <summary>
    /// ���� ���� �Լ�.
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }

    /// <summary>
    /// ���� �̾��ϱ� �Լ�. �Ͻ������� ���¿��� ������ �ٽ� �簳�ϵ��� �Ѵ�.
    /// </summary>
    public void ResumeGame()
    {
        Time.timeScale = 1f;
        isPaused = false;
        UIManager.instance.HidePauseMenu();
    }

    /// <summary>
    /// ���� �޴��� ���ư��� �Լ�. ���θ޴� ���� �ҷ��´�.
    /// </summary>
    public void QuitToMain()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// �Ͻ����� �Լ�.
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
