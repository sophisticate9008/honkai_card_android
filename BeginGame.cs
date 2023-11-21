using System;
using System.Collections.Generic;
using System.Linq; // 引入 LINQ 命名空间
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BeginGame : MonoBehaviour
{
    List<int> order = new();
    CardSlot cardSlot;

    private void Awake()
    {
        cardSlot = GameObject.Find("CardSlot").GetComponent<CardSlot>();
    }

    private void OnMouseDown()
    {
        if (cardSlot.cardsInSlot.Count != 8)
        {
            return;
        }

        foreach (var card in cardSlot.cardsInSlot)
        {
            String text = card.name;
            int lastIndex = text.Length - 1; // 获取字符串最后一个字符的索引
            char lastChar = text[lastIndex]; // 获取字符串最后一个字符
            int lastInt = (int)Char.GetNumericValue(lastChar); // 将最后一个字符转换为整数
            order.Add(lastInt);
        }
        Debug.Log(order);
        // 使用OrderBy对字符串列表进行排序
        List<string> sortedList = new();
        foreach (var idx in order) {
            sortedList.Add(GameProcess.cardPacks[1][idx]);
        }

        // 用排序后的列表替换原始列表
        GameProcess.cardPacks[1] = sortedList;
        SceneManager.LoadScene("GameProcess");
    }
}
