using UnityEngine;

public class BowlingBallController : MonoBehaviour
{
    public float speed = 10f;
    public float maxLeft = -3f;
    public float maxRight = 3f;
    public float initialShotPower = 2600f;
    public float decelerationRate = 0.99f; // نرخ کاهش شتاب
    private bool isShot = false;
    private Rigidbody rb;
    private AudioSource audioSource;
    private bool audioPlayed = false;
    public Camera followCamera; // دوربین دنبال‌کننده
    public Camera mainCamera; // دوربین اصلی

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

        // بررسی اتصالات دوربین‌ها
        if (followCamera == null || mainCamera == null)
        {
            Debug.LogError("دوربین‌ها به درستی به اسکریپت متصل نشده‌اند!");
            return;
        }

        followCamera.enabled = true;
        mainCamera.enabled = false;
    }

    void Update()
    {
        if (!isShot)
        {
            float horizontal = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
            Vector3 newPosition = transform.position + new Vector3(horizontal, 0, 0);

            // محدود کردن حرکت به چپ و راست
            newPosition.x = Mathf.Clamp(newPosition.x, maxLeft, maxRight);
            transform.position = newPosition;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                isShot = true;
                rb.AddForce(Vector3.forward * initialShotPower);
            }
        }
        else
        {
            // کاهش شتاب در طول زمان
            // rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, rb.velocity.z * decelerationRate);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isShot)
        {
            Rigidbody otherRb = collision.collider.attachedRigidbody;
            if (otherRb != null)
            {
                // اعمال نیروی برخورد به شیء دیگر
                Vector3 impactForce = rb.velocity * rb.mass;
                otherRb.AddForce(impactForce, ForceMode.Impulse);
            }

            // پخش صدا هنگام برخورد
            if (audioSource != null && !audioPlayed)
            {
                audioSource.Play();
                audioPlayed = true;
            }

            // تغییر دوربین به مین‌کَمرا
            followCamera.enabled = false;
            mainCamera.enabled = true;
        }
    }
}
