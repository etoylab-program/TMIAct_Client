using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderTargetWeapon : FMonoSingleton<RenderTargetWeapon>
{
    public Camera camDefault;
    public GameObject kWeaponPosistionObj;
    public List<PositionList> WeaponPositionList;

    private int _tableid = -1;
    private long _uid = -1;
    private int _type = -1;
    private float _defaultorthographicSize = 0.8f;
    private AttachObject _weaponObject_First;
    private AttachObject _weaponObject_Second;
    private Unit _addedWeaponObject;

    private ePlayerCharType _weaponOwner = ePlayerCharType.None;

    public void Awake()
    {
        _defaultorthographicSize = camDefault.orthographicSize;
    }
    public void InitRenderTargetWeapon(int tableid, long uid, bool bchangecostume)
    {
        int type = 0;
        if (bchangecostume != true)
        {
            if (_tableid == tableid && _uid == uid && _type == type)
                return;
        }

        _tableid = tableid;
        _uid = uid;
        _type = type;

        camDefault.orthographicSize = _defaultorthographicSize;

        if (_weaponObject_First != null)
            DestroyImmediate(_weaponObject_First.gameObject);

        if (_weaponObject_Second != null)
            DestroyImmediate(_weaponObject_Second.gameObject);

        if ( _addedWeaponObject != null ) {
            DestroyImmediate( _addedWeaponObject.gameObject );
        }


        WeaponData weapondata = GameInfo.Instance.GetWeaponData(uid);
        GameTable.Weapon.Param weapontabledata;

        if (weapondata != null)
            weapontabledata = weapondata.TableData;
        else
            weapontabledata = GameInfo.Instance.GameTable.FindWeapon(_tableid);

        _weaponOwner = (ePlayerCharType)Utility.GetWeaponCharId( weapontabledata.CharacterID, 0 );
        PositionList charpos = GetWeaponCharPosition(weapontabledata);
        if (charpos == null)
            return;
        PositionList typepos = charpos.GetPosition(type);
        if (typepos == null)
            return;

        camDefault.orthographicSize = typepos.kSize;

        bool bmaxwake = false;
        if (weapondata != null)
            bmaxwake = GameSupport.IsWeaponOpenTerms_Effect(weapondata);

        string strmodelR = "Item/" + weapontabledata.ModelR + ".prefab";
        string strmodelL = "Item/" + weapontabledata.ModelL + ".prefab";

        AttachObject attachobj_First = null;
        AttachObject attachobj_Second = null;

        if (weapontabledata.ModelR != "" || weapontabledata.ModelR != string.Empty)
        {
            attachobj_First = ResourceMgr.Instance.CreateFromAssetBundle<AttachObject>("item", strmodelR);
            if (attachobj_First == null)
                return;

            attachobj_First.transform.parent = typepos.GetGameObject(0).transform;
            attachobj_First.transform.localPosition = Vector3.zero;
            attachobj_First.transform.localRotation = Quaternion.identity;
            attachobj_First.transform.localScale = Vector3.one;
            attachobj_First.ActiveEffect(bmaxwake);
            Utility.ChangeLayersRecursively(attachobj_First.transform, "RenderTargetWeapon");
            _weaponObject_First = attachobj_First;

            if (weapontabledata.SubModelR != "" || weapontabledata.SubModelR != string.Empty)
            {
                Transform subModelParent = GameSupport.GetSubWeaponBoneOrNull(attachobj_First.transform, false);
                if (subModelParent != null)
                {
                    string strSubModelR = "Item/" + weapontabledata.SubModelR + ".prefab";
                    AttachObject attachObj_SubFirst = ResourceMgr.Instance.CreateFromAssetBundle<AttachObject>("item", strSubModelR);
                    if (attachObj_SubFirst != null)
                    {
                        attachObj_SubFirst.transform.parent = subModelParent;
                        attachObj_SubFirst.transform.localPosition = Vector3.zero;
                        attachObj_SubFirst.transform.localRotation = Quaternion.identity;
                        attachObj_SubFirst.transform.localScale = Vector3.one;
                        attachObj_SubFirst.ActiveEffect(bmaxwake);

                        Utility.ChangeLayersRecursively(attachObj_SubFirst.transform, "RenderTargetWeapon");
                    }
                }
            }

            if (weapontabledata.Sub2ModelR != "" || weapontabledata.Sub2ModelR != string.Empty)
            {
                Transform subModelParent = GameSupport.GetSubWeaponBoneOrNull(attachobj_First.transform, false, 2);
                if (subModelParent != null)
                {
                    string strSub2ModelR = "Item/" + weapontabledata.Sub2ModelR + ".prefab";
                    AttachObject attachObj_Sub2First = ResourceMgr.Instance.CreateFromAssetBundle<AttachObject>("item", strSub2ModelR);
                    if (attachObj_Sub2First != null)
                    {
                        attachObj_Sub2First.transform.parent = subModelParent;
                        attachObj_Sub2First.transform.localPosition = Vector3.zero;
                        attachObj_Sub2First.transform.localRotation = Quaternion.identity;
                        attachObj_Sub2First.transform.localScale = Vector3.one;
                        attachObj_Sub2First.ActiveEffect(bmaxwake);

                        Utility.ChangeLayersRecursively(attachObj_Sub2First.transform, "RenderTargetWeapon");
                    }
                }
            }
        }
        

        if (weapontabledata.ModelL != "" || weapontabledata.ModelL != string.Empty)
        {
            attachobj_Second = ResourceMgr.Instance.CreateFromAssetBundle<AttachObject>("item", strmodelL);
            if (attachobj_Second == null)
                return;


            attachobj_Second.transform.parent = typepos.GetGameObject(1).transform;
            attachobj_Second.transform.localPosition = Vector3.zero;
            attachobj_Second.transform.localRotation = Quaternion.identity;
            attachobj_Second.transform.localScale = Vector3.one;
            attachobj_Second.ActiveEffect(bmaxwake);
            Utility.ChangeLayersRecursively(attachobj_Second.transform, "RenderTargetWeapon");
            _weaponObject_Second = attachobj_Second;

            if (weapontabledata.SubModelL != "" || weapontabledata.SubModelL != string.Empty)
            {
                Transform subModelParent = GameSupport.GetSubWeaponBoneOrNull(attachobj_Second.transform, true);
                if (subModelParent != null)
                {
                    string strSubModelL = "Item/" + weapontabledata.SubModelL + ".prefab";
                    AttachObject attachObj_SubSecond = ResourceMgr.Instance.CreateFromAssetBundle<AttachObject>("item", strSubModelL);
                    if (attachObj_SubSecond != null)
                    {
                        attachObj_SubSecond.transform.parent = subModelParent;
                        attachObj_SubSecond.transform.localPosition = Vector3.zero;
                        attachObj_SubSecond.transform.localRotation = Quaternion.identity;
                        attachObj_SubSecond.transform.localScale = Vector3.one;
                        attachObj_SubSecond.ActiveEffect(bmaxwake);

                        Utility.ChangeLayersRecursively(attachObj_SubSecond.transform, "RenderTargetWeapon");
                    }
                }
            }

            if (weapontabledata.Sub2ModelL != "" || weapontabledata.Sub2ModelL != string.Empty)
            {
                Transform subModelParent = GameSupport.GetSubWeaponBoneOrNull(attachobj_Second.transform, true, 2);
                if (subModelParent != null)
                {
                    string strSub2ModelL = "Item/" + weapontabledata.Sub2ModelL + ".prefab";
                    AttachObject attachObj_Sub2Second = ResourceMgr.Instance.CreateFromAssetBundle<AttachObject>("item", strSub2ModelL);
                    if (attachObj_Sub2Second != null)
                    {
                        attachObj_Sub2Second.transform.parent = subModelParent;
                        attachObj_Sub2Second.transform.localPosition = Vector3.zero;
                        attachObj_Sub2Second.transform.localRotation = Quaternion.identity;
                        attachObj_Sub2Second.transform.localScale = Vector3.one;
                        attachObj_Sub2Second.ActiveEffect(bmaxwake);

                        Utility.ChangeLayersRecursively(attachObj_Sub2Second.transform, "RenderTargetWeapon");
                    }
                }
            }
        }


        Unit addWeaponObj = null;
        if(!string.IsNullOrEmpty(weapontabledata.AddedUnitWeapon) || weapontabledata.AddedUnitWeapon != "")
        {
            addWeaponObj = GameSupport.CreateUnitWeapon(weapontabledata.AddedUnitWeapon, false);
            if (addWeaponObj == null)
                return;

            addWeaponObj.transform.parent = typepos.GetGameObject(1).transform;
            addWeaponObj.transform.localPosition = Vector3.zero;
            addWeaponObj.transform.localRotation = Quaternion.identity;
            addWeaponObj.transform.localScale = Vector3.one;

            AttachObject attachObj = addWeaponObj.GetComponent<AttachObject>();
            if ( attachObj ) {
                attachObj.ActiveEffect( bmaxwake );
            }

            Utility.ChangeLayersRecursively(addWeaponObj.transform, "RenderTargetWeapon");
            _addedWeaponObject = addWeaponObj;
        }

        //ActiveDepthOnlyCamera(false);
    }

    //public void ActiveDepthOnlyCamera(bool active)
    //{
    //    camDepthOnly.gameObject.SetActive(active);
    //    camDefault.gameObject.SetActive(!active);
        
    //    if (active == true && _weaponOwner != ePlayerCharType.None)
    //    {
    //        switch (_weaponOwner)
    //        {
    //            case ePlayerCharType.Asagi:
    //                camDepthOnly.transform.localPosition = new Vector3(-0.5f, -0.2f, 0.5f);
    //                break;
    //            case ePlayerCharType.Sakura:
    //                camDepthOnly.transform.localPosition = new Vector3(-0.72f, 0.1f, 0.5f);
    //                break;
    //            case ePlayerCharType.Yukikaze:
    //                camDepthOnly.transform.localPosition = new Vector3(-0.7f, 0f, 0.5f);
    //                break;
    //            case ePlayerCharType.Num:
    //            case ePlayerCharType.None:
    //            default:
    //                Debug.Log("Do Not Know Owner");
    //                break;
    //        }
    //    }
    //}

    public void MoveType(int type)
    {
        GameTable.Weapon.Param weapontabledata = GameInfo.Instance.GameTable.FindWeapon(_tableid);

        PositionList charpos = GetWeaponCharPosition(weapontabledata);
        if (charpos == null)
            return;
        PositionList typepos = charpos.GetPosition(type);
        if (typepos == null)
            return;

        _type = type;
        if (_weaponObject_First != null)
        {
            _weaponObject_First.transform.parent = typepos.GetGameObject(0).transform;
            _weaponObject_First.transform.localPosition = Vector3.zero;
            _weaponObject_First.transform.localRotation = Quaternion.identity;
            _weaponObject_First.transform.localScale = Vector3.one;
        }
        if (_weaponObject_Second != null)
        {
            _weaponObject_Second.transform.parent = typepos.GetGameObject(1).transform;
            _weaponObject_Second.transform.localPosition = Vector3.zero;
            _weaponObject_Second.transform.localRotation = Quaternion.identity;
            _weaponObject_Second.transform.localScale = Vector3.one;
        }

        if(_addedWeaponObject != null)
        {
            _addedWeaponObject.transform.parent = typepos.GetGameObject(1).transform;
            _addedWeaponObject.transform.localPosition = Vector3.zero;
            _addedWeaponObject.transform.localRotation = Quaternion.identity;
            _addedWeaponObject.transform.localScale = Vector3.one;
        }
    }
    /// <summary>
    /// 무기 이펙트 활성화/비활성화
    /// </summary>
    public void ShowWeaponEffect(bool bshow)
    {
        if ( _addedWeaponObject ) {
            AttachObject attachObj = _addedWeaponObject.GetComponent<AttachObject>();
            if ( attachObj ) {
                attachObj.ActiveEffect( bshow );
            }
        }

        if (_weaponObject_First == null)
            return;

        _weaponObject_First.ActiveEffect(bshow);

        //  2자루의 무기
        if (_weaponObject_Second != null)
            _weaponObject_Second.ActiveEffect(bshow);
    }

    private PositionList GetWeaponCharPosition(GameTable.Weapon.Param weapontabledata)
    {
        string[] split = Utility.Split( weapontabledata.CharacterID, ',' );
        return WeaponPositionList.Find( x => x.kIndex == Utility.SafeIntParse( split[0] ) );
        //return WeaponPositionList.Find(x => x.kIndex == weapontabledata.CharacterID);
    }
}
