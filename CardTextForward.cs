using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardTextForward : MonoBehaviour
{
    //mana star des name
    private Vector2[] offsets = {
        new(0, -35),
        new(0, -55),
        new(-2, 25),
        new(0, 24)
    };

    public static List<Transform> objectToFollows = new();
    private int screenWidth;
    private int screenHeight;

    private void Start() {
        screenWidth = Screen.width;
        screenHeight = Screen.height;
        for(int i = 0; i < 4; i++) {
            offsets[i].x = offsets[i].x * screenWidth / 908;
            offsets[i].y = offsets[i].y * screenHeight / 511;
        }

    }
    void Update()
    {
        
        foreach (var objectToFollow in objectToFollows) {
            if(objectToFollow != null) {
                Transform canvasObj = objectToFollow.GetChild(0);
                    // 设置UI Text的位置为对象的世界坐标
                Vector2 screenPos = Camera.main.WorldToScreenPoint(objectToFollow.position);
                for(int i = 0; i < 4; i++) {
                    Text uiText = canvasObj.GetChild(i).GetComponent<Text>();
                    uiText.rectTransform.position = screenPos + offsets[i];
                    // 设置UI Text的尺寸，可根据需要自定义
                    uiText.rectTransform.sizeDelta = new Vector2(Mathf.RoundToInt((float)screenWidth / 908 * 70), Mathf.RoundToInt((float)screenHeight / 511 * 90));
                    // 调整尺寸以适应你的需求                
                }
            }


                      
        }

    }
    public static void BindTextForward(Transform obj) {
        objectToFollows.Add(obj);
    }
}