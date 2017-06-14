using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleControl : MonoBehaviour {

    public enum GameState { Battle, World, Market};
    GameState CurrentState;

    public List<Character> Party;
    public List<Character> EnemyParty;
    public int TurnIndex;
    float CursorOffset;
    Skill SelectedSkill;
    public GameObject UI_Window;
    public GameObject CurrentUI_Canvas;
    public GameObject SkillButton;
    public GameObject TargetBox;
    public List<GameObject> TargetBoxes=new List<GameObject> { };
    public GameObject CurrentPortrait;

    public GameObject UI_Status_Window;
    
    void HandleBattle()
    {
        /*
         * Display Char info
         *Select Skill
         *Select target
         * 
         */
    }

    void DisplayCurrentSkills()
    {
        CurrentPortrait.GetComponent<SpriteRenderer>().sprite = Party[TurnIndex].Portrait;
        for (int i = 0; i < 4; i++)
        {
            CurrentPortrait.transform.GetChild(0).GetChild(i).GetChild(0).GetComponent<Text>().text = Party[TurnIndex].Current_Resource[i].ToString() + "/" + Party[TurnIndex].Max_Resource[i].ToString();
            CurrentPortrait.transform.GetChild(0).GetChild(i).GetChild(1).GetComponent<Text>().text = Party[TurnIndex].Regen[i].ToString();
        }
        CurrentPortrait.transform.GetChild(0).GetChild(4).GetChild(0).GetComponent<Text>().text = Party[TurnIndex].health.ToString() + "/" + Party[TurnIndex].MaxHealth.ToString();
        for (int i = 0; i < Party[TurnIndex].SkillSet.Count; i++)
        {
            GameObject go=Instantiate(SkillButton,CurrentUI_Canvas.transform.GetChild(i).position, Quaternion.identity, CurrentUI_Canvas.transform) as GameObject;
            go.transform.GetChild(0).GetComponent<Text>().text = Party[TurnIndex].SkillSet[i].Name;
            for(int j=0;j<4;j++)
            {
                go.transform.GetChild(1 + j).GetComponent<Text>().text = Party[TurnIndex].SkillSet[i].Costs[j].ToString();
            }
            int t = i;
            go.GetComponent<Button>().onClick.AddListener(delegate { SelectSkill(t); });
            go.GetComponent<Button>().interactable = checkCost(Party[TurnIndex], Party[TurnIndex].SkillSet[i]);
                
        }
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

        if (Party[TurnIndex].SkillSet[i].skillType==0)
        {
            SelectTargetSetUp();
        }
        else if(Party[TurnIndex].SkillSet[i].skillType == 1)
        {
            CastSkillOnTarget(Party[TurnIndex], Party[TurnIndex]);
            NextTurn();
        }
        else if(Party[TurnIndex].SkillSet[i].skillType==2)
        {
            foreach(Character C in EnemyParty)
                CastSkillOnTarget(Party[TurnIndex],C);
            NextTurn();
        }
        else if (Party[TurnIndex].SkillSet[i].skillType == 3)
        {
            foreach (Character C in Party)
                CastSkillOnTarget(Party[TurnIndex],C);
            NextTurn();
        }
    }

    void SelectTargetSetUp()
    {
        CurrentUI_Canvas.transform.GetChild(6).GetComponent<Text>().text = "Select Target";
        foreach (Character C in EnemyParty)
        {
            GameObject Go=Instantiate(TargetBox, C.transform.position, Quaternion.identity, C.transform) as GameObject;
            Go.GetComponent<TargetSelectScript>().BC = this;
            TargetBoxes.Add(Go);
        }

        foreach (Character C in Party)
        {
            GameObject Go = Instantiate(TargetBox, C.transform.position, Quaternion.identity, C.transform) as GameObject;
            Go.GetComponent<TargetSelectScript>().BC = this;
            TargetBoxes.Add(Go);
        }
    }

    public void CastSkillOnTarget(Character U, Character T)
    {
        SelectedSkill.fight(U, T);
        //play skill animation here eventually
    }

    public void NextTurn()
    {
        
        foreach (GameObject g in TargetBoxes)
            Destroy(g);
        TargetBoxes.Clear();
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
                Destroy(CurrentUI_Canvas.transform.parent.gameObject);
                GameObject Go = Instantiate(UI_Window, new Vector2(1.58f, -3.05f), Quaternion.identity) as GameObject;
                CurrentUI_Canvas = Go.transform.GetChild(0).gameObject;
                Party[TurnIndex].HandleStatusEffects(true);
                Party[TurnIndex].GenerateResources();
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
        Party[0].SkillSet.Add(Skill.searchID(0));
        Party[0].SkillSet.Add(Skill.searchID(1));
        Party[0].SkillSet.Add(Skill.searchID(2));
        Party[0].SkillSet.Add(Skill.searchID(3));
        Party[0].SkillSet.Add(Skill.searchID(4));
        Party[0].SkillSet.Add(Skill.searchID(5));
        Party[1].SkillSet.Add(Skill.searchID(11));
        Party[1].SkillSet.Add(Skill.searchID(6));
        Party[1].SkillSet.Add(Skill.searchID(0));
        EnemyParty[0].SkillSet.Add(Skill.searchID(0));
        EnemyParty[1].SkillSet.Add(Skill.searchID(0));
        DisplayCurrentSkills();
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
