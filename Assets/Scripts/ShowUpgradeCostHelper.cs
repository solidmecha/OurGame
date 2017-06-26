using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShowUpgradeCostHelper : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public int id;
    public int index;

    public void OnPointerEnter(PointerEventData P)
    {
        transform.root.GetComponent<UpgradeScript>().ShowCost(id, index);

    }

    public void OnPointerExit(PointerEventData P)
    {
        transform.root.GetComponent<UpgradeScript>().ClearCost();

    }
}
