using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class PinCounter : MonoBehaviour
{
    private int fallenPins = 0;
    private static int totalFallenPins = 0;
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

            // بررسی تعداد پین‌های افتاده
            if (fallenPins > 4)
            {
                // نمایش پنل مرحله بعدی 
                nextLevelPanel.SetActive(true);
                // نمایش پنل ری‌استارت 
                restartPanel.SetActive(false);
            }
            else
            {
                // نمایش پنل مرحله بعدی 
                nextLevelPanel.SetActive(false);
                // نمایش پنل ری‌استارت 
                restartPanel.SetActive(true);
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
        }
    }

    void UpdatePinCountText()
    {
        pinCountText.text = "Pins fallen : " + fallenPins + " / " + totalPins;
        totalPinCountText.text = "Total Pins fallen: " + totalFallenPins;
    }

    void RestartGame()
    {
        // ریست کردن تعداد پین‌های افتاده بدون از دست دادن امتیاز کل
        fallenPins = 0;
        countedPins.Clear();
        UpdatePinCountText();
        restartPanel.SetActive(false);
        nextLevelPanel.SetActive(false);
        // بارگذاری مجدد صحنه فعلی
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void GoToNextLevel()
    {
        // ذخیره‌سازی تعداد پین‌های افتاده و نمایش در TextMeshPro
        totalFallenPins += fallenPins;
        Debug.Log("Total Pins fallen: " + totalFallenPins);
        UpdatePinCountText();
        // بازنشانی تعداد پین‌های افتاده برای مرحله بعدی
        fallenPins = 0;
        SceneManager.LoadScene("GameScene1");
    }
}
