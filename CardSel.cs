using UnityEngine;
using UnityEngine.SceneManagement;

public class CardSel : MonoBehaviour {
    CardDrawing cardParent;
    private void Start() {
        cardParent = GameObject.Find("ThreeCard").GetComponent<CardDrawing>();
    }
    private void OnMouseDown() {
        if(CardDrawing.cardDatas.Count < 8) {
            Debug.Log(cardParent.modifiedCardPack[int.Parse(gameObject.name)]);
            CardDrawing.cardDatas.Add(cardParent.modifiedCardPack[int.Parse(gameObject.name)]);
            CardDrawing.cardObjs.Add(transform);
            cardParent.DrawingCard();
            return;
        }
        
        GameProcess.cardPacks.Add(CardDrawing.cardDatas);
        SceneManager.LoadScene("GameProcess");
    }
}