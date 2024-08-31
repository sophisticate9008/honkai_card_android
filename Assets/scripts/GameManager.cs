using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject pausePanel; // 引用暂停面板
    public int sel;
    Text playerName1;
    Text playerName2;
    private void Start() {
        if(BeginSel.modeSel == "onLine") {
            playerName1 = GameObject.Find("PlayerName1").GetComponent<Text>();
            playerName2 = GameObject.Find("PlayerName2").GetComponent<Text>();
            playerName1.text = PlayerPrefs.GetString("playerName");
            playerName2.text = Data.enemyName;
        }
        try {
            if(sel == 0) {
                GetComponent<Button>().onClick.AddListener(ResumeGame);
                pausePanel.SetActive(false);
            }else if(sel == 1) {
                GetComponent<Button>().onClick.AddListener(RestartGame);
            }
        }catch(Exception e) {
            Debug.Log(e);
        }
        
    }
    void Update()
    {
        if(sel == 0) {
            if (Input.GetKeyDown(KeyCode.Escape)) // 按下 Escape 键暂停游戏
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0f; // 暂停游戏时间流逝
        pausePanel.SetActive(true); // 显示暂停面板
        
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f; // 恢复游戏时间流逝
        pausePanel.SetActive(false); // 隐藏暂停面板
    }
    public void RestartGame() {
        Time.timeScale = 1f;
        GameEnd.EndGame();
    }
}
