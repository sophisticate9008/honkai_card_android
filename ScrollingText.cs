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
    private float upBeilv = 1.2f;
    private float colorChange = 1.5f;
    private float CD = 0.1f;
    private List<Text> textList = new();
    private List<Text> runningList = new();
    private void Start() {
        textPrefab = GetComponent<Text>();
        textContainer = transform.parent;
        InvokeRepeating("RepeatShowText", 0.0f, CD);
    }
    private void Update()
    {

       // 删除已经淡出的文本
    }
    private void RepeatShowText()
    {
        foreach(var text in textList) {
            if(!runningList.Contains(text)) {
                StartCoroutine(ChangeTextProperties(text));
                textList.Remove(text);
                break;
            }
        }
    }
    IEnumerator ChangeTextProperties(Text text)
    {
        runningList.Add(text);
        text.enabled = true;
        while (text.color.a > 0)
        {
            text.rectTransform.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime * upBeilv;
            text.color -= new Color(0, 0, 0, colorChange) * Time.deltaTime;
            yield return null;
        }
        runningList.Remove(text);
        Destroy(text.gameObject);
    }
    public void ShowScrollingText(string message)
    {
        Text newText = Instantiate(textPrefab, textContainer);
        newText.enabled = false;
        newText.text = message;
        textList.Add(newText);
    }
}