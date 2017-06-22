using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EncounterScript : MonoBehaviour {

    public GameObject Message;
    public GameObject CurrentMessage;
    public WorldControl WC;
    public Vector2 Message_Location;
    public int ID; //2 for battle

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (ID != 2)
        {
            CurrentMessage = Instantiate(Message, Message_Location, Quaternion.identity) as GameObject;
            SetMessageByID();
            WC.InEncounter = true;
        }
        else
            WC.GetComponent<BattleControl>().HandleBattle();
        //Destroy(this);
    }

    public void OnMouseDown()
    {
        if (!WC.InEncounter)
        {
            WC.WorldCharacter.transform.position = (Vector2)transform.position + new Vector2(0, 0.1f);
            WC.CurrentNode = gameObject;
        }
    }

    public void SetMessageText(string S)
    {
        CurrentMessage = Instantiate(Message, Message_Location, Quaternion.identity) as GameObject;
        CurrentMessage.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = S;
        SetMessageByID();
    }

    public void SetMessageByID()
    {
        switch(ID)
        {
            case 1:
                CurrentMessage.transform.GetChild(0).GetChild(1).GetComponent<Button>().onClick.AddListener(delegate { DestroyMessage(); });
                break;
            case 2:
                CurrentMessage.transform.GetChild(0).GetChild(1).GetComponent<Button>().onClick.AddListener(delegate { DestroyMessage(); });
                break;
            default:
                break;
        }
    }

    public void DestroyMessage()
    {
        Destroy(CurrentMessage);
        WC.InEncounter = false;
    }

   

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
