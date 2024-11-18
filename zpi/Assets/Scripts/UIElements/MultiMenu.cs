using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiMenu : MonoBehaviour
{
    public List<MultiMenuWrapper> menus;

    public void SwitchActive(int i)
    {
        for (int j = 0; j < menus.Count; j++)
        {
            menus[j].ChangeActive(j == i);
        }
    }
}
