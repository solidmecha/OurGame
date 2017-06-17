using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleControl : MonoBehaviour {

    public WorldControl WC;

    public List<Character> Party;
    public List<Character> EnemyParty;
    public int TurnIndex;
    float CursorOffset;
    Skill SelectedSkill;
    public GameObject UI_Window;
    public GameObject CurrentUI_Canvas;
    public GameObject SkillButton;
    public GameObject CurrentPortrait;
    public int DisplaySkillIndex;
    public List<GameObject> CurrentSkillButtons = new List<GameObject> { };
    public GameObject UI_Status_Window;

    public GameObject Background;
    public GameObject EnemyPrefab;
    
    public void HandleBattle()
    {
        WC.MoveMap();
        Background=Instantiate(Background, Vector2.zero, Quaternion.identity);
        int R = WC.RNG.Next(1, 7);
        for(int i=0;i<R;i++)
        {
           GameObject go = Instantiate(EnemyPrefab, Background.transform.GetChild(0).GetChild(i).position, Quaternion.identity) as GameObject;
            EnemyParty.Add(go.GetComponent<Character>());
        }
        for(int i=0;i<WC.CurrentParty.Count;i++)
        {
            GameObject go = Instantiate(WC.CurrentParty[i], Background.transform.GetChild(1).GetChild(i).position, Quaternion.identity) as GameObject;
            Party.Add(go.GetComponent<Character>());
            foreach (Skill S in WC.CurrentParty[i].GetComponent<Character>().SkillSet)
                Party[i].SkillSet.Add(S);
        }
        CurrentPortrait = Instantiate(CurrentPortrait, new Vector2(-5.9f, -2.29f), Quaternion.identity) as GameObject;
        TurnIndex = -1;
        NextTurn();
    }

    void DisplayCurrentSkills()
    {
        print(Party[TurnIndex].SkillSet.Count);
        foreach (GameObject g in CurrentSkillButtons)
            Destroy(g);
        CurrentSkillButtons.Clear();
        int value= Party[TurnIndex].SkillSet.Count;
        if (DisplaySkillIndex + 6 < Party[TurnIndex].SkillSet.Count)
        {
            value = DisplaySkillIndex + 6;
        }

        for (int i = DisplaySkillIndex; i < value; i++)
            {
                GameObject go = Instantiate(SkillButton, CurrentUI_Canvas.transform.GetChild(i-DisplaySkillIndex).position, Quaternion.identity, CurrentUI_Canvas.transform) as GameObject;
                go.transform.GetChild(0).GetComponent<Text>().text = Party[TurnIndex].SkillSet[i].Name;
                for (int j = 0; j < 4; j++)
                {
                    go.transform.GetChild(1 + j).GetComponent<Text>().text = Party[TurnIndex].SkillSet[i].Costs[j].ToString();
                }
                int t = i;
                go.GetComponent<Button>().onClick.AddListener(delegate { SelectSkill(t); });
                go.GetComponent<Button>().interactable = checkCost(Party[TurnIndex], Party[TurnIndex].SkillSet[i]);
            CurrentSkillButtons.Add(go);

            }
        
    }

    void DisplayNextSkillPage()
    {
        if (DisplaySkillIndex + 6 < Party[TurnIndex].SkillSet.Count)
        {
            DisplaySkillIndex += 6;
            DisplayCurrentSkills();
        }
        
    }

    void DisplayPrevSkillPage()
    {
        if (DisplaySkillIndex - 6 >= 0)
        {
            DisplaySkillIndex -= 6;
            DisplayCurrentSkills();
        }

    }

    void SetUpPortrait()
    {
        CurrentPortrait.GetComponent<SpriteRenderer>().sprite = Party[TurnIndex].Portrait;
        for (int i = 0; i < 4; i++)
        {
            CurrentPortrait.transform.GetChild(0).GetChild(i).GetChild(0).GetComponent<Text>().text = Party[TurnIndex].Current_Resource[i].ToString() + "/" + Party[TurnIndex].Max_Resource[i].ToString();
            CurrentPortrait.transform.GetChild(0).GetChild(i).GetChild(1).GetComponent<Text>().text = Party[TurnIndex].Regen[i].ToString();
        }
        CurrentPortrait.transform.GetChild(0).GetChild(4).GetChild(0).GetComponent<Text>().text = Party[TurnIndex].health.ToString() + "/" + Party[TurnIndex].MaxHealth.ToString();
    }

    bool checkCost(Character C, Skill S)
    {
        for(int i=0;i<4;i++)
        {
            if (C.Current_Resource[i] < S.Costs[i])
                return false;
        }

        return true;
    }

    void SelectSkill(int i)
    {
        SelectedSkill = Party[TurnIndex].SkillSet[i];

        if (Party[TurnIndex].SkillSet[i].skillType==0 || Party[TurnIndex].SkillSet[i].skillType==2)
        {
            SelectTargetSetUp();
        }
        else if(Party[TurnIndex].SkillSet[i].skillType == 1)
        {
            CastSkillOnTarget(Party[TurnIndex], Party[TurnIndex]);
            NextTurn();
        }       
        else if (Party[TurnIndex].SkillSet[i].skillType == 3)
        {
            foreach (Character C in Party)
                CastSkillOnTarget(Party[TurnIndex],C);
            NextTurn();
        }
        else if (Party[TurnIndex].SkillSet[i].skillType == 4)
        {
            foreach (Character C in EnemyParty)
                CastSkillOnTarget(Party[TurnIndex], C);
            NextTurn();
        }
    }

    void SelectTargetSetUp()
    {
        CurrentUI_Canvas.transform.GetChild(8).GetComponent<Text>().text = "Select Target";
        foreach (Character C in EnemyParty)
        {
           TargetSelectScript tss= C.gameObject.AddComponent<TargetSelectScript>();
            tss.BC = this;
        }

        foreach (Character C in Party)
        {
            TargetSelectScript tss = C.gameObject.AddComponent<TargetSelectScript>();
            tss.BC = this;
        }
    }

    public void CastSkillOnTarget(Character U, Character T)
    {
        SelectedSkill.fight(U, T);
        //play skill animation here eventually
    }

    public void NextTurn()
    {

        foreach (Character C in EnemyParty)
        {
            Destroy(C.gameObject.GetComponent<TargetSelectScript>());
        }

        foreach (Character C in Party)
        {
            Destroy(C.gameObject.GetComponent<TargetSelectScript>());
        }

        if (TurnIndex >= 0)
        {
            Party[TurnIndex].PayForSkill(SelectedSkill.Costs);
            Party[TurnIndex].HandleStatusEffects(false);
        }

        TurnIndex++;
        if(TurnIndex==Party.Count)
        {
            TurnIndex = 0;
            EnemyTurn();
        }
        else
        {
            if (Party[TurnIndex].health > 0)
            {
                if(CurrentUI_Canvas!=null)
                    Destroy(CurrentUI_Canvas.transform.parent.gameObject);
                GameObject Go = Instantiate(UI_Window, new Vector2(1.58f, -3.05f), Quaternion.identity) as GameObject;
                CurrentUI_Canvas = Go.transform.GetChild(0).gameObject;
                CurrentUI_Canvas.transform.GetChild(6).GetComponent<Button>().onClick.AddListener(delegate { DisplayNextSkillPage(); });
                CurrentUI_Canvas.transform.GetChild(7).GetComponent<Button>().onClick.AddListener(delegate { DisplayPrevSkillPage(); });
                Party[TurnIndex].HandleStatusEffects(true);
                Party[TurnIndex].GenerateResources();
                DisplaySkillIndex = 0;
                SetUpPortrait();
                DisplayCurrentSkills();
            }
            else
                NextTurn();
        }

    }

    void EnemyTurn()
    {
            if (EnemyParty[TurnIndex].health > 0)
            {
            CurrentPortrait.GetComponent<SpriteRenderer>().sprite = EnemyParty[TurnIndex].Portrait;
            Destroy(CurrentUI_Canvas.transform.parent.gameObject);
            GameObject go = Instantiate(UI_Window, new Vector2(1.58f, -3.05f), Quaternion.identity) as GameObject;
            CurrentUI_Canvas = go.transform.GetChild(0).gameObject;

            for (int i = 0; i < 4; i++)
            {
                CurrentPortrait.transform.GetChild(0).GetChild(i).GetChild(0).GetComponent<Text>().text = EnemyParty[TurnIndex].Current_Resource[i].ToString() + "/" + EnemyParty[TurnIndex].Max_Resource[i].ToString();
                CurrentPortrait.transform.GetChild(0).GetChild(i).GetChild(1).GetComponent<Text>().text = EnemyParty[TurnIndex].Regen[i].ToString();
            }
            CurrentPortrait.transform.GetChild(0).GetChild(4).GetChild(0).GetComponent<Text>().text = EnemyParty[TurnIndex].health.ToString() + "/" + EnemyParty[TurnIndex].MaxHealth.ToString();


            go = Instantiate(SkillButton, CurrentUI_Canvas.transform.GetChild(0).position, Quaternion.identity, CurrentUI_Canvas.transform) as GameObject;
            go.transform.GetChild(0).GetComponent<Text>().text = "Continue";
            go.GetComponent<Button>().onClick.AddListener(delegate { ContinueEnemyTurn(); });
            EnemyParty[TurnIndex].HandleStatusEffects(true);
            EnemyParty[TurnIndex].GenerateResources();
            EnemyParty[TurnIndex].GetComponent<EnemyLogic>().chooseSkill();
            EnemyParty[TurnIndex].HandleStatusEffects(false);
            }
    }

    void ContinueEnemyTurn()
    {
        if (TurnIndex == EnemyParty.Count-1)
        {
            TurnIndex = -1;
            NextTurn();
        }
        else
        {
            TurnIndex++;
            EnemyTurn();
        }
    }

	// Use this for initialization
	void Start () {

        WC = GetComponent<WorldControl>();
        /*
        for(int i=0;i<26;i++)
            Party[0].SkillSet.Add(Skill.searchID(i));

        Party[1].SkillSet.Add(Skill.searchID(11));
        Party[1].SkillSet.Add(Skill.searchID(6));
        Party[1].SkillSet.Add(Skill.searchID(0));
        EnemyParty[0].SkillSet.Add(Skill.searchID(0));
        EnemyParty[0].SkillSet.Add(Skill.searchID(6));
        EnemyParty[0].SkillSet.Add(Skill.searchID(7));
        EnemyParty[0].SkillSet.Add(Skill.searchID(21));
        EnemyParty[1].SkillSet.Add(Skill.searchID(0));
        EnemyParty[0].SkillSet.Add(Skill.searchID(6));
        EnemyParty[1].SkillSet.Add(Skill.searchID(15));
        EnemyParty[1].SkillSet.Add(Skill.searchID(21));

        TurnIndex = -1;
        NextTurn(); 
        */
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
