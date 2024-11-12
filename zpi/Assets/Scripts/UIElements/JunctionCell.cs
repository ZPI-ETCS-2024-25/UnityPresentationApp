using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PolyAndCode.UI;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class JunctionCell : MonoBehaviour,ICell
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
        List<string> newOptions = contactInfo.AllowedPositions.ConvertAll(x => x.ToString()) ;
        position.AddOptions(newOptions);
    }
}
