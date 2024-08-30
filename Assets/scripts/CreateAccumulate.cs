using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateAccumulate : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject prefabToTile;
    public GameObject anchor1;
    public GameObject anchor2;
    public int tilesPerRow = 8;
    public float spacing = 10.0f;
    private Roles[] roleList = new Roles[2];
    private Roles firstRole;
    private Roles secondRole;
    private List<GameObject> copys1 = new();
    private List<GameObject> copys2 = new();
    private List<GameObject> anchors = new();
    private List<List<GameObject>> prefabCopys = new List<List<GameObject>> { new List<GameObject>(), new List<GameObject>() };
    void Start()
    {
        firstRole = GameProcess.Instance.role_list[0];
        secondRole = GameProcess.Instance.role_list[1];
        roleList[0] = firstRole;
        roleList[1] = secondRole;  
        anchors.Add(anchor1);
        anchors.Add(anchor2);

    }

    // Update is called once per frame
    public void CopyPrefab()
    {
        
        foreach(var role in roleList) {
            foreach(var roleCopys in prefabCopys[role.role_index]) {
                Destroy(roleCopys);
            }
            Vector2 startPos = anchors[role.role_index].transform.position; 
            int copyNum = 0;
            foreach(var accu in role.accumulateList) {
                string[] temp = accu.Replace(" ", "").Split(">");
                string countNow = temp[1];
                string color = temp[5];
                string accuNum = temp[6];
                if(color != "none") {
                    int row =  copyNum / tilesPerRow;
                    int column = copyNum % tilesPerRow;
                    Vector2 spawnPos = new Vector2(startPos.x + column * spacing, startPos.y - row * spacing);
                    GameObject newPrefab = Instantiate(prefabToTile, spawnPos, Quaternion.identity);
                    SpriteRenderer spriteRenderer = newPrefab.GetComponent<SpriteRenderer>();
                    Color newColor;
                    if (ColorUtility.TryParseHtmlString(color, out newColor))
                    {
                        spriteRenderer.color = newColor;
                    }
                    Text text1 = newPrefab.transform.GetChild(0).GetChild(0).GetComponent<Text>();
                    Text text2 = newPrefab.transform.GetChild(0).GetChild(1).GetComponent<Text>();
                    text1.text = accuNum;
                    text2.text = countNow;
                    prefabCopys[role.role_index].Add(newPrefab);
                    copyNum++;
                }
            }
        }
    }
}
