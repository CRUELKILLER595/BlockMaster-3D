using UnityEngine;

public class TwinFangs : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float rotationSpeed = 2f; 
    public float rotationAngle = 45f; // Maximum rotation angle in degrees
    // Speed of rotation in degrees per second
    // Update is called once per frame
    private float startAngle;
    void Start()
    {
        // Set the initial rotation to 45 degrees on the X-axis
        startAngle=transform.eulerAngles.z;
    }
    void Update()
    {
        // Calculate the current rotation angle based on time
       float angle = Mathf.PingPong(Time.time * rotationSpeed,rotationAngle);

        transform.rotation = Quaternion.Euler(45f, 0f, angle);
    }
}
