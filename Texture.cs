using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Texture : MonoBehaviour
{
    public Material purple; // 不需要在Inspector面板中分配Material1的初始值
    public Material gold; // 不需要在Inspector面板中分配Material2的初始值
    public Material red; 
    private Transform self;
    public static Dictionary<string, string> colorMap = new()
    {
        { "迅捷", "#55D1A7" },
        { "法力", "#499ACD" },
        { "魔阵", "#A855F1" },
        { "唯一", "#252C81" },
        { "流血", "#B3082F" },
        { "护盾", "#A74722" },
        { "力量", "#B2381E" },
        { "自愈", "#458F31" },
        { "幸运币", "#EEB34D" },
        { "虚弱", "#2E503F" },
        { "易伤", "#A774AC" },
        { "乐符", "#A1BEDC" }
    };
    private Transform enemy;
    private int screenWidth;
    private int screenHeight;    
    private Roles firstRole;
    private Roles secondRole;
    private void Start() {
        screenWidth = Screen.width;
        screenHeight = Screen.height;
        self = transform.Find("self");
        enemy = transform.Find("enemy");
        firstRole = GameProcess.Instance.role_list[0];
        secondRole = GameProcess.Instance.role_list[1];

        Transform[] firstObjects = GetChildObjects(self);
        Transform[] secondObjects = GetChildObjects(enemy);
        int temp = 0;
        foreach(var card in firstRole.card_pack_instance) {
            Renderer childRenderer = firstObjects[temp].GetComponent<Renderer>();
            Transform childCanvas = firstObjects[temp].GetChild(0);
            AllSetFontsize(childCanvas, screenWidth / 908 * 9);
            Transform[] canvasChild = GetChildObjects(childCanvas);
            Text nameText = canvasChild[0].GetComponent<Text>();
            Text desText = canvasChild[1].GetComponent<Text>();
            Text manaText = canvasChild[2].GetComponent<Text>();
            Text starText = canvasChild[3].GetComponent<Text>();
            nameText.text = card.card_name;
            desText.text = ColorizeText(card.describe);
            manaText.text = card.mana.ToString();
            starText.text = new string('★', card.level);
            if(card.color == "红") {
                childRenderer.material = red;
            }else if(card.color == "金") {
                childRenderer.material = gold;
            }else {
                childRenderer.material = purple;
            }
            temp++;
        }
        temp = 0;
        foreach(var card in secondRole.card_pack_instance) {
            Renderer childRenderer = secondObjects[temp].GetComponent<Renderer>();
            Transform childCanvas = secondObjects[temp].GetChild(0);
            AllSetFontsize(childCanvas, screenWidth / 908 * 9);
            Transform[] canvasChild = GetChildObjects(childCanvas);
            Text nameText = canvasChild[0].GetComponent<Text>();
            Text desText = canvasChild[1].GetComponent<Text>();
            Text manaText = canvasChild[2].GetComponent<Text>();
            Text starText = canvasChild[3].GetComponent<Text>();
            nameText.text = card.card_name;
            desText.text = ColorizeText(card.describe);
            manaText.text = card.mana.ToString();
            starText.text = new string('★', card.level);
            if(card.color == "红") {
                childRenderer.material = red;
            }else if(card.color == "金") {
                childRenderer.material = gold;
            }else {
                childRenderer.material = purple;
            }
            temp++;
        }        
    }

    private Transform[] GetChildObjects(Transform parent)
    {
        int childCount = parent.childCount;
        Transform[] children = new Transform[childCount];

        for (int i = 0; i < childCount; i++)
        {
            children[i] = parent.GetChild(i);
        }

        return children;
    }
    private void AllSetFontsize(Transform obj, int size) {
        Transform[] canvasChild = GetChildObjects(obj);
        for(int i = 0; i < obj.childCount; i++) {
            Text temp = canvasChild[i].GetComponent<Text>();
            temp.fontSize = size;
        }
    }
    public static string ColorizeText(string inputText)
    {
        string result = inputText;

        foreach (var kvp in colorMap)
        {
            result = result.Replace(kvp.Key, $"<color={kvp.Value}><b>{kvp.Key}</b></color>");
        }

        return result;
    }
}


