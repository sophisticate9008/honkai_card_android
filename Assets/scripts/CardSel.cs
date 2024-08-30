using UnityEngine;
using UnityEngine.SceneManagement;

public class CardSel : MonoBehaviour {
    CardDrawing cardParent;
    BeginSel beginSel;
    private void Awake() {
         beginSel = GameObject.Find("BeginSel").GetComponent<BeginSel>();
    }
    private void Start() {
        cardParent = GameObject.Find("ThreeCard").GetComponent<CardDrawing>();
    }
    private void OnMouseDown() {

        Debug.Log(cardParent.modifiedCardPack[int.Parse(gameObject.name)]);
        CardDrawing.cardDatas.Add(cardParent.modifiedCardPack[int.Parse(gameObject.name)]);

        cardParent.DrawingCard();
        if(CardDrawing.cardDatas.Count == 8) {
            GameProcess.cardPacks.Add(CardDrawing.cardDatas);
            SceneManager.LoadScene("CardOrder");
        }
        
        
    }
}