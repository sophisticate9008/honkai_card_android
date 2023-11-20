using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CardDrawing : MonoBehaviour
{
    public Material purple; // 不需要在Inspector面板中分配Material1的初始值
    public Material gold; // 不需要在Inspector面板中分配Material2的初始值
    public Material red; 
    private System.Random random = new();
    public static List<Transform> cardObjs = new();
    public static List<string> cardDatas = new();
    private List<string> cardPack = new();
    private List<Cards> cardInstance = new();
    private Transform floatWindow;
    public int cardPackSel;
    public int cardNum;
    public string cardParent;    
    public List<string> modifiedCardPack = new();

    private void Start() {
        floatWindow = GameObject.Find(cardParent).GetComponent<Transform>();
        DrawingCard();
    }
    public void DrawingCard() {
        
        cardPack = GameProcess.ChooseRandomElements(GameProcess.lightAndNight, cardNum);
        if(GameProcess.cardPacks.Count > cardPackSel && GameProcess.cardPacks[cardPackSel] != null) {
            modifiedCardPack = GameProcess.cardPacks[cardPackSel];
        }else {
            modifiedCardPack = cardPack.Select(item => $"{item}_{random.Next(1, 4)}").ToList();
            if(cardNum == 8) {
                GameProcess.cardPacks.Add(modifiedCardPack);
            }              
        }
        
      
        foreach (var card in modifiedCardPack)
        {
            Roles emptyRole = new();
            var temp = card.Split("_");
            var card_name = temp[0];
            var card_level = int.Parse(temp[1]);
            cardInstance.Add(new Cards(emptyRole, card_name, 0, card_level));
        }
        Transform[] childCard = Texture.GetChildObjects(floatWindow);
        int tem = 0;
        
        foreach(var card in cardInstance) {
            Renderer childRenderer = childCard[tem].GetComponent<Renderer>();
            Transform childCanvas = childCard[tem].GetChild(0);
            Texture.AllSetFontsize(childCanvas, Screen.width / 908 * 9);
            Transform[] canvasChild = Texture.GetChildObjects(childCanvas);
            Text nameText = canvasChild[0].GetComponent<Text>();
            Text desText = canvasChild[1].GetComponent<Text>();
            Text manaText = canvasChild[2].GetComponent<Text>();
            Text starText = canvasChild[3].GetComponent<Text>();
            nameText.text = card.card_name;
            desText.text = Texture.ColorizeText(card.describe);
            manaText.text = card.mana.ToString();
            starText.text = new string('★', card.level);
            if(card.color == "红") {
                childRenderer.material = red;
            }else if(card.color == "金") {
                childRenderer.material = gold;
            }else {
                childRenderer.material = purple;
            }
            tem++;
        }
        cardInstance.Clear();      
    }
    
}