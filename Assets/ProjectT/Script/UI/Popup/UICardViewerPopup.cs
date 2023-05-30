using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CardViewer
{
	public static UICardViewerPopup ShowCardPopup(int tableid, bool wakeup)
	{
		UICardViewerPopup popup = null;
		popup = LobbyUIManager.Instance.GetUI<UICardViewerPopup>("CardViewerPopup");
		popup.InitCardViewer(tableid, wakeup);
		return popup;
	}
}

public class UICardViewerPopup : FComponent
{
	enum eViewerType
	{
		Horizontal,
		Vertical,
	}

	enum eViewState
	{
		CamMove,
		Zoom,
		None,
	}


	[Header("Vertical")]
	public Transform kVerBackRoot;
	public Transform kVerRefeshRoot;

	[Header("Horizontal")]
	public Transform kHorBackRoot;
	public Transform kHorRefeshRoot;

	public UIButton kRefeshBtn;
	public UISprite kRefeshHorSpr;
	public UISprite kRefeshVerSpr;
	public UIButton kBackBtn;
	public UITexture kCardTex;

	private int _tableId;
	private bool _bWakeup = false;
	private GameTable.Card.Param _cardTableData;

	private eViewState _viewState = eViewState.None;
	private eViewerType _viewType = eViewerType.Vertical;

	private TweenRotation _tweenRoration = null;
	private float _tweenTime = 0.4f;

	private float _texScale = 1f;
	private Vector3 _vecTexScale = Vector3.one;
	private float _moveSpeed = 1f;

 
	public void InitCardViewer(int tableid, bool wakeup)
	{
		_tableId = tableid;
		_bWakeup = wakeup;

		_cardTableData = GameInfo.Instance.GameTable.FindCard(_tableId);
		if (null == _cardTableData)
			return;

		if (!_bWakeup)
			kCardTex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("card", string.Format("Card/{0}_{1}.png", _cardTableData.Icon, 0));
		else
			kCardTex.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("card", string.Format("Card/{0}_{1}.png", _cardTableData.Icon, 1));

		kCardTex.MakePixelPerfect();
		kCardTex.transform.localRotation = Quaternion.identity;
		kCardTex.transform.localPosition = Vector3.zero;
		_viewType = eViewerType.Horizontal;

		_vecTexScale = Vector3.one;
		_texScale = 1.0f;

		if (_tweenRoration != null)
			_tweenRoration.enabled = false;

		SetViewerType();
		SetUIActive(true);
	}
 
	public void OnClick_RefeshBtn()
	{
		if (_viewType == eViewerType.Vertical)
			_viewType = eViewerType.Horizontal;
		else
			_viewType = eViewerType.Vertical;

		SetViewerType();
		ChangeViewerType();
	}

	private void SetViewerType()
	{
		kRefeshHorSpr.gameObject.SetActive(_viewType == eViewerType.Horizontal);
		kRefeshVerSpr.gameObject.SetActive(_viewType == eViewerType.Vertical);
		switch (_viewType)
		{
			case eViewerType.Horizontal:
				{
					kBackBtn.transform.SetParent(kHorBackRoot);
					kRefeshBtn.transform.SetParent(kHorRefeshRoot);
				}
				break;
			case eViewerType.Vertical:
				{
					kBackBtn.transform.SetParent(kVerBackRoot);
					kRefeshBtn.transform.SetParent(kVerRefeshRoot);
				}
				break;
		}

		Utility.InitTransform(kBackBtn.gameObject);
		Utility.InitTransform(kRefeshBtn.gameObject);
	}

	private void ChangeViewerType()
	{
		if (_tweenRoration == null)
			_tweenRoration = kCardTex.gameObject.AddComponent<TweenRotation>();

		switch (_viewType)
		{
			case eViewerType.Horizontal:
				{
					TweenRotation.SetTweenRotation(_tweenRoration, UITweener.Style.Once, kCardTex.transform.localRotation.eulerAngles, new Vector3(0, 0, 0f), _tweenTime, 0f, null);
				}
				break;
			case eViewerType.Vertical:
				{
					TweenRotation.SetTweenRotation(_tweenRoration, UITweener.Style.Once, kCardTex.transform.localRotation.eulerAngles, new Vector3(0, 0, 90f), _tweenTime, 0f, null);
				}
				break;
		}
	}
	
	public void OnClick_BackBtn()
	{
		OnClickClose();
	}

    private void Update()
    {
		if (Mathf.Abs(AppMgr.Instance.CustomInput.Delta) > 0.1f)
		{
			_texScale += AppMgr.Instance.CustomInput.Delta * Time.deltaTime;

			if (_texScale >= 2f)
			{
				_texScale = 2f;
			}

			if (_texScale <= 0.5f)
			{
				_texScale = 0.5f;
			}
			_vecTexScale.x = _texScale;
			_vecTexScale.y = _texScale;
			kCardTex.transform.localScale = _vecTexScale;
		}

		MoveCardTex(Vector3.zero);
	}

	

	void MoveCardTex(Vector2 moveTouch)
	{
		Vector3 v = AppMgr.Instance.CustomInput.MultiTouchDeltaPos;
		if (v == Vector3.zero)
		{
			return;
		}

		if (_viewType == eViewerType.Horizontal)
		{
			v.x = AppMgr.Instance.CustomInput.MultiTouchDeltaPos.y;
			v.y = AppMgr.Instance.CustomInput.MultiTouchDeltaPos.x;
		}

		v *= AppMgr.Instance.CustomInput.Sensitivity;

		kCardTex.transform.localPosition = new Vector3(
				kCardTex.transform.localPosition.x + (v.y * _moveSpeed * Time.deltaTime),
			 kCardTex.transform.localPosition.y + (v.x * _moveSpeed * Time.deltaTime),
			  kCardTex.transform.localPosition.z);

		if (kCardTex.transform.localPosition.x >= AppMgr.Instance.DeviceWidth * 0.25f)
		{
			kCardTex.transform.localPosition = new Vector3(AppMgr.Instance.DeviceWidth * 0.25f,
				kCardTex.transform.localPosition.y,
				kCardTex.transform.localPosition.z);
		}
		if (kCardTex.transform.localPosition.x <= -AppMgr.Instance.DeviceWidth * 0.25f)
		{
			kCardTex.transform.localPosition = new Vector3(-AppMgr.Instance.DeviceWidth * 0.25f,
				kCardTex.transform.localPosition.y,
				kCardTex.transform.localPosition.z);
		}
		if (kCardTex.transform.localPosition.y >= AppMgr.Instance.DeviceHeight * 0.25f)
		{
			kCardTex.transform.localPosition = new Vector3(
				kCardTex.transform.localPosition.x,
				AppMgr.Instance.DeviceHeight * 0.25f,
				kCardTex.transform.localPosition.z);
		}
		if (kCardTex.transform.localPosition.y <= -AppMgr.Instance.DeviceHeight * 0.25f)
		{
			kCardTex.transform.localPosition = new Vector3(
				kCardTex.transform.localPosition.x,
				-AppMgr.Instance.DeviceHeight * 0.25f,
				kCardTex.transform.localPosition.z);
		}

	}
}
