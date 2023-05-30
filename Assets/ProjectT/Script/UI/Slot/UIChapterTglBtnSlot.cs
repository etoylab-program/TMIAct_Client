using UnityEngine;
using System.Collections;

public class UIChapterTglBtnSlot : FSlot
{
    public enum ePos
    {
        Story = 0,
        TimeAttack,
    }

    public UISprite kOffSpr;
    public UILabel kOffNumberLabel;
    public UISprite kOnSelSpr;
    public UILabel kOnNumberLabel;
    public GameObject kLock;

    private ePos _pos;
    private int _chapterNumber = 0;

    /// <summary>
    ///   é�� ����Ʈ�� �ε���, é�� ID, ���� ����
    /// </summary>
    public void UpdateSlot(ePos pos, int idx, int chapterNumber,bool bSelect, bool bLocked)  //Fill parameter if you need
    {
        _pos = pos;
        _chapterNumber = chapterNumber;

        kLock.SetActive(!bLocked);

        kOffNumberLabel.textlocalize = _chapterNumber.ToString("D2");
        kOnNumberLabel.textlocalize = _chapterNumber.ToString("D2");

        kOffSpr.gameObject.SetActive(!bSelect);
        kOnSelSpr.gameObject.SetActive(bSelect);
    }

    public void OnClick_Slot()
    {
        if (ParentGO == null)
            return;
        //é�Ͱ� ��������� �ȴ�������
        if (kLock.activeSelf)
            return;

        if(_pos == ePos.Story)
        {
            UIStoryPanel storyPanel = ParentGO.GetComponent<UIStoryPanel>();
            if (storyPanel != null)
                storyPanel.OnChapterSelect(_chapterNumber);
        }
        else
        {
            UITimeAttackPanel storyPanel = ParentGO.GetComponent<UITimeAttackPanel>();
            if (storyPanel != null)
                storyPanel.OnChapterSelect(_chapterNumber);
        }
        
    }
}
