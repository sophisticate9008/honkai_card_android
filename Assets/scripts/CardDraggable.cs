using UnityEngine;

public class CardDraggable : MonoBehaviour {
    private bool isDragging = false;
    private Vector3 offset;
    public bool isInsideSlot = false; // 是否在卡槽内
    private Vector3 startPosition; // 卡牌的初始位置
    private CardSlot cardSlot;
    private GameObject beginGame;
    private void Awake() {
        beginGame = GameObject.Find("BeginGame");
    }
    private void Start() {
        cardSlot = GameObject.Find("CardSlot").GetComponent<CardSlot>();  
        beginGame.SetActive(false); 
    }
    void OnMouseDown() {
        isDragging = true;
        offset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        startPosition = transform.position; // 记录卡牌的初始位置
    }

    void OnMouseDrag() {
        if (isDragging) {
            Vector3 newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) + offset;
            transform.position = new Vector3(newPosition.x, newPosition.y, 0);
        }
    }

    void OnMouseUp() {
        isDragging = false;

        if(cardSlot.cardsInSlot.Count == 8) {
            beginGame.SetActive(true);
        }else {
            beginGame.SetActive(false);
        }
        if (isInsideSlot) {
            // 卡牌在卡槽内，不执行回到初始位置的逻辑
            cardSlot.SortCardsInSlot();
            return;
            
        }


        // 卡牌不在卡槽内，回到初始位置
        // transform.position = startPosition;
    }

}