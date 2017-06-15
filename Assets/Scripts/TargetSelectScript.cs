using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSelectScript : MonoBehaviour {

    public BattleControl BC;

    private void OnMouseDown()
    {
        BC.CastSkillOnTarget(BC.Party[BC.TurnIndex], GetComponent<Character>());
        Destroy(GetComponent<ShowStatusOnHover>().CurrStatWindow);
        GetComponent<ShowStatusOnHover>().SetStatusWindow();
        BC.NextTurn();
    }

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
