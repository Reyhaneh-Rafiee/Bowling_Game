using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PinCounter : MonoBehaviour
{
    private int fallenPins = 0;
    public string quadTag = "Quad"; // تگ زمین یا ظرف (Quad)
    public string pinTag = "Pin"; // تگ پین‌ها
    public string ballTag = "Ball"; // تگ توپ برای جلوگیری از شمارش برخورد
    public TMP_Text pinCountText; // متن UI برای نمایش تعداد پین‌ها
    public int totalPins = 10; // کل تعداد پین‌ها
    private bool audioPlayed = false; // فلگ برای پیگیری پخش صدا
    private AudioSource audioSource; // منبع صدا

    private HashSet<Collider> countedPins = new HashSet<Collider>(); // لیست برای پیگیری پین‌های شمارش‌شده

    void Start()
    {
        UpdatePinCountText();
        audioSource = GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(pinTag) && !countedPins.Contains(other) && !other.CompareTag(ballTag))
        {
            fallenPins++;
            countedPins.Add(other);
            Debug.Log("Pins fallen: " + fallenPins);
            UpdatePinCountText();

            // پخش صدا هنگام اولین برخورد
            if (audioSource != null && !audioPlayed)
            {
                audioSource.Play();
                audioPlayed = true;
            }
        }
    }

    void UpdatePinCountText()
    {
        pinCountText.text = "Pins fallen : " + fallenPins + " / " + totalPins;
    }
}
