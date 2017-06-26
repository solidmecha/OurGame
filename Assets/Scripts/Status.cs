using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Status {

    public string name;
    public int ID;
    public enum Stat_Target_type{Attack, Defence, Regen, Health};
    public enum Status_Behavior { DoT, OneTime, Unique}; //DoTs are called post combat
    public int[] ArrayModifiers=new int[4]; //one time changes to stats or the modifiers for DoTs
    public Stat_Target_type Stat_Target;
    public Status_Behavior Behavior;
    public int duration;
    public Action<Character> action;
    public int BaseValue; //set equal to duration to apply 1 time effects, otherwise base dmg for dots, -1 and -1 duration for perma buffs
    public List<Skill> skillSet=new List<Skill> { }; //when needed
    public Character Attacker;
    public string InflictingSkillName;
    public int TotalStatusCount = 27;

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
        float[] mods = new float[4] { 0, 0, 0, 0 };

        float totalMods = ArrayModifiers[0] + ArrayModifiers[1] + ArrayModifiers[2] + ArrayModifiers[3];

        if (BaseValue > 0)
        {
            for (int i = 0; i < 4; i++)
            {
                mods[i] = ArrayModifiers[i] / totalMods;
                Dmg += ((Attacker.Attack[i] *Attacker.Attack[i]) / (4f*C.Defence[i] + 100f)) * BaseValue * mods[i];
            }
            MonoBehaviour.print("DoT dmg " +Dmg);
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
            C.SkillSet.Add(Skill.searchID(0));
            C.SkillSet.Add(Skill.searchID(12));
            C.SkillSet.Add(Skill.searchID(13));
            C.SkillSet.Add(Skill.searchID(14));
            foreach (Skill S in skillSet)
                C.SkillSet[3].BuffList[0].skillSet.Add(S);
        }
        duration--;
    }

    public void HumanForm(Character C)
    {
        if (duration == BaseValue && skillSet.Count>0)
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

    public void DecimateAction(Character C)
    {
        float f = C.health * BaseValue/100f;
        C.takeDamage(Mathf.RoundToInt(f));
        duration--;
    }

    public void WispAction(Character C)
    {
        InflictingSkillName = "Cursed by Wisp";
        if(C.Defence[2]>-14)
            C.Defence[2] -= 2;
        RollStatus(C);
    }

    public void DoubleTimeAction(Character C)
    {
        if(duration%2==0)
            GameObject.FindGameObjectWithTag("GameController").GetComponent<BattleControl>().TurnIndex--;
        duration--;
    }

    public void ConfuseTargetAction(Character C)
    {
        if(duration==BaseValue)
        {
            foreach (Skill S in C.SkillSet)
                skillSet.Add(S);
            C.SkillSet.Clear();
            C.SkillSet.Add(Skill.searchID(39));
            foreach (Skill S in skillSet)
                C.SkillSet[0].DebuffList[0].skillSet.Add(S);
        }
        if(duration==1)
        {
            C.SkillSet.Clear();
            foreach (Skill S in skillSet)
                C.SkillSet.Add(S);
        }
        duration--;
    }

    public void UseRandomSkilRandomly(Character C)
    {
        List<int> TempInts = new List<int> { };
        for(int i=0;i<skillSet.Count;i++)
        {
            if (C.hasResources(skillSet[i].Costs))
                TempInts.Add(i);
        }

        System.Random rng = new System.Random();
        int r = rng.Next(TempInts.Count);
        BattleControl BC = GameObject.FindGameObjectWithTag("GameController").GetComponent<BattleControl>();
        List<Character> targets = new List<Character> { };
        foreach (Character p in BC.Party)
            targets.Add(p);
        foreach (Character e in BC.EnemyParty)
            targets.Add(e);
        int t = rng.Next(targets.Count);
        skillSet[r].fight(C, targets[t]);
        MonoBehaviour.print("In Confusion, " + skillSet[r].Name + " " + targets[t].name);
        duration--;
    }

    public void OverloadAction(Character C)
    {
        if (duration == BaseValue)
        {
            foreach (Skill S in C.SkillSet)
            {
                Skill s = new Skill(S.Name, S.EffectText, S.BaseDamage, S.Costs, S.levelReq, S.skillType);
                skillSet.Add(s);
                if (S.skillType == 0)
                    S.skillType = 4;
            }
        }
        if(duration==1)
        {
            C.SkillSet.Clear();
            foreach (Skill S in skillSet)
                C.SkillSet.Add(S);
        }
        duration--;
    }

    public void WheelOfFateAction(Character C)
    {
        System.Random rng = new System.Random();
        int r;
        List<int> StatusesThatCrashGame = new List<int> {12,25 };
        do
        {
            r = rng.Next(TotalStatusCount);
        }
        while (StatusesThatCrashGame.Contains(r));
        Status S = StatusByID(r);
        S.Attacker = Attacker;
        S.InflictingSkillName = InflictingSkillName;
        C.Statuses.Add(S);
        duration--;
    }

    public void WishAction(Character C)
    {
        System.Random rng = new System.Random();
        int r=rng.Next(Skill.TotalSkillCount);
        C.SkillSet.Add(Skill.searchID(r));
        duration--;
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
                Status Poison = new Status(0, "Poison", new int[4] { 1,0,0,0 }, Stat_Target_type.Health, Status_Behavior.DoT, 5, 20);
                Poison.action = Poison.RollStatus;
                return Poison;
            case 1:
                Status GenFire = new Status(1, "Gen 2 Chi", new int[4] { 0, 2, 0, 0 }, Stat_Target_type.Regen, Status_Behavior.OneTime, 4, 4);
                GenFire.action = GenFire.RollStatus;
                return GenFire;
            case 2:
                Status Burn=new Status(2, "Burning", new int[4] { 0, 2, 0, 0 }, Stat_Target_type.Health, Status_Behavior.DoT, 5, 30);
                Burn.action = Burn.RollStatus;
                return Burn;
            case 3:
                Status Heal = new Status(3, "Healing", new int[4] { 0, 0, 3, 0 }, Stat_Target_type.Health, Status_Behavior.DoT, 5, -9);
                Heal.action = Heal.RollStatus;
                return Heal;
            case 4:
                Status BuffFireAtk = new Status(4, "+20 Fire Atk", new int[4] { 0, 20, 0, 0 }, Stat_Target_type.Attack, Status_Behavior.OneTime, 7, 7);
                BuffFireAtk.action = BuffFireAtk.RollStatus;
                return BuffFireAtk;
            case 5:
                Status BuffFireDef = new Status(5, "+100 Fire Def", new int[4] { 0, 100, 0, 0 }, Stat_Target_type.Defence, Status_Behavior.OneTime, 7, 7);
                BuffFireDef.action = BuffFireDef.RollStatus;
                return BuffFireDef;
            case 6:
                Status BeserkStatus = new Status(6, "Beserk", Status_Behavior.Unique, 3, 3);
                BeserkStatus.action = BeserkStatus.Beserker;
                return BeserkStatus;
            case 7:
                Status BuffAtkandFire = new Status(7, "+Stam and Chi atk", new int[4] { 30, 30, 0, 0 }, Stat_Target_type.Attack, Status_Behavior.OneTime, 3, 3);
                BuffAtkandFire.action = BuffAtkandFire.RollStatus;
                return BuffAtkandFire;
            case 8:
                Status ArcaneIntellectRegen = new Status(8, "Regen Mana", new int[4] { -1, -1, 5, -1 }, Stat_Target_type.Regen, Status_Behavior.OneTime, 5, 5);
                ArcaneIntellectRegen.action = ArcaneIntellectRegen.RollStatus;
                return ArcaneIntellectRegen;
            case 9:
                Status StingingSwarmStatus = new Status(9, "Wasps swarm", Status_Behavior.DoT, 1, 10);
                StingingSwarmStatus.action = StingingSwarmStatus.StingingSwarmDamage;
                return StingingSwarmStatus;
            case 10:
                Status DropPhysDef = new Status(10, "-Phys Def", new int[4] { -25, 0, 0, 0 }, Stat_Target_type.Defence, Status_Behavior.OneTime, 5, 5);
                DropPhysDef.action = DropPhysDef.RollStatus;
                return DropPhysDef;
            case 11:
                Status WolfTransform = new Status(11, "werewolf", Status_Behavior.Unique, 1, 1);
                WolfTransform.action = WolfTransform.WolfForm;
                return WolfTransform;
            case 12:
                Status HumanTransform = new Status(12, "human", Status_Behavior.Unique, 1, 1);
                HumanTransform.action = HumanTransform.HumanForm;
                return HumanTransform;
            case 13:
                Status BuffAllAtk = new Status(13, "+All Atk", new int[4] {6,6,6,6}, Stat_Target_type.Attack, Status_Behavior.OneTime, 4, 4);
                BuffAllAtk.action = BuffAllAtk.RollStatus;
                return BuffAllAtk;
            case 14:
                Status Blocking = new Status(14, "+400 Phys Def", new int[4] { 400, 0, 0, 0 }, Stat_Target_type.Defence, Status_Behavior.OneTime, 2, 2);
                Blocking.action = Blocking.RollStatus;
                return Blocking;
            case 15:
                Status FireHose = new Status(15, "-15 Fire Atk", new int[4] { 0, -15, 0, 0 }, Stat_Target_type.Attack, Status_Behavior.OneTime, 5, 5);
                FireHose.action = FireHose.RollStatus;
                return FireHose;
            case 16:
                Status ReduceStam = new Status(16, "Stamina Reduced", new int[4] { -50, 0, 0, 0 }, Stat_Target_type.Regen, Status_Behavior.OneTime, 2, 2);
                ReduceStam.action = ReduceStam.RollStatus;
                return ReduceStam;
            case 17:
                Status PrepareStat = new Status(17, "Prepared", Status.Status_Behavior.Unique, -1, -1);
                PrepareStat.SetAction();
                return PrepareStat;
            case 18:
                Status DecimateStatus = new Status(18, "Decimate", Status_Behavior.DoT, 5, 10);
                DecimateStatus.action = DecimateStatus.DecimateAction;
                return DecimateStatus;
            case 19:
                Status Wisp = new Status(19, "Angry Wisp", new int[4] { 0, 0, 1, 0 }, Stat_Target_type.Health, Status_Behavior.DoT, -1, 5);
                Wisp.action = Wisp.WispAction;
                return Wisp;
            case 20:
                Status FocusBuff = new Status(20, "Focused", new int[4] { 0, 0, 12, 0 }, Stat_Target_type.Attack, Status_Behavior.OneTime, 3, 3);
                FocusBuff.action = FocusBuff.RollStatus;
                return FocusBuff;
            case 21:
                Status Melted = new Status(21, "Melted", new int[4] { 0, 0, -2, 0 }, Stat_Target_type.Regen, Status_Behavior.OneTime, 4, 4);
                Melted.action = Melted.RollStatus;
                return Melted;
            case 22:
                Status Overloaded = new Status(22, "Overloaded", Status_Behavior.DoT, 2, 2);
                Overloaded.action = Overloaded.OverloadAction;
                return Overloaded;
            case 23:
                Status DoubleTimed = new Status(23, "DoubleTimed", Status_Behavior.DoT, 4, 4);
                DoubleTimed.action = DoubleTimed.DoubleTimeAction;
                return DoubleTimed;
            case 24:
                Status Confused = new Status(24, "Confused", Status_Behavior.Unique, 3, 3);
                Confused.action = Confused.ConfuseTargetAction;
                return Confused;
            case 25:
                Status ConfusedandAttacking = new Status(25, "Attacking while Confused", Status_Behavior.DoT, 1, 1);
                ConfusedandAttacking.action = ConfusedandAttacking.UseRandomSkilRandomly;
                return ConfusedandAttacking;
            case 26:
                Status DefenseShred = new Status(26, "Lower Defense", new int[4] { -25, -25, -25, -50 }, Stat_Target_type.Defence, Status_Behavior.OneTime, 6, 6);
                DefenseShred.action = DefenseShred.RollStatus;
                return DefenseShred;
            case 27:
                Status Wheel = new Status(27, "Wheel of Fate", Status_Behavior.Unique, 6, 6);
                Wheel.action = Wheel.WheelOfFateAction;
                return Wheel;
            case 28:
                Status Wish = new Status(28, "Wishing", Status_Behavior.DoT, 1, 1);
                Wish.action = Wish.WishAction;
                return Wish;

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
            case 6:
                return Beserker;
            case 9:
                return StingingSwarmDamage;
            case 11:
                return WolfForm;
            case 12:
                return HumanForm;
            case 17:
                return Prepare;
            case 18:
                return DecimateAction;
            case 19:
                return WispAction;
            case 22:
                return OverloadAction;
            case 23:
                return DoubleTimeAction;
            case 24:
                return ConfuseTargetAction;
            case 25:
                return UseRandomSkilRandomly;
            case 27:
                return WheelOfFateAction;
            case 28:
                return WishAction;

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
