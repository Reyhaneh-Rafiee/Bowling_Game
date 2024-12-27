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

    public string quadTag = "Quad"; // تگ زمین یا ظرف (Quad)
    public string pinTag = "Pin"; // تگ پین‌ها
    public string ballTag = "Ball"; // تگ توپ برای جلوگیری از شمارش برخورد
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
            Debug.Log("Pins fallen: " + fallenPins);
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
            // اگر توپ به هیچ پینی برخورد نکرد، پنل ری‌استارت نمایش داده شود
            if (fallenPins == 0)
            {
                restartPanel.SetActive(true);
            }
            else
            {
                restartPanel.SetActive(false);
            }

            // پس از خروج توپ، شمارش پرتاب‌ها افزایش می‌یابد
            currentThrows++;
            if (currentThrows ==2)
            {
                // اگر دو پرتاب انجام شده باشد، پنل مرحله بعدی نمایش داده می‌شود
                totalFallenPins += fallenPins; // به‌روزرسانی امتیاز کل
                nextLevelPanel.SetActive(true);
                restartPanel.SetActive(false);
                UpdatePinCountText(); // به‌روزرسانی متن امتیاز
                RestartGame();
                
            }
           else
          {
                totalFallenPins += fallenPins;
                RestartGame(); 
          }
        }
    }

    void UpdatePinCountText()
    {
        pinCountText.text = "Pins fallen: " + fallenPins + " / " + totalPins;
        totalPinCountText.text = "Total Pins fallen: " + totalFallenPins;
    }

    void RestartGame()
    {
        // ریست کردن شمارش پرتاب‌ها و تعداد پین‌های افتاده بدون از دست دادن امتیاز کل
        fallenPins = 0;
        countedPins.Clear();
        //currentThrows = 0; // ریست شماره پرتاب‌ها
        UpdatePinCountText();
        restartPanel.SetActive(false);
        nextLevelPanel.SetActive(false);
        
        // بارگذاری مجدد صحنه فعلی
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void GoToNextLevel()
    {
        // بارگذاری سطح بعدی
        SceneManager.LoadScene("GameScene1"); // نام صحنه را بر اساس نیاز خود تغییر دهید
    }
}