﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill {

    public string Name;
    public string EffectText;
    public int[] Costs = new int[4];
    public int BaseDamage;
    public List<Status> DebuffList = new List<Status> { };
    public List<Status> BuffList = new List<Status> { };
    public int levelReq;
    public int skillType; //0= single target, 1=self target, 2=Party target, 3=all enemies, 4=all allies


    public Skill() { }

    public Skill(string n, string e, int d, int[] costs, List<Status> debuffs, List<Status> buffs, int lr, int st)
    {

        Name = n;
        EffectText = e;
        BaseDamage = d;
        for (int i = 0; i < costs.Length; i++)
        {
            Costs[i] = costs[i];
        }
        for (int i = 0; i < debuffs.Count; i++)
        {
            DebuffList.Add(debuffs[i]);
        }
        for (int i = 0; i < buffs.Count; i++)
        {
            BuffList.Add(buffs[i]);
        }
        levelReq = lr;
        skillType = st;
    }
    public Skill(string n, string e, int d, int[] costs, int lr, int st)
    {

        Name = n;
        EffectText = e;
        BaseDamage = d;
        for (int i = 0; i < costs.Length; i++)
        {
            Costs[i] = costs[i];
        }
        levelReq = lr;
        skillType = st;
    }

    //deal dmg and apply Statuses
    public void fight(Character user, Character target)
    {
        float Dmg = 0;
        float[] mods = new float[4];

        float totalMods = Costs[0] + Costs[1] + Costs[2] + Costs[3];

        if (BaseDamage > 0)
        {
            for (int i = 0; i < 4; i++)
            {
                mods[i] = Costs[i] / totalMods;
                Dmg += ((user.Attack[i] + 100f) / (target.Defence[i] + 100f)) * BaseDamage * mods[i];
            }
        }
        else //healing
        {
            for (int i = 0; i < 4; i++)
            {
                mods[i] = Costs[i] / totalMods;
                Dmg += ((user.Attack[i] + 100) / 100) * BaseDamage * mods[i];
            }
        }

        foreach (Status S in BuffList)
        {
            Status newStatus = new Status(S);
            newStatus.Attacker = user;
            newStatus.SetAction();
            if (!hasStatusSameSkillandAttacker(newStatus, user))
            {
                
                user.Statuses.Add(newStatus);
            }
        }

        foreach (Status S in DebuffList)
        {
            Status newStatus = new Status(S);
            newStatus.Attacker = user;
            if (!hasStatusSameSkillandAttacker(newStatus, target))
            {
                target.Statuses.Add(newStatus);
            }
        }

        target.takeDamage(Mathf.RoundToInt(Dmg));
        //MonoBehaviour.print(user.name+" uses "+Name+" on "+ reciever.name + " dealing " +Dmg);
    }

    bool hasStatusSameSkillandAttacker(Status S, Character C)
    {
        foreach(Status stat in C.Statuses)
        {
            if(stat.ID==S.ID && stat.Attacker.name.Equals(S.Attacker.name))
            {
                return true;
            }
        }
        return false;

    }


    public static Skill searchID(int i)
    {
        int[] costs;
        List<Status> Buffs=new List<Status> { };
        List<Status> Debuffs=new List<Status> { };
        
        switch(i)
        {

            case 0:
                costs = new int[4] { -1, 0, 0, 0 };
                Skill BasicAttack = new Skill("Basic Attack", "basically attacks them", 10, costs, 0,0);
                return BasicAttack;
            case 1:
                costs = new int[4] { 0, -1, 0, 0 };
                Skill MagmaJet = new Skill("Magma Jet", "The Pyromancer's first lesson", 10, costs, 0, 0);
                return MagmaJet;
            case 2:
                costs = new int[4] { 0, 0, -1, 0 };
                Skill IceNeedle = new Skill("Ice Needle", "A simple, cold thread", 10, costs, 0, 0);
                return IceNeedle;
            case 3:
                costs = new int[4] {0,0,0,-2};
                Skill Pray=new Skill("Pray", "heal self", -10, costs, 0,1);
                return Pray;
            case 4:
                costs = new int[4] { 0, 10, 4, 0};
                Debuffs.Add(Status.StatusByID(0));//poison
                Debuffs.Add(Status.StatusByID(2));//burn
                Buffs.Add(Status.StatusByID(1));//genFire
                Skill VenomFlame=new Skill("Venom Flame", "Generates Chi while poisoning and burning enemy", 0, costs, Debuffs, Buffs, 1, 0);
                return VenomFlame;
            case 5:
                costs = new int[4] {9,13,0,0};
                Buffs.Add(Status.StatusByID(1)); //genFire
                Buffs.Add(Status.StatusByID(2)); //burn
                Buffs.Add(Status.StatusByID(4)); //fireAtk+
                Buffs.Add(Status.StatusByID(5)); //fireDef+
                Skill LivingInferno = new Skill("Living Inferno", "Become the flame", 0, costs, Debuffs, Buffs, 1, 1);
                return LivingInferno;
            case 6:
                costs = new int[4] { 0, 0, 7, 7 };
                Debuffs.Add(Status.StatusByID(3)); //heal
                Skill HealingWind = new Skill("Healing Wind", "heal over time on party", 0, costs, Debuffs, Buffs, 1, 3);
                return HealingWind;
            case 7:
                costs = new int[4] { 2, 1, 0, 0 };
                Skill Smash = new Skill("Smash", "smash", 25, costs, Debuffs, Buffs, 0, 0);
                return Smash;
            case 8:
                costs = new int[4] { 10, 8, 0, 0 };
                Buffs.Add(Status.StatusByID(6)); //beserk
                Buffs.Add(Status.StatusByID(7)); //buff phys and fire atk
                Skill Beserk = new Skill("Beserk", "Go Beserk", 0, costs, Debuffs, Buffs, 4, 1);
                return Beserk;
            case 9:
                costs = new int[4] { 1, 2, 4, 0 };
                Buffs.Add(Status.StatusByID(8));//Arcane Intel
                Skill ArcaneIntel = new Skill("Arcane Intel", "Draw some cards", 0, costs, Debuffs, Buffs, 2, 1);
                return ArcaneIntel;
            case 10:
                costs = new int[4] { 0,10,10,0};
                Debuffs.Add(Status.StatusByID(9));//stingswarm
                Skill StingingSwarm = new Skill("Stinging Swarm", "thousand needles", 0, costs, Debuffs, Buffs, 2, 0);
                return StingingSwarm;
            case 11:
                costs = new int[4] { 7, 5, 0, 0 };
                Buffs.Add(Status.StatusByID(11)); //WolfTransform
                Skill Lycanthropy = new Skill("Lycanthropy", "Howl", 0, costs, Debuffs, Buffs, 3, 1);
                return (Lycanthropy);
            case 12:
                costs = new int[4] {6,0,0,0};
                Debuffs.Add(Status.StatusByID(10));//phys def lowered
                Skill Claw = new Skill("Claw", "scratch", 45, costs, Debuffs, Buffs, 3, 0);
                return Claw;
            case 13:
                costs = new int[4] { 8, 0, 0, 0 };
                Buffs.Add(Status.StatusByID(10));//phys def lowered
                Skill Bite = new Skill("Bite", "Om nom nom", 65, costs, Debuffs, Buffs, 3, 0);
                return Bite;
            case 14:
                costs = new int[4] { 1, 0, 0, 0 };
                Buffs.Add(Status.StatusByID(12));
                Skill HumanForm = new Skill("Human form", "Revert to normal", 0, costs, Debuffs, Buffs, 3, 1);
                return HumanForm;
            case 15:
                costs = new int[4] {2,3,3,1};
                Skill MagicMissle = new Skill("Magic Missle", "A litte bit of everything", 35, costs, 1, 0);
                return MagicMissle;
            case 16:
                costs = new int[4] {5,5,5,5};
                Skill Comet = new Skill("Comet", "It's highly eccentric", 60, costs, 1, 0);
                return Comet;
            case 17:
                costs = new int[4] { 0, 12, 0, 0 };
                Skill Fireball = new Skill("Fireball", "It's quite warm", 60, costs, 1, 0);
                return Fireball;
            case 18:
                costs = new int[4] { 5, 7, 0, 0 };
                Buffs.Add(Status.StatusByID(3));//heal
                Skill Syphon = new Skill("Syphon", "Attack while healing", 30, costs, Debuffs, Buffs, 1, 0);
                return Syphon;
            case 19:
                costs = new int[4] {3,0,0,0};
                Buffs.Add(Status.StatusByID(14));
                Skill Block = new Skill("Block", "Defend against physical damage", 0, costs, Debuffs, Buffs, 1, 1);
                return Block;
            case 20:
                costs = new int[4] {20,20,20,0};
                Skill Ultima = new Skill("Ultima", "The ultimate magic", 500, costs, 5, 0);
                return Ultima;
            case 21:
                costs = new int[4] {3,3,3,0};
                Buffs.Add(Status.StatusByID(0));
                Buffs.Add(Status.StatusByID(1));
                Buffs.Add(Status.StatusByID(13));
                Skill DarkRit = new Skill("Dark Ritual", "Poison self for buffs", 0, costs, Debuffs, Buffs, 1, 1);
                return DarkRit;
            case 22:
                costs = new int[4] { 0, 0, 6, 0 };
                Debuffs.Add(Status.StatusByID(15));
                Skill Blizzard = new Skill("Blizzard", "Snowstorm", 45, costs, Debuffs, Buffs, 2, 0);
                return Blizzard;
            case 23:
                costs = new int[4] {0,0,10,0};
                Debuffs.Add(Status.StatusByID(16));
                Skill Avalanche = new Skill("Avalance", "Reduce their stamina", 55, costs, Debuffs, Buffs, 2, 0);
                return Avalanche;

            default:
                costs = new int[4] { 1, 0, 0, 0 };
                Skill wait = new Skill("Wait", "waits", 0, costs, Debuffs, Buffs, 0, 1);
                return wait;
        }

    }


}