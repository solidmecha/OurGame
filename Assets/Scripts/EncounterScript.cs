using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterScript : MonoBehaviour {

    public GameObject Message;
    public WorldControl WC;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Message != null)
            Instantiate(Message, new Vector2(0, 2), Quaternion.identity);
        else
            WC.GetComponent<BattleControl>().HandleBattle();
        //Destroy(this);
    }

    public void OnMouseDown()
    {
        WC.WorldCharacter.transform.position = (Vector2)transform.position+new Vector2(0,0.1f);
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
