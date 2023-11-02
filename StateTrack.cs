using System;
using System.Collections;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class StateTrack : MonoBehaviour
{
    public GameObject scrollTextObj1;
    public GameObject scrollTextObj2;
    ScrollingText scrollingText1;
    ScrollingText scrollingText2;
    private bool isRunning = false;
    private Roles firstRole;
    private Roles secondRole;
    private Roles[] roleList = new Roles[2];
    public Text desL;
    public Text desR;
    private void Start()
    {
        firstRole = GameProcess.Instance.role_list[0];
        secondRole = GameProcess.Instance.role_list[1];
        roleList[0] = firstRole;
        roleList[1] = secondRole;
        scrollingText1 = scrollTextObj1.GetComponent<ScrollingText>();
        scrollingText2 = scrollTextObj2.GetComponent<ScrollingText>();
        desL.text = Texture.ColorizeText(firstRole.role_describe);
        desR.text = Texture.ColorizeText(secondRole.role_describe);
    }

    private void Update()
    {
        if (!isRunning)
        {
            StartCoroutine(RunParallelFunctions());
        }
    }

    private IEnumerator RunParallelFunctions()
    {
        isRunning = true;

        yield return StartCoroutine(CalHarm());
        yield return StartCoroutine(LifeRecover());
        yield return StartCoroutine(HarmSettlement());
        yield return StartCoroutine(BleedHarmSettlement());

        isRunning = false;
    }

    private IEnumerator LifeRecover()
    {
        foreach (var role in roleList)
        {
            if (role["life_recover"] > 0)
            {
                role["life_change"] += 1;
                role["recover_count"] += 1;
                float temp = role["life_recover"] * 30 * role["heal_beilv"];
                role["life_now"] += temp;
                role["life_max"] += temp;
                role["life_recover"] = 0;
                ShowScrollingText(role, temp, "green");
            }
        }
        yield return null;
    }

    private IEnumerator HarmSettlement()
    {
        foreach (var role in roleList)
        {
            var enemy = role.process.role_list[(role.role_index + 1) % 2];
            if (!enemy.harm_to_life_next && enemy["harm_to_life"] > 0)
            {
                enemy["life_recover"] += role["harm"] / 30;
            }
            else
            {
                enemy["life_now"] -= enemy["shield"] >= role["harm"] ? 0 : (role["harm"] - enemy["shield"]);
                enemy["shield"] -= role["harm"] <= enemy["shield"] ? role["harm"] : enemy["shield"];
                enemy["life_now"] = (float)Math.Round(enemy["life_now"]);
            }
            if (role["harm"] > 0)
            {
                enemy["life_change"] += 1;
            }
            role["harm"] = 0;
        }
        yield return null;
    }

    private IEnumerator BleedHarmSettlement()
    {
        foreach (var role in roleList)
        {
            var enemy = role.process.role_list[(role.role_index + 1) % 2];
            float bleed_count = role["bleed_count"];
            float bleed_harm = role["bleed_harm"];

            if (!enemy.harm_to_life_next && enemy["harm_to_life"] > 0)
            {
                enemy["life_recover"] += role["bleed_harm"] / 30;
            }
            else
            {
                enemy["life_now"] -= role["bleed_harm"];
            }
            if (role["bleed_harm"] > 0)
            {
                enemy["life_change"] += 1;
            }
            role["bleed_harm"] = 0;
            role["bleed_count"] = 0;
            for(int i = 0; i < bleed_count; i++) {
                if(bleed_harm / bleed_count > 1) {
                    ShowScrollingText(role, bleed_harm / bleed_count, "red");
                }
                
            }
        }
        yield return null;
    }

    private IEnumerator CalHarm()
    {
        foreach (var role in roleList)
        {
            if (role["attack_count"] > 0 || role["attack"] != 0)
            {
                var enemy = role.process.role_list[(role.role_index + 1) % 2];
            
                role["harm"] += role["attack"] * 30;
                role["harm"] += role["attack_count"] * ((int)role["power"] + role["note"]) * 30;
                role["attack"] = 0;
                if (role["weak"] > 0)
                {
                    role["harm"] *= 0.6f;
                }
                if (enemy["easy_hurt"] > 0)
                {
                    role["harm"] *= 1.5f;
                }
                float harm = role["harm"];
                float attack_count = role["attack_count"];
                role["attack_count"] = 0;
                for(int i = 0; i < attack_count; i++) {
                    if (harm / attack_count > 1) {
                        ShowScrollingText(role, harm / attack_count, "red");
                    }
                    
                }
            }
        }
        yield return null;
    }

    private void ShowScrollingText(Roles role, float num, string color)
    {

        if (role.role_index == 0)
        {
            if(color == "green") {
                scrollingText1.ShowScrollingText($"<color={color}>{num}</color>");
            }else {
                scrollingText2.ShowScrollingText($"<color={color}>{num}</color>");
            }
            
        }
        else
        {
            if(color == "green") {
                scrollingText2.ShowScrollingText($"<color={color}>{num}</color>");
            }else {
                scrollingText1.ShowScrollingText($"<color={color}>{num}</color>");
            }
        }
    }
}