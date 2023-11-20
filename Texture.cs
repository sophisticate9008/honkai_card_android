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

        
    }

    public static Transform[] GetChildObjects(Transform parent)
    {
        int childCount = parent.childCount;
        Transform[] children = new Transform[childCount];

        for (int i = 0; i < childCount; i++)
        {
            children[i] = parent.GetChild(i);
        }

        return children;
    }
    public static void AllSetFontsize(Transform obj, int size) {
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


