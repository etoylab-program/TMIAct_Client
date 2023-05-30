using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CostumeUnit : MonoBehaviour
{
    public class sWeaponItem
    {
        public long         UID;
        public WeaponData   Data;               // PVP에 상대편은 무기 UID를 안갖고있음
        public AttachObject LeftObj;
        public AttachObject RightObj;
        public AttachObject LeftSubObj;
        public AttachObject RightSubObj;
        public AttachObject LeftSub2Obj;
        public AttachObject RightSub2Obj;
        public Unit         AddedUnitWeapon;
        public string       OriginalName;


		public void Show( bool show, string changeName = "" ) {
			if ( LeftObj ) {
				LeftObj.gameObject.SetActive( show );
			}

			if ( RightObj ) {
				RightObj.gameObject.SetActive( show );
			}

			if ( LeftSubObj ) {
				LeftSubObj.gameObject.SetActive( show );
			}

			if ( RightSubObj ) {
				RightSubObj.gameObject.SetActive( show );
			}

			if ( LeftSub2Obj ) {
				LeftSub2Obj.gameObject.SetActive( show );
			}

			if ( RightSub2Obj ) {
				RightSub2Obj.gameObject.SetActive( show );
			}

			if ( AddedUnitWeapon ) {
				if ( show ) {
					AddedUnitWeapon.Activate();
				}
				else {
					AddedUnitWeapon.Deactivate();
				}
			}

			ChangeName( show ? changeName : OriginalName );
		}

		public void ChangeName( string name ) {
			if ( string.IsNullOrEmpty( name ) ) {
				return;
			}

			if ( LeftObj ) {
				LeftObj.gameObject.name = name;
			}

			if ( RightObj ) {
				RightObj.gameObject.name = name;
			}

			if ( LeftSubObj ) {
				LeftSubObj.gameObject.name = name;
			}

			if ( RightSubObj ) {
				RightSubObj.gameObject.name = name;
			}

			if ( LeftSub2Obj ) {
				LeftSub2Obj.gameObject.name = name;
			}

			if ( RightSub2Obj ) {
				RightSub2Obj.gameObject.name = name;
			}

			if ( AddedUnitWeapon ) {
				AddedUnitWeapon.gameObject.name = name;
			}
		}

		public void ShowEffect( bool show ) {
            if ( LeftObj && !LeftObj.AlwaysHideEffect ) {
                LeftObj.ActiveEffect( show );
            }

            if ( RightObj && !RightObj.AlwaysHideEffect ) {
                RightObj.ActiveEffect( show );
            }

            if ( LeftSubObj && !LeftSubObj.AlwaysHideEffect ) {
                LeftSubObj.ActiveEffect( show );
            }

            if ( RightSubObj && !RightSubObj.AlwaysHideEffect ) {
                RightSubObj.ActiveEffect( show );
            }

            if ( LeftSub2Obj && !LeftSub2Obj.AlwaysHideEffect ) {
                LeftSub2Obj.ActiveEffect( show );
            }

            if ( RightSub2Obj && !RightSub2Obj.AlwaysHideEffect ) {
                RightSub2Obj.ActiveEffect( show );
            }
        }
    }


    public GameTable.Costume.Param Param { get; private set; } = null;

    public CharData charData { get { return m_charData; } }
    public CostumeBody CostumeBody { get { return _costumebody; } }

    private List<AttachObject> _attachobj = new List<AttachObject>();
    private CharData m_charData;
    private CostumeBody _costumebody;

    private sWeaponItem[] mWeaponItem = new sWeaponItem[(int)eCOUNT.WEAPONSLOT];
    

    public void InitCostumeChar(CharData chardata, bool bweaponshow)    //캐릭터 최초 생성시
    {
        if (chardata == null)
            return;

        m_charData = chardata;

        if (chardata.EquipCostumeID == (int)eCOUNT.NONE)
            return;

        Param = GameInfo.Instance.GameTable.FindCostume(x => x.ID == chardata.EquipCostumeID);
        if (Param == null)
            return;

        if (Param.Model == "" || Param.Model == string.Empty)
            return;

        AniEvent fbxmodel = this.gameObject.GetComponentInChildren<AniEvent>();
        if (fbxmodel == null)
            return;

        _costumebody = LoadModel(Param.Model);
        DestroyImmediate(fbxmodel.gameObject);
    }

    public void InitCostumeChar(int tableid, bool bweaponshow)    //캐릭터 최초 생성시
    {
        if (tableid == (int)eCOUNT.NONE)
            return;

        Param = GameInfo.Instance.GameTable.FindCostume(x => x.ID == tableid);
        if (Param == null)
            return;

        if (Param.Model == "" || Param.Model == string.Empty)
            return;

        AniEvent fbxmodel = this.gameObject.GetComponentInChildren<AniEvent>();
        if (fbxmodel == null)
            return;

        _costumebody = LoadModel(Param.Model);
        DestroyImmediate(fbxmodel.gameObject);
    }
     
    public void SetCostume(CharData chardata, bool bweaponshow)
    {
        if (chardata == null)
            return;

        SetWeaponAttach(chardata, bweaponshow);
        SetCostumeBody(chardata);
    }

    public void SetCostume(CharData charData)
    {
        if(charData == null)
        {
            return;
        }

        SetCostumeBody(charData);

        mWeaponItem[(int)eWeaponSlot.MAIN] = CreateWeaponItemOrNull(charData, charData.EquipWeaponUID, eWeaponSlot.MAIN);
        mWeaponItem[(int)eWeaponSlot.SUB] = CreateWeaponItemOrNull(charData, charData.EquipWeapon2UID, eWeaponSlot.SUB);

        ShowWeaponByWeaponUID(charData.EquipWeaponUID);
    }

    public void SetCostume(CharData charData, WeaponData mainWeaponData, WeaponData subWeaponData)
    {
        if (charData == null)
        {
            return;
        }

        SetCostumeBody(charData);

        mWeaponItem[(int)eWeaponSlot.MAIN] = CreateWeaponItemOrNull(charData, mainWeaponData, true);
        mWeaponItem[(int)eWeaponSlot.SUB] = CreateWeaponItemOrNull(charData, subWeaponData, false);

        ShowWeaponByIndex(0);
    }

    public sWeaponItem GetWeaponItemOrNull(int index)
    {
        if(index < 0 || index > mWeaponItem.Length)
        {
            return null;
        }

        return mWeaponItem[index];
    }

    public void ShowWeaponByWeaponUID(long weaponUID)
    {
        HideWeapon();

		if ( m_charData.IsHideWeapon ) {
			return;
		}

		for (int i = 0; i < (int)eCOUNT.WEAPONSLOT; i++)
        {
            if (mWeaponItem[i] == null)
            {
                continue;
            }

            if (mWeaponItem[i].UID == weaponUID)
            {
                mWeaponItem[i].Show(true);
                return;
            }
        }
    }

    public void ShowWeaponByIndex(int index, string changeWeaponName = "")
    {
        HideWeapon();

        if ( m_charData.IsHideWeapon ) {
            return;
        }

        for (int i = 0; i < (int)eCOUNT.WEAPONSLOT; i++)
        {
            if (mWeaponItem[i] == null)
            {
                continue;
            }

            if (i == index)
            {
                mWeaponItem[i].Show(true, changeWeaponName);
                return;
            }
        }
    }

    public void ChangeAllWeaponName(string name)
    {
        if(string.IsNullOrEmpty(name))
        {
            return;
        }

        for (int i = 0; i < (int)eCOUNT.WEAPONSLOT; i++)
        {
            if (mWeaponItem[i] == null)
            {
                continue;
            }

            mWeaponItem[i].ChangeName(name);
        }
    }

    public void HideWeapon()
    {
        for (int i = 0; i < (int)eCOUNT.WEAPONSLOT; i++)
        {
            if (mWeaponItem[i] == null)
            {
                continue;
            }

            mWeaponItem[i].Show(false);
        }
    }

    public void ShowObject(string objName, bool show)
    {
        if (string.IsNullOrEmpty(objName))
        {
            return;
        }

        SkinnedMeshRenderer[] renderers = _costumebody.GetComponentsInChildren<SkinnedMeshRenderer>(true);
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i].name.CompareTo(objName) == 0)
            {
                renderers[i].gameObject.SetActive(show);
                return;
            }
        }
    }

    public void SetCostumeBody(int costumeid, int costumecolor, int costumestateflag, DyeingData dyeingData)
    {
        _costumebody.SetCostumeBody(costumeid, costumecolor, costumestateflag, dyeingData);
    }

    public void SetWeaponAttach(CharData chardata, bool bweaponshow )
    {
        WeaponData weapondata = GameInfo.Instance.GetWeaponData(chardata.EquipWeaponUID);
        if (weapondata != null)
        {
            RemoveAttachItem(AttachObject.eTYPE.WEAPON_R);
            RemoveAttachItem(AttachObject.eTYPE.WEAPON_L);

            string modelr = weapondata.TableData.ModelR;
            string modell = weapondata.TableData.ModelL;
            string subModelR = weapondata.TableData.SubModelR;
            string subModelL = weapondata.TableData.SubModelL;
            string sub2ModelR = weapondata.TableData.Sub2ModelR;
            string sub2ModelL = weapondata.TableData.Sub2ModelL;

            if (chardata.EquipWeaponSkinTID != (int)eCOUNT.NONE)
            {
                var tabledata = GameInfo.Instance.GameTable.FindWeapon(chardata.EquipWeaponSkinTID);
                if (tabledata != null)
                {
                    modelr = tabledata.ModelR;
                    modell = tabledata.ModelL;
                    subModelR = tabledata.SubModelR;
                    subModelL = tabledata.SubModelL;
                    sub2ModelR = tabledata.Sub2ModelR;
                    sub2ModelL = tabledata.Sub2ModelL;
                }
            }

            if (modelr != "")
                AttachItem(modelr, GameSupport.IsWeaponOpenTerms_Effect(weapondata), true);
            if (modell != "")
                AttachItem(modell, GameSupport.IsWeaponOpenTerms_Effect(weapondata), true);

            if(!string.IsNullOrEmpty(subModelR))
            {
                AttachItem(subModelR, GameSupport.IsWeaponOpenTerms_Effect(weapondata), true);
            }

            if (!string.IsNullOrEmpty(subModelL))
            {
                AttachItem(subModelL, GameSupport.IsWeaponOpenTerms_Effect(weapondata), true);
            }

            if (!string.IsNullOrEmpty(sub2ModelR))
            {
                AttachItem(sub2ModelR, GameSupport.IsWeaponOpenTerms_Effect(weapondata), true);
            }

            if (!string.IsNullOrEmpty(sub2ModelL))
            {
                AttachItem(sub2ModelL, GameSupport.IsWeaponOpenTerms_Effect(weapondata), true);
            }
        }

        for (int i = 0; i < _attachobj.Count; i++)
            _attachobj[i].gameObject.SetActive(bweaponshow);
    }

    public void SetWeaponAttachTableData(int weapontableid, bool wake, bool bweaponshow)
    {
        var weapondata = GameInfo.Instance.GameTable.FindWeapon(weapontableid);
        if (weapondata == null)
            return;

        RemoveAttachItem(AttachObject.eTYPE.WEAPON_R);
        RemoveAttachItem(AttachObject.eTYPE.WEAPON_L);

        string modelr = weapondata.ModelR;
        string modell = weapondata.ModelL;
        string subModelR = weapondata.SubModelR;
        string subModelL = weapondata.SubModelL;
        string sub2ModelR = weapondata.Sub2ModelR;
        string sub2ModelL = weapondata.Sub2ModelL;

        if (modelr != "")
            AttachItem(modelr, wake, true);
        if (modell != "")
            AttachItem(modell, wake, true);

        if(!string.IsNullOrEmpty(subModelR))
        {
            AttachItem(subModelR, wake, true);
        }

        if (!string.IsNullOrEmpty(subModelL))
        {
            AttachItem(subModelL, wake, true);
        }

        if (!string.IsNullOrEmpty(sub2ModelR))
        {
            AttachItem(sub2ModelR, wake, true);
        }

        if (!string.IsNullOrEmpty(sub2ModelL))
        {
            AttachItem(sub2ModelL, wake, true);
        }

        for (int i = 0; i < _attachobj.Count; i++)
            _attachobj[i].gameObject.SetActive(bweaponshow);
    }


    private void SetCostumeBody(CharData chardata)
    {
        _costumebody.InitCostumeBody(chardata);
    }
    
    private CostumeBody LoadModel(string strmodel)
    {
        GameObject attachres = null;
        if (AppMgr.Instance.ResPlatform == AppMgr.eResPlatform.aos)
            attachres = ResourceMgr.Instance.LoadFromAssetBundle("item", "Item/" + strmodel + "_aos.prefab") as GameObject;
        else
            attachres = ResourceMgr.Instance.LoadFromAssetBundle("item", "Item/" + strmodel + ".prefab") as GameObject;


        if (attachres == null)
            return null;

        Unit unit = this.gameObject.GetComponent<Unit>();

        GameObject GOAttach = GameObject.Instantiate(attachres, Vector3.zero, Quaternion.identity, this.gameObject.transform );

        return GOAttach.GetComponent<CostumeBody>();
    }

    public AttachObject AttachItem(string strmodel, bool bmaxwake, bool skipRemoveAttachItem)
    {
        GameObject attachres = ResourceMgr.Instance.LoadFromAssetBundle("item", "Item/" + strmodel + ".prefab") as GameObject;
        if (attachres == null)
            return null;

        Unit unit = this.gameObject.GetComponent<Unit>();

        GameObject GOAttach = GameObject.Instantiate(attachres);

        AttachObject attachobj = GOAttach.GetComponent<AttachObject>();

        if (!skipRemoveAttachItem)
        {
            RemoveAttachItem(attachobj.kSlotType);
        }

        Transform tf = unit.aniEvent.GetBoneByName(attachobj.kBoneName);

        if (attachobj.kInitTrans)
        {
            GOAttach.transform.parent = tf;

            GOAttach.transform.localPosition = Vector3.zero;
            GOAttach.transform.localRotation = Quaternion.identity;
            GOAttach.transform.localScale = Vector3.one;
        }
        else
        {
            GOAttach.transform.localPosition = Vector3.zero;
            GOAttach.transform.localRotation = Quaternion.identity;
            GOAttach.transform.localScale = Vector3.one;

            GOAttach.transform.parent = tf;

            GOAttach.transform.localPosition = attachres.transform.localPosition;
            GOAttach.transform.localRotation = attachres.transform.localRotation;
        }

        if (attachobj.kEffectList != null)
        {
            if (attachobj.kEffectList.Count != 0)
            {
                for (int i = 0; i < attachobj.kEffectList.Count; i++)
                {
                    if (attachobj.kEffectList[i] == null)
                    {
                        continue;
                    }

                    attachobj.kEffectList[i].SetActive(bmaxwake);
                }
            }
        }
        _attachobj.Add(attachobj);

        return attachobj;
    }

    public void RemoveAttachItem(AttachObject.eTYPE slottype)
    {
        for (int i = 0; i < _attachobj.Count; i++)
        {
            if (_attachobj[i].kSlotType == slottype)
            {
                GameObject destoygo = _attachobj[i].gameObject;
                _attachobj.Remove(_attachobj[i]);
                DestroyImmediate(destoygo);
                i--;                                //제거한뒤 인덱스 조정
            }
        }
    }

    public void ChangeAttachWeaponName(string name)
    {
        for(int i = 0; i < _attachobj.Count; ++i)
        {
            AttachObject attachObj = _attachobj[i];
            if (attachObj == null || (attachObj.kSlotType != AttachObject.eTYPE.WEAPON_R && attachObj.kSlotType != AttachObject.eTYPE.WEAPON_L))
            {
                continue;
            }

            attachObj.name = name;
        }
    }

    void AddTagMesh(ref List<SkinnedMeshRenderer> meshlist, string tag)
    {
        SkinnedMeshRenderer[] childs = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();

        foreach (SkinnedMeshRenderer o in childs)
        {
            if (o.tag == tag)
                meshlist.Add(o);
        }
    }

    public Transform FindChildByName(string name, Transform ThisGObj)
    {
        Transform t;

        if (ThisGObj.name == name)
            return ThisGObj.transform;

        foreach (Transform child in ThisGObj)
        {
            t = FindChildByName(name, child);
            if (t)
                return t;
        }

        return null;
    }

    public AttachObject GetAttachObject(AttachObject.eTYPE type)
    {
        AttachObject find = _attachobj.Find(x => x.kSlotType == type);
        return find;
    }
    
    public void ShowAttachedObject(bool show)
    {
        for (int i = 0; i < _attachobj.Count; i++)
        {
            if(_attachobj[i] == null)
            {
                continue;
            }

            _attachobj[i].gameObject.SetActive(show);
        }
    }

	public void SetGuardianOwnerPlayer( Player ownerPlayer, bool isDeactivate ) {
		for ( int i = 0; i < mWeaponItem.Length; i++ ) {
			if ( mWeaponItem[i] == null ) {
				continue;
			}

			if ( mWeaponItem[i].AddedUnitWeapon == null ) {
				continue;
			}

			PlayerGuardian playerGuardian = mWeaponItem[i].AddedUnitWeapon as PlayerGuardian;
			if ( playerGuardian == null ) {
				continue;
			}

			playerGuardian.SetOwnerPlayer( ownerPlayer );

			if ( isDeactivate ) {
				playerGuardian.Deactivate();
			}
		}
	}

    public void SetGuardianOwnerPlayer( int curWeaponIndex, Player ownerPlayer ) {
		PlayerGuardian playerGuardian = mWeaponItem[curWeaponIndex].AddedUnitWeapon as PlayerGuardian;
		if ( playerGuardian == null ) {
			return;
		}

		ownerPlayer.SetGuardian( playerGuardian );
	}

    private AttachObject LoadWeaponOrNull(string path, bool isWeaponMaxWake)
    {
        AttachObject attachObj = ResourceMgr.Instance.Instantiate<AttachObject>("item", "Item/" + path + ".prefab", true);
        if(attachObj == null)
        {
            Debug.LogError(path + "무기가 없습니다.");
            return null;
        }

        BoxCollider boxCol = attachObj.GetComponent<BoxCollider>();
        if(boxCol)
        {
            DestroyImmediate(boxCol);
        }

        Unit unit = gameObject.GetComponent<Unit>();
        Transform tf = unit.aniEvent.GetBoneByName(attachObj.kBoneName);

        if (attachObj.kInitTrans)
        {
            attachObj.transform.parent = tf;

            attachObj.transform.localPosition = Vector3.zero;
            attachObj.transform.localRotation = Quaternion.identity;
            attachObj.transform.localScale = Vector3.one;
        }
        else
        {
            attachObj.transform.localPosition = Vector3.zero;
            attachObj.transform.localRotation = Quaternion.identity;
            attachObj.transform.localScale = Vector3.one;

            attachObj.transform.parent = tf;

            attachObj.transform.localPosition = attachObj.transform.localPosition;
            attachObj.transform.localRotation = attachObj.transform.localRotation;
        }

        if (attachObj.kEffectList != null)
        {
            if (attachObj.kEffectList.Count != 0)
            {
                for (int i = 0; i < attachObj.kEffectList.Count; i++)
                {
                    if (attachObj.kEffectList[i] == null)
                    {
                        continue;
                    }

                    attachObj.AlwaysHideEffect = !isWeaponMaxWake;
                    attachObj.kEffectList[i].SetActive(isWeaponMaxWake);
                }
            }
        }

        return attachObj;
    }

    private sWeaponItem CreateWeaponItemOrNull(CharData charData, long weaponUID, eWeaponSlot wpnSlot)
    {
        WeaponData data = GameInfo.Instance.GetWeaponData(weaponUID);
        if (data != null)
        {
            string pathRight = data.TableData.ModelR;
            string pathLeft = data.TableData.ModelL;
            string pathSubRight = data.TableData.SubModelR;
            string pathSubLeft = data.TableData.SubModelL;
            string pathSub2Right = data.TableData.Sub2ModelR;
            string pathSub2Left = data.TableData.Sub2ModelL;
            string pathAddWeapon = data.TableData.AddedUnitWeapon;

            if (wpnSlot == eWeaponSlot.MAIN && charData.EquipWeaponSkinTID != (int)eCOUNT.NONE)
            {
                var tabledata = GameInfo.Instance.GameTable.FindWeapon(charData.EquipWeaponSkinTID);
                if (tabledata != null)
                {
                    pathRight = tabledata.ModelR;
                    pathLeft = tabledata.ModelL;
                    pathSubRight = tabledata.SubModelR;
                    pathSubLeft = tabledata.SubModelL;
                    pathSub2Right = tabledata.Sub2ModelR;
                    pathSub2Left = tabledata.Sub2ModelL;
                    pathAddWeapon = tabledata.AddedUnitWeapon;
                }
            }
            else if (wpnSlot == eWeaponSlot.SUB && charData.EquipWeapon2SkinTID != (int)eCOUNT.NONE)
            {
                var tabledata = GameInfo.Instance.GameTable.FindWeapon(charData.EquipWeapon2SkinTID);
                if (tabledata != null)
                {
                    pathRight = tabledata.ModelR;
                    pathLeft = tabledata.ModelL;
                    pathSubRight = tabledata.SubModelR;
                    pathSubLeft = tabledata.SubModelL;
                    pathSub2Right = tabledata.Sub2ModelR;
                    pathSub2Left = tabledata.Sub2ModelL;
                    pathAddWeapon = tabledata.AddedUnitWeapon;
                }
            }

            sWeaponItem item = new sWeaponItem();
            item.UID = data.WeaponUID;

            if (!string.IsNullOrEmpty(pathLeft))
            {
                item.LeftObj = LoadWeaponOrNull(pathLeft, GameSupport.IsWeaponOpenTerms_Effect(data));
                item.OriginalName = item.LeftObj.name;
            }

            if (!string.IsNullOrEmpty(pathRight))
            {
                item.RightObj = LoadWeaponOrNull(pathRight, GameSupport.IsWeaponOpenTerms_Effect(data));
                item.OriginalName = item.RightObj.name;
            }

            if (!string.IsNullOrEmpty(pathSubLeft))
            {
                item.LeftSubObj = LoadWeaponOrNull(pathSubLeft, GameSupport.IsWeaponOpenTerms_Effect(data));
                item.OriginalName = item.LeftSubObj.name;
            }

            if (!string.IsNullOrEmpty(pathSubRight))
            {
                item.RightSubObj = LoadWeaponOrNull(pathSubRight, GameSupport.IsWeaponOpenTerms_Effect(data));
                item.OriginalName = item.RightSubObj.name;
            }

            if (!string.IsNullOrEmpty(pathSub2Left))
            {
                item.LeftSub2Obj = LoadWeaponOrNull(pathSub2Left, GameSupport.IsWeaponOpenTerms_Effect(data));
                item.OriginalName = item.LeftSub2Obj.name;
            }

            if (!string.IsNullOrEmpty(pathSub2Right))
            {
                item.RightSub2Obj = LoadWeaponOrNull(pathSub2Right, GameSupport.IsWeaponOpenTerms_Effect(data));
                item.OriginalName = item.RightSub2Obj.name;
            }

            if (!string.IsNullOrEmpty(pathAddWeapon))
            {
                item.AddedUnitWeapon = GameSupport.CreateUnitWeapon(pathAddWeapon);
                item.OriginalName = item.AddedUnitWeapon.name;

                PlayerGuardian playerGuardian = item.AddedUnitWeapon as PlayerGuardian;
				if ( playerGuardian ) {
					playerGuardian.ShowMaxEffect( GameSupport.IsWeaponOpenTerms_Effect( data ) );
				}
			}

            return item;
        }
         
        return null;
    }

    private sWeaponItem CreateWeaponItemOrNull(CharData charData, WeaponData weaponData, bool isMainWeapon)
    {
        WeaponData data = weaponData;
        if (data != null)
        {
            string pathRight = data.TableData.ModelR;
            string pathLeft = data.TableData.ModelL;
            string pathSubRight = data.TableData.SubModelR;
            string pathSubLeft = data.TableData.SubModelL;
            string pathSub2Right = data.TableData.Sub2ModelR;
            string pathSub2Left = data.TableData.Sub2ModelL;
            string pathAddWeapon = data.TableData.AddedUnitWeapon;

            if (isMainWeapon && charData.EquipWeaponSkinTID != (int)eCOUNT.NONE)
            {
                var tabledata = GameInfo.Instance.GameTable.FindWeapon(charData.EquipWeaponSkinTID);
                if (tabledata != null)
                {
                    pathRight = tabledata.ModelR;
                    pathLeft = tabledata.ModelL;
                    pathSubRight = tabledata.SubModelR;
                    pathSubLeft = tabledata.SubModelL;
                    pathSub2Right = tabledata.Sub2ModelR;
                    pathSub2Left = tabledata.Sub2ModelL;
                    pathAddWeapon = tabledata.AddedUnitWeapon;
                }
            }
            else if (!isMainWeapon && charData.EquipWeapon2SkinTID != (int)eCOUNT.NONE)
            {
                var tabledata = GameInfo.Instance.GameTable.FindWeapon(charData.EquipWeapon2SkinTID);
                if (tabledata != null)
                {
                    pathRight = tabledata.ModelR;
                    pathLeft = tabledata.ModelL;
                    pathSubRight = tabledata.SubModelR;
                    pathSubLeft = tabledata.SubModelL;
                    pathSub2Right = tabledata.Sub2ModelR;
                    pathSub2Left = tabledata.Sub2ModelL;
                    pathAddWeapon = tabledata.AddedUnitWeapon;
                }
            }

            sWeaponItem item = new sWeaponItem();
            item.UID = data.WeaponUID;
            item.Data = data;

            if (!string.IsNullOrEmpty(pathLeft))
            {
                item.LeftObj = LoadWeaponOrNull(pathLeft, GameSupport.IsWeaponOpenTerms_Effect(data));
            }

            if (!string.IsNullOrEmpty(pathRight))
            {
                item.RightObj = LoadWeaponOrNull(pathRight, GameSupport.IsWeaponOpenTerms_Effect(data));
            }

            if (!string.IsNullOrEmpty(pathSubLeft))
            {
                item.LeftSubObj = LoadWeaponOrNull(pathSubLeft, GameSupport.IsWeaponOpenTerms_Effect(data));
            }

            if (!string.IsNullOrEmpty(pathSubRight))
            {
                item.RightSubObj = LoadWeaponOrNull(pathSubRight, GameSupport.IsWeaponOpenTerms_Effect(data));
            }

            if (!string.IsNullOrEmpty(pathSub2Left))
            {
                item.LeftSub2Obj = LoadWeaponOrNull(pathSub2Left, GameSupport.IsWeaponOpenTerms_Effect(data));
            }

            if (!string.IsNullOrEmpty(pathSub2Right))
            {
                item.RightSub2Obj = LoadWeaponOrNull(pathSub2Right, GameSupport.IsWeaponOpenTerms_Effect(data));
            }

            if (!string.IsNullOrEmpty(pathAddWeapon))
            {
                item.AddedUnitWeapon = GameSupport.CreateUnitWeapon(pathAddWeapon);
            }

            return item;
        }

        return null;
    }

    /*
    private List<SkinnedMeshRenderer> _facemesh = new List<SkinnedMeshRenderer>();
    private List<SkinnedMeshRenderer> _hairmesh = new List<SkinnedMeshRenderer>();
    private List<SkinnedMeshRenderer> _bodymesh = new List<SkinnedMeshRenderer>();
    private List<SkinnedMeshRenderer> _weaponmesh = new List<SkinnedMeshRenderer>();
    private List<AttachObject> _attachobj = new List<AttachObject>();
    private CharData m_charData;
    public GameObject kBone;
    public List<Material> kMatFaceList;
    public List<Material> kMatSkinList;

    public CharData charData { get { return m_charData; } }

    public void InitCostumeChar(CharData chardata, bool blobby)    //캐릭터 최초 생성시
    {
        if (chardata == null)
            return;


        m_charData = chardata;

        if (chardata.EquipCostumeID == (int)eCOUNT.NONE)
            return;

        AddTagMesh(ref _facemesh, "CharFace");
        AddTagMesh(ref _hairmesh, "CharHair");
        AddTagMesh(ref _bodymesh, "CharBody");
        AddTagMesh(ref _bodymesh, "CharSkin");

        int color = 0;


        GameTable.Costume.Param tabledata = GameInfo.Instance.GameTable.FindCostume(x => x.ID == chardata.EquipCostumeID);
        if (tabledata == null)
            return;

        if( tabledata.Model == "" || tabledata.Model == string.Empty )
            return;


        RemoveEquipMesh(ref _bodymesh);
        EquipMesh(tabledata.Model);


        AttachObject[] attachchilds = gameObject.GetComponentsInChildren<AttachObject>();

        foreach (AttachObject o in attachchilds)
        {
            _attachobj.Add(o);
        }

        if (!blobby)
        {
            WeaponData weapondata = GameInfo.Instance.GetWeaponData(chardata.EquipWeaponUID);
            if (weapondata != null)
            {
                RemoveAttachItem(AttachObject.eTYPE.WEAPON_R);
                RemoveAttachItem(AttachObject.eTYPE.WEAPON_L);
                if (weapondata.TableData.ModelR != "")
                    AttachItem(weapondata.TableData.ModelR);
                if (weapondata.TableData.ModelL != "")
                    AttachItem(weapondata.TableData.ModelL);
            }
        }

        if (blobby)
        {
            for (int i = 0; i < _attachobj.Count; i++)
            {
                //_attachobj[i].gameObject.SetActive(false);
                _attachobj[i].gameObject.SetActive(true);
            }
        }


        //for (int i = 0; i < chardata.EquipCostume.Length; i++)
        //{
        //var costumdata = GameInfo.Instance.CostumeList.Find(x => x.UID == chardata.EquipCostume[i]);
        //if (costumdata == null)
        //continue;

        //GameTable.Costume.Param tabledata = GameInfo.Instance.GameTable.FindCostume(x => x.ID == costumdata.TableID);
        //if (tabledata == null)
        //continue;

        //if (tabledata.Category == (int)AttachObject.eTYPE.COSTUME)
        //{
        //color = tabledata.Color;
        //RemoveEquipMesh(ref _bodymesh);
        //EquipMesh(tabledata.Model);
        //}
        //else if (tabledata.Category == (int)AttachObject.eTYPE.HAIR)
        //{
        //RemoveAttachItem(AttachObject.eTYPE.HAIR);
        //AttachItem(tabledata.Model);
        //}
        //else if (tabledata.Category == (int)AttachObject.eTYPE.FACE)
        //{
        //RemoveAttachItem(AttachObject.eTYPE.FACE);
        //AttachItem(tabledata.Model);
        //}
        //else if (tabledata.Category == (int)AttachObject.eTYPE.BACK)
        //{
        //RemoveAttachItem(AttachObject.eTYPE.BACK);
        //AttachItem(tabledata.Model);
        //}

        //else if (tabledata.Category == (int)AttachObject.eTYPE.WEAPON_R)
        //{
        //    RemoveAttachItem(AttachObject.eTYPE.WEAPON_R);
        //    RemoveAttachItem(AttachObject.eTYPE.WEAPON_L);
        //    if (!blobby)
        //    {
        //        AttachItem(tabledata.Model);
        //    }
        //}

        //}


        //var weaponskin = GameInfo.Instance.GameTable.FindCharacterWeaponSkin(x => x.CharacterID == chardata.TableID && x.Index == chardata.WeaponSkin);
        //if (weaponskin != null)
        //{
        //    RemoveAttachItem(AttachObject.eTYPE.WEAPON_R);
        //    RemoveAttachItem(AttachObject.eTYPE.WEAPON_L);
        //    if (!blobby)
        //    {
        //        AttachItem(weaponskin.Model);
        //    }
        // }


        //스킨교체
        //color = 0;
        //for ( int i = 0; i < _facemesh.Count; i++ )
        //{
        //    if (_facemesh[i].tag != "CharFace")
        //        continue;
        //
        //        Material[] newMats = new Material[_facemesh[i].materials.Length];
        //    for(int j = 0; j < newMats.Length; j++)
        //        newMats[j] = kMatFaceList[color];
        //
        //    _facemesh[i].materials = newMats;
        //}
        //
        //
        //for (int i = 0; i < _bodymesh.Count; i++)
        //{
        //    if (_bodymesh[i].tag != "CharSkin")
        //        continue;
        //
        //    Material[] newMats = new Material[_bodymesh[i].materials.Length];
        //    for (int j = 0; j < newMats.Length; j++)
        //        newMats[j] = kMatFaceList[color];
        //
        //    _bodymesh[i].materials = newMats;
        //}
        //
        //
        //헤어교체
        //RemoveEquipMesh(ref _hairmesh);
        //EquipMesh("Asagi/prf_asagi_hair_B.prefab");
    }
    */
    /*
    public void InitCostumeChar(CharData chardata, bool blobby )    //캐릭터 최초 생성시
    {
        if (chardata == null)
            return;

        m_charData = chardata;

        AddTagMesh(ref _facemesh, "CharFace");
        AddTagMesh(ref _hairmesh, "CharHair");
        AddTagMesh(ref _bodymesh, "CharBody");
        AddTagMesh(ref _bodymesh, "CharSkin");


        int color = 0;


        for (int i = 0; i < chardata.EquipCostume.Length; i++)
        {
            var costumdata = GameInfo.Instance.CostumeList.Find(x => x.UID == chardata.EquipCostume[i]);
            if (costumdata == null)
                continue;

            GameTable.Costume.Param tabledata = GameInfo.Instance.GameTable.FindCostume(x => x.ID == costumdata.TableID);
            if (tabledata == null)
                continue;

            if ( tabledata.Category == (int)AttachObject.eTYPE.COSTUME )
            {
                color = tabledata.Color;
                RemoveEquipMesh(ref _bodymesh);
                EquipMesh(tabledata.Model);
            }
            else if (tabledata.Category == (int)AttachObject.eTYPE.HAIR)
            {
                RemoveAttachItem(AttachObject.eTYPE.HAIR);
                AttachItem(tabledata.Model);
            }
            else if (tabledata.Category == (int)AttachObject.eTYPE.FACE)
            {
                RemoveAttachItem(AttachObject.eTYPE.FACE);
                AttachItem(tabledata.Model);
            }
            else if (tabledata.Category == (int)AttachObject.eTYPE.BACK)
            {
                RemoveAttachItem(AttachObject.eTYPE.BACK);
                AttachItem(tabledata.Model);
            }
            /*
            //else if (tabledata.Category == (int)AttachObject.eTYPE.WEAPON_R)
            //{
            //    RemoveAttachItem(AttachObject.eTYPE.WEAPON_R);
            //    RemoveAttachItem(AttachObject.eTYPE.WEAPON_L);
            //    if (!blobby)
            //    {
            //        AttachItem(tabledata.Model);
            //    }
            //}

        }

        /*
        //var weaponskin = GameInfo.Instance.GameTable.FindCharacterWeaponSkin(x => x.CharacterID == chardata.TableID && x.Index == chardata.WeaponSkin);
        //if (weaponskin != null)
        //{
        //    RemoveAttachItem(AttachObject.eTYPE.WEAPON_R);
        //    RemoveAttachItem(AttachObject.eTYPE.WEAPON_L);
        //    if (!blobby)
        //    {
        //        AttachItem(weaponskin.Model);
        //    }
        // }


        //스킨교체
        //color = 0;
        //for ( int i = 0; i < _facemesh.Count; i++ )
        //{
        //    if (_facemesh[i].tag != "CharFace")
        //        continue;
        //
        //        Material[] newMats = new Material[_facemesh[i].materials.Length];
        //    for(int j = 0; j < newMats.Length; j++)
        //        newMats[j] = kMatFaceList[color];
        //
        //    _facemesh[i].materials = newMats;
        //}
        //
        //
        //for (int i = 0; i < _bodymesh.Count; i++)
        //{
        //    if (_bodymesh[i].tag != "CharSkin")
        //        continue;
        //
        //    Material[] newMats = new Material[_bodymesh[i].materials.Length];
        //    for (int j = 0; j < newMats.Length; j++)
        //        newMats[j] = kMatFaceList[color];
        //
        //    _bodymesh[i].materials = newMats;
        //}
        //
        //
        //헤어교체
        //RemoveEquipMesh(ref _hairmesh);
        //EquipMesh("Asagi/prf_asagi_hair_B.prefab");

    }
    */
    /*
    public void EquipMesh(string strcharprefab)
    {
        GameObject res = ResourceMgr.Instance.LoadFromAssetBundle("item", "Item/" + strcharprefab + ".prefab") as GameObject;
        
        SkinnedMeshRenderer[] childs = res.GetComponentsInChildren<SkinnedMeshRenderer>();

        foreach (SkinnedMeshRenderer o in childs)
        {
            GameObject newObj = new GameObject(o.name);
            if (tag != null)
                newObj.tag = o.tag;

            newObj.transform.parent = kBone.transform.parent;
            newObj.layer = LayerMask.NameToLayer("Player");

            newObj.transform.localPosition = o.transform.localPosition;
            newObj.transform.localRotation = o.transform.localRotation;
            newObj.transform.localScale = o.transform.localScale;

            SkinnedMeshRenderer NewRenderer = newObj.AddComponent(typeof(SkinnedMeshRenderer)) as SkinnedMeshRenderer;
            
            Transform[] MyBones = new Transform[o.bones.Length];

            for (int i = 0; i < o.bones.Length; i++)
            {
                MyBones[i] = FindChildByName(o.bones[i].name, kBone.transform);
            }
            
            
            NewRenderer.bones = MyBones;
            NewRenderer.sharedMesh = o.sharedMesh;
            NewRenderer.sharedMaterials = o.sharedMaterials;
            NewRenderer.rootBone = FindChildByName(o.rootBone.name, kBone.transform);

            if (newObj.tag == "CharFace")
                _facemesh.Add(NewRenderer);
            else if (newObj.tag == "CharHair")
                _hairmesh.Add(NewRenderer);
            else if (newObj.tag == "CharBody" || newObj.tag == "CharSkin")
                _bodymesh.Add(NewRenderer);
            else
            {
                Debug.Log("EquipMesh Tag Error");
            }
        }
    }
    */
    /*
    void RemoveEquipMesh(ref List<SkinnedMeshRenderer> meshlist)
    {
        for (int i = 0; i < meshlist.Count; i++)
            DestroyImmediate(meshlist[i].gameObject);

        meshlist.Clear();
    }
    */
}
