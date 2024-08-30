
using UnityEngine;

public class CardAnimation : MonoBehaviour
{
    private Vector2 initialPosition;
    public float upDistance = 10.0f;
    public float downDistance = 10.0f; // 新增：下降距离
    public float animationSpeed = 5.0f;
    public bool isAnimating = false;
    private Vector2 targetPosition;

    private void Start()
    {
        initialPosition = transform.position;
        targetPosition = initialPosition;
    }

    private void Update()
    {
        if (isAnimating)
        {
            float step = animationSpeed * Time.deltaTime;

            if (Vector2.Distance(transform.position, targetPosition) < 0.01f)
            {
                // 切换方向
                if (targetPosition == initialPosition)
                {
                    targetPosition = initialPosition + Vector2.up * upDistance;
                }
                else
                {
                    targetPosition = initialPosition;
                }
            }

            transform.position = Vector2.MoveTowards(transform.position, targetPosition, step);
            if (Vector2.Distance(transform.position, initialPosition) < 0.01f && targetPosition == initialPosition) 
            {
                isAnimating = false;
            }
        }
    }

    // 启动上升动画
    public void StartAnimation()
    {
        isAnimating = true;
        if(targetPosition == initialPosition) {
            targetPosition = initialPosition + Vector2.up * upDistance;
        }
        
    }

    // 新增：启动下降动画
    public void StartDescentAnimation()
    {
        isAnimating = true;
        if(targetPosition == initialPosition) {
            targetPosition = initialPosition - Vector2.up * downDistance;
        }
    }
}
