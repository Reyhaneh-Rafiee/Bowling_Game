using UnityEngine;

public class BowlingBallController : MonoBehaviour
{
    public float speed = 10f;
    public float initialShotPower = 2600f;
    public float decelerationRate = 0.99f; // نرخ کاهش شتاب
    private bool isShot = false;
    private Rigidbody rb;
    private AudioSource audioSource;
    private bool audioPlayed = false;
    public Camera followCamera; // دوربین دنبال‌کننده
    public Camera mainCamera; // دوربین اصلی
    public BoxCollider playArea; // زمین بازی

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

        // بررسی اتصالات دوربین‌ها
        if (followCamera == null || mainCamera == null)
        {
            Debug.LogError("Error,Fixed the camera");
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

            // محدود کردن حرکت به محدوده زمین
            Vector3 playAreaMin = playArea.bounds.min;
            Vector3 playAreaMax = playArea.bounds.max;
            newPosition.x = Mathf.Clamp(newPosition.x, playAreaMin.x, playAreaMax.x);
            newPosition.z = Mathf.Clamp(newPosition.z, playAreaMin.z, playAreaMax.z);
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

                // پخش صدا هنگام برخورد
                if (audioSource != null && !audioPlayed)
                {
                    audioSource.Play();
                    audioPlayed = true;
                }
            }

            // تغییر دوربین به مین‌کَمرا
            followCamera.enabled = false;
            mainCamera.enabled = true;
        }
    }
}
