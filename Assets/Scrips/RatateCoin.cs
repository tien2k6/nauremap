using UnityEngine;

public class RotateCoin : MonoBehaviour
{
    [Header("Cài đặt xoay")]
    public float rotateSpeed = 100f; // Tốc độ xoay (độ/giây)

    void Update()
{
    // Xoay
    transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);

    // Nhấp nhô lên xuống bằng hàm Sin
    float newY = transform.position.y + Mathf.Sin(Time.time * 3f) * 0.002f;
    transform.position = new Vector3(transform.position.x, newY, transform.position.z);
}
}