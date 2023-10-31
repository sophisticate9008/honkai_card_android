using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image healthBarFill;
    private Roles firstRole;
    private Roles secondRole;    
    List<Roles> roleList = new();
    public int sel = 0;
    public float maxHealth;
    public float currentHealth;
    private void Start() {
        healthBarFill = GetComponent<Image>();
        firstRole = GameProcess.Instance.role_list[0];
        secondRole = GameProcess.Instance.role_list[1];    
        roleList.Add(firstRole);
        roleList.Add(secondRole);    
    }
    private void Update()
    {

        maxHealth = roleList[sel]["life_max"];
        currentHealth = roleList[sel]["life_now"];
        // 根据当前血量计算填充百分比
        float fillAmount = currentHealth / maxHealth;
        // 更新血条的填充百分比
        healthBarFill.fillAmount = fillAmount;
    

    }
}
