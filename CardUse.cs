
using System;
using UnityEngine;
using UnityEngine.UI;

public class CardUse : MonoBehaviour {
    private Roles firstRole;
    private Roles secondRole;
    public Text state1;
    public Text state2;
    private GameObject foundObject;
    private CardAnimation targetScript;
    private int roleSel = 0;
    private int roleSelNow = 0;
    public Material purple; 
    public Material gold; 
    public Material red;    
    private  Cards cardNow;
    private Renderer childRenderer; 
    private void Start() {
        firstRole = GameProcess.Instance.role_list[0];
        secondRole = GameProcess.Instance.role_list[1];
    }
    
        private void Update() {
            if(roleSel % 2 == 0 && roleSelNow == roleSel) {
                roleSelNow++;
                firstRole.TurnBegin();
                cardNow = firstRole.card_pack_instance[(int)firstRole["card_use_index"]];
                firstRole.UseCard();
                foundObject = GameObject.Find($"card_self_0{cardNow.index}");
                targetScript = foundObject.GetComponent<CardAnimation>();
                if(!targetScript.isAnimating) {
                    targetScript.StartAnimation();
                }
                if(cardNow.broken) {
                    childRenderer = foundObject.GetComponent<Renderer>();
                    Invoke("ChangeMaterial", 1.0f);                    
                }
                ShowStates();
                Invoke("ChangeVariable", 1.0f);

            }else if(roleSel % 2 == 1 && roleSelNow == roleSel) {
                roleSelNow++;
                secondRole.TurnBegin();
                cardNow = secondRole.card_pack_instance[(int)secondRole["card_use_index"]];
                secondRole.UseCard();
                foundObject = GameObject.Find($"card_enemy_0{cardNow.index}");
                targetScript = foundObject.GetComponent<CardAnimation>();
                if(!targetScript.isAnimating) {
                    targetScript.StartDescentAnimation();    
                }
                if(cardNow.broken) {
                    childRenderer = foundObject.GetComponent<Renderer>();
                    Invoke("ChangeMaterial", 1.0f);                  
                }
                ShowStates();             
                Invoke("ChangeVariable", 1.0f);
            }

        }
    private void ChangeVariable()
    {
        if(roleSel % 2 == 0) {
            firstRole.turnEnd();
            ShowStates();
        }
        else {
            secondRole.turnEnd();
            ShowStates();
        }
        roleSel++;
        
         // 变量在此处发生变化
    }
    private void ChangeMaterial() {
        if(cardNow.color == "红") {
            childRenderer.material = red;
        }else if(cardNow.color == "金") {
            childRenderer.material = gold;
        }else {
            childRenderer.material = purple;
        }   
    }
    private void ShowStates() {
        string stateOutput = "";
        foreach(var kvp in Roles.name_args) {
            if(firstRole[$"{kvp.Key}"] > 0) {
                stateOutput += $"{kvp.Value}:{(int)firstRole[kvp.Key]}";
            }
        }
        state1.text = Texture.ColorizeText(stateOutput);
        stateOutput = "";
        foreach(var kvp in Roles.name_args) {
            if(secondRole[$"{kvp.Key}"] > 0) {
                stateOutput += $"{kvp.Value}:{(int)secondRole[kvp.Key]}";
            }
        }
        state2.text = Texture.ColorizeText(stateOutput);    
    }    
}
