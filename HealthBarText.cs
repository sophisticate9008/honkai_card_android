using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarText : MonoBehaviour
{
    
    public Text healthText;
    public Text state;
    private Roles firstRole;
    private Roles secondRole;
    private List<Roles> roleList = new();
    public int sel = 0;

    private float maxHealth;
    private float currentHealth;
    private void Start() {
        healthText = GetComponent<Text>();
        firstRole = GameProcess.Instance.role_list[0];
        secondRole = GameProcess.Instance.role_list[1];    
        roleList.Add(firstRole);
        roleList.Add(secondRole);              
    }
    private void Update()
    {

        maxHealth = roleList[sel]["life_max"];
        currentHealth = roleList[sel]["life_now"];
        healthText.text = ((long)currentHealth).ToString() + "/" + ((long)maxHealth).ToString();
    }
    // private void ShowStates() {
    //     string stateOutput = "";

    //     foreach(var kvp in Roles.name_args) {
    //         if(roleList[sel][$"{kvp.Key}"] > 0) {
    //             stateOutput += $"{kvp.Value}:{(int)roleList[sel][kvp.Key]}";
    //         }
    //     }
    //     state.text = Texture.ColorizeText(stateOutput);
    
    // }
}
