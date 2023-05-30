using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderTargetChar : FMonoSingleton<RenderTargetChar>
{
    public GameObject kRenderTargetDirectionalLight;
    public Camera kRenderTargetCharCamera;
    private int _tableid = -1;
    private long _uid = -1;

    private LobbyPlayer _renderplayer = null;
    private Enemy _enemy = null;
    private FigureUnit _figure = null;
    private Unit _addedWeapon;         //추가 무기 - 예시)에밀리 드론
    private long _addedWeaponTID = -1;

    public int RenderPlayerTableID { get { return _tableid; } }
    public long RenderPlayerUID { get { return _uid; } }

    public LobbyPlayer RenderPlayer { get { return _renderplayer;  } }
    public Enemy RenderEnemy { get { return _enemy; } }
    public FigureUnit Figure { get { return _figure; } }
    public Unit AddedWeapon { get { return _addedWeapon; } }

    private readonly float _defaultCamSize = 0.88f;
    private WeaponData mWeaponData = null;


    public void InitRenderTargetChar( int tableid, long uid, bool bchangecostume,eCharacterType charType)
    {
        if (bchangecostume != true)
        {
            if (_tableid == tableid && _uid == uid && charType == eCharacterType.Monster)
                return;
        }

        _tableid = tableid;
        _uid = uid;

        DestroyObject();


        Log.Show(uid, Log.ColorType.Red);
        CharData chardata = GameInfo.Instance.GetCharData(uid);

		_renderplayer = GameSupport.CreateLobbyPlayer(_tableid, chardata);
		if(_renderplayer == null)
		{
			return;
		}

        _renderplayer.transform.parent = this.gameObject.transform;
        _renderplayer.CharLight_On();
        _renderplayer.CharLight.cullingMask = 1 << (int)eLayer.RenderTarget; //&= ~(1 << (int)eLayer.RenderTarget);  //LayerMask.NameToLayer("RenderTarget");
        _renderplayer.Init(eAnimation.None);
        _renderplayer.SetInitialPosition(new Vector3(0, 0, _renderplayer.LobbyPos.z), Quaternion.Euler(0.0f, 0.0f, 0.0f));
        
        LayerMask.NameToLayer(name);
        Utility.ChangeLayersRecursively(_renderplayer.transform, "RenderTarget");

        kRenderTargetCharCamera.orthographicSize = _renderplayer.LobbyRenderCamSize;

        //7789  _renderplayer.GetComponent<LobbyPlayer>().enabled = false;

        _renderplayer.costumeUnit.ShowObject(_renderplayer.costumeUnit.Param.InGameOnly, false);
        _renderplayer.costumeUnit.ShowObject(_renderplayer.costumeUnit.Param.LobbyOnly, true);

        _renderplayer.PlayFaceAni(eFaceAnimation.Idle01);

    }

    public void InitRenderTargetChar(int tableid, long uid, int costumeId, eCharacterType charType, bool isBook = false)
    {
        _tableid = tableid;
        _uid = uid;

        DestroyObject();


        Log.Show(uid, Log.ColorType.Red);
        CharData chardata = GameInfo.Instance.GetCharData(uid);

        _renderplayer = GameSupport.CreateLobbyPlayer(_tableid, costumeId, chardata, isBook);
		if (_renderplayer == null)
		{
			return;
		}

		_renderplayer.transform.parent = this.gameObject.transform;
        _renderplayer.CharLight_On();
        _renderplayer.CharLight.cullingMask = 1 << (int)eLayer.RenderTarget; //&= ~(1 << (int)eLayer.RenderTarget);  //LayerMask.NameToLayer("RenderTarget");
        _renderplayer.Init(eAnimation.None);
        _renderplayer.SetInitialPosition(new Vector3(0, 0, _renderplayer.LobbyPos.z), Quaternion.Euler(0.0f, 0.0f, 0.0f));

        LayerMask.NameToLayer(name);
        Utility.ChangeLayersRecursively(_renderplayer.transform, "RenderTarget");

        kRenderTargetCharCamera.orthographicSize = _renderplayer.LobbyRenderCamSize;

        _renderplayer.costumeUnit.ShowObject(_renderplayer.costumeUnit.Param.InGameOnly, false);
        _renderplayer.costumeUnit.ShowObject(_renderplayer.costumeUnit.Param.LobbyOnly, true);

    }

    public void InitRenderTargetChar(int tableid, long uid, CharData charData, bool bchangecostume, eCharacterType charType)
    {
        if (bchangecostume != true)
        {
            if (_tableid == tableid && _uid == uid && charType == eCharacterType.Monster)
                return;
        }

        _tableid = tableid;
        _uid = uid;

        DestroyObject();

        _renderplayer = GameSupport.CreateLobbyPlayer(_tableid, charData);
		if (_renderplayer == null)
		{
			return;
		}

		_renderplayer.transform.parent = this.gameObject.transform;
        _renderplayer.CharLight_On();
        _renderplayer.CharLight.cullingMask = 1 << (int)eLayer.RenderTarget; //&= ~(1 << (int)eLayer.RenderTarget);  //LayerMask.NameToLayer("RenderTarget");
        _renderplayer.Init(eAnimation.None);
        LayerMask.NameToLayer(name);
        Utility.ChangeLayersRecursively(_renderplayer.transform, "RenderTarget");

        kRenderTargetCharCamera.orthographicSize = _renderplayer.LobbyRenderCamSize;

        _renderplayer.costumeUnit.ShowObject(_renderplayer.costumeUnit.Param.InGameOnly, false);
        _renderplayer.costumeUnit.ShowObject(_renderplayer.costumeUnit.Param.LobbyOnly, true);
    }

    public void SetCostumeBody(int costumeid, int costumecolor, int costumestateflag, DyeingData dyeingData)
    {
		if(_renderplayer == null || _renderplayer.costumeUnit == null)
		{
			return;
		}

        _renderplayer.costumeUnit.SetCostumeBody(costumeid, costumecolor, costumestateflag, dyeingData);
        Utility.ChangeLayersRecursively(_renderplayer.transform, "RenderTarget");
    }

    public void SetCostumeWeapon(CharData chardata, bool bweaponshow)
    {
        _renderplayer.costumeUnit.SetWeaponAttach(chardata, bweaponshow);
        Utility.ChangeLayersRecursively(_renderplayer.transform, "RenderTarget");

        Log.Show("Skin Id : " + chardata.EquipWeaponSkinTID);

        SetAddedWeapon(chardata, bweaponshow);

        _renderplayer.costumeUnit.ChangeAttachWeaponName(_renderplayer.ChangeWeaponName);

        if (_renderplayer.aniEvent)
        {
            _renderplayer.aniEvent.Rebind();
        }
    }

    public void SetWeaponAttachTableData(int weapontableid, bool wake, bool bweaponshow)
    {
        _renderplayer.costumeUnit.SetWeaponAttachTableData(weapontableid, wake, bweaponshow);
        Utility.ChangeLayersRecursively(_renderplayer.transform, "RenderTarget");

        _renderplayer.costumeUnit.ChangeAttachWeaponName(_renderplayer.ChangeWeaponName);

        if (_renderplayer.aniEvent)
        {
            _renderplayer.aniEvent.Rebind();
        }
    }

    //추가 무기 on/off
    private void SetAddedWeapon(CharData chardata, bool bweaponshow)
    {
        if (chardata == null)
            return;

        long weaponUID = chardata.EquipWeaponUID;
        long weaponMainSkinTID = chardata.EquipWeaponSkinTID;

        GameTable.Weapon.Param weaponTableData = null;
        mWeaponData = GameInfo.Instance.GetWeaponData( weaponUID );

        if (weaponMainSkinTID != (int)eCOUNT.NONE)
        {
            weaponTableData = GameInfo.Instance.GameTable.FindWeapon(x => x.ID == weaponMainSkinTID);
        }            
        else
        {
            weaponTableData = mWeaponData?.TableData;
        }



        //= GameInfo.Instance.GameTable.FindWeapon(x => x.ID == addWeaponTID);

        if (weaponTableData != null)
        {
            if(_addedWeaponTID == weaponTableData.ID)
            {
                if(_addedWeapon != null)
                {
                    _addedWeapon.gameObject.SetActive(bweaponshow);
                }
                else
                {
                    CreateAddedWeapon(weaponTableData, bweaponshow);
                }
            }
            else
            {
                CreateAddedWeapon(weaponTableData, bweaponshow);
            }
        }
        else
        {
            _addedWeaponTID = -1;
            if (_addedWeapon != null)
                _addedWeapon.gameObject.SetActive(false);
        }

		PlayerGuardian playerGuardian = _addedWeapon as PlayerGuardian;
		if ( playerGuardian ) {
			bool isMaxLevel = mWeaponData != null ? GameSupport.IsWeaponOpenTerms_Effect( mWeaponData ) : false;
			playerGuardian.ShowMaxEffect( isMaxLevel );

			playerGuardian.Resume();
			playerGuardian.PlayAniImmediate( eAnimation.Lobby_Weapon );
		}
	}

    //추가 무기 생성
    private void CreateAddedWeapon(GameTable.Weapon.Param weaponTableData, bool bweaponshow)
    {
        if (!string.IsNullOrEmpty(weaponTableData.AddedUnitWeapon))
        {
            if(_addedWeapon != null)
            {
                DestroyImmediate(_addedWeapon.gameObject);
            }

            _addedWeapon = GameSupport.CreateUnitWeapon(weaponTableData.AddedUnitWeapon, false);
            Utility.ChangeLayersRecursively(AddedWeapon.transform, "RenderTarget");
            Transform parent = _renderplayer.transform.Find("AddedUnitWeaponPos");
            if (parent != null)
            {
                AddedWeapon.transform.parent = parent;
                AddedWeapon.transform.localScale = Vector3.one;
                AddedWeapon.transform.localPosition = Vector3.zero;
                AddedWeapon.transform.localRotation = Quaternion.identity;
            }

			PlayerGuardian playerGuardian = _addedWeapon as PlayerGuardian;
			if ( playerGuardian ) {
				playerGuardian.Init( weaponTableData.ID, eCharacterType.Other, string.Empty );
				playerGuardian.aniEvent.transform.rotation = RenderPlayer.aniEvent.transform.rotation;
			}

			_addedWeaponTID = weaponTableData.ID;
            _addedWeapon.gameObject.SetActive(bweaponshow);
        }
        else
        {
            _addedWeaponTID = -1;
            if (_addedWeapon != null)
                _addedWeapon.gameObject.SetActive(false);
        }
    }

	public void ShowAttachedObject(bool bweaponshow)
	{
		if (_renderplayer == null || _renderplayer.costumeUnit == null)
		{
			return;
		}

        _renderplayer.costumeUnit.ShowAttachedObject(bweaponshow);

        //추가 무기 on/off
        if (_addedWeapon != null)
        {
            _addedWeapon.gameObject.SetActive(bweaponshow);
        }
    }

    public void InitRenderTargetFigure(int tableid, eCharacterType charType)
    {
        DestroyObject();

        Log.Show( "###### : " + tableid, Log.ColorType.Red );
        if( _tableid == tableid && charType == eCharacterType.Figure ) {
            return;
        }

        kRenderTargetCharCamera.orthographicSize = _defaultCamSize;

        _tableid = tableid;
        //charType = eCharacterType.Figure;
        var tabledata = GameInfo.Instance.GameClientTable.FindBookMonster(tableid);
        
        _figure = GameSupport.CreateFigure(tabledata.RoomFigureID, null, false);
        Log.Show("###### : " + tabledata.RoomFigureID + " / " + _figure, Log.ColorType.Red);
        if (_figure == null)
            return;

        _figure.transform.parent = this.gameObject.transform;
        _figure.transform.localScale = new Vector3(tabledata.Scale, tabledata.Scale, tabledata.Scale);
        _figure.transform.localPosition = new Vector3(0, tabledata.Height, 0);
        _figure.PlayAniImmediate(eAnimation.Idle02);
        LayerMask.NameToLayer(name);
        Utility.ChangeLayersRecursively(_figure.transform, "RenderTarget");

        kRenderTargetDirectionalLight.SetActive(true);
    }

    public void DestroyRenderTarget()
    {
        DestroyObject();

        // 파괴하면서 tableid, uid의 값도 날려준다. (init 시에 조건 체크 되는 부분 때문에 재생성이 안되는 버그가 생기므로)
        _tableid = -1;
        _uid = -1;
        _addedWeaponTID = -1;
    }

    private void DestroyObject() {
        if( _renderplayer != null ) {
            DestroyImmediate( _renderplayer.gameObject );
            DestroyImmediate( _renderplayer );

            _renderplayer = null;
        }

        if( _enemy != null ) {
            DestroyImmediate( _enemy.gameObject );
            DestroyImmediate( _enemy );

            _enemy = null;
        }

        if( _figure != null ) {
            DestroyImmediate( _figure.gameObject );
            DestroyImmediate( _figure );

            _figure = null;
        }

		if ( _addedWeapon != null ) {
			DestroyImmediate( _addedWeapon.gameObject );
			DestroyImmediate( _addedWeapon );

			_addedWeapon = null;
			_addedWeaponTID = -1;
		}

		System.GC.Collect();
    }

    /*
    public void InitRenderTargetMonster(int tableid, eCharacterType charType)
    {
        if (_renderplayer != null)
        {
            DestroyImmediate(_renderplayer.gameObject);
        }
        if (_figure != null)
        {
            DestroyImmediate(_figure.gameObject);
        }

        if (_tableid == tableid && charType == eCharacterType.Monster)
        {
            return;
        }

        if (_enemy != null)
        {
            Destroy(_enemy.gameObject);
        }

        _tableid = tableid;

        var tabledata = GameInfo.Instance.GameClientTable.FindBookMonster(tableid);

        string strModel = Utility.AppendString("Unit/", tabledata.ModelPb, ".prefab");

        Enemy enemy = ResourceMgr.Instance.CreateFromAssetBundle<Enemy>("unit", strModel);
        enemy.LobbyInit(tableid, eCharacterType.Monster);
        //enemy.cl
        enemy.SetKinematicRigidBody();
        
        enemy.transform.parent = this.gameObject.transform;
        
        LayerMask.NameToLayer(name);
        Utility.ChangeLayersRecursively(enemy.transform, "RenderTarget");

        _enemy = enemy;
    }
    */
}
