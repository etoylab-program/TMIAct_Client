using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButtonLock : MonoBehaviour
{
    public enum eLOCKSHOWTYPE
    {
        HIDE = 0,   //잠김
        LOCKICON,   //잠김 아이콘 보여주기
    }
    public enum eLOCKCHECKTYPE
    {
        RANK = 0,
        CHAPTERCLEAR,
        STAGECLEAR,
    }

    public eLOCKSHOWTYPE kLockType;
    public eLOCKCHECKTYPE kCheckType;
    public int kCheckValue;
    public GameObject kLock;
    public UILabel kLockLabel;
    public EventDelegate onClick;
    private bool _block;
    public bool IsLock { get { return _block;  } }

    void OnEnable()
    {
        
        if (AppMgr.Instance.Review)
        {
            kCheckType = eLOCKCHECKTYPE.RANK;
            kCheckValue = 1;
        }
        
        Init();
    }


    public void Init()
    {
        _block = IsCheckLock();
        if (kLockLabel != null)
            kLockLabel.text = "";

        if (kLockType == eLOCKSHOWTYPE.HIDE)
        {
            if (_block)
            {
                this.gameObject.SetActive(false);
            }
            else
            {
                this.gameObject.SetActive(true);
            }
        }
        else
        {
            if (kLock != null)
            {
                if (_block)
                {
                    kLock.SetActive(true);
                    if (kLockLabel != null)
                    {
                        if (kCheckType == eLOCKCHECKTYPE.RANK)
                        {
                            kLockLabel.text = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.LOCK + (int)eLOCKCHECKTYPE.RANK), kCheckValue);
                        }
                        else if (kCheckType == eLOCKCHECKTYPE.CHAPTERCLEAR)
                        {
                            var stagecleardata = GameInfo.Instance.StageClearList.Find(x => x.TableID == kCheckValue);
                            if (stagecleardata == null)
                            {
                                var stagedata = GameInfo.Instance.GameTable.FindStage(kCheckValue);
                                if (stagedata != null)
                                {
                                    kLockLabel.text = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.LOCK + (int)eLOCKCHECKTYPE.CHAPTERCLEAR), stagedata.Chapter);
                                }
                            }
                        }
                        else if (kCheckType == eLOCKCHECKTYPE.STAGECLEAR)
                        {
                            var stagedata = GameInfo.Instance.GameTable.FindStage(kCheckValue);
                            if (stagedata != null)
                            {
                                kLockLabel.text = string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.LOCK + (int)eLOCKCHECKTYPE.STAGECLEAR), FLocalizeString.Instance.GetText(stagedata.Name));
                            }
                        }
                    }
                }
                else
                {
                    kLock.SetActive(false);
                }
            }
        }

    }

    public bool IsCheckLock()
    {
        if (kCheckType == eLOCKCHECKTYPE.RANK)
        {
            if (GameInfo.Instance.UserData.Level >= kCheckValue)
            {
                return false;
            }
        }
        else if (kCheckType == eLOCKCHECKTYPE.CHAPTERCLEAR || kCheckType == eLOCKCHECKTYPE.STAGECLEAR)
        {
            var stagecleardata = GameInfo.Instance.StageClearList.Find(x => x.TableID == kCheckValue);
            if (stagecleardata != null)
            {
                return false;
            }
        }
        return true;
    }

    public bool OnLock()
    {
        if (IsCheckLock())
        {
            if (kCheckType == eLOCKCHECKTYPE.RANK)
            {
                MessageToastPopup.Show(string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.LOCKMSG + (int)eLOCKCHECKTYPE.RANK), kCheckValue));
            }
            else if (kCheckType == eLOCKCHECKTYPE.CHAPTERCLEAR)
            {
                var stagecleardata = GameInfo.Instance.StageClearList.Find(x => x.TableID == kCheckValue);
                if (stagecleardata == null)
                {
                    var stagedata = GameInfo.Instance.GameTable.FindStage(kCheckValue);
                    if (stagedata != null)
                    {
                        MessageToastPopup.Show(string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.LOCKMSG + (int)eLOCKCHECKTYPE.CHAPTERCLEAR), stagedata.Chapter));
                    }
                }
            }
            else if (kCheckType == eLOCKCHECKTYPE.STAGECLEAR)
            {
                var stagedata = GameInfo.Instance.GameTable.FindStage(kCheckValue);
                if (stagedata != null)
                {
                    MessageToastPopup.Show(string.Format(FLocalizeString.Instance.GetText((int)eTEXTID.LOCKMSG + (int)eLOCKCHECKTYPE.STAGECLEAR), FLocalizeString.Instance.GetText(stagedata.Name)));
                }
            }

            return true; 
        }
        return false;
    }

    public void OnClick_Button()
    {
        if (OnLock())
            return;
        if (onClick == null)
            return;

        onClick.Execute();
    }

    /*
    protected virtual void OnClick()
    {
        if (current == null && isEnabled && UICamera.currentTouchID != -2 && UICamera.currentTouchID != -3)
        {
            if (OnLock())
                return;

            current = this;
            EventDelegate.Execute(onClick);
            current = null;

            if (psOnClick != null)
                psOnClick.Play();
        }
    }
    */
}
