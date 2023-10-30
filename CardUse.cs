
using UnityEngine;

public class CardUse : MonoBehaviour {
    public Roles firstRole = GameProcess.Instance.role_list[0];
    public Roles secondRole = GameProcess.Instance.role_list[1];
    public GameObject foundObject;
    private CardAnimation targetScript;
    private int only = 0;
    private void Start() {
        foundObject = GameObject.Find("card_self_00");
        
    }
    
    private void Update() {
        if(foundObject != null && only == 0) {
            targetScript = foundObject.GetComponent<CardAnimation>();
            targetScript.StartAnimation();     
            only++;
        }

    }
}
