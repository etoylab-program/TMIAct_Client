
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GachaItemOpen : MonoBehaviour
{
    public GameObject supporter;
    public GameObject weapon;
    public GameObject item;
    public ParticleSystem psIcon;

    [Header("Weapon Socket")]
    public GameObject asagi;
    public GameObject sakuraL;
    public GameObject sakuraR;
    public GameObject yukikazeL;
    public GameObject yukikazeR;
    public GameObject rinkoL;
    public GameObject rinkoR;
    public GameObject murasaki;
    public GameObject jingleiL;
    public GameObject jingleiR;
    public GameObject shiranui;
    public GameObject emily;
    public GameObject emilyDrone;
    public GameObject kurenaiL;
    public GameObject kurenaiR;
    public GameObject oboroL;
    public GameObject oboroR;
    public GameObject asukaL;
    public GameObject asukaR;
    public GameObject kiraraL;
    public GameObject kiraraR;
    public GameObject Ingrid;
    public GameObject Noah;
    public GameObject AstarothL;
    public GameObject AstarothR;
    public GameObject Tokiko;
    public GameObject Shizuru;
	public GameObject Felicia;
	public GameObject Maika;
    public GameObject RinL;
    public GameObject RinR;
    public GameObject SoraL;
    public GameObject SoraR;
    public GameObject Annerose;
    public GameObject NagiL;
    public GameObject NagiR;
    public GameObject AinaL;
    public GameObject AinaR;
    public GameObject Saika;
    public GameObject Shisui;

    private eGachaParticleIcon m_icon;
    private AttachObject m_weaponL = null;
    private AttachObject m_weaponR = null;
    private Unit m_addedWeapon = null;

    private AttachObject m_weaponSubL = null;
    private AttachObject m_weaponSubR = null;

    private AttachObject m_weaponSubL2 = null;
    private AttachObject m_weaponSubR2 = null;

    public void Init(RewardData data)
    {
        if(data.Type == (int)eREWARDTYPE.CARD)
        {
            supporter.gameObject.SetActive(true);
            weapon.gameObject.SetActive(false);
            item.gameObject.SetActive(false);

            m_icon = eGachaParticleIcon.Supporter;

            CardData card = GameInfo.Instance.GetCardData(data.UID);

            MeshRenderer mesh = supporter.GetComponent<MeshRenderer>();
            mesh.material.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("card", string.Format("Card/{0}_{1}.png", card.TableData.Icon, 0));
        }
        else if(data.Type == (int)eREWARDTYPE.WEAPON)
        {
            supporter.gameObject.SetActive(false);
            weapon.gameObject.SetActive(true);
            item.gameObject.SetActive(false);

            m_icon = eGachaParticleIcon.Weapon;

            WeaponData weaponData = GameInfo.Instance.GetWeaponData(data.UID);

            if (m_weaponR)
                DestroyImmediate(m_weaponR.gameObject);

            if(m_weaponL)
                DestroyImmediate(m_weaponL.gameObject);

            if (m_addedWeapon)
                DestroyImmediate(m_addedWeapon.gameObject);

            if (m_weaponSubL)
                DestroyImmediate(m_weaponSubL.gameObject);

            if (m_weaponSubR)
                DestroyImmediate(m_weaponSubR.gameObject);

            if (m_weaponSubL2)
                DestroyImmediate(m_weaponSubL2.gameObject);
            if (m_weaponSubR2)
                DestroyImmediate(m_weaponSubR2.gameObject);

            int weaponCharId = weaponData.ListCharId[0];

            //오른손 무기
            if (!string.IsNullOrEmpty(weaponData.TableData.ModelR))
            {
                m_weaponR = ResourceMgr.Instance.CreateFromAssetBundle<AttachObject>("item", "Item/" + weaponData.TableData.ModelR + ".prefab");
                m_weaponR.ActiveEffect(false);

                if( weaponCharId == (int)ePlayerCharType.Asagi )
                    m_weaponR.transform.parent = asagi.transform;
                else if( weaponCharId == (int)ePlayerCharType.Sakura )
                    m_weaponR.transform.parent = sakuraR.transform;
                else if( weaponCharId == (int)ePlayerCharType.Yukikaze )
                    m_weaponR.transform.parent = yukikazeR.transform;
                else if( weaponCharId == (int)ePlayerCharType.Rinko )
                    m_weaponR.transform.parent = rinkoR.transform;
                else if( weaponCharId == (int)ePlayerCharType.Murasaki )
                    m_weaponR.transform.parent = murasaki.transform;
                else if( weaponCharId == (int)ePlayerCharType.Jinglei )
                    m_weaponR.transform.parent = jingleiR.transform;
                else if( weaponCharId == (int)ePlayerCharType.Shiranui )
                    m_weaponR.transform.parent = shiranui.transform;
                else if( weaponCharId == (int)ePlayerCharType.Emily )
                    m_weaponR.transform.parent = emily.transform;
                else if( weaponCharId == (int)ePlayerCharType.Kurenai )
                    m_weaponR.transform.parent = kurenaiR.transform;
                else if( weaponCharId == (int)ePlayerCharType.Oboro )
                    m_weaponR.transform.parent = oboroR.transform;
                else if( weaponCharId == (int)ePlayerCharType.Asuka )
                    m_weaponR.transform.parent = asukaR.transform;
                else if( weaponCharId == (int)ePlayerCharType.Kirara )
                    m_weaponR.transform.parent = kiraraR.transform;
                else if( weaponCharId == (int)ePlayerCharType.Ingrid )
                    m_weaponR.transform.parent = Ingrid.transform;
                else if( weaponCharId == (int)ePlayerCharType.Noah )
                    m_weaponR.transform.parent = Noah.transform;
                else if( weaponCharId == (int)ePlayerCharType.Astaroth )
                    m_weaponR.transform.parent = AstarothR.transform;
                else if( weaponCharId == (int)ePlayerCharType.Tokiko )
                    m_weaponR.transform.parent = Tokiko.transform;
                else if( weaponCharId == (int)ePlayerCharType.Shizuru )
                    m_weaponR.transform.parent = Shizuru.transform;
                else if( weaponCharId == (int)ePlayerCharType.Felicia )
                    m_weaponR.transform.parent = Felicia.transform;
                else if( weaponCharId == (int)ePlayerCharType.Maika )
                    m_weaponR.transform.parent = Maika.transform;
                else if( weaponCharId == (int)ePlayerCharType.Rin )
                    m_weaponR.transform.parent = RinR.transform;
                else if( weaponCharId == (int)ePlayerCharType.Sora ) {
                    m_weaponR.transform.parent = SoraR.transform;
                }
                else if( weaponCharId == (int)ePlayerCharType.Nagi ) {
                    m_weaponR.transform.parent = NagiR.transform;
                }
                else if ( weaponCharId == (int)ePlayerCharType.Aina ) {
                    m_weaponR.transform.parent = AinaR.transform;
                }
				else if ( weaponCharId == (int)ePlayerCharType.Saika ) {
					m_weaponR.transform.parent = Saika.transform;
				}
                else if ( weaponCharId == (int)ePlayerCharType.Shisui ) {
                    m_weaponR.transform.parent = Shisui.transform;
                }

                Utility.InitTransform(m_weaponR.gameObject);
                Utility.ChangeLayersRecursively(m_weaponR.transform, eLayer.Director);

                SetSubWeapon(weaponData.TableData.SubModelR, false, m_weaponR.transform, m_weaponSubR);
                SetSubWeapon(weaponData.TableData.Sub2ModelR, false, m_weaponR.transform, m_weaponSubR2, 2);

            }

            //왼손 무기
            if (!string.IsNullOrEmpty(weaponData.TableData.ModelL))
            {
                m_weaponL = ResourceMgr.Instance.CreateFromAssetBundle<AttachObject>("item", "Item/" + weaponData.TableData.ModelL + ".prefab");
                m_weaponL.ActiveEffect(false);

                if( weaponCharId == (int)ePlayerCharType.Asagi )
                    m_weaponL.transform.parent = asagi.transform;
                else if( weaponCharId == (int)ePlayerCharType.Sakura )
                    m_weaponL.transform.parent = sakuraL.transform;
                else if( weaponCharId == (int)ePlayerCharType.Yukikaze )
                    m_weaponL.transform.parent = yukikazeL.transform;
                else if( weaponCharId == (int)ePlayerCharType.Rinko )
                    m_weaponL.transform.parent = rinkoL.transform;
                else if( weaponCharId == (int)ePlayerCharType.Murasaki )
                    m_weaponL.transform.parent = murasaki.transform;
                else if( weaponCharId == (int)ePlayerCharType.Jinglei )
                    m_weaponL.transform.parent = jingleiL.transform;
                else if( weaponCharId == (int)ePlayerCharType.Shiranui )
                    m_weaponL.transform.parent = shiranui.transform;
                else if( weaponCharId == (int)ePlayerCharType.Emily )
                    m_weaponL.transform.parent = emily.transform;
                else if( weaponCharId == (int)ePlayerCharType.Kurenai )
                    m_weaponL.transform.parent = kurenaiL.transform;
                else if( weaponCharId == (int)ePlayerCharType.Oboro )
                    m_weaponL.transform.parent = oboroL.transform;
                else if( weaponCharId == (int)ePlayerCharType.Asuka )
                    m_weaponL.transform.parent = asukaL.transform;
                else if( weaponCharId == (int)ePlayerCharType.Kirara )
                    m_weaponL.transform.parent = kiraraL.transform;
                else if( weaponCharId == (int)ePlayerCharType.Ingrid )
                    m_weaponL.transform.parent = Ingrid.transform;
                else if( weaponCharId == (int)ePlayerCharType.Noah )
                    m_weaponL.transform.parent = Noah.transform;
                else if( weaponCharId == (int)ePlayerCharType.Astaroth )
                    m_weaponL.transform.parent = AstarothL.transform;
                else if( weaponCharId == (int)ePlayerCharType.Tokiko )
                    m_weaponL.transform.parent = Tokiko.transform;
                else if( weaponCharId == (int)ePlayerCharType.Shizuru )
                    m_weaponL.transform.parent = Shizuru.transform;
                else if( weaponCharId == (int)ePlayerCharType.Felicia )
                    m_weaponL.transform.parent = Felicia.transform;
                else if( weaponCharId == (int)ePlayerCharType.Maika )
                    m_weaponL.transform.parent = Maika.transform;
                else if( weaponCharId == (int)ePlayerCharType.Rin )
                    m_weaponL.transform.parent = RinL.transform;
                else if( weaponCharId == (int)ePlayerCharType.Sora ) {
                    m_weaponL.transform.parent = SoraL.transform;
                }
                else if( weaponCharId == (int)ePlayerCharType.Nagi ) {
                    m_weaponL.transform.parent = NagiL.transform;
                }
                else if ( weaponCharId == (int)ePlayerCharType.Aina ) {
                    m_weaponL.transform.parent = AinaL.transform;
                }
				else if ( weaponCharId == (int)ePlayerCharType.Saika ) {
                    m_weaponL.transform.parent = Saika.transform;
				}
                else if ( weaponCharId == (int)ePlayerCharType.Shisui ) {
                    m_weaponL.transform.parent = Shisui.transform;
                }

                Utility.InitTransform(m_weaponL.gameObject);
                Utility.ChangeLayersRecursively(m_weaponL.transform, eLayer.Director);

                SetSubWeapon(weaponData.TableData.SubModelL, true, m_weaponL.transform, m_weaponSubL);
                SetSubWeapon(weaponData.TableData.Sub2ModelL, true, m_weaponL.transform, m_weaponSubL2, 2);
            }

            //추가 무기
            if (!string.IsNullOrEmpty(weaponData.TableData.AddedUnitWeapon))
            {
                m_addedWeapon = GameSupport.CreateUnitWeapon(weaponData.TableData.AddedUnitWeapon, false);

                if((ePlayerCharType)weaponCharId == ePlayerCharType.Emily)
                {
                    m_addedWeapon.transform.parent = emilyDrone.transform;
                }

                /*
                if (weaponData.TableData.CharacterID == (int)ePlayerCharType.Asagi)
                {

                }
                else if (weaponData.TableData.CharacterID == (int)ePlayerCharType.Sakura)
                {

                }
                else if (weaponData.TableData.CharacterID == (int)ePlayerCharType.Yukikaze)
                {

                }
                else if (weaponData.TableData.CharacterID == (int)ePlayerCharType.Rinko)
                {

                }
                else if (weaponData.TableData.CharacterID == (int)ePlayerCharType.Murasaki)
                {

                }
                else if (weaponData.TableData.CharacterID == (int)ePlayerCharType.Jinglei)
                {

                }
                else if (weaponData.TableData.CharacterID == (int)ePlayerCharType.Shiranui)
                {

                }
                else if (weaponData.TableData.CharacterID == (int)ePlayerCharType.Emily)
                {
                    m_addedWeapon.transform.parent = emilyDrone.transform;
                }
                else if (weaponData.TableData.CharacterID == (int)ePlayerCharType.Kurenai)
                {

                }
                else if (weaponData.TableData.CharacterID == (int)ePlayerCharType.Oboro)
                {
                }
                */

                Utility.InitTransform(m_addedWeapon.gameObject);
                Utility.ChangeLayersRecursively(m_addedWeapon.transform, eLayer.Director);
            }
        }
        else if (data.Type == (int)eREWARDTYPE.GEM)
        {
            supporter.gameObject.SetActive(false);
            weapon.gameObject.SetActive(false);
            item.gameObject.SetActive(true);

            m_icon = eGachaParticleIcon.Jewel;

            GemData gem = GameInfo.Instance.GetGemData(data.UID);

            MeshRenderer mesh = item.GetComponent<MeshRenderer>();
            mesh.material.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Item/" + gem.TableData.Icon);
        }
        else if(data.Type == (int)eREWARDTYPE.ITEM)
        {
            supporter.gameObject.SetActive(false);
            weapon.gameObject.SetActive(false);
            item.gameObject.SetActive(true);

            m_icon = eGachaParticleIcon.Item;

            ItemData itemData = GameInfo.Instance.GetItemData(data.UID);

            MeshRenderer mesh = item.GetComponent<MeshRenderer>();
            mesh.material.mainTexture = (Texture)ResourceMgr.Instance.LoadFromAssetBundle("icon", "Icon/Item/" + itemData.TableData.Icon);
        }

        ParticleSystem.TextureSheetAnimationModule texSheetAniMoudle = psIcon.textureSheetAnimation;
        texSheetAniMoudle.startFrame = new ParticleSystem.MinMaxCurve((float)m_icon / (float)(texSheetAniMoudle.numTilesX * texSheetAniMoudle.numTilesY));
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
}
