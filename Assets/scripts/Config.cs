using UnityEngine;

public class Config : MonoBehaviour
{
    void Awake()
    {
        // 防止这个对象在场景切换时被销毁
        DontDestroyOnLoad(gameObject);

        // 设置目标帧率为 60
        Application.targetFrameRate = 60;
    }
}