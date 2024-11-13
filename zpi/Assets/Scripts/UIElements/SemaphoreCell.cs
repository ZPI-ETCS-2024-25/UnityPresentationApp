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
    private JunctionInfo _contactInfo;
    private int _cellIndex;

    //This is called from the SetCell method in DataSource
    public void ConfigureCell(JunctionInfo contactInfo, int cellIndex)
    {
        _cellIndex = cellIndex;
        _contactInfo = contactInfo;



        nameLabel.text = contactInfo.Name;

        position.ClearOptions();
        List<string> newOptions = contactInfo.AllowedPositions.ConvertAll(x => x.Name);
        position.AddOptions(newOptions);

        position.onValueChanged.AddListener(OnDropdownValueChange);
    }


    private void OnDropdownValueChange(int value)
    {
        int changed = _contactInfo.AllowedPositions[value].Index;
        _contactInfo.PathManager.ChangeJunction(_contactInfo.Position, _contactInfo.Backward, changed);
    }
}