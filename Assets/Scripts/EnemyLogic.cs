using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyLogic : MonoBehaviour {

    public BattleControl BC;
    Character ThisChar;
    System.Random RNG = new System.Random();

    public void chooseSkill()
    {
        List<Skill> UsableSkills = new List<Skill> { };
        foreach(Skill skill in ThisChar.SkillSet)
        {
            if (ThisChar.hasResources(skill.Costs))
            {
                if (skill.BuffList.Count > 0)
                {
                    foreach (Status stat in skill.BuffList)
                    {
                        if (!skill.hasStatusOfSameSkill(stat, ThisChar))
                        {
                            int multi = skill.Costs[0] + skill.Costs[1] + skill.Costs[2] + skill.Costs[3];
                            for (int i = 0; i < multi; i++)
                                UsableSkills.Add(skill);
                        }
                    }
                }
                else
                {
                   int multi = skill.Costs[0] + skill.Costs[1] + skill.Costs[2] + skill.Costs[3];
                   for (int i = -3; i < multi; i++)
                       UsableSkills.Add(skill);
                }
            }
        }
        Skill S = UsableSkills[RNG.Next(UsableSkills.Count)];
        ThisChar.PayForSkill(S.Costs);
        BC.CurrentUI_Canvas.transform.GetChild(8).GetComponent<Text>().text = ThisChar.name + " uses " + S.Name;
        if (S.skillType==0)
        {
            List<Character> tempCharList = new List<Character>() { };
            foreach(Character c in BC.Party)
            {
                if (c.health > 0)
                    tempCharList.Add(c);
            }
            int r= RNG.Next(tempCharList.Count);
            S.fight(ThisChar, tempCharList[r]);
            BC.CurrentUI_Canvas.transform.GetChild(8).GetComponent<Text>().text += "On "+ tempCharList[r].name;
        }
        else if(S.skillType==1)
        {
            S.fight(ThisChar, ThisChar);
        }
        else if(S.skillType==2)
        {
            List<Character> tempCharList = new List<Character>() { };
            foreach (Character c in BC.EnemyParty)
            {
                if (c.health > 0)
                    tempCharList.Add(c);
            }
            int r = RNG.Next(tempCharList.Count);
            S.fight(ThisChar, tempCharList[r]);
            BC.CurrentUI_Canvas.transform.GetChild(8).GetComponent<Text>().text += "On" + tempCharList[r].name;
        }
        else if(S.skillType==3)
        {
            foreach (Character c in BC.EnemyParty)
            {
                if (c.health > 0)
                    S.fight(ThisChar, c);
            }
        }
        else if (S.skillType == 4)
        {
            foreach (Character c in BC.Party)
            {
                if (c.health > 0)
                    S.fight(ThisChar, c);
            }
        }

    }

    public void GenerateEnemy()
    {
        ThisChar.health = BC.WC.RNG.Next(750, 4000);
        int N = BC.WC.RNG.Next(4, 9);
        ThisChar.SkillSet.Add(Skill.searchID(RNG.Next(3)));
        for (int i=0; i<N;i++)
        {
            ThisChar.SkillSet.Add(Skill.searchID(RNG.Next(25)));
        }
    }

	// Use this for initialization
	void Start () {
        ThisChar = GetComponent<Character>();
        BC = GameObject.FindGameObjectWithTag("GameController").GetComponent<BattleControl>();
        GenerateEnemy();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
