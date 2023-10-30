using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FollowObject : MonoBehaviour
{
    public Vector3 offset = new Vector3(0, -10, 0);
    public Transform objectToFollow; // 你要跟随的对象
    public Text uiText; // UI Text组件
    public Canvas canvas; // UI Canvas

    void Update()
    {
        if (objectToFollow != null && uiText != null && canvas != null)
        {
            // 设置UI Text的位置为对象的世界坐标
            Vector3 screenPos = Camera.main.WorldToScreenPoint(objectToFollow.position);
            uiText.rectTransform.position = screenPos + offset;

            // 设置UI Text的尺寸，可根据需要自定义
            uiText.rectTransform.sizeDelta = new Vector2(70, 90); // 调整尺寸以适应你的需求
        }
    }
}
