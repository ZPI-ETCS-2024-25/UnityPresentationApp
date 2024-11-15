using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PolyAndCode.UI;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class CrossingCell : MonoBehaviour, ICell
{
    public TMP_Text nameLabel;
    public TMP_Dropdown position;

    //Model
    private CrossingInfo _contactInfo;
    private int _cellIndex;

    //This is called from the SetCell method in DataSource
    public void ConfigureCell(CrossingInfo contactInfo, int cellIndex)
    {
        _cellIndex = cellIndex;
        _contactInfo = contactInfo;



        nameLabel.text = contactInfo.Name;

        position.ClearOptions();
        List<string> newOptions = contactInfo.AllowedStates.ConvertAll(x => x.Name);
        position.AddOptions(newOptions);

        position.onValueChanged.AddListener(OnDropdownValueChange);
    }


    private void OnDropdownValueChange(int value)
    {
        int changed = _contactInfo.AllowedStates[value].Index;
        _contactInfo.CrossingSystem.ChangeCrossingState(changed);
    }
}