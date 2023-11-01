
using System;
using UnityEngine;

public class CardUse : MonoBehaviour {
    private Roles firstRole;
    private Roles secondRole;
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
                Invoke("ChangeVariable", 1.0f);

            }else if(roleSel % 2 == 1 && roleSelNow == roleSel) {
                roleSelNow++;
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
                Invoke("ChangeVariable", 1.0f);
            }

        }
    private void ChangeVariable()
    {
        if(roleSel % 2 == 0) {
            firstRole.turnEnd();
        }
        else {
            secondRole.turnEnd();
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
}
