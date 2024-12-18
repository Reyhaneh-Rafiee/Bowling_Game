using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // شیء مورد نظر برای دنبال کردن
    public Vector3 offset; // فاصله دوربین از شیء

    void LateUpdate()
    {
        if (target != null)
        {
            transform.position = target.position + offset;
        }
    }
}
