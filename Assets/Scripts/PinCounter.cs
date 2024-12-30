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
            currentThrows++;
            totalFallenPins += fallenPins;
            UpdatePinCountText();
            ResetFallenPins(); 

           

       

            // پرتاب دوم: بررسی امتیاز کل
            if (currentThrows == 2)
            {
 // اگر هیچ پینی سقوط نکرده باشد
            if (fallenPins == 0)
            {
                
                ShowRestartPanel();
                
            }
                else if (totalFallenPins >= 10)
                {
                    ShowNextLevelPanel();
                }
                else
                {
                    ShowRestartPanel();
                }
                Time.timeScale = 0; // متوقف کردن بازی
            }
            else
            {
                // بازی ادامه می‌یابد و منتظر پرتاب دوم می‌ماند
            }
        }
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
        currentThrows = 0; // ریست شمارش پرتاب‌ها
    }

    void ShowNextLevelPanel()
    {
        nextLevelPanel.SetActive(true);
        restartPanel.SetActive(false);
    }

    void ResetFallenPins()
    {
        fallenPins = 0;
        countedPins.Clear(); // لیست پین‌های شمرده شده را پاک می‌کنیم
    }

    void RestartGame()
    {
        // بازنشانی مقیاس زمان به حالت عادی و بارگذاری مجدد صحنه فعلی
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void GoToNextLevel()
    {
        // بازنشانی مقیاس زمان به حالت عادی و بارگذاری سطح بعدی
        Time.timeScale = 1;
        SceneManager.LoadScene("GameScene1"); // نام صحنه را بر اساس نیاز خود تغییر دهید
    }
}
