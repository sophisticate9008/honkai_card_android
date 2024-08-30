using UnityEngine;
using UnityEngine.UI;

public class BeginSel : MonoBehaviour
{
    private GameObject startGame;
    private GameObject enemyCards;
    InputField inputField;
    Button onLineButton;
    Button offLineButton;
    Button setNickname;
    Button confirm;
    Button setBlood; // 新增的血量设置按钮
    GameObject inputContainer;
    public static string modeSel;
    private enum InputMode { Nickname, Blood }
    private InputMode currentInputMode;
    private void Start()
    {
        if (!PlayerPrefs.HasKey("playerName"))
        {
            PlayerPrefs.SetString("playerName", "default");
        }
        offLineButton = GameObject.Find("OffLine").GetComponent<Button>();
        onLineButton = GameObject.Find("OnLine").GetComponent<Button>();
        setNickname = GameObject.Find("SetNickname").GetComponent<Button>();
        setBlood = GameObject.Find("SetBlood").GetComponent<Button>();
        inputContainer = GameObject.Find("InputContainer");
        inputField = GameObject.Find("Input").GetComponent<InputField>();
        confirm = GameObject.Find("Confirm").GetComponent<Button>();
        startGame = GameObject.Find("ThreeCard");
        enemyCards = GameObject.Find("enemy");
        inputContainer.SetActive(false);
        offLineButton.onClick.AddListener(OffLine);
        onLineButton.onClick.AddListener(OnLine);
        setNickname.onClick.AddListener(SetNickname);
        setBlood.onClick.AddListener(SetBlood);
        confirm.onClick.AddListener(Confirm);

        startGame.SetActive(false);

        enemyCards.SetActive(false);
        Application.runInBackground = true;
        // 非全屏模式
        Screen.fullScreen = false;
    }
    private void GameBegin()
    {
        startGame.SetActive(true);
        gameObject.SetActive(false);
        enemyCards.SetActive(true);
    }
    public void OffLine()
    {
        modeSel = "offLine";
        GameBegin();
    }
    public void OnLine()
    {
        modeSel = "onLine";
        GameBegin();
    }

    public void SetNickname()
    {
        inputContainer.SetActive(true);
        inputField.placeholder.GetComponent<Text>().text = "Enter nickname";
        currentInputMode = InputMode.Nickname;
    }

    public void Confirm()
    {
        string text = inputField.text;
        if (text.Length == 0)
        {
            text = "default";
        }

        if (currentInputMode == InputMode.Nickname)
        {
            PlayerPrefs.SetString("playerName", text);
            Debug.Log(PlayerPrefs.GetString("playerName", text));
            inputContainer.SetActive(false);
        }
        else if (currentInputMode == InputMode.Blood)
        {
            if (int.TryParse(text, out int bloodValue))
            {
                PlayerPrefs.SetInt("bloodCustom", bloodValue);
                Debug.Log("Blood set to: " + bloodValue);
            }
            else
            {
                Debug.Log("Invalid blood value");
            }
            inputContainer.SetActive(false);
        }
        inputField.text = "";
    }
    // public void reload() {
    //     gameObject.SetActive(true);
    //     startGame.SetActive(false);
    //     enemyCards.SetActive(false);
    // }
    public void SetBlood()
    {
        inputContainer.SetActive(true);
        inputField.placeholder.GetComponent<Text>().text = "Enter blood amount";
        currentInputMode = InputMode.Blood;
    }
}