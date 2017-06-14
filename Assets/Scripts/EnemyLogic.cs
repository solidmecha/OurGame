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
        Skill S=ThisChar.SkillSet[RNG.Next(ThisChar.SkillSet.Count)];
        if(S.skillType==0)
        {
            List<Character> tempCharList = new List<Character>() { };
            foreach(Character c in BC.Party)
            {
                if (c.health > 0)
                    tempCharList.Add(c);
            }
            int r= RNG.Next(tempCharList.Count);
            S.fight(ThisChar, tempCharList[r]);
            BC.CurrentUI_Canvas.transform.GetChild(6).GetComponent<Text>().text = ThisChar.name+ " uses " +S.Name+ "On "+ tempCharList[r].name;
        }
            
    }

	// Use this for initialization
	void Start () {
        ThisChar = GetComponent<Character>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
