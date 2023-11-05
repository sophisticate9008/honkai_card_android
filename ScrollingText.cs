using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;

public class ScrollingText : MonoBehaviour
{
    private Text textPrefab;
    private Transform textContainer;
    public float scrollSpeed = 50f;

    public float CD = 0.05f;
    private float lastTime = 0.0f;
    

    private Queue<Text> textQueue = new Queue<Text>();
    private void Start() {
        textPrefab = GetComponent<Text>();
        textContainer = transform.parent;

    }
    private void Update()
    {
        int colorChange = 1;
        int upBeilv = 1;
        if(textQueue.Count >= 3) {
            colorChange = 12;
            upBeilv = 3;
        }
        // 移动和淡出现有文本
        foreach (Text text in textQueue)
        {
            text.rectTransform.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime * upBeilv;
            text.color -= new Color(0, 0, 0, colorChange) * Time.deltaTime;
            
            if(Time.time - lastTime > CD) {
                lastTime = Time.time;
            }else {
                break;
            }
            
        }

        // 删除已经淡出的文本
        while (textQueue.Count > 0 && textQueue.Peek().color.a <= 0)
        {
            Destroy(textQueue.Dequeue().gameObject);
        }
    }

    public void ShowScrollingText(string message)
    {
        Text newText = Instantiate(textPrefab, textContainer);
        newText.text = message;
        textQueue.Enqueue(newText);
    }
}