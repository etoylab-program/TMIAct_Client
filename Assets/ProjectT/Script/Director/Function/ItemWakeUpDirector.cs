using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemWakeUpDirector : MonoBehaviour
{
    [System.Serializable]
    public class ItemGO
    {
        public GameObject m_SocketR = null;
        public GameObject m_SocketL = null;
    }


    [Header("Socket")]
    public List<ItemGO> kWeaponSockets = null;
    public GameObject kCardSockets = null;
    public GameObject kGemSockets = null;
    // Start is called before the first frame update
    public float kDelayShowPopup = 1.0f;

    public eContentsPosKind ItemType { get; private set; } = eContentsPosKind._NONE_;

    private AttachObject _weaponR = null;
    private AttachObject _weaponL = null;
    private Unit _addedWeapon = null;

    private AttachObject _weaponSubR = null;
    private AttachObject _weaponSubL = null;

    private AttachObject _weaponSubR2 = null;
    private AttachObject _weaponSubL2 = null;


	public void InitWeapon( WeaponData weaponData ) {
		ItemType = eContentsPosKind.WEAPON;
		DestroyItem();

		if( kWeaponSockets.Count < weaponData.ListCharId[0] ) {
			return;
		}

		//  오른쪽 셋팅
		if( !string.IsNullOrEmpty( weaponData.TableData.ModelR ) ) {
			_weaponR = ResourceMgr.Instance.CreateFromAssetBundle<AttachObject>( "item", "Item/" + weaponData.TableData.ModelR + ".prefab" );
			_weaponR.HideEffect();

			_weaponR.transform.parent = kWeaponSockets[weaponData.ListCharId[0] - 1].m_SocketR.transform;
			kWeaponSockets[weaponData.ListCharId[0] - 1].m_SocketR.transform.parent.gameObject.SetActive( true );

			Utility.InitTransform( _weaponR.gameObject );
			Utility.ChangeLayersRecursively( _weaponR.transform, eLayer.Director );
			kWeaponSockets[(int)weaponData.ListCharId[0] - 1].m_SocketR.gameObject.SetActive( true );

			SetSubWeapon( weaponData.TableData.SubModelR, false, _weaponR.transform, _weaponSubR );
			SetSubWeapon( weaponData.TableData.Sub2ModelR, false, _weaponR.transform, _weaponSubR2, 2 );
		}

		//  왼쪽 셋팅
		if( !string.IsNullOrEmpty( weaponData.TableData.ModelL ) ) {
			_weaponL = ResourceMgr.Instance.CreateFromAssetBundle<AttachObject>( "item", "Item/" + weaponData.TableData.ModelL + ".prefab" );
			_weaponL.HideEffect();

			_weaponL.transform.parent = kWeaponSockets[weaponData.ListCharId[0] - 1].m_SocketL.transform;
			kWeaponSockets[weaponData.ListCharId[0] - 1].m_SocketL.transform.parent.gameObject.SetActive( true );

			Utility.InitTransform( _weaponL.gameObject );
			Utility.ChangeLayersRecursively( _weaponL.transform, eLayer.Director );
			kWeaponSockets[(int)weaponData.ListCharId[0] - 1].m_SocketR.gameObject.SetActive( true );

			SetSubWeapon( weaponData.TableData.SubModelL, true, _weaponL.transform, _weaponSubL );
			SetSubWeapon( weaponData.TableData.Sub2ModelL, true, _weaponL.transform, _weaponSubL2, 2 );
		}

		//추가 무기
		if( !string.IsNullOrEmpty( weaponData.TableData.AddedUnitWeapon ) ) {
			_addedWeapon = GameSupport.CreateUnitWeapon( weaponData.TableData.AddedUnitWeapon, false );

            PlayerGuardian playerGuardian = _addedWeapon as PlayerGuardian;
            if ( playerGuardian ) {
                playerGuardian.ShowMaxEffect( false );
            }

            if ( (ePlayerCharType)weaponData.ListCharId[0] == ePlayerCharType.Emily ) {
				_addedWeapon.transform.parent = kWeaponSockets[weaponData.ListCharId[0] - 1].m_SocketL.transform;
				kWeaponSockets[weaponData.ListCharId[0] - 1].m_SocketL.transform.parent.gameObject.SetActive( true );
			}
            else if ( (ePlayerCharType)weaponData.ListCharId[0] == ePlayerCharType.Shisui ) {
                _addedWeapon.transform.parent = kWeaponSockets[weaponData.ListCharId[0] - 1].m_SocketR.transform;
                kWeaponSockets[weaponData.ListCharId[0] - 1].m_SocketR.transform.parent.gameObject.SetActive( true );
            }

            Utility.InitTransform( _addedWeapon.gameObject );
			Utility.ChangeLayersRecursively( _addedWeapon.transform, eLayer.Director );
			kWeaponSockets[(int)weaponData.ListCharId[0] - 1].m_SocketR.gameObject.SetActive( true );
		}
	}

	private void SetSubWeapon(string subWeaponModelName, bool dir, Transform parent, AttachObject target, int idx = 1)
    {
        if (string.IsNullOrEmpty(subWeaponModelName))
            return;

        Transform subModelParent = GameSupport.GetSubWeaponBoneOrNull(parent, dir, idx);

        if (subModelParent == null)
            return;

        string strSubModelName = "Item/" + subWeaponModelName + ".prefab";
        target = ResourceMgr.Instance.CreateFromAssetBundle<AttachObject>("item", strSubModelName);
        if (target != null)
        {
            target.transform.parent = subModelParent;
            Utility.InitTransform(target.gameObject);
            Utility.ChangeLayersRecursively(target.transform, eLayer.Director);
        }
    }

    //  서포터 외형 변경
    public void InitCard(CardData carddata)
    {
        ItemType = eContentsPosKind.CARD;
        DestroyItem();

        if (!string.IsNullOrEmpty(carddata.TableData.Icon))
        {
            Material beforeMat = kCardSockets.GetComponent<Renderer>().material;


            beforeMat.mainTexture = ResourceMgr.Instance.LoadFromAssetBundle("card", string.Format("Card/{0}_{1}.png", carddata.TableData.Icon, GameSupport.GetCardImageNum(carddata))) as Texture2D; //ResourceMgr.Instance.LoadFromAssetBundle("card", "Card/" + carddata.TableData.Icon + "_0.png") as Texture2D;
            kCardSockets.transform.parent.gameObject.SetActive(true);
        }
    }

    public void InitGem(GemData gemdata)
    {
        ItemType = eContentsPosKind.GEM;
        DestroyItem();

        if (!string.IsNullOrEmpty(gemdata.TableData.Icon))
        {
            Material beforeMat = kGemSockets.GetComponent<Renderer>().material;
            beforeMat.mainTexture = ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Item/" + gemdata.TableData.Icon ) as Texture2D;
            

            kGemSockets.transform.parent.gameObject.SetActive(true);
        }
    }

    private void DestroyItem()
    {
        if (_weaponR != null) DestroyImmediate(_weaponR.gameObject);
        if (_weaponL != null) DestroyImmediate(_weaponL.gameObject);
        if (_addedWeapon != null) DestroyImmediate(_addedWeapon.gameObject);

        if (kCardSockets != null) kCardSockets.transform.parent.gameObject.SetActive(false);
        if (kGemSockets != null) kGemSockets.transform.parent.gameObject.SetActive(false);

        if (_weaponSubL != null) DestroyImmediate(_weaponSubL.gameObject);
        if (_weaponSubR != null) DestroyImmediate(_weaponSubR.gameObject);

        if (_weaponSubL2 != null) DestroyImmediate(_weaponSubL2.gameObject);
        if (_weaponSubR2 != null) DestroyImmediate(_weaponSubR2.gameObject);
    }
}
