using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ShopSelectCharacter : MonoBehaviour {

    public int Index;
    public Action<int> action;

    private void OnMouseDown()
    {
        action(Index);
    }
}
