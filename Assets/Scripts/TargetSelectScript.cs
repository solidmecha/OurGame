using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSelectScript : MonoBehaviour {

    public BattleControl BC;

    private void OnMouseDown()
    {
        BC.CastSkillOnTarget(BC.Party[BC.TurnIndex], transform.parent.GetComponent<Character>());
        BC.NextTurn();
    }

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
