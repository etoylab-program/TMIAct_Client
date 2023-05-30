using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemChangeDirector : MonoBehaviour
{
    [System.Serializable]
    public class ItemGO
    {
        public GameObject m_SocketR = null;
        public GameObject m_SocketL = null;
    }

    [Header("Socket")]
    public List<ItemGO> m_beforeCharSockets = null;
    public List<ItemGO> m_afterCharSockets = null;

    public GameObject m_beforeCardSockets = null;
    public GameObject m_afterCardSockets = null;

    private AttachObject m_weaponBeforeR = null;
    private AttachObject m_weaponBeforeL = null;
    private AttachObject m_weaponAfterR = null;
    private AttachObject m_weaponAfterL = null;

    private Unit m_addedWeaponBefore = null;
    private Unit m_addedWeaponAfter = null;

    private AttachObject m_weaponSubBeforeR = null;
    private AttachObject m_weaponSubBeforeL = null;
    private AttachObject m_weaponSubAfterR = null;
    private AttachObject m_weaponSubAfterL = null;

    private AttachObject m_weaponSubBeforeR2 = null;
    private AttachObject m_weaponSubBeforeL2 = null;
    private AttachObject m_weaponSubAfterR2 = null;
    private AttachObject m_weaponSubAfterL2 = null;

	public void InitWeapon( WeaponData weaponData ) {
		Eraser();

		if( m_beforeCharSockets.Count < weaponData.ListCharId[0] || m_afterCharSockets.Count < weaponData.ListCharId[0] ) {
			return;
		}

		//  오른쪽 셋팅
		if( !string.IsNullOrEmpty( weaponData.TableData.ModelR ) ) {
			m_weaponBeforeR = ResourceMgr.Instance.CreateFromAssetBundle<AttachObject>( "item", "Item/" + weaponData.TableData.ModelR + ".prefab" );
			m_weaponAfterR = ResourceMgr.Instance.CreateFromAssetBundle<AttachObject>( "item", "Item/" + weaponData.TableData.ModelR + ".prefab" );

			//  변경시 이전오브젝트는 이펙트가 보여지지 않아야 합니다.
            m_weaponBeforeR.HideEffect();

			m_weaponBeforeR.transform.parent = m_beforeCharSockets[weaponData.ListCharId[0] - 1].m_SocketR.transform;
			m_weaponAfterR.transform.parent = m_afterCharSockets[weaponData.ListCharId[0] - 1].m_SocketR.transform;

			Utility.InitTransform( m_weaponBeforeR.gameObject );
			Utility.ChangeLayersRecursively( m_weaponBeforeR.transform, eLayer.Director );

			SetSubWeapon( weaponData.TableData.SubModelR, false, false, m_weaponBeforeR.transform, m_weaponSubBeforeR );
			SetSubWeapon( weaponData.TableData.Sub2ModelR, false, false, m_weaponBeforeR.transform, m_weaponSubBeforeR2, 2 );

			Utility.InitTransform( m_weaponAfterR.gameObject );
			Utility.ChangeLayersRecursively( m_weaponAfterR.transform, eLayer.Director );

			SetSubWeapon( weaponData.TableData.SubModelR, false, true, m_weaponAfterR.transform, m_weaponSubAfterR );
			SetSubWeapon( weaponData.TableData.Sub2ModelR, false, true, m_weaponAfterR.transform, m_weaponSubAfterR2, 2 );
		}

		//  왼쪽 셋팅
		if( !string.IsNullOrEmpty( weaponData.TableData.ModelL ) ) {
			m_weaponBeforeL = ResourceMgr.Instance.CreateFromAssetBundle<AttachObject>( "item", "Item/" + weaponData.TableData.ModelL + ".prefab" );
			m_weaponAfterL = ResourceMgr.Instance.CreateFromAssetBundle<AttachObject>( "item", "Item/" + weaponData.TableData.ModelL + ".prefab" );

			//  변경시 이전오브젝트는 이펙트가 보여지지 않아야 합니다.
            m_weaponBeforeL.HideEffect();

			m_weaponBeforeL.transform.parent = m_beforeCharSockets[weaponData.ListCharId[0] - 1].m_SocketL.transform;
			m_weaponAfterL.transform.parent = m_afterCharSockets[weaponData.ListCharId[0] - 1].m_SocketL.transform;

			Utility.InitTransform( m_weaponBeforeL.gameObject );
			Utility.ChangeLayersRecursively( m_weaponBeforeL.transform, eLayer.Director );

			SetSubWeapon( weaponData.TableData.SubModelL, true, false, m_weaponBeforeL.transform, m_weaponSubBeforeL );
			SetSubWeapon( weaponData.TableData.Sub2ModelL, true, false, m_weaponBeforeL.transform, m_weaponSubBeforeL2, 2 );

			Utility.InitTransform( m_weaponAfterL.gameObject );
			Utility.ChangeLayersRecursively( m_weaponAfterL.transform, eLayer.Director );

			SetSubWeapon( weaponData.TableData.SubModelL, true, true, m_weaponAfterL.transform, m_weaponSubAfterL );
			SetSubWeapon( weaponData.TableData.Sub2ModelL, true, true, m_weaponAfterL.transform, m_weaponSubAfterL2, 2 );
		}

		//추가 무기
		if( !string.IsNullOrEmpty( weaponData.TableData.AddedUnitWeapon ) ) {
			m_addedWeaponBefore = GameSupport.CreateUnitWeapon( weaponData.TableData.AddedUnitWeapon, false );
			m_addedWeaponAfter = GameSupport.CreateUnitWeapon( weaponData.TableData.AddedUnitWeapon, false );

            PlayerGuardian playerGuardian = m_addedWeaponBefore as PlayerGuardian;
            if ( playerGuardian ) {
                playerGuardian.ShowMaxEffect( false );
            }

            if ( (ePlayerCharType)weaponData.ListCharId[0] == ePlayerCharType.Emily ) {
				m_addedWeaponBefore.transform.parent = m_beforeCharSockets[(int)ePlayerCharType.Emily - 1].m_SocketL.transform;
				m_addedWeaponAfter.transform.parent = m_afterCharSockets[(int)ePlayerCharType.Emily - 1].m_SocketL.transform;
			}
            else if ( (ePlayerCharType)weaponData.ListCharId[0] == ePlayerCharType.Shisui ) {
                m_addedWeaponBefore.transform.parent = m_beforeCharSockets[(int)ePlayerCharType.Shisui - 1].m_SocketR.transform;
                m_addedWeaponAfter.transform.parent = m_afterCharSockets[(int)ePlayerCharType.Shisui - 1].m_SocketR.transform;
            }

            Utility.InitTransform( m_addedWeaponBefore.gameObject );
			Utility.ChangeLayersRecursively( m_addedWeaponBefore.transform, eLayer.Director );

			Utility.InitTransform( m_addedWeaponAfter.gameObject );
			Utility.ChangeLayersRecursively( m_addedWeaponAfter.transform, eLayer.Director );
		}
	}

	private void SetSubWeapon(string subWeaponModelName, bool dir, bool showEffect, Transform parent, AttachObject target, int idx = 1)
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
            if (!showEffect)
                target.HideEffect();
        }
    }

    //  서포터 외형 변경
    public void InitCard(CardData cardData)
    {
        Eraser();

        if (!string.IsNullOrEmpty(cardData.TableData.Icon))
        {
            Material beforeMat = m_beforeCardSockets.GetComponent<Renderer>().material;
            Material afterMat = m_afterCardSockets.GetComponent<Renderer>().material;

       
            beforeMat.mainTexture = ResourceMgr.Instance.LoadFromAssetBundle("card", "Card/" + cardData.TableData.Icon + "_0.png") as Texture2D;
            afterMat.mainTexture = ResourceMgr.Instance.LoadFromAssetBundle("card", "Card/" + cardData.TableData.Icon + "_1.png") as Texture2D;

            m_beforeCardSockets.SetActive(true);
            m_afterCardSockets.SetActive(true);

        }
    }

    private void Eraser()
    {
        if (m_weaponBeforeR != null) DestroyImmediate(m_weaponBeforeR.gameObject);
        if (m_weaponBeforeL != null) DestroyImmediate(m_weaponBeforeL.gameObject);
        if (m_weaponAfterR != null) DestroyImmediate(m_weaponAfterR.gameObject);
        if (m_weaponAfterL != null) DestroyImmediate(m_weaponAfterL.gameObject);

        if (m_beforeCardSockets != null) m_beforeCardSockets.SetActive(false);
        if (m_afterCardSockets != null) m_afterCardSockets.SetActive(false);

        if (m_addedWeaponBefore != null) DestroyImmediate(m_addedWeaponBefore.gameObject);
        if (m_addedWeaponAfter != null) DestroyImmediate(m_addedWeaponAfter.gameObject);

        if (m_weaponSubBeforeL != null) DestroyImmediate(m_weaponSubBeforeL.gameObject);
        if (m_weaponSubBeforeR != null) DestroyImmediate(m_weaponSubBeforeR.gameObject);
        if (m_weaponSubAfterL != null) DestroyImmediate(m_weaponSubAfterL.gameObject);
        if (m_weaponSubAfterR != null) DestroyImmediate(m_weaponSubAfterR.gameObject);

        if (m_weaponSubBeforeL2 != null) DestroyImmediate(m_weaponSubBeforeL2.gameObject);
        if (m_weaponSubBeforeR2 != null) DestroyImmediate(m_weaponSubBeforeR2.gameObject);
        if (m_weaponSubAfterL2 != null) DestroyImmediate(m_weaponSubAfterL2.gameObject);
        if (m_weaponSubAfterR2 != null) DestroyImmediate(m_weaponSubAfterR2.gameObject);
    }
}
