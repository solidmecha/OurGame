﻿using System.Collections;
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

    public List<Skill> SkillSet = new List<Skill> { };
    public List<Status> Statuses = new List<Status> { };

    public void takeDamage(int dmg)
    {
        health -= dmg;
    }

    public void GenerateResources()
    {
        for(int i=0;i<4;i++)
        {
            if (Current_Resource[i] + Regen[i] <= Max_Resource[i])
                Current_Resource[i] += Regen[i];
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
            if (isPreCombat && !(Statuses[i].Behavior == Status.Status_Behavior.DoT)) //unique effects handled precombat
                Statuses[i].action(this);
            else if (!isPreCombat && Statuses[i].Behavior == Status.Status_Behavior.DoT)
                Statuses[i].action(this);
        }
        for(int i=0;i<Statuses.Count;i++)
        {
            if(Statuses[i].duration==0)
            {
                Statuses.RemoveAt(i);
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