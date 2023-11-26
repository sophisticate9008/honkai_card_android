using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameEnd : MonoBehaviour
{
    Text textResult;
    
    private void Awake() {
        textResult = GameObject.Find("result").GetComponent<Text>();
    }
    // Start is called before the first frame update
    public void PushResult(string result) {
        textResult.text = result;
        gameObject.SetActive(true);
    }

    public void OnMouseDown() {
        EndGame();
    }
    public static void EndGame() {
        GameProcess.cardPacks.Clear();
        CardDrawing.cardDatas.Clear();
        GameProcess.roleSelList.Clear();
        SceneManager.LoadScene("SelCard");
    }
}
