using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class PinCounter : MonoBehaviour
{
    private int fallenPins = 0;
    private static int totalFallenPins = 0; // مجموع پین‌های افتاده در تمام مراحل
    private static int currentThrows = 0; // متغیر برای پیگیری تعداد پرتاب‌های جاری

    public string pinTag = "Pin"; // تگ پین‌ها
    public string ballTag = "Ball"; // تگ توپ
    public TMP_Text pinCountText; // متن UI برای نمایش تعداد پین‌ها
    public TMP_Text totalPinCountText; // متن UI برای نمایش تعداد کل پین‌ها
    public int totalPins = 10; // کل تعداد پین‌ها

    public GameObject restartPanel; // پنل ری‌استارت
    public GameObject nextLevelPanel; // پنل مرحله بعدی
    public Button restartButton; // دکمه ری‌استارت
    public Button nextLevelButton; // دکمه مرحله بعدی
    private AudioSource audioSource; // منبع صدا

    private HashSet<Collider> countedPins = new HashSet<Collider>(); // لیست برای پیگیری پین‌های شمارش‌شده

    private bool ballThrown = false; // متغیر برای پیگیری پرتاب توپ

void Start()
{
    UpdatePinCountText();
    audioSource = GetComponent<AudioSource>();

    // پنهان کردن پنل‌ها در شروع بازی
    restartPanel.SetActive(false);
    nextLevelPanel.SetActive(false);

    // اضافه کردن listener به دکمه‌ها
    if (restartButton != null)
    {
        restartButton.onClick.AddListener(RestartGame);
        Debug.Log("Restart Button Listener Added");
    }
    if (nextLevelButton != null)
    {
        nextLevelButton.onClick.AddListener(OnNextLevelClicked);
        Debug.Log("Next Level Button Listener Added");
    }

    // بازگرداندن Time.timeScale به ۱
    Time.timeScale = 1;
}



    void Update()
    {
        // بررسی برخورد توپ با پین‌ها
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

            // پخش صدا هنگام اولین برخورد
            if (audioSource != null)
            {
                audioSource.Play();
            }
        }

        if (other.CompareTag(ballTag))
        {
            ballThrown = true; // تنظیم پرتاب توپ
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(ballTag))
        {
            if (fallenPins > 0)
            {
                totalFallenPins += fallenPins; // به‌روزرسانی امتیاز کل
                fallenPins = 0; // بازنشانی پین‌های افتاده برای پرتاب بعدی

                UpdatePinCountText();

                // اگر تعداد پرتاب‌ها کمتر از 2 باشد، صحنه لود شود
                if (currentThrows < 2)
                {
                    SceneManager.sceneLoaded += OnSceneLoaded;
                    if (SceneManager.GetActiveScene() != null)
                    {
                        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                    }
                }
                else
                {
                    Time.timeScale = 0; // توقف بازی
                    if (nextLevelPanel != null)
                    {
                        ShowNextLevelPanel();
                    }
                }
            }
            else
            {
                ShowRestartPanel();
            }

            ballThrown = false; // تنظیم پرتاب توپ برای پرتاب بعدی
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // بازگرداندن وضعیت بازی برای پرتاب دوم
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Time.timeScale = 1;
        UpdatePinCountText(); // به روز رسانی متن پین‌ها

        currentThrows++; // افزایش تعداد پرتاب‌ها
        Debug.Log("Current Throws: " + currentThrows); // اضافه کردن لاگ برای بررسی تعداد پرتاب‌ها
    }

    void UpdatePinCountText()
    {
        pinCountText.text = "Pins fallen: " + fallenPins + " / " + totalPins;
        totalPinCountText.text = "Total Pins fallen: " + totalFallenPins;
    }

    void ShowRestartPanel()
    {
        if (restartPanel != null)
        {
            Debug.Log("Showing Restart Panel"); // اضافه کردن لاگ
            restartPanel.SetActive(true);
        }
        if (nextLevelPanel != null)
        {
            nextLevelPanel.SetActive(false);
        }
        Time.timeScale = 0;
    }

    void ShowNextLevelPanel()
    {
        if (nextLevelPanel != null)
        {
            Debug.Log("Showing Next Level Panel"); // اضافه کردن لاگ
            nextLevelPanel.SetActive(true);
        }
        if (restartPanel != null)
        {
            restartPanel.SetActive(false);
        }
        Time.timeScale = 0;
    }

    void ResetFallenPins()
    {
        fallenPins = 0;
        countedPins.Clear();
    }

void RestartGame()
{
    Debug.Log("Restart Game Button Clicked");
    Time.timeScale = 1;
    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
}

void OnNextLevelClicked()
{
    Debug.Log("Next Level Button Clicked");
    Time.timeScale = 1;
    currentThrows = 0;
    
    string currentSceneName = SceneManager.GetActiveScene().name;
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


}
