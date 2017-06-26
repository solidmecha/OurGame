using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeScript : MonoBehaviour {

    public WorldControl WC;
    public GameObject ShopCharacter;
    public GameObject SelectionArrow;
    public GameObject CurrentPortrait;
    public int SelectedCharIndex;
    float dir = -1;

    // Use this for initialization
    void Start () {

        WC = GameObject.FindGameObjectWithTag("GameController").GetComponent<WorldControl>();
        SelectionArrow = transform.GetChild(5).gameObject;
        CurrentPortrait = transform.GetChild(0).GetChild(0).gameObject;
        for (int i = 0; i < WC.CurrentParty.Count; i++)
        {
            GameObject G = Instantiate(ShopCharacter, transform.GetChild(1 + i).position, Quaternion.identity) as GameObject;
            G.transform.SetParent(transform);
            G.GetComponent<ShopSelectCharacter>().Index = i;
            G.GetComponent<ShopSelectCharacter>().action = SetSelectedChar;
            G.GetComponent<SpriteRenderer>().sprite = WC.CurrentParty[i].GetComponent<SpriteRenderer>().sprite;
        }
        SetSelectedChar(0);

        for (int i = 0; i < 4; i++)
        {
            int t = i;
            CurrentPortrait.transform.GetChild(0).GetChild(0).GetChild(i).GetComponent<Button>().onClick.AddListener(delegate { BuffAttack(t); });
            ShowUpgradeCostHelper such=CurrentPortrait.transform.GetChild(0).GetChild(0).GetChild(i).gameObject.AddComponent<ShowUpgradeCostHelper>();
            such.id = 0;
            such.index = i;

           CurrentPortrait.transform.GetChild(0).GetChild(1).GetChild(i).GetComponent<Button>().onClick.AddListener(delegate { BuffDefense(t); });
            such = CurrentPortrait.transform.GetChild(0).GetChild(1).GetChild(i).gameObject.AddComponent<ShowUpgradeCostHelper>();
            such.id = 1;
            such.index = i;

            CurrentPortrait.transform.GetChild(0).GetChild(2).GetChild(i).GetComponent<Button>().onClick.AddListener(delegate { BuffRegen(t); });
            such = CurrentPortrait.transform.GetChild(0).GetChild(2).GetChild(i).gameObject.AddComponent<ShowUpgradeCostHelper>();
            such.id = 2;
            such.index = i;

            CurrentPortrait.transform.GetChild(0).GetChild(3).GetChild(i).GetComponent<Button>().onClick.AddListener(delegate { BuffMax(t); });
            such = CurrentPortrait.transform.GetChild(0).GetChild(3).GetChild(i).gameObject.AddComponent<ShowUpgradeCostHelper>();
            such.id = 3;
            such.index = i;
        }
        CurrentPortrait.transform.GetChild(0).GetChild(4).GetChild(0).GetComponent<Button>().onClick.AddListener(delegate { BuffHealth(); });
        ShowUpgradeCostHelper sch = CurrentPortrait.transform.GetChild(0).GetChild(4).GetChild(0).gameObject.AddComponent<ShowUpgradeCostHelper>();
        sch.id = 4;
        sch.index = 0;
        transform.GetChild(0).GetChild(1).GetComponent<Button>().onClick.AddListener(delegate { CleanUp(); });

    }

    public void ShowCost(int id, int Index)
    {
        Text t = transform.GetChild(0).GetChild(7).GetComponent<Text>();
        switch (id)
        {
            case 0:
                t.text = (WC.CurrentParty[SelectedCharIndex].GetComponent<Character>().Attack[Index] + 1).ToString();
                break;
            case 1:
                int cost =  (WC.CurrentParty[SelectedCharIndex].GetComponent<Character>().Defence[Index] - 100) / 50;
                t.text = cost.ToString();
                break;
            case 2:
                t.text= (WC.CurrentParty[SelectedCharIndex].GetComponent<Character>().Regen[Index] * 20).ToString();
                break;
            case 3:
                t.text= (WC.CurrentParty[SelectedCharIndex].GetComponent<Character>().Max_Resource[Index] * 10).ToString();
                break;
            case 4:
                t.text = "10";
                break;
        }
    }

    public void ClearCost()
    {
        transform.GetChild(0).GetChild(7).GetComponent<Text>().text="";
    }

    public void SetSelectedChar(int index)
    {
        SelectedCharIndex = index;
        SelectionArrow.transform.SetParent(transform.GetChild(1 + index));
        SelectionArrow.transform.localPosition = new Vector2(0, .7f);
        SetUpPortrait();
    }

    void CleanUp()
    {
        WC.InEncounter = false;
        Destroy(gameObject);
    }

    public void BuffAttack(int index)
    {
        WC.CurrentParty[SelectedCharIndex].GetComponent<Character>().Attack[index] += 1;
        PayForBuff(0, index);
        SetUpPortrait();
    }

    public void BuffDefense(int index)
    {
        WC.CurrentParty[SelectedCharIndex].GetComponent<Character>().Defence[index] += 50;
        PayForBuff(1, index);
        SetUpPortrait();
    }

    public void BuffRegen(int index)
    {
        WC.CurrentParty[SelectedCharIndex].GetComponent<Character>().Regen[index] += 1;
        PayForBuff(2, index);
        SetUpPortrait();
    }

    public void BuffMax(int index)
    {
        WC.CurrentParty[SelectedCharIndex].GetComponent<Character>().Max_Resource[index] += 2;
        PayForBuff(3, index);
        SetUpPortrait();
    }

    public void BuffHealth()
    {
        WC.CurrentParty[SelectedCharIndex].GetComponent<Character>().MaxHealth += WC.RNG.Next(10,21);
        WC.CurrentParty[SelectedCharIndex].GetComponent<Character>().health = WC.CurrentParty[SelectedCharIndex].GetComponent<Character>().MaxHealth;
        PayForBuff(4, 0);
        SetUpPortrait();
    }

    public void PayForBuff(int type, int Index) // Attack, defense, regen, max, health
    {
        switch(type)
        {
            case 0:
               WC.UpdateCurrency(0,-1*WC.CurrentParty[SelectedCharIndex].GetComponent<Character>().Attack[Index]);
                break;
            case 1:
                int XpCost = -1 * (WC.CurrentParty[SelectedCharIndex].GetComponent<Character>().Defence[Index] - 100) / 50;
                WC.UpdateCurrency(0, XpCost);
                break;
            case 2:
                XpCost = -1 * WC.CurrentParty[SelectedCharIndex].GetComponent<Character>().Regen[Index] * 20;
                WC.UpdateCurrency(0, XpCost);
                break;
            case 3:
                XpCost = -1 * WC.CurrentParty[SelectedCharIndex].GetComponent<Character>().Max_Resource[Index] * 10;
                WC.UpdateCurrency(0, XpCost);
                break;
            case 4:
                XpCost = -10;
                WC.UpdateCurrency(0, XpCost);
                break;
        }
    }

    public void SetUpPortrait()
    {
        Character C=WC.CurrentParty[SelectedCharIndex].GetComponent<Character>();
        CurrentPortrait.GetComponent<SpriteRenderer>().sprite = C.Portrait;
        for(int i=0;i<4;i++)
        {
            CurrentPortrait.transform.GetChild(0).GetChild(0).GetChild(i).GetChild(0).GetComponent<Text>().text = C.Attack[i].ToString();
            CurrentPortrait.transform.GetChild(0).GetChild(1).GetChild(i).GetChild(0).GetComponent<Text>().text = C.Defence[i].ToString();
            CurrentPortrait.transform.GetChild(0).GetChild(2).GetChild(i).GetChild(0).GetComponent<Text>().text = C.Regen[i].ToString();
            CurrentPortrait.transform.GetChild(0).GetChild(3).GetChild(i).GetChild(0).GetComponent<Text>().text = C.Max_Resource[i].ToString();
        }
        CurrentPortrait.transform.GetChild(0).GetChild(4).GetChild(0).GetChild(0).GetComponent<Text>().text = C.MaxHealth.ToString();

    }

    // Update is called once per frame
    void Update () {
        if (SelectionArrow.transform.localPosition.y > .65f && dir == 1 || SelectionArrow.transform.localPosition.y < .55f && dir == -1)
            dir *= -1;
        SelectionArrow.transform.Translate(new Vector2(0, .15f * dir * Time.deltaTime));

    }
}
