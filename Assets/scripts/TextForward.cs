using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextForward : MonoBehaviour
{
    public Transform parentObj;
    
    private void Start() {
        Transform[] childObjs = Texture.GetChildObjects(parentObj);
        foreach(var obj in childObjs){
            CardTextForward.BindTextForward(obj);
        }
    }

}
