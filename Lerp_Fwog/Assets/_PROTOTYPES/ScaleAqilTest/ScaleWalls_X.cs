using UnityEngine;

public class ScaleWalls : MonoBehaviour
{
    public enum ScaleAxis { X, Y }  // Enum to define the scaling axis
    [SerializeField] private ScaleAxis scalingAxis = ScaleAxis.X;  // Default to X-axis scaling

    [SerializeField] private float baseScalingSpeed = 1.0f;  // Base speed at which the object scales
    [SerializeField] private float maxScalingSpeed = 10.0f;  // Maximum scaling speed limit
    [SerializeField] private float speedIncreaseRate = 1.0f;  // Rate at which the scaling speed increases
    [SerializeField] private float minScale = 0.1f;  // Minimum scale limit
    [SerializeField] private float maxScale = 3.0f;  // Maximum scale limit

    private Vector3 initialScale;
    private bool isMouseOver = false;  // To track if the mouse is over the sprite
    private float currentScalingSpeed;

    void Start()
    {
        initialScale = transform.localScale;  // Store the initial scale
        currentScalingSpeed = baseScalingSpeed;  // Initialize scaling speed
    }

    void Update()
    {
        if (isMouseOver)  // Only scale if the mouse is over the object
        {
            Vector3 scale = transform.localScale;

            if (Input.GetMouseButton(0))  // LMB held down, scale up
            {
                currentScalingSpeed = Mathf.Min(currentScalingSpeed + speedIncreaseRate * Time.deltaTime, maxScalingSpeed);
                if (scalingAxis == ScaleAxis.X)
                {
                    scale.x += currentScalingSpeed * Time.deltaTime;  // Scale along the x-axis
                }
                else if (scalingAxis == ScaleAxis.Y)
                {
                    scale.y += currentScalingSpeed * Time.deltaTime;  // Scale along the y-axis
                }
            }
            else if (Input.GetMouseButton(1))  // RMB held down, scale down
            {
                currentScalingSpeed = Mathf.Min(currentScalingSpeed + speedIncreaseRate * Time.deltaTime, maxScalingSpeed);
                if (scalingAxis == ScaleAxis.X)
                {
                    scale.x -= currentScalingSpeed * Time.deltaTime;  // Scale along the x-axis
                }
                else if (scalingAxis == ScaleAxis.Y)
                {
                    scale.y -= currentScalingSpeed * Time.deltaTime;  // Scale along the y-axis
                }
            }
            else
            {
                currentScalingSpeed = baseScalingSpeed;  // Reset scaling speed when mouse button is released
            }

            // Clamp the scale to stay within the defined limits
            if (scalingAxis == ScaleAxis.X)
            {
                scale.x = Mathf.Clamp(scale.x, minScale, maxScale);
            }
            else if (scalingAxis == ScaleAxis.Y)
            {
                scale.y = Mathf.Clamp(scale.y, minScale, maxScale);
            }

            transform.localScale = scale;
        }
    }

    void OnMouseOver()
    {
        isMouseOver = true;  // Set flag to true when mouse is over the sprite
    }

    void OnMouseExit()
    {
        isMouseOver = false;  // Reset flag when mouse leaves the sprite
        currentScalingSpeed = baseScalingSpeed;  // Reset scaling speed when the mouse exits
    }
}