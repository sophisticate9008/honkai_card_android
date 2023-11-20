using System.Collections.Generic;
using UnityEngine;

public class CardSlot : MonoBehaviour {
    public List<GameObject> cardsInSlot = new List<GameObject>();

    public void OnTriggerEnter2D(Collider2D other) {
        CardDraggable isCard = other.GetComponent<CardDraggable>();

        if (isCard != null) {
            CardDraggable card = other.GetComponent<CardDraggable>();
            if (card != null) {
                card.isInsideSlot = true; // 标记卡牌在卡槽内
                AddCardToSlot(other.gameObject); // 将卡牌添加到卡槽中
                
            }
        }
    }

    public void OnTriggerExit2D(Collider2D other) {
        CardDraggable isCard = other.GetComponent<CardDraggable>();
        

        if (isCard != null) {
            CardDraggable card = other.GetComponent<CardDraggable>();
            if (card != null) {
                card.isInsideSlot = false; // 标记卡牌不在卡槽内
                cardsInSlot.Remove(other.gameObject); // 移除卡牌
                 // 重新排序
            }
        }
    }
    
    // 当卡牌被拖拽到卡槽上时调用
    public void AddCardToSlot(GameObject card) {
        cardsInSlot.Add(card);
        SortCardsInSlot();
    }

    // 对卡槽内的卡牌进行排序
    public void SortCardsInSlot() {
        cardsInSlot.Sort((a, b) => a.transform.position.x.CompareTo(b.transform.position.x));

        int totalSlots = 8; // 预留的卡牌位置数量
        float totalWidth = 0f;
        float cardWidth = cardsInSlot.Count > 0 ? cardsInSlot[0].GetComponent<SpriteRenderer>().bounds.size.x : 0f;

        // 计算总宽度
        totalWidth = cardWidth * totalSlots;

        // 获取卡槽的 X 轴边界
        float leftBoundary = transform.position.x - totalWidth / 2f;

        // 计算卡牌之间的间距
        float cardOffset = totalWidth / (totalSlots - 1);

        // 计算实际卡牌之间的间距
        float actualCardOffset = cardWidth + cardOffset - cardWidth;

        // 获取卡槽的 Y 坐标作为固定值
        float centerY = transform.position.y;

        // 重新排列卡牌位置，从左到右排序
        float currentPositionX = leftBoundary - cardWidth / 2f;
        for (int i = 0; i < totalSlots; i++) {
            // 如果有卡牌，放置卡牌，否则创建空位
            if (i < cardsInSlot.Count) {
                cardsInSlot[i].transform.position = new Vector2(currentPositionX + cardWidth / 2f, centerY);
                currentPositionX += actualCardOffset;
            } else {
                // 创建空位
                currentPositionX += actualCardOffset;
            }
        }
    }
}