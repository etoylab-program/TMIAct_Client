using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UICharTypeChartListSlot : FSlot {


    [SerializeField]
    private FList _advantageList;
    [SerializeField]
    private FList _disAdvantageList;


    public UISprite kbgSpr;
	public UITexture kCharTex;
	public UISprite kgrdSpr;
	public UILabel kNameLabel;
	public UILabel kTypeLabel;

    private int _index;
    private GameClientTable.HelpCharInfo.Param _helpCharInfo;

    private List<GameTable.Character.Param> _strongCharIDList = new List<GameTable.Character.Param>();
    private List<GameTable.Character.Param> _weakCharIDList = new List<GameTable.Character.Param>();
    public void Awake()
    {
        if (this._advantageList == null) return;
        if (this._disAdvantageList == null) return;

        this._advantageList.EventUpdate = this._UpdateAdvantageListSlot;
        this._advantageList.EventGetItemCount = this._GetAdvantageListElementCount;

        this._disAdvantageList.EventUpdate = this._UpdateDisAdvantageListSlot;
        this._disAdvantageList.EventGetItemCount = this._GetDisAdvantageListElementCount;
    }

    public void UpdateSlot(int index, GameClientTable.HelpCharInfo.Param data) 	//Fill parameter if you need
	{
        _index = index;
        _helpCharInfo = data;

        if (_helpCharInfo == null)
            return;

        GameTable.Character.Param charTableData = GameInfo.Instance.GameTable.FindCharacter(x => x.ID == _helpCharInfo.CharID);
        if (charTableData == null)
            return;
        kNameLabel.textlocalize = FLocalizeString.Instance.GetText(charTableData.Name);
        kCharTex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Char/MainSlot/MainSlot_" + charTableData.Icon + "_" + charTableData.InitCostume + ".png");
        FLocalizeString.SetLabel(kTypeLabel, FLocalizeString.Instance.GetText((int)eTEXTID.MON_TYPE_TEXT_START + charTableData.MonType));

        _strongCharIDList.Clear();
        _weakCharIDList.Clear();

        string strongIds = data.StrongID;
        string weakIds = data.WeakID;

        strongIds = strongIds.Replace(" ", "");
        weakIds = weakIds.Replace(" ", "");
		string[] strongIDArray = Utility.Split(strongIds, ','); //strongIds.Split(',');
		string[] weakIDArray = Utility.Split(weakIds, ','); //weakIds.Split(',');

		if (strongIDArray.Length > 0)
        {
            for (int i = 0; i < strongIDArray.Length; i++)
            {
                if (string.IsNullOrEmpty(strongIDArray[i]))
                    continue;
                GameTable.Character.Param tempCharTableData = GameInfo.Instance.GameTable.FindCharacter(x => x.ID == int.Parse(strongIDArray[i]));
                if (tempCharTableData == null)
                    continue;
                _strongCharIDList.Add(tempCharTableData);
            }
        }
        
        if(weakIDArray.Length > 0)
        {
            for (int i = 0; i < weakIDArray.Length; i++)
            {
                if (string.IsNullOrEmpty(weakIDArray[i]))
                    continue;
                GameTable.Character.Param tempCharTableData = GameInfo.Instance.GameTable.FindCharacter(x => x.ID == int.Parse(weakIDArray[i]));
                if (tempCharTableData == null)
                    continue;
                _weakCharIDList.Add(tempCharTableData);
            }
        }
        _advantageList.ScrollPositionSet();
        _disAdvantageList.ScrollPositionSet();
        _advantageList.UpdateList();
        _disAdvantageList.UpdateList();
        //_advantageList.RefreshNotMove();
        //_disAdvantageList.RefreshNotMove();
    }
 
    //강세
    private void _UpdateAdvantageListSlot(int index, GameObject slotObject)
    {
        do
        {
            UIAdvantageListSlot slot = slotObject.GetComponent<UIAdvantageListSlot>();
            if (null == slot) break;

            GameTable.Character.Param charTableData = null;

            if(0 <= index && _strongCharIDList.Count > index)
            {
                charTableData = _strongCharIDList[index];
            }

            slot.ParentGO = this.gameObject;
            slot.UpdateSlot(index, charTableData);

        } while (false);
    }

    private int _GetAdvantageListElementCount()
    {
        if(_strongCharIDList == null)
            return 0;

        return _strongCharIDList.Count;
    }

    //열세
    private void _UpdateDisAdvantageListSlot(int index, GameObject slotObject)
    {
        do
        {
            UIAdvantageListSlot slot = slotObject.GetComponent<UIAdvantageListSlot>();
            if (null == slot) break;

            GameTable.Character.Param charTableData = null;

            if (0 <= index && _weakCharIDList.Count > index)
            {
                charTableData = _weakCharIDList[index];
            }

            slot.ParentGO = this.gameObject;
            slot.UpdateSlot(index, charTableData);
        } while (false);
    }

    private int _GetDisAdvantageListElementCount()
    {
        if(_weakCharIDList == null)
            return 0;

        return _weakCharIDList.Count;
    }

    public void OnClick_Slot()
	{
	}
 
}
