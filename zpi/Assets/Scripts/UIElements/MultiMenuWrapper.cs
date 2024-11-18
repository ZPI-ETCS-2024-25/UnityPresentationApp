using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiMenuWrapper : MonoBehaviour, MultiMenuComponent
{
    public void ChangeActive(bool active)
    {
        gameObject.SetActive(active);
    }

    public void Start()
    {
        gameObject.SetActive(false);
    }
}
