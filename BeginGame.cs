using UnityEngine;

public class BeginGame : MonoBehaviour {
    private GameObject startGame;
    private void Start() {
        startGame = GameObject.Find("ThreeCard");
        startGame.SetActive(false);
    }
    private void OnMouseDown() {
        startGame.SetActive(true);
        gameObject.SetActive(false);
    }
}