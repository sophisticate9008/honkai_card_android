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

    private Queue<Text> textQueue = new Queue<Text>();
    private void Start() {
        textPrefab = GetComponent<Text>();
        textContainer = transform.parent;
    }
    private void Update()
    {
        
        // 移动和淡出现有文本
        foreach (Text text in textQueue)
        {
            text.rectTransform.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;
            text.color -= new Color(0, 0, 0, 1) * Time.deltaTime;
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