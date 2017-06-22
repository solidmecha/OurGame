using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShowPriceHelper : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public int id;

    public void OnPointerEnter(PointerEventData P)
    {
        transform.root.GetComponent<ShopScript>().ShowCost(id);

    }

    public void OnPointerExit(PointerEventData P)
    {
        transform.root.GetComponent<ShopScript>().ClearCost();

    }
}
