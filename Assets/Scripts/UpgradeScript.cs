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

    }

    public void SetSelectedChar(int index)
    {
        SelectedCharIndex = index;
        SelectionArrow.transform.SetParent(transform.GetChild(1 + index));
        SelectionArrow.transform.localPosition = new Vector2(0, .7f);
        SetUpPortrait();
        for (int i = 0; i < 4; i++)
        {
            int t = i;
            CurrentPortrait.transform.GetChild(0).GetChild(0).GetChild(i).GetComponent<Button>().onClick.AddListener(delegate { BuffAttack(t); });
            CurrentPortrait.transform.GetChild(0).GetChild(1).GetChild(i).GetComponent<Button>().onClick.AddListener(delegate { BuffDefense(t); });
            CurrentPortrait.transform.GetChild(0).GetChild(2).GetChild(i).GetComponent<Button>().onClick.AddListener(delegate { BuffRegen(t); });
            CurrentPortrait.transform.GetChild(0).GetChild(3).GetChild(i).GetComponent<Button>().onClick.AddListener(delegate { BuffMax(t); });
        }
        CurrentPortrait.transform.GetChild(0).GetChild(4).GetChild(0).GetComponent<Button>().onClick.AddListener(delegate { BuffHealth(); });
        transform.GetChild(0).GetChild(1).GetComponent<Button>().onClick.AddListener(delegate { CleanUp(); });
    }

    void CleanUp()
    {
        WC.InEncounter = false;
        Destroy(gameObject);
    }

    public void BuffAttack(int index)
    {
        WC.CurrentParty[SelectedCharIndex].GetComponent<Character>().Attack[index] += 1;
        SetUpPortrait();
    }

    public void BuffDefense(int index)
    {
        WC.CurrentParty[SelectedCharIndex].GetComponent<Character>().Defence[index] += 50;
        SetUpPortrait();
    }

    public void BuffRegen(int index)
    {
        WC.CurrentParty[SelectedCharIndex].GetComponent<Character>().Regen[index] += 1;
        SetUpPortrait();
    }

    public void BuffMax(int index)
    {
        WC.CurrentParty[SelectedCharIndex].GetComponent<Character>().Max_Resource[index] += 2;
        SetUpPortrait();
    }

    public void BuffHealth()
    {
        WC.CurrentParty[SelectedCharIndex].GetComponent<Character>().MaxHealth += WC.RNG.Next(10,21);
        WC.CurrentParty[SelectedCharIndex].GetComponent<Character>().health = WC.CurrentParty[SelectedCharIndex].GetComponent<Character>().MaxHealth;
        SetUpPortrait();
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
