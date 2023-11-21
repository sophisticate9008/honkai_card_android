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
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void PushResult(string result) {
        textResult.text = result;
        gameObject.SetActive(true);
    }

    public void OnMouseDown() {
        GameProcess.cardPacks.Clear();
        CardDrawing.cardDatas.Clear();
        GameProcess.roleSelList.Clear();
        SceneManager.LoadScene("SelCard");
    }
}
