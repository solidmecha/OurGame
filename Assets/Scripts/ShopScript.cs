using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopScript : MonoBehaviour {

    public WorldControl WC;
    public List<Skill> SkillsForSale=new List<Skill>{};
    public int SelectedCharIndex;
    public List<GameObject> CurrentSkillButtons = new List<GameObject> { };
    public int DisplayShopSkillIndex;
    public int CharacterSkillIndex;
    public GameObject SkillButton;
    public GameObject CurrentUI_Canvas;
    public GameObject ShopCharacter;
    public GameObject SelectionArrow;
    public GameObject CurrentPortrait;

    public List<GameObject> CurrentCharacterButtons = new List<GameObject> { };
    public GameObject Character_Skill_UI;

    float dir = 1;

    void DisplayShopSkills()
    {
        foreach (GameObject g in CurrentSkillButtons)
            Destroy(g);
        CurrentSkillButtons.Clear();
        int value = SkillsForSale.Count;

        if (DisplayShopSkillIndex + 6 < SkillsForSale.Count)
        {
            value = DisplayShopSkillIndex + 6;
        }

        for (int i = DisplayShopSkillIndex; i < value; i++)
        {
            GameObject go = Instantiate(SkillButton, CurrentUI_Canvas.transform.GetChild(i - DisplayShopSkillIndex).position, Quaternion.identity, CurrentUI_Canvas.transform) as GameObject;
            go.transform.GetChild(0).GetComponent<Text>().text = SkillsForSale[i].Name;
            for (int j = 0; j < 4; j++)
            {
                go.transform.GetChild(1 + j).GetComponent<Text>().text = SkillsForSale[i].Costs[j].ToString();
            }
            int t = i;
            go.GetComponent<Button>().onClick.AddListener(delegate { BuySkill(t); });
            go.GetComponent<Button>().interactable = (checkMaxRes(SkillsForSale[i]) && !hasSkill(SkillsForSale[i]));
            CurrentSkillButtons.Add(go);

        }

    }

    void DisplayCharSkills()
    {
        Character C = WC.CurrentParty[SelectedCharIndex].GetComponent<Character>();
        foreach (GameObject g in CurrentCharacterButtons)
            Destroy(g);
        CurrentCharacterButtons.Clear();
        int value = C.SkillSet.Count;

        if (CharacterSkillIndex + 6 < C.SkillSet.Count)
        {
            value = CharacterSkillIndex + 6;
        }

        for (int i = CharacterSkillIndex; i < value; i++)
        {
            GameObject go = Instantiate(SkillButton, Character_Skill_UI.transform.GetChild(0).GetChild(i - CharacterSkillIndex).position, Quaternion.identity, Character_Skill_UI.transform.GetChild(0)) as GameObject;
            go.transform.GetChild(0).GetComponent<Text>().text = C.SkillSet[i].Name;
            for (int j = 0; j < 4; j++)
            {
                go.transform.GetChild(1 + j).GetComponent<Text>().text = C.SkillSet[i].Costs[j].ToString();
            }
            int t = i;
            CurrentCharacterButtons.Add(go);

        }
    }

    void SetUpPortrait()
    {
        Character C = WC.CurrentParty[SelectedCharIndex].GetComponent<Character>();
        CurrentPortrait.GetComponent<SpriteRenderer>().sprite = C.Portrait;
        for (int i = 0; i < 4; i++)
        {
            CurrentPortrait.transform.GetChild(0).GetChild(i).GetChild(0).GetComponent<Text>().text = C.Current_Resource[i].ToString() + "/" + C.Max_Resource[i].ToString();
            CurrentPortrait.transform.GetChild(0).GetChild(i).GetChild(1).GetComponent<Text>().text = C.Regen[i].ToString();
        }
        CurrentPortrait.transform.GetChild(0).GetChild(4).GetChild(0).GetComponent<Text>().text = C.health.ToString() + "/" + C.MaxHealth.ToString();
    }

    void DisplayNextSkillPage()
    {
        if (DisplayShopSkillIndex + 6 < SkillsForSale.Count)
        {
            DisplayShopSkillIndex += 6;
            DisplayShopSkills();
        }

    }

    void DisplayPrevSkillPage()
    {
        if (DisplayShopSkillIndex - 6 >= 0)
        {
            DisplayShopSkillIndex -= 6;
            DisplayShopSkills();
        }

    }

    void NextCharacterSkillPage()
    {
        if (CharacterSkillIndex + 6 < WC.CurrentParty[SelectedCharIndex].GetComponent<Character>().SkillSet.Count)
        {
            CharacterSkillIndex += 6;
            DisplayCharSkills();
        }

    }

    void PrevCharacterSkillPage()
    {
        if (CharacterSkillIndex - 6 >= 0)
        {
            CharacterSkillIndex -= 6;
            DisplayCharSkills();
        }
    }

    bool checkMaxRes(Skill S)
    {
        Character C = WC.CurrentParty[SelectedCharIndex].GetComponent<Character>();
        for(int i=0;i<4;i++)
        {
            if(C.Max_Resource[i]<S.Costs[i])
            {
                return false;
            }
        }
        return true;
    }

    bool hasSkill(Skill S)
    {
        Character C = WC.CurrentParty[SelectedCharIndex].GetComponent<Character>();
        foreach(Skill skill in C.SkillSet)
        {
            if(skill.Name.Equals(S.Name))
            {
                return true;
            }
        }
        return false;
    }

    public void SetSelectedChar(int index)
    {
        SelectedCharIndex = index;
        SelectionArrow.transform.SetParent(transform.GetChild(1 + index));
        SelectionArrow.transform.localPosition = new Vector2(0,.7f);
        SetUpPortrait();
        DisplayCharSkills();
    }


    void BuySkill(int index)
    {
        WC.CurrentParty[SelectedCharIndex].GetComponent<Character>().SkillSet.Add(SkillsForSale[index]);
        while (WC.CurrentParty[SelectedCharIndex].GetComponent<Character>().SkillSet.Count > 6 + CharacterSkillIndex)
            NextCharacterSkillPage();
        DisplayCharSkills();
        DisplayShopSkills();
    }

    // Use this for initialization
    void Start () {
        WC = GameObject.FindGameObjectWithTag("GameController").GetComponent<WorldControl>();

        for (int i = 0; i < 26; i++)
            SkillsForSale.Add(Skill.searchID(i));

        CurrentPortrait = Instantiate(CurrentPortrait, new Vector2(-5.9f, -2.29f), Quaternion.identity) as GameObject;
       
        SelectionArrow = transform.GetChild(5).gameObject;
        Character_Skill_UI = Instantiate(Character_Skill_UI, new Vector2(1.58f, -3.05f), Quaternion.identity) as GameObject;
        Character_Skill_UI.transform.GetChild(0).GetChild(6).GetComponent<Button>().onClick.AddListener(delegate { NextCharacterSkillPage(); });
        Character_Skill_UI.transform.GetChild(0).GetChild(7).GetComponent<Button>().onClick.AddListener(delegate { PrevCharacterSkillPage(); });
        CurrentUI_Canvas = transform.GetChild(0).gameObject;
        CurrentUI_Canvas.transform.GetChild(6).GetComponent<Button>().onClick.AddListener(delegate { DisplayNextSkillPage(); });
        CurrentUI_Canvas.transform.GetChild(7).GetComponent<Button>().onClick.AddListener(delegate { DisplayPrevSkillPage(); });
        CurrentUI_Canvas.transform.GetChild(8).GetComponent<Button>().onClick.AddListener(delegate { CleanUpShop(); });
        for (int i=0; i<WC.CurrentParty.Count;i++)
        {
            GameObject G=Instantiate(ShopCharacter, transform.GetChild(1 + i).position, Quaternion.identity) as GameObject;
            G.transform.SetParent(transform);
            G.GetComponent<ShopSelectCharacter>().Index = i;
            G.GetComponent<SpriteRenderer>().sprite = WC.CurrentParty[i].GetComponent<SpriteRenderer>().sprite;
        }
        SetSelectedChar(0);
        DisplayShopSkills();
        DisplayCharSkills();

    }

    void CleanUpShop()
    {
        Destroy(CurrentPortrait);
        Destroy(CurrentUI_Canvas.transform.root.gameObject);
        Destroy(Character_Skill_UI);
    }
	
	// Update is called once per frame
	void Update () {
        if (SelectionArrow.transform.localPosition.y > .65f && dir==1 || SelectionArrow.transform.localPosition.y < .55f && dir == -1)
            dir *= -1;
        SelectionArrow.transform.Translate(new Vector2(0, .15f * dir * Time.deltaTime));

    }
}
