using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class PinCounter : MonoBehaviour
{
    private int fallenPins = 0;
    private static int totalFallenPins = 0;
    private static int currentThrows = 0;
    private bool ballThrown = false;
    private static int stageFallenPins = 0;
    private static bool nextLevelClicked = false;

    public string pinTag = "Pin";
    public string ballTag = "Ball";
    public TMP_Text pinCountText;
    public TMP_Text totalPinCountText;
    public TMP_Text bestScoreText;
    public int totalPins = 10;

    public GameObject restartPanel;
    public GameObject nextLevelPanel;
    public GameObject lastPanel;
    public Button restartButton;
    public Button playAgainButton;
    public Button nextLevelButton;
    public Button exitButton;
    private static AudioSource audioSource;
    private static bool musicPlaying = false;

    private HashSet<Collider> countedPins = new HashSet<Collider>();

    public AudioClip scene1Music;
    public AudioClip scene2Music;
    public AudioClip scene3Music;

    void Awake()
    {
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            DontDestroyOnLoad(audioSource);
        }
    }

    void Start()
    {
        UpdatePinCountText();
        SetPanelVisibility(false, false, false);
        PlaySceneMusic();
        AddButtonListeners();
        EnableButtons();
        Time.timeScale = 1;
    }

    void Update()
    {
        if (ballThrown && fallenPins == 0)
        {
            ShowRestartPanel();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(pinTag) && !countedPins.Contains(other))
        {
            fallenPins++;
            countedPins.Add(other);
            UpdatePinCountText();
        }
        if (other.CompareTag(ballTag))
        {
            ballThrown = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(ballTag))
        {
            if (fallenPins > 0)
            {
                stageFallenPins += fallenPins;
                totalFallenPins += fallenPins;  // اضافه کردن امتیازات به مجموع
                fallenPins = 0;
                countedPins.Clear();
                UpdatePinCountText();
            }
            currentThrows++;
            if (currentThrows < 2)
            {
                SceneManager.sceneLoaded += OnSceneLoaded;
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            else
            {
                string currentSceneName = SceneManager.GetActiveScene().name;
                if (currentSceneName == "GameScene2")
                {
                    ShowLastPanel();
                }
                else
                {
                    ShowNextLevelPanel();
                }
                nextLevelClicked = true;
            }
            ballThrown = false;
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Time.timeScale = 1;
        EnableButtons();
        UpdatePinCountText();
        ResetFallenPins();
        Debug.Log("Current Throws: " + currentThrows);
    }

    void UpdatePinCountText()
    {
        if (pinCountText != null)
            pinCountText.text = "Pins fallen: " + fallenPins + " / " + totalPins;

        if (totalPinCountText != null)
            totalPinCountText.text = "Total Pins fallen: " + totalFallenPins;
    }

    void ShowRestartPanel()
    {
        if (restartPanel != null)
        {
            restartPanel.SetActive(true);
        }
        SetPanelVisibility(restartPanel != null, false, false);
        Time.timeScale = 0;
    }

    void ShowNextLevelPanel()
    {
        if (nextLevelPanel != null)
        {
            nextLevelPanel.SetActive(true);
            UpdatePinCountText();
        }
        SetPanelVisibility(false, nextLevelPanel != null, false);
        Time.timeScale = 0;
    }

    void ShowLastPanel()
    {
        if (lastPanel != null)
        {
            lastPanel.SetActive(true);
            UpdatePinCountText();
            bestScoreText.text = "Best Score: " + totalFallenPins;  // نمایش مجموع امتیازات در Best Score
        }
        SetPanelVisibility(false, false, lastPanel != null);
        Time.timeScale = 0;
    }

    void SetPanelVisibility(bool showRestart, bool showNextLevel, bool showLast)
    {
        if (restartPanel != null)
            restartPanel.SetActive(showRestart);

        if (nextLevelPanel != null)
            nextLevelPanel.SetActive(showNextLevel);

        if (lastPanel != null)
            lastPanel.SetActive(showLast);
    }

    void ResetFallenPins()
    {
        fallenPins = 0;
        countedPins.Clear();
    }

    void EnableButtons()
    {
        if (restartButton != null)
            restartButton.interactable = true;

        if (playAgainButton != null)
            playAgainButton.interactable = true;

        if (nextLevelButton != null)
            nextLevelButton.interactable = true;

        if (exitButton != null)
            exitButton.interactable = true;
    }

    void RestartGame()
    {
        Debug.Log("Restart Game Button Clicked");
        ResetGame();
        totalFallenPins = 0;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void PlayAgain()
    {
        Debug.Log("Play Again Button Clicked");
        ResetGame();
        totalFallenPins = 0;
        SceneManager.LoadScene("GameScene");
    }

    void OnNextLevelClicked()
    {
        Debug.Log("Next Level Button Clicked");
        nextLevelClicked = false;
        ResetGame();
        StopCurrentMusic();
        currentThrows = 0;
        fallenPins = 0;
        string currentSceneName = SceneManager.GetActiveScene().name;
        Debug.Log("Current Scene: " + currentSceneName);

        if (currentSceneName == "GameScene")
        {
            SceneManager.LoadScene("GameScene1");
        }
        else if (currentSceneName == "GameScene1")
        {
            SceneManager.LoadScene("GameScene2");
        }
        else if (currentSceneName == "GameScene2")
        {
            Debug.Log("Last level reached");
        }
    }

    void ExitGame()
    {
        Debug.Log("Exit Game Button Clicked");
        Application.Quit();
    }

    void PlaySceneMusic()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        AudioClip clipToPlay = null;

        switch (currentSceneName)
        {
            case "GameScene":
                clipToPlay = scene1Music;
                break;
            case "GameScene1":
                clipToPlay = scene2Music;
                break;
            case "GameScene2":
                clipToPlay = scene3Music;
                break;
        }

        if (clipToPlay != null && (!audioSource.isPlaying || audioSource.clip != clipToPlay))
        {
            audioSource.clip = clipToPlay;
            audioSource.Play();
            musicPlaying = true;
        }
    }

    void StopCurrentMusic()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
            musicPlaying = false;
        }
    }

    void AddButtonListeners()
    {
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
            Debug.Log("Restart Button Listener Added");
        }
        else
        {
            Debug.LogError("Restart Button is NULL!");
        }

        if (playAgainButton != null)
        {
            playAgainButton.onClick.AddListener(PlayAgain);
            Debug.Log("Play Again Button Listener Added");
        }
        else
        {
            Debug.LogError("Play Again Button is NULL!");
        }

        if (nextLevelButton != null)
        {
            nextLevelButton.onClick.AddListener(OnNextLevelClicked);
            Debug.Log("Next Level Button Listener Added");
        }
        else
        {
            Debug.LogError("Next Level Button is NULL!");
        }

        if (exitButton != null)
        {
            exitButton.onClick.AddListener(ExitGame);
            Debug.Log("Exit Button Listener Added");
        }
        else
        {
            Debug.LogError("Exit Button is NULL!");
        }
    }

    void ResetGame()
    {
        currentThrows = 0;
        countedPins.Clear();
        ballThrown = false;
        fallenPins = 0;
        stageFallenPins = 0;
    }
}
