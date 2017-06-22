using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class WorldControl : MonoBehaviour {

    public List<GameObject> EncounterList = new List<GameObject> { };
    public List<GameObject> CurrentParty=new List<GameObject> { };
    public GameObject WorldCharacter;
    public GameObject Map;
    public System.Random RNG = new System.Random();
    public bool InEncounter;
    public int Gold;
    public int Xp;
    public Text GoldText;
    public Text XpText;
    public GameObject CurrentNode;

    public void MoveMap()
    {
        if (Map.transform.position.z == -10)
            Map.transform.position = Vector3.zero;
        else
            Map.transform.position = new Vector3(-10, -10, -10);//offscreen
    }

    public void UpdateCurrency(int G, int X)
    {
        Gold += G;
        Xp += X;
        GoldText.text = "$" + Gold.ToString();
        XpText.text = Xp.ToString() + " XP";
    }

    void SetUpEncounters()
    {

    }

	// Use this for initialization
	void Start () {
        UpdateCurrency(999,99);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
