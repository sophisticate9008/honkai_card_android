using UnityEngine;

public class BeginSel : MonoBehaviour {
    private GameObject startGame;
    private GameObject enemyCards;

    private void Start() {
        startGame = GameObject.Find("ThreeCard");
        startGame.SetActive(false);
        enemyCards = GameObject.Find("enemy");
        enemyCards.SetActive(false);
        Application.runInBackground = true;
        // 非全屏模式
        Screen.fullScreen = false;
    }
    private void OnMouseDown() {
        startGame.SetActive(true);
        enemyCards.SetActive(true);
        gameObject.SetActive(false);
    }

    // public void reload() {
    //     gameObject.SetActive(true);
    //     startGame.SetActive(false);
    //     enemyCards.SetActive(false);
    // }
}