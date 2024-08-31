using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CardTextForward : MonoBehaviour
{
    private Vector2[] offsets = {
        new(0, -35),
        new(0, -55),
        new(-2, 25),
        new(0, 24)
    };

    public static List<Transform> objectToFollows = new();
    private int screenWidth;
    private int screenHeight;
    private Dictionary<Transform, Vector3> lastPositions = new();
    private Dictionary<Transform, Coroutine> updateCoroutines = new Dictionary<Transform, Coroutine>();
    private float updateInterval = 0.5f; // 更新间隔为1秒
    private void Start()
    {
        screenWidth = Screen.width;
        screenHeight = Screen.height;
        for (int i = 0; i < 4; i++)
        {
            offsets[i].x = offsets[i].x * screenWidth / 908;
            offsets[i].y = offsets[i].y * screenHeight / 511;
        }


    }

    void Update()
    {
        foreach (var objectToFollow in objectToFollows)
        {
            if (objectToFollow != null)
            {
                if (lastPositions.ContainsKey(objectToFollow))
                {
                    if (lastPositions[objectToFollow] != objectToFollow.position)
                    {
                        lastPositions[objectToFollow] = objectToFollow.position;

                        // 停止之前的协程（如果存在）
                        if (updateCoroutines.ContainsKey(objectToFollow))
                        {
                            StopCoroutine(updateCoroutines[objectToFollow]);
                            updateCoroutines.Remove(objectToFollow);
                        }

                        // 启动新的协程
                        var coroutine = StartCoroutine(UpdateUIRepeatedly(objectToFollow));
                        updateCoroutines[objectToFollow] = coroutine;
                    }
                }
                else
                {
                    lastPositions[objectToFollow] = objectToFollow.position;

                    // 启动新的协程
                    var coroutine = StartCoroutine(UpdateUIRepeatedly(objectToFollow));
                    updateCoroutines[objectToFollow] = coroutine;
                }
            }
        }
    }

    private void UpdateUI(Transform objectToFollow)
    {
        if (objectToFollow != null)
        {
            Transform canvasObj = objectToFollow.GetChild(0);
            Vector2 screenPos = Camera.main.WorldToScreenPoint(objectToFollow.position);
            for (int i = 0; i < 4; i++)
            {
                Text uiText = canvasObj.GetChild(i).GetComponent<Text>();
                uiText.rectTransform.position = screenPos + offsets[i];
                uiText.rectTransform.sizeDelta = new Vector2(Mathf.RoundToInt((float)screenWidth / 908 * 70) - 30, Mathf.RoundToInt((float)screenHeight / 511 * 90));
            }
        }
    }
    private IEnumerator UpdateUIRepeatedly(Transform objectToFollow)
    {
        float startTime = Time.time;
        while (Time.time - startTime < updateInterval)
        {
            UpdateUI(objectToFollow);
            yield return null;
        }
    }
    public static void BindTextForward(Transform obj)
    {
        objectToFollows.Add(obj);
        // Initialize last position

    }
}
