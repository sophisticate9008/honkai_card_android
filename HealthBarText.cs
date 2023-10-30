using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarText : MonoBehaviour
{
    
    public Text healthText;
    private Roles firstRole;
    private Roles secondRole;
    private List<Roles> roleList = new();
    public int sel = 0;
    private bool notGetValue = true;
    public float maxHealth;
    public float currentHealth;
    private void Awake() {
        healthText = GetComponent<Text>();
    }
    private void Update()
    {
        if(notGetValue) {
            if(GameProcess.Instance != null) {
                firstRole = GameProcess.Instance.role_list[0];
                secondRole = GameProcess.Instance.role_list[1];
                roleList.Add(firstRole);
                roleList.Add(secondRole);  
                notGetValue = false;               
            }
        }
        if(!notGetValue) {
            maxHealth = roleList[sel]["life_max"];
            currentHealth = roleList[sel]["life_now"];
            healthText.text = currentHealth.ToString() + "/" + maxHealth.ToString();
        }

    }
}
