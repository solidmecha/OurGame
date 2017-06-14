﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Status {

    public string name;
    public int ID;
    public enum Stat_Target_type{Attack, Defence, Regen, Health};
    public enum Status_Behavior { DoT, OneTime, Unique}; //DoTs are called post combat
    public int[] ArrayModifiers=new int[4]; //one time changes to stats or the modifiers for DoTs
    Stat_Target_type Stat_Target;
    public Status_Behavior Behavior;
    public int duration;
    public Action<Character> action;
    public int BaseValue; //set equal to duration to apply 1 time effects, otherwise base dmg for dots, -1 and -1 duration for perma buffs
    public List<Skill> skillSet=new List<Skill> { }; //when needed
    public Character Attacker;

    public Status() { }

    public Status(int id, string n, int[] change, Stat_Target_type stt, Status_Behavior behave, int dur, int baseVal)
    {
        ID = id;
        name = n;
        ArrayModifiers = change;
        Stat_Target = stt;
        Behavior = behave;
        duration = dur;
        BaseValue = baseVal; 
    }

    public Status(Status S)
    {
        ID = S.ID;
        name = S.name;
        ArrayModifiers = S.ArrayModifiers;
        Stat_Target = S.Stat_Target;
        Behavior = S.Behavior;
        duration = S.duration;
        foreach (Skill skill in S.skillSet)
            skillSet.Add(skill);
        BaseValue = S.BaseValue;
        SetAction();
    }

    public Status(int id, string n, Status_Behavior behave, int dur, int baseVal)
    {
        ID = id;
        name = n;
        Behavior = behave;
        duration = dur;
        BaseValue = baseVal;
    }

    public void RollStatus(Character C)
    {
        if ( Behavior==Status_Behavior.OneTime)
        {
            if(duration==BaseValue)
            {
                switch (Stat_Target)
                {
                    case Stat_Target_type.Attack:
                        for (int i = 0; i < 4; i++)
                        {
                            C.Attack[i] += ArrayModifiers[i];
                        }
                        break;
                    case Stat_Target_type.Defence:
                        for (int i = 0; i < 4; i++)
                        {
                            C.Defence[i] += ArrayModifiers[i];
                        }
                        break;
                    case Stat_Target_type.Regen:
                        for (int i = 0; i < 4; i++)
                        {
                            C.Regen[i] += ArrayModifiers[i];
                        }
                        break;
                }

            }
            if (duration == 1)
            {
                switch (Stat_Target)
                {
                    case Stat_Target_type.Attack:
                        for (int i = 0; i < 4; i++)
                        {
                            C.Attack[i] -= ArrayModifiers[i];
                        }
                        break;
                    case Stat_Target_type.Defence:
                        for (int i = 0; i < 4; i++)
                        {
                            C.Defence[i] -= ArrayModifiers[i];
                        }
                        break;
                    case Stat_Target_type.Regen:
                        for (int i = 0; i < 4; i++)
                        {
                            C.Regen[i] -= ArrayModifiers[i];
                        }
                        break;
                }
            }
        }
        else if(Behavior==Status_Behavior.DoT)
        {
            Tick(C);
        }

        duration--;

    }

    public void Tick(Character C)
    {
        float Dmg = 0;
        float[] mods = new float[4];

        float totalMods = ArrayModifiers[0] + ArrayModifiers[1] + ArrayModifiers[2] + ArrayModifiers[3];

        if (BaseValue > 0)
        {
            for (int i = 0; i < 4; i++)
            {
                mods[i] = ArrayModifiers[i] / totalMods;
                Dmg += ((Attacker.Attack[i] + 100) / (C.Defence[i] + 100)) * BaseValue * mods[i];
            }
        }
        else //healing
        {           
            for (int i = 0; i < 4; i++)
            {
                mods[i] = ArrayModifiers[i] / totalMods;
                Dmg += ((Attacker.Attack[i] + 100) /100) * BaseValue * mods[i];
            }
        }
        C.takeDamage((int)Mathf.Round(Dmg));
    }

    public void Beserker(Character C)
    {
        
        if (duration == BaseValue)
        {
            C.Current_Resource[0] = 2;
            C.Current_Resource[1] = 1;
            C.Current_Resource[2] = 0;
            C.Current_Resource[3] = 0;
            foreach (Skill S in C.SkillSet)
                skillSet.Add(S);
            C.SkillSet.Clear();
            C.SkillSet.Add(Skill.searchID(7));
        }
        else if(duration==1)
        {
            C.SkillSet.Clear();
            foreach (Skill S in skillSet)
                C.SkillSet.Add(S);
        }
        else
        {
            C.Current_Resource[0] = 2;
            C.Current_Resource[1] = 1;
            C.Current_Resource[2] = 0;
            C.Current_Resource[3] = 0;
        }
        duration--;
    }

    public void WolfForm(Character C)
    {
        if (duration == BaseValue)
        {
            foreach (Skill S in C.SkillSet)
            {
                skillSet.Add(S);
            }
            C.SkillSet.Clear();
            C.SkillSet.Add(Skill.searchID(12));
            C.SkillSet.Add(Skill.searchID(13));
            C.SkillSet.Add(Skill.searchID(14));
            foreach (Skill S in skillSet)
                C.SkillSet[2].BuffList[0].skillSet.Add(S);
        }
        duration--;
    }

    public void HumanForm(Character C)
    {
        if (duration == BaseValue)
        {
            C.SkillSet.Clear();
            foreach (Skill S in skillSet)
                C.SkillSet.Add(S);
        }
        duration--;
    }

    public void StingingSwarmDamage(Character C)
    {
        System.Random RNG = new System.Random();
        int dmg = 0;
        for(int i=0;i<1000;i++)
        {
            if (RNG.Next(100) < BaseValue)
                dmg++;
        }
        MonoBehaviour.print("Stinging Swarm deals " + dmg);
        C.takeDamage(dmg);
        duration--;
    }

    public void Prepare(Character C)
    {
        foreach (Skill S in C.SkillSet)
        {
            float D = S.BaseDamage * 1.25f;
            S.BaseDamage = Mathf.RoundToInt(D);
            foreach(Status stat in S.DebuffList)
            {
                if(stat.Behavior==Status_Behavior.DoT)
                {
                    stat.BaseValue = Mathf.RoundToInt(stat.BaseValue * 1.5f);
                }
            }
        }
    }

    public void SetAction()
    {
        action = ActionByID(ID);
    }

    public static Status StatusByID(int id)
    {
        switch(id)
        {
            case 0:
                Status Poison = new Status(0, "Poison", new int[4] { 1,0,0,0 }, Stat_Target_type.Health, Status_Behavior.DoT, 5, 10);
                Poison.action = Poison.RollStatus;
                return Poison;
            case 1:
                Status GenFire = new Status(1, "Generate Fire", new int[4] { 0, 2, 0, 0 }, Stat_Target_type.Regen, Status_Behavior.OneTime, 5, 5);
                GenFire.action = GenFire.RollStatus;
                return GenFire;
            case 2:
                Status Burn=new Status(2, "Burning", new int[4] { 0, 2, 0, 0 }, Stat_Target_type.Health, Status_Behavior.DoT, 5, 10);
                Burn.action = Burn.RollStatus;
                return Burn;
            case 3:
                Status Heal = new Status(3, "Healing", new int[4] { 0, 0, 3, 0 }, Stat_Target_type.Health, Status_Behavior.DoT, 5, 8);
                Heal.action = Heal.RollStatus;
                return Heal;
            case 4:
                Status BuffFireAtk = new Status(4, "Fire Attack+", new int[4] { 0, 100, 0, 0 }, Stat_Target_type.Attack, Status_Behavior.OneTime, 7, 7);
                BuffFireAtk.action = BuffFireAtk.RollStatus;
                return BuffFireAtk;
            case 5:
                Status BuffFireDef = new Status(5, "Fire Defence+", new int[4] { 0, 50, 0, 0 }, Stat_Target_type.Defence, Status_Behavior.OneTime, 7, 7);
                BuffFireDef.action = BuffFireDef.RollStatus;
                return BuffFireDef;
            case 6:
                Status BeserkStatus = new Status(6, "Beserk", Status_Behavior.Unique, 6, 6);
                BeserkStatus.action = BeserkStatus.Beserker;
                return BeserkStatus;
            case 7:
                Status BuffAtkandFire = new Status(7, "Phys and Fire Atk+", new int[4] { 250, 250, 0, 0 }, Stat_Target_type.Attack, Status_Behavior.OneTime, 6, 6);
                BuffAtkandFire.action = BuffAtkandFire.RollStatus;
                return BuffAtkandFire;
            case 8:
                Status ArcaneIntellectRegen = new Status(8, "Regen Mana", new int[4] { -1, -1, 5, -1 }, Stat_Target_type.Regen, Status_Behavior.OneTime, 5, 5);
                ArcaneIntellectRegen.action = ArcaneIntellectRegen.RollStatus;
                return ArcaneIntellectRegen;
            case 9:
                Status StingingSwarmStatus = new Status(9, "Surrounded by wasps", Status_Behavior.DoT, 1, 10);
                StingingSwarmStatus.action = StingingSwarmStatus.StingingSwarmDamage;
                return StingingSwarmStatus;
            case 10:
                Status DropPhysDef = new Status(10, "Physical Defense Lowered", new int[4] { -10, 0, 0, 0 }, Stat_Target_type.Defence, Status_Behavior.OneTime, 5, 5);
                DropPhysDef.action = DropPhysDef.RollStatus;
                return DropPhysDef;
            case 11:
                Status WolfTransform = new Status(11, "Turn into werewolf", Status_Behavior.Unique, 1, 1);
               // WolfTransform.action = WolfTransform.WolfForm;
                return WolfTransform;
            case 12:
                Status HumanTransform = new Status(12, "Turn back human", Status_Behavior.Unique, 1, 1);
               // HumanTransform.action = HumanTransform.HumanForm;
                return HumanTransform;
            case 13:
                Status BuffAllAtk = new Status(13, "Buff all Attack stats", new int[4] {20,20,20,0}, Stat_Target_type.Attack, Status_Behavior.OneTime, 4, 4);
                BuffAllAtk.action = BuffAllAtk.RollStatus;
                return BuffAllAtk;
            case 14:
                Status Blocking = new Status(14, "Block phyical", new int[4] { 200, 0, 0, 0 }, Stat_Target_type.Defence, Status_Behavior.OneTime, 2, 2);
                Blocking.action = Blocking.RollStatus;
                return Blocking;
            case 15:
                Status FireHose = new Status(15, "Reduced Fire Attack", new int[4] { 0, -100, 0, 0 }, Stat_Target_type.Attack, Status_Behavior.OneTime, 5, 5);
                FireHose.action = FireHose.RollStatus;
                return FireHose;
            case 16:
                Status ReduceStam = new Status(16, "Stamina Reduced", new int[4] { -50, 0, 0, 0 }, Stat_Target_type.Regen, Status_Behavior.OneTime, 2, 2);
                ReduceStam.action = ReduceStam.RollStatus;
                return ReduceStam;
            case 17:
                Status PrepareStat = new Status(17, "PrepareStat", Status.Status_Behavior.Unique, -1, -1);
                PrepareStat.SetAction();
                return PrepareStat;
            default:
                Status Safe = new Status();
                return Safe;
        }
    }

    Action<Character> ActionByID(int ID)
    {
        //used by copy constructor to ensure action is called by the copy instance;
        switch(ID)
        {
            case 5:
                return Beserker;
            case 9:
                return StingingSwarmDamage;
            case 11:
                return WolfForm;
            case 12:
                return HumanForm;
            case 17:
                return Prepare;

            default:
                return RollStatus;
        }
    }


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}