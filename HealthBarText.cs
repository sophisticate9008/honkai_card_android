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

    public float maxHealth;
    public float currentHealth;
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
}
