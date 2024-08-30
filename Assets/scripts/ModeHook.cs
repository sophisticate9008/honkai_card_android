using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeHook : MonoBehaviour
{

    public bool reversion = false;
    // Start is called before the first frame update
    void Start()
    {
        if(!reversion) {
            gameObject.SetActive(false);
            if(BeginSel.modeSel == "offLine") {
                gameObject.SetActive(true);
            }            
        }else {
            gameObject.SetActive(true);
            if(BeginSel.modeSel == "offLine") {
                gameObject.SetActive(false);
            }                
        }

    }

    // Update is called once per frame

}
