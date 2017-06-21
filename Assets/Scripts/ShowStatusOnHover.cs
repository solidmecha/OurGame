using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowStatusOnHover : MonoBehaviour {

    public GameObject CurrStatWindow;
    public BattleControl BC;
    public Vector2 offset;
    public Character ThisChar;

    private void OnMouseEnter()
    {
        SetStatusWindow();
    }
    private void OnMouseExit()
    {
        Destroy(CurrStatWindow);
    }

    public void SetStatusWindow()
    {
        CurrStatWindow = Instantiate(BC.UI_Status_Window, (Vector2)transform.position + offset, Quaternion.identity) as GameObject;
        CurrStatWindow.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>().text = ThisChar.health + " / " + ThisChar.MaxHealth;
        CurrStatWindow.transform.GetChild(0).GetChild(0).GetChild(1).localScale = new Vector3(ThisChar.health * 11.82f / ThisChar.MaxHealth, 1, 0);
        Text T = CurrStatWindow.transform.GetChild(0).GetChild(1).GetComponent<Text>();
        foreach (Status S in ThisChar.Statuses)
            T.text += "\n" + S.name + " " + S.duration + " turns";
        CurrStatWindow.transform.SetParent(transform);
    }

    private void Start()
    {
        ThisChar = GetComponent<Character>();
        BC = GameObject.FindGameObjectWithTag("GameController").GetComponent<BattleControl>();
    }
}
