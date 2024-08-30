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
    GameEnd gameEnd;
    GameObject gameEndObj;
    CardUse cardUse;
    private void Awake() {
        gameEndObj = GameObject.Find("GameEnd");
        gameEnd = gameEndObj.GetComponent<GameEnd>();
        cardUse = GameObject.Find("CardUse").GetComponent<CardUse>();
    }
    private void Start()
    {
        gameEndObj.SetActive(false);
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
            if(role.log != "") {
                ShowScrollingText(role, role.log, "#499ACD");
                role.log = "";
            }
            if (role["life_recover"] > 0)
            {
                role["life_change"] += 1;
                role["recover_count"] += 1;
                float temp = role["life_recover"] * 30 * role["heal_beilv"];
                role["life_now"] += temp;
                role["life_max"] += temp;
                role["life_recover"] = 0;
                ShowScrollingText(role, ((int)temp).ToString(), "green");

            }
        }
        yield return null;
    }

    private IEnumerator HarmSettlement()
    {
        foreach (var role in roleList)
        {
            var enemy = role.process.role_list[(role.role_index + 1) % 2];
            string color = "red";
            if (!enemy.harm_to_life_next && enemy["harm_to_life"] > 0)
            {
                enemy["life_recover"] += role["harm"] / 30;
                color = "white";
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
            if(role["harm"] > 0 && role["attack_count"] == 0) {
                ShowScrollingText(role, ((int)role["harm"]).ToString(), color);
            }else {
                for(int i = 0; i < role["attack_count"]; i++) {
                    if(role["harm"] / role["attack_count"] > 1) {
                        ShowScrollingText(role, ((int)(role["harm"] / role["attack_count"])).ToString(), color);
                    }
                    
                }
            }
            role["attack_count"] = 0;
            role["harm"] = 0;
            judgeEnd();
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
            string color = "#B3082F";
            if (!enemy.harm_to_life_next && enemy["harm_to_life"] > 0)
            {
                color = "#C99DD9";
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
                    ShowScrollingText(role, (bleed_harm / bleed_count).ToString(), color);
                }    
            }
            judgeEnd();
        }
        yield return null;
    }

    private IEnumerator CalHarm()
    {
        foreach (var role in roleList)
        {
            if (role["attack"] != 0)
            {
                var enemy = role.process.role_list[(role.role_index + 1) % 2];
            
                role["harm"] += role["attack"] * 30;
                role["harm"] += role["attack_count"] * ((int)role["power"]) * 30;
                role["harm"] += role["attack_note"] * (int)role["note"] * 30;
                if(role.return_note && role["attack_note"] > 0) {
                    float noteBackup = role["note"];
                    role["note"] = 0;
                    role["note"] += noteBackup;
                    role.return_note = false;
                }else if(role["attack_note"] > 0){
                    role["note"] = 0;
                }

                role["attack"] = 0;
                role["attack_note"] = 0;
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
        yield return null;
    }

    private void ShowScrollingText(Roles role, string str, string color)
    {

        if (role.role_index == 0)
        {
            if(color == "green" || color == "#499ACD") {
                scrollingText1.ShowScrollingText($"<color={color}>{str}</color>");
            }else {
                scrollingText2.ShowScrollingText($"<color={color}>{str}</color>");
            }
            
        }
        else
        {
            if(color == "green" || color == "#499ACD") {
                scrollingText2.ShowScrollingText($"<color={color}>{str}</color>");
            }else {
                scrollingText1.ShowScrollingText($"<color={color}>{str}</color>");
            }
        }
    }
    private void judgeEnd() {
        for(int i = 0; i < 2; i++) {
            if(roleList[i]["life_now"] <= 0) {
                cardUse.isGameOver = true;
                gameEndObj.SetActive(true);
                if(i == 0) {
                    gameEnd.PushResult("你输了");
                }else {
                    gameEnd.PushResult("你赢了");
                }
                return;
            }
            
        }
    }
}