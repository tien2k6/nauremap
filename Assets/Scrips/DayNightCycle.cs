using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [SerializeField] private float dayDurationInSeconds = 600f; // Một ngày dài 60 giây

    void Update()
    {
        // Tính toán góc xoay dựa trên thời gian
        float rotationSpeed = 360f / dayDurationInSeconds;
        transform.Rotate(Vector3.right, rotationSpeed * Time.deltaTime);
    }
}