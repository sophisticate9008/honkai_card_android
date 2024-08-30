using UnityEngine;
using UnityEngine.UI;

public class BeginSel : MonoBehaviour {
    private GameObject startGame;
    private GameObject enemyCards;
    InputField inputField;
    Button onLineButton;
    Button offLineButton;
    Button setNickname;
    Button confirm;
    GameObject inputContainer;
    public static string modeSel;
    private void Start() {
        if(!PlayerPrefs.HasKey("playerName")) {
            PlayerPrefs.SetString("playerName", "default");
        }
        offLineButton = GameObject.Find("OffLine").GetComponent<Button>();
        onLineButton = GameObject.Find("OnLine").GetComponent<Button>();
        setNickname = GameObject.Find("SetNickname").GetComponent<Button>();
        inputContainer = GameObject.Find("InputContainer");
        inputField = GameObject.Find("Input").GetComponent<InputField>();
        confirm = GameObject.Find("Confirm").GetComponent<Button>();
        startGame = GameObject.Find("ThreeCard");     
        enemyCards = GameObject.Find("enemy");
        inputContainer.SetActive(false);
        offLineButton.onClick.AddListener(OffLine);
        onLineButton.onClick.AddListener(OnLine);
        setNickname.onClick.AddListener(SetNickname);
        confirm.onClick.AddListener(Confirm);

        startGame.SetActive(false);

        enemyCards.SetActive(false);
        Application.runInBackground = true;
        // 非全屏模式
        Screen.fullScreen = false;
    }
    private void gameBegin() {
        startGame.SetActive(true);
        gameObject.SetActive(false); 
        enemyCards.SetActive(true); 
    }
    public void OffLine() {
        modeSel = "offLine";
        gameBegin();
    }
    public void OnLine() {
        modeSel = "onLine";
        gameBegin();
    }

    public void SetNickname() {
        inputContainer.SetActive(true);
    }

    public void Confirm(){
        string text = inputField.text;
        if(text.Length == 0) {
            text = "default";
        } 
        PlayerPrefs.SetString("playerName", text);
        Debug.Log(PlayerPrefs.GetString("playerName", text));
        inputContainer.SetActive(false);
    }
    // public void reload() {
    //     gameObject.SetActive(true);
    //     startGame.SetActive(false);
    //     enemyCards.SetActive(false);
    // }
}