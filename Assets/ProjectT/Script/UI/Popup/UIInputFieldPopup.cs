using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputFieldPopup
{
    public static UIInputFieldPopup GetInputFieldPopup()
    {
        UIInputFieldPopup mpopup = null;
        mpopup = LobbyUIManager.Instance.GetUI<UIInputFieldPopup>("InputFieldPopup");

        return mpopup;
    }

    public static void Show(string strTitle, string strDesc, string originStr, int strLength, int addLength, bool multiLine, UIInputFieldPopup.OnClickOKCallBack okCallBack = null)
    {
        UIInputFieldPopup mpopup = GetInputFieldPopup();
        mpopup.InitInputFieldPopup(strTitle, strDesc, originStr, strLength, addLength, multiLine, okCallBack);
    }

    public static string GetInputTextWithClose()
    {
        UIInputFieldPopup mpopup = GetInputFieldPopup();
        mpopup.OnClickClose();
        return mpopup.GetInputText();
    }
    public static string GetInputText()
    {
        UIInputFieldPopup mpopup = GetInputFieldPopup();
        return mpopup.GetInputText();
    }
}

public class UIInputFieldPopup : FComponent
{
    public delegate void OnClickOKCallBack();
    public UISprite kBGSpr;
    
	public UILabel kTitleLb;
	public UILabel kDescLb;
    public UILabel kInputFieldLb;
    public UIInput kInputField;
	public UISprite kOkBtn;
	public UILabel kOkBtnLb;
	public UISprite kCancelBtn;
	public UILabel kCancelBtnLb;

    private OnClickOKCallBack successCallBack;

    private CommandCheckListTable m_keywordTable;
    private List<string> m_keywords = new List<string>();

    private string _originStr;
    private int _strLength;
    private int _addLength;


    public override void OnDisable()
    {
		if (AppMgr.Instance.IsQuit)
		{
			return;
		}

		base.OnDisable();
        m_keywordTable = null;
    }

    //입력된 텍스트 반환
    public string GetInputText()
    {
        return kInputFieldLb.text;
    }

    public void InitInputFieldPopup(string strTitle, string strDesc, string originStr, int strLength, int addLength, bool multiLine, OnClickOKCallBack okCallBack = null)
    {
        _originStr = originStr;
        _strLength = strLength;
        _addLength = addLength;
        kTitleLb.textlocalize = strTitle;
        kDescLb.textlocalize = strDesc;
        kInputFieldLb.multiLine = multiLine;
        kInputFieldLb.textlocalize = originStr;
        
        kInputField.value = kInputFieldLb.text;
        
        successCallBack = okCallBack;

        if(m_keywordTable == null)
        {
            m_keywordTable = (CommandCheckListTable)ResourceMgr.Instance.LoadFromAssetBundle("system", "System/Table/CommandCheckList.asset");

            for (int i = 0; i < m_keywordTable.Keywords.Count; i++)
                m_keywords.Add(m_keywordTable.Keywords[i].keyword);
        }
            
        SetUIActive(true);
    }

    bool CheckCommand(string str)
    {
        bool flag = false;
		string[] stringSpace = Utility.Split(str, ' '); //str.Split(' ');

        for(int i = 0; i < stringSpace.Length; i++)
        {
            for(int j = 0; j < m_keywords.Count; j++)
            {
                if (stringSpace[i].Contains(m_keywords[j]))
                {
                    flag = true;
                    return flag;
                }
            }
            
        }
        return flag;
    }

    public override void Renewal(bool bChildren)
	{
		base.Renewal(bChildren);
	}
 
    public void OnClickOKBtn()
    {
        if(kInputFieldLb.text.Equals(_originStr))
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3096));
            return;
        }

        //빈 항목 일때
        if(kInputFieldLb.text.Length == 0)
        {
            MessageToastPopup.Show(FLocalizeString.Instance.GetText(3093));
            return;
        }

        //제한 길이를 벗어낫을 때
        if(kInputFieldLb.text.Length > _strLength)
        {
            MessageToastPopup.Show(string.Format(FLocalizeString.Instance.GetText(3091), kTitleLb.text, _strLength));
            return;
        }

        // 사용할 수 없는 유니코드 체크
        for( int i = 0; i < GameInfo.Instance.UnicodeCheckTable.Infos.Count; i++ ) {
            if( CheckUnicodePosition( kInputField.value, GameInfo.Instance.UnicodeCheckTable.Infos[i].Min, GameInfo.Instance.UnicodeCheckTable.Infos[i].Max ) ) {
                MessageToastPopup.Show( string.Format( FLocalizeString.Instance.GetText( 3141 ), kTitleLb.text, _strLength ) );
                return;
            }
        }

        // 색상 BB코드 체크
        for( int i = 0; i < kInputField.value.Length; i++ ) {
            if( kInputField.value[i] == '[' ) {
                if( CheckBBCode( kInputField.value, i ) ) {
                    MessageToastPopup.Show( string.Format( FLocalizeString.Instance.GetText( 3141 ), kTitleLb.text, _strLength ) );
                    return;
                }
            }
        }

        if(CheckCommand(kInputFieldLb.text))
        {
            MessageToastPopup.Show(string.Format(FLocalizeString.Instance.GetText(3141), kTitleLb.text, _strLength));
            return;
        }

        /*
        if ( CheckUnicodePosition( kInputFieldLb.text, 12288, 12351 ) ) {
            MessageToastPopup.Show( string.Format( FLocalizeString.Instance.GetText( 3141 ), kTitleLb.text, _strLength ) );
            return;
        }
        */

        if (successCallBack != null)
        {
            successCallBack();
        }
        else
        {
            OnClickCancelBtn();
        }
    }

    public void OnClickCancelBtn()
    {
        OnClickClose();
    }

    public override void OnClickClose()
    {
        successCallBack = null;
        base.OnClickClose();
    }

    public void InputFieldChangeValue()
    {
        if (kInputField.value.Length <= _strLength + _addLength)
        {
            kInputFieldLb.textlocalize = kInputField.value;
            
        }
        else
        {
            kInputField.value = kInputField.value.Substring(0, _strLength + _addLength);
            kInputFieldLb.textlocalize = kInputField.value;
        }
    }

    public void InputFieldSubmit()
    {
        kInputFieldLb.textlocalize = kInputField.value;
    }

    private bool CheckUnicodePosition( string str, int minDecPosition, int maxDecPosition ) {
        for( int i = 0; i < str.Length; i += char.IsSurrogatePair( str, i ) ? 2 : 1 ) {
            int codePoint = char.ConvertToUtf32( str, i );

            // 첫 번째 문자 공백 금지
            if( i == 0 && codePoint == 20 ) {
                return false;
			}

            if( codePoint >= minDecPosition && codePoint <= maxDecPosition ) {
                return true;
            }
        }

        return false;
    }

    private bool CheckBBCode( string str, int startIndex ) {
        if( startIndex + 1 >= str.Length ) {
            return false;
        }

        int lastIndex = -1;

        for( int i = startIndex; i < str.Length; i++ ) {
            if( str[i] == ']' ) {
                lastIndex = i;
                break;
            }
        }

        if( lastIndex == -1 ) {
            return false;
        }

        int length = ( lastIndex - startIndex ) - 1;
        if( length != 6 && length != 8 && length != 1 ) {
            return false;
        }

        for( int i = startIndex + 1; i < lastIndex - 1; i++ ) {
            if( ( str[i] >= '0' && str[i] <= '9' ) || ( str[i] >= 'a' && str[i] <= 'f' ) || ( str[i] >= 'A' && str[i] <= 'F' ) || ( str[i] == '-' ) ) {
            }
            else {
                return false;
            }
        }

        return true;
    }
}
