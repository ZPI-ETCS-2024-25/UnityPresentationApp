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
    private void Start()
    {
        RectTransform dropdownRectTransform = position.GetComponent<RectTransform>();
        dropdownRectTransform.localScale = new Vector3(2f, 2f, 1f);
        dropdownRectTransform.localPosition = dropdownRectTransform.localPosition - new Vector3(100f, 0f, 0f);
        dropdownRectTransform.sizeDelta = dropdownRectTransform.sizeDelta + new Vector2(100f, 0f);

        nameLabel.fontSize = 120f;
        nameLabel.color = Color.black;
        nameLabel.alignment = TextAlignmentOptions.Left;
    }
    //This is called from the SetCell method in DataSource
    public void ConfigureCell(JunctionInfo contactInfo, int cellIndex)
    {
        _cellIndex = cellIndex;
        _contactInfo = contactInfo;


        nameLabel.text = contactInfo.Name;

        position.ClearOptions();
        List<string> newOptions = contactInfo.AllowedPositions.ConvertAll(x => x.Name);
        position.AddOptions(newOptions);


        int currentStateIdx = _contactInfo.PathManager.GetJunctionCurrentPosition(contactInfo.Position);
        position.value = currentStateIdx;

        position.onValueChanged.AddListener(OnDropdownValueChange);
    }


    private void OnDropdownValueChange(int value)
    {
        int changed = _contactInfo.AllowedPositions[value].Index;
        _contactInfo.PathManager.ChangeJunction(_contactInfo.Position,_contactInfo.Backward,changed);
    }
}
