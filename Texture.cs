using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Texture : MonoBehaviour
{
    public Material purple; // 不需要在Inspector面板中分配Material1的初始值
    public Material gold; // 不需要在Inspector面板中分配Material2的初始值
    public Material red; 
    public Transform self;
    public Transform enemy;
    
    private Transform selfChild;
    private Transform enemyChild;

    private Roles firstRole;
    private Roles secondRole;
    private void Start() {
        Debug.Log(Screen.width);
        Debug.Log(Screen.height);
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
            Transform[] canvasChild = GetChildObjects(childCanvas);
            Text nameText = canvasChild[0].GetComponent<Text>();
            Text desText = canvasChild[1].GetComponent<Text>();
            Text manaText = canvasChild[2].GetComponent<Text>();
            Text starText = canvasChild[3].GetComponent<Text>();
            nameText.text = card.card_name;
            desText.text = card.describe;
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
            Transform[] canvasChild = GetChildObjects(childCanvas);
            Text nameText = canvasChild[0].GetComponent<Text>();
            Text desText = canvasChild[1].GetComponent<Text>();
            Text manaText = canvasChild[2].GetComponent<Text>();
            Text starText = canvasChild[3].GetComponent<Text>();
            nameText.text = card.card_name;
            desText.text = card.describe;
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
    public void addTexture(Cards card) {
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
}


