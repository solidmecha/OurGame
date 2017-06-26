using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {

    public int health;
    public int MaxHealth;
    public Sprite Portrait;
    public int[] Attack = new int[4];
    public int[] Defence = new int[4];
    public int[] Regen = new int[4];
    public int[] Current_Resource = new int[4];
    public int[] Max_Resource = new int[4];
    public int Level;

    public List<Skill> SkillSet = new List<Skill> { };
    public List<Status> Statuses = new List<Status> { };

    public void takeDamage(int dmg)
    {
        print(name + " " + dmg);
        health -= dmg;
        if (health > MaxHealth)
            health = MaxHealth;
        else if (health <= 0)
        {
            health = 0;
            GameObject.FindGameObjectWithTag("GameController").GetComponent<BattleControl>().CheckVictory();
        }
    }

    public bool hasResources(int[] ia)
    {
        for (int i = 0; i < 4; i++)
        {
            if (Current_Resource[i] < ia[i])
                return false;
        }
        return true;
     }

    public void GenerateResources()
    {
        for(int i=0;i<4;i++)
        {
            if(Current_Resource[i] + Regen[i] > Max_Resource[i])
            {
                Current_Resource[i] = Max_Resource[i];
            }
            else if (Current_Resource[i] + Regen[i] <= Max_Resource[i])
                Current_Resource[i] += Regen[i];
            if (Current_Resource[i] < 0)
                Current_Resource[i] = 0;
        }

    }

    public void PayForSkill(int[] costs)
    {
        for (int i = 0; i < 4; i++)
            Current_Resource[i] -= costs[i];
    }

    public void HandleStatusEffects(bool isPreCombat)
    {
        for (int i = 0; i < Statuses.Count; i++)
        {
            if (isPreCombat && Statuses[i].Behavior != Status.Status_Behavior.DoT && Statuses[i].Stat_Target != Status.Stat_Target_type.Defence) //unique effects handled precombat
                Statuses[i].action(this);
            else if (!isPreCombat && (Statuses[i].Behavior == Status.Status_Behavior.DoT || Statuses[i].Stat_Target==Status.Stat_Target_type.Defence))
                Statuses[i].action(this);
        }
        for(int i=0;i<Statuses.Count;i++)
        {
            if(Statuses[i].duration==0)
            {
                Statuses.RemoveAt(i);
                i--;
            }
        }

    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	}

}
