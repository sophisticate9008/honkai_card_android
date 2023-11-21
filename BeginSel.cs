using UnityEngine;

public class BeginSel : MonoBehaviour {
    private GameObject startGame;
    private GameObject enemyCards;
    private void Start() {
        startGame = GameObject.Find("ThreeCard");
        startGame.SetActive(false);
        enemyCards = GameObject.Find("enemy");
        enemyCards.SetActive(false);
        
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