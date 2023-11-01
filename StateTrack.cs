using System;
using System.Linq;
using System.Threading;
using UnityEngine;

public class StateTrack : MonoBehaviour {
    private bool isRunning = false;
    private Roles firstRole;
    private Roles secondRole;
    private Roles[] roleList = new Roles[2]; 
    private void Start() {
        firstRole = GameProcess.Instance.role_list[0];
        secondRole = GameProcess.Instance.role_list[1];
        roleList[0] = firstRole;
        roleList[1] = secondRole;
    }
    

    private void Update()
    {
        if (!isRunning)
        {
            Thread thread = new Thread(RunParallelFunctions);
            thread.Start();
        }
    }

    private void RunParallelFunctions()
    {
        isRunning = true;

        Thread calHarmThread = new Thread(CalHarm);
        Thread lifeRecoverThread = new Thread(LifeRecover);
        Thread harmSettlementThread = new Thread(HarmSettlement);
        Thread bleedHarmSettlementThread = new Thread(BleedHarmSettlement);

        calHarmThread.Start();
        lifeRecoverThread.Start();
        harmSettlementThread.Start();
        bleedHarmSettlementThread.Start();

        calHarmThread.Join();
        lifeRecoverThread.Join();
        harmSettlementThread.Join();
        bleedHarmSettlementThread.Join();

        isRunning = false;
    }
    private void LifeRecover() {

        foreach(var role in roleList) {
            if(role["life_recover"] > 0) {
                role["life_change"] += 1;
                role["recover_count"] += 1;
                role["life_now"] += role["life_recover"] * 30 * role["heal_beilv"];
                role["life_max"] += role["life_recover"] * 30 * role["heal_beilv"];
                role["life_recover"] = 0;
            }            
        }
    }
    private void HarmSettlement() {
        foreach(var role in roleList) {
            var enemy = role.process.role_list[(role.role_index + 1) % 2];
            if(!enemy.harm_to_life_next && enemy["harm_to_life"] > 0) {
                enemy["life_recover"] += role["harm"] / 30;               
            }else {
                enemy["life_now"] -= enemy["shield"] >= role["harm"] ? 0 : (role["harm"] - enemy["shield"]);
                enemy["shield"] -= role["harm"] <= enemy["shield"] ? role["harm"] : enemy["shield"];
                enemy["life_now"] = (float)Math.Round(enemy["life_now"]);
            }
            if(role["harm"] > 0) {
                enemy["life_change"] += 1;
            }
            role["harm"] = 0;
        }
    }
    private void BleedHarmSettlement() {
        foreach(var role in roleList) {
            var enemy = role.process.role_list[(role.role_index + 1) % 2];
            if(!enemy.harm_to_life_next && enemy["harm_to_life"] > 0) {
                enemy["life_recover"] += role["bleed_harm"] / 30;               
            }else {
                enemy["life_now"] -= role["bleed_harm"];
            }
            if(role["bleed_harm"] > 0) {
                enemy["life_change"] += 1;
            }
            role["bleed_harm"] = 0;
        }
    }
    private void CalHarm() {
        foreach(var role in roleList) {
            if (role["attack_count"] > 0 || role["attack"] != 0)
            {
                var enemy = role.process.role_list[(role.role_index + 1) % 2];
                role["harm"] += role["attack"] * 30;
                role["harm"] += role["attack_count"] * ((int)role["power"] + role["note"]) * 30;
                // var backup = role["note"];
                // if (role["attack_count"] > 0)
                // {
                //     role["note"] = 0;
                //     if (role["return_note"])
                //     {
                //         role["return_note"] = false;
                //         role["note"] = backup;
                //     }
                // }
                role["attack"] = 0;
                role["attack_count"] = 0;
                if (role["weak"] > 0)
                {
                    role["harm"] *= 0.6f;
                    
                }
                if (enemy["easy_hurt"] > 0)
                {
                    role["harm"] *= 1.5f;
                }
            }
        }        
    }
}