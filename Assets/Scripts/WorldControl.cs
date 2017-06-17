using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldControl : MonoBehaviour {

    public List<GameObject> EncounterList = new List<GameObject> { };
    public List<GameObject> CurrentParty=new List<GameObject> { };
    public GameObject WorldCharacter;
    public GameObject Map;
    public System.Random RNG = new System.Random();

    public void MoveMap()
    {
        if (Map.transform.position.z == -10)
            Map.transform.position = Vector3.zero;
        else
            Map.transform.position = new Vector3(-10, -10, -10);//offscreen
    }

    void SetUpEncounters()
    {

    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
