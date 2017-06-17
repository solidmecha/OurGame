using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopSelectCharacter : MonoBehaviour {

    public int Index;

    private void OnMouseDown()
    {
        transform.root.GetComponent<ShopScript>().SetSelectedChar(Index);
    }
}
