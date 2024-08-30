using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class AccumulatePrefabs : MonoBehaviour
{
    // Start is called before the first frame update
    Text text1;
    Text text2;
    void Start()
    {
        text1 = transform.GetChild(0).GetChild(0).GetComponent<Text>();
        text2 = transform.GetChild(0).GetChild(1).GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 selfPos = Camera.main.WorldToScreenPoint(transform.position);
        Vector2 offset1 = new(0, 0);
        Vector2 offset2 = new(0, 15);
        text1.rectTransform.position = selfPos + offset1;
        text2.rectTransform.position = selfPos + offset2;
    }
}
