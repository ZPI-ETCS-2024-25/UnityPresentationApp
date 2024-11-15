using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PolyAndCode.UI;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class SemaphoreCell : MonoBehaviour, ICell
{
    public TMP_Text nameLabel;
    public TMP_Dropdown position;

    //Model
    private SemaphoreInfo _contactInfo;
    private int _cellIndex;

    //This is called from the SetCell method in DataSource
    public void ConfigureCell(SemaphoreInfo contactInfo, int cellIndex)
    {
        _cellIndex = cellIndex;
        _contactInfo = contactInfo;



        nameLabel.text = contactInfo.Name;

        position.ClearOptions();
        List<string> newOptions = contactInfo.AllowedSignals.ConvertAll(x => x.Name);
        position.AddOptions(newOptions);

        position.onValueChanged.AddListener(OnDropdownValueChange);
    }


    private void OnDropdownValueChange(int value)
    {
        int changed = _contactInfo.AllowedSignals[value].Index;
        _contactInfo.SemaphoreController.SetSignal(changed);
    }
}