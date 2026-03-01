using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Popuptext : MonoBehaviour
{
    Text popuptext;
    [HideInInspector]
    public Color textcolour;
    [HideInInspector]
    public bool colourShift;
    float colourTimer;
    public float destroyTimer = 0.4f;
    public float moveSpeed = 1f;
    private void Awake() {
        popuptext = GetComponent<Text>();
    }

    public void SetText(string amount) {
        if (!colourShift) popuptext.color = textcolour;
        popuptext.text = amount;
    }
    private void Update() {
        if(colourShift) {
            colourTimer += Time.deltaTime;
            if (colourTimer >= 1.0f) colourTimer = 0;
            popuptext.color = Color.HSVToRGB(colourTimer, 1, 1);
        }
        transform.position += transform.up * Time.deltaTime * moveSpeed;
        Destroy(this.gameObject, destroyTimer);
    }
}
