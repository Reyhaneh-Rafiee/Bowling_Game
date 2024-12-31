using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class PinCounter : MonoBehaviour
{
    private int fallenPins = 0;
    private static int totalFallenPins = 0;
    private int currentThrows = 0; // متغیر برای پیگیری تعداد پرتاب‌های جاری

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

    void Start()
    {
        UpdatePinCountText();
        audioSource = GetComponent<AudioSource>();

        // پنهان کردن پنل‌ها در شروع بازی
        restartPanel.SetActive(false);
        nextLevelPanel.SetActive(false);

        // اضافه کردن listener به دکمه‌ها
        restartButton.onClick.AddListener(RestartGame);
        nextLevelButton.onClick.AddListener(GoToNextLevel);

        // بازگرداندن Time.timeScale به ۱
        Time.timeScale = 1;
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
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(ballTag))
        {
            totalFallenPins += fallenPins; // به‌روزرسانی امتیاز کل

            if (fallenPins > 0)
            {
                UpdatePinCountText();
                ResetFallenPins(); // صفر کردن پین‌ها برای پرتاب بعدی
                currentThrows++;

                // اگر دو پرتاب انجام شده باشد و امتیازات بیشتر از 10 باشد
                if (currentThrows >= 2)
                {
                    if (totalFallenPins > 10)
                    {
                        Time.timeScale = 0; // توقف بازی
                        ShowNextLevelPanel();
                    }
                    else
                    {
                        ShowRestartPanel();
                        totalFallenPins = 0; // ریست کردن امتیازات
                        currentThrows = 0; // ریست کردن تعداد پرتاب‌ها
                    }
                }
                else
                {
                    // بازنشانی صحنه برای پرتاب بعدی
                    SceneManager.sceneLoaded += OnSceneLoaded;
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }
            }
            else
            {
                ShowRestartPanel();
                totalFallenPins = 0; // ریست کردن امتیازات
                currentThrows = 0; // ریست کردن تعداد پرتاب‌ها
            }
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // بازگرداندن وضعیت بازی برای پرتاب دوم
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Time.timeScale = 1;
    }

    void UpdatePinCountText()
    {
        pinCountText.text = "Pins fallen: " + fallenPins + " / " + totalPins;
        totalPinCountText.text = "Total Pins fallen: " + totalFallenPins;
    }

    void ShowRestartPanel()
    {
        restartPanel.SetActive(true);
        nextLevelPanel.SetActive(false);
        Time.timeScale = 0;
    }

    void ShowNextLevelPanel()
    {
        nextLevelPanel.SetActive(true);
        restartPanel.SetActive(false);
        Time.timeScale = 0;
    }

    void ResetFallenPins()
    {
        fallenPins = 0;
        countedPins.Clear();
    }

    void RestartGame()
    {
        // بازگرداندن Time.timeScale به ۱
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void GoToNextLevel()
    {
        // بازگرداندن Time.timeScale به ۱
        Time.timeScale = 1;
        SceneManager.LoadScene("GameScene1"); // تغییر نام صحنه بر اساس نیاز
    }
}
