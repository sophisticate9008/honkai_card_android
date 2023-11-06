
using System;
using UnityEngine;
using UnityEngine.UI;

public class CardUse : MonoBehaviour {

    private Roles firstRole;
    private Roles secondRole;
    public Text state1;
    public Text state2;
    private GameObject foundObject;
    private GameObject foundObject1;
    private CardAnimation targetScript;
        private CardAnimation targetScript1;
    private int roleSel = 0;
    private int roleSelNow = 0;
    public Material purple; 
    public Material gold; 
    public Material red;    
    private  Cards cardNow;
    private  Cards cardNext;
    private Renderer childRenderer; 
    private Roles[] roleList = new Roles[2];
    private Renderer childRenderer1; 
    private void Start() {
        firstRole = GameProcess.Instance.role_list[0];
        secondRole = GameProcess.Instance.role_list[1];
        roleList[0] = firstRole;
        roleList[1] = secondRole;
    }
    
        private void Update() {
            if(roleSel % 2 == 0 && roleSelNow == roleSel) {
                roleSelNow++;
                AllBegin();
                firstRole.TurnBegin();
                ShowStates();
                cardNow = firstRole.card_pack_instance[(int)firstRole["card_use_index"]];
                cardNext = firstRole.card_pack_instance[((int)firstRole["card_use_index"] + 1) % firstRole.card_pack_instance.Count];
                foundObject = GameObject.Find($"card_self_0{cardNow.index}");                
                foundObject1 = GameObject.Find($"card_self_0{cardNext.index}");
                targetScript = foundObject.GetComponent<CardAnimation>();
                targetScript1 = foundObject1.GetComponent<CardAnimation>();
                int tempCount = firstRole.UseCard();
                if (tempCount == 0) {
                }else if(tempCount == 1) {
                    targetScript.StartAnimation();
                    if(cardNow.broken) {
                        childRenderer = foundObject.GetComponent<Renderer>();
                        Invoke("ChangeMaterial", 1.0f);  
                    }
                }else {
                    targetScript.StartAnimation();
                    targetScript1.StartAnimation();
                    if(cardNow.broken) {
                        childRenderer = foundObject.GetComponent<Renderer>();
                        Invoke("ChangeMaterial", 1.0f);
                    }
                    if(cardNext.broken) {
                        childRenderer1 = foundObject1.GetComponent<Renderer>();
                        Invoke("ChangeMaterial1", 1.0f);
                    }                                   
                }
                ShowStates();
                Invoke("ChangeVariable", 1.0f);

            }else if(roleSel % 2 == 1 && roleSelNow == roleSel) {
                roleSelNow++;
                secondRole.TurnBegin();
                ShowStates();
                cardNow = secondRole.card_pack_instance[(int)secondRole["card_use_index"]];
                cardNext = secondRole.card_pack_instance[((int)secondRole["card_use_index"] + 1) % secondRole.card_pack_instance.Count];
                foundObject = GameObject.Find($"card_enemy_0{cardNow.index}");                
                foundObject1 = GameObject.Find($"card_enemy_0{cardNext.index}");
                targetScript = foundObject.GetComponent<CardAnimation>();
                targetScript1 = foundObject1.GetComponent<CardAnimation>();
                int tempCount = secondRole.UseCard();
                if (tempCount == 0) {

                }else if(tempCount == 1) {
                    targetScript.StartDescentAnimation();
                    if(cardNow.broken) {
                        childRenderer = foundObject.GetComponent<Renderer>();
                        Invoke("ChangeMaterial", 1.0f);  
                    }
                }else {
                    targetScript.StartDescentAnimation();
                    targetScript1.StartDescentAnimation();
                    if(cardNow.broken) {
                        childRenderer = foundObject.GetComponent<Renderer>();
                        Invoke("ChangeMaterial", 1.0f);
                    }
                    if(cardNext.broken) {
                        childRenderer1 = foundObject1.GetComponent<Renderer>();
                        Invoke("ChangeMaterial1", 1.0f);
                    }                                   
                }
                ShowStates();
                Invoke("ChangeVariable", 1.0f);

            }

        }
    private void AllBegin() {
        foreach (var role in roleList)
        {
            role.harm_to_life_next = false;
        }
    }
    private void AllEnd() {
        foreach (var role in roleList)
        {
            if(!role.harm_to_life_next) {
                role["harm_to_life"] -= role["harm_to_life"] > 0 ? 1 : 0;
            }
            
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
            AllEnd();
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
    private void ChangeMaterial1() {
        if(cardNext.color == "红") {
            childRenderer1.material = red;
        }else if(cardNext.color == "金") {
            childRenderer1.material = gold;
        }else {
            childRenderer1.material = purple;
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
