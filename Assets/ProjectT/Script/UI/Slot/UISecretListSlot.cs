using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UISecretListSlot : FSlot
{
    [SerializeField] private UISprite openSpr = null;
    [SerializeField] private UISprite closeSpr = null;
    [SerializeField] private UISprite selSpr = null;
    [SerializeField] private UILabel clearLabel = null;
    [SerializeField] private UILabel chapterLabel = null;
    [SerializeField] private UISprite typeSpr = null;
    [SerializeField] private List<UISprite> boSetOptionList = null;
    [SerializeField] private GameObject lockObj = null;

    private UIStageDetailPopup _stageDetailPopup;
    private int _index;
    private bool _clear;
    
    private void Awake()
    {
        if (ParentGO == null)
        {
            return;
        }

        _stageDetailPopup = ParentGO.GetComponent<UIStageDetailPopup>();
    }

    public void UpdateSlot(int index, int selectIndex, GameTable.Stage.Param stageParam)
    {
        _index = index;
        _clear = GameInfo.Instance.StageClearList.Any(x => x.TableID == stageParam.ID);

        SecretQuestOptionData optionData = GameInfo.Instance.ServerData.SecretQuestOptionList.Find(x => x.GroupId == stageParam.ID);
        typeSpr.spriteName = $"TribeType_{optionData.LevelId}";

        List<GameClientTable.StageBOSet.Param> stageBoSetList =
            GameInfo.Instance.GameClientTable.FindAllStageBOSet(x => x.Group == optionData.BoSetId);
        for (int i = 0; i < boSetOptionList.Count; i++)
        {
            string spriteName = string.Empty;
            if (i < stageBoSetList.Count)
            {
                spriteName = stageBoSetList[i].Icon;
            }

            boSetOptionList[i].spriteName = spriteName;
            boSetOptionList[i].SetActive(!string.IsNullOrEmpty(spriteName));
        }

        selSpr.SetActive(index == selectIndex);
        
        bool enable = true;
        if (0 < stageParam.LimitStage)
        {
            enable = GameInfo.Instance.StageClearList.Any(x => x.TableID == stageParam.LimitStage);
        }
        openSpr.SetActive(enable);
        closeSpr.SetActive(!enable);
        lockObj.SetActive(!enable);
        
        clearLabel.SetActive(_clear);
        chapterLabel.textlocalize = FLocalizeString.Instance.GetText(1821, stageParam.Section);
    }

    public void OnClick_Slot()
    {
        if (_stageDetailPopup == null)
        {
            return;
        }

        if (selSpr.gameObject.activeSelf)
        {
            return;
        }
        
        _stageDetailPopup.SetSelectSecretSlot(_index, closeSpr.gameObject.activeSelf);
    }
}
