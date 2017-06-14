using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterScript : MonoBehaviour {

    public GameObject Message;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Instantiate(Message, Vector2.zero, Quaternion.identity);
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
