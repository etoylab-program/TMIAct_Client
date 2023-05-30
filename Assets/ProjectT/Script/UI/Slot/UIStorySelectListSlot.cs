using UnityEngine;
using System.Collections;

public class UIStorySelectListSlot : FSlot
{
    public UILabel kSelectLabel;

    private StorySelectItemValue _selectValue;

	public void UpdateSlot(ScenarioParam scenarioData, StorySelectItemValue selectValue) 	//Fill parameter if you need
	{
        kSelectLabel.textlocalize = scenarioData.Value2;
        _selectValue = selectValue;
	}
 
	public void OnClick_Slot()
	{
        if (ParentGO == null)
            return;

        UIStroyPopup stroyPopup = ParentGO.GetComponent<UIStroyPopup>();
        if (stroyPopup != null)
        {
            stroyPopup.OnClick_SelectSlot(_selectValue);
        }

        UIBookCardCinemaPopup uIBookCardCinemaPopup = ParentGO.GetComponent<UIBookCardCinemaPopup>();
        if(uIBookCardCinemaPopup != null)
        {
            uIBookCardCinemaPopup.OnClick_SelectSlot(_selectValue);
        }
	}
 
}
