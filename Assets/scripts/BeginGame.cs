using System;
using System.Collections.Generic;
using System.Linq; // 引入 LINQ 命名空间
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;
public class BeginGame : MonoBehaviour
{
    List<int> order = new();
    CardSlot cardSlot;
    WebRequestManager requestManager = new();

    private void Awake()
    {
        cardSlot = GameObject.Find("CardSlot").GetComponent<CardSlot>();
    }
    private void Start() {
        
        if(BeginSel.modeSel == "onLine") {
            GetComponent<Button>().onClick.AddListener(GameMatch);
        }else {
            GetComponent<Button>().onClick.AddListener(GameBegin);
        }
        
    }
    private void GameBegin()
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
        if(BeginSel.modeSel == "offLine") {
            Roles.life_init = PlayerPrefs.GetInt("bloodCustom");
            SceneManager.LoadScene("GameProcess");  
        }else {
            GameMatch();
        }
    }
    private string CardsToString() {
        string strCards = "";
        foreach (var cardStr in GameProcess.cardPacks[1]) {
            strCards += cardStr;
            strCards += ";";
        }
        return strCards;
    }
    private string NicknameCat() {
        string playerName = PlayerPrefs.GetString("playerName");
        string roleName = GameProcess.roleSelList[0];
        return playerName + ";" + roleName;
    }
    private Action<MyData> DataHandle = (myData) =>
    {
        Roles.life_init = 1000000;
        string[] cards = myData.card_str.Split(";", StringSplitOptions.RemoveEmptyEntries);
        List<string> cardPack = cards.ToList();
        GameProcess.cardPacks[0] = cardPack;
        string[] names = myData.nickname.Split(";");
        Data.enemyName = names[0];
        GameProcess.roleSelList[1] = names[1];
        SceneManager.LoadScene("GameProcess");
    };
    private void GameMatch() {

        string url = "https://honkai-card-honkai-card-rhbdvhegec.cn-hangzhou.fcapp.run/game_match";
        
        //先请求一下激活云函数
        StartCoroutine(requestManager.SendPostRequest(url, "",
            (response) => Debug.Log("POST Success: " + response), 
            (error) => Debug.LogError("POST Error: " + error)));
        
        MyData myData = new MyData
        {
           nickname = NicknameCat(),
           card_str = CardsToString()
        };
        string jsonData = JsonUtility.ToJson(myData);
        Debug.Log(jsonData);
        StartCoroutine(requestManager.SendPostRequest(url, jsonData, DataHandle,
            (error) => Debug.LogError("POST Error: " + error)));
    }
}
