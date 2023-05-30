
using UnityEngine;
using UnityEngine.AI;
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;


public abstract partial class Unit : MonoBehaviour
{
	public bool		    isClone					{ get { return m_clone; } }
	public Unit		    cloneOwner				{ get { return m_cloneOwner; } }
	public List<Unit>   ListClone				{ get { return m_listClone; } }

	public bool		    UseAttack02				{ get; set; }			= false;
	public bool		    SendBattleOptionToOwner	{ get; set; }			= false;
    public bool         LockFollowAni           { get; set; }           = false;
	public int		    showCloneIndex			{ get; private set; }
	public bool		    IsUsing					{ get; private set; }	= false;
	public bool		    IsFollowOwner			{ get; private set; }	= false;
	
	protected Unit		    m_cloneOwner    = null;
    protected bool		    m_clone		    = false;
    protected List<Unit>    m_listClone	    = new List<Unit>();

	private Coroutine       mCrAni      = null;
    private CapsuleCollider mCapsuleCol = null;


    public void CreateClone(int count)
    {
        m_listClone.Clear();

        for (int i = 0; i < count; i++)
        {
            GameTable.Character.Param param = GameInfo.Instance.GetCharacterData(m_tableId);
            if (param == null)
                continue;

            string folder = Utility.GetFolderFromPath(param.Model);
            string name = Utility.GetNameFromPath(param.Model);

            string strModel = string.Format("Unit/{0}/{1}_G/{1}_G.prefab", folder, name);
            Unit clone = ResourceMgr.Instance.CreateFromAssetBundle<Unit>("unit", strModel);

            if(m_costumeUnit.charData != null)
                clone.costumeUnit.InitCostumeChar(m_costumeUnit.charData, true);

            clone.InitClone(this, folder);

            if (m_costumeUnit.charData != null)
                clone.costumeUnit.SetCostume(m_costumeUnit.charData);

            clone.aniEvent.GetBones();

            clone.aniEvent.SetShaderColor("_Color", CloneColor);
            clone.aniEvent.SetShaderColor("_HColor", CloneHColor);
            clone.aniEvent.SetShaderColor("_SColor", CloneSColor);

            if (FSaveData.Instance.Graphic >= 2)
            {
                clone.aniEvent.SetShaderColor("_RimColor", CloneRimColor);
                clone.aniEvent.SetShaderFloat("_RimMin", CloneRimMin);

                clone.aniEvent.SetShaderColor("_OutlineColor", CloneOutlineColor);
                clone.aniEvent.SetShaderFloat("_Outline", CloneOutlineWidth);
            }

            Utility.SetLayer(clone.gameObject, (int)eLayer.PlayerClone, true);
            m_listClone.Add(clone);
        }
    }

    public void CreateClone(int count, CharData charData, WeaponData mainWeaponData, WeaponData subWeaponData)
    {
        m_listClone.Clear();

        for (int i = 0; i < count; i++)
        {
            GameTable.Character.Param param = GameInfo.Instance.GetCharacterData(m_tableId);
            if (param == null)
                continue;

            string folder = Utility.GetFolderFromPath(param.Model);
            string name = Utility.GetNameFromPath(param.Model);

            string strModel = string.Format("Unit/{0}/{1}_G/{1}_G.prefab", folder, name);
            Unit clone = ResourceMgr.Instance.CreateFromAssetBundle<Unit>("unit", strModel);

            if (charData != null)
                clone.costumeUnit.InitCostumeChar(charData, true);

            clone.InitClone(this, folder);

            if (charData != null)
                clone.costumeUnit.SetCostume(charData, mainWeaponData, subWeaponData);

            clone.aniEvent.GetBones();

            clone.aniEvent.SetShaderColor("_Color", CloneColor);
            clone.aniEvent.SetShaderColor("_HColor", CloneHColor);
            clone.aniEvent.SetShaderColor("_SColor", CloneSColor);

            if (FSaveData.Instance.Graphic >= 2)
            {
                clone.aniEvent.SetShaderColor("_RimColor", CloneRimColor);
                clone.aniEvent.SetShaderFloat("_RimMin", CloneRimMin);

                clone.aniEvent.SetShaderColor("_OutlineColor", CloneOutlineColor);
                clone.aniEvent.SetShaderFloat("_Outline", CloneOutlineWidth);
            }

            Utility.SetLayer(clone.gameObject, (int)eLayer.PlayerClone, true);
            m_listClone.Add(clone);
        }
    }

    public void CreateMonsterClone(int count)
    {
        m_listClone.Clear();

        for (int i = 0; i < count; i++)
        {
            GameClientTable.Monster.Param param = GameInfo.Instance.GetMonsterData(m_tableId);
            if (param == null)
            {
                continue;
            }

            Unit clone = ResourceMgr.Instance.CreateFromAssetBundle<Unit>("unit", string.Format("Unit/{0}.prefab", param.ModelPb));
            if(clone == null)
            {
                continue;
            }

            clone.InitClone(this, folder);

            clone.aniEvent.GetBones();
            clone.aniEvent.SetShaderColor("_Color", CloneColor);
            clone.aniEvent.SetShaderColor("_HColor", CloneHColor);
            clone.aniEvent.SetShaderColor("_SColor", CloneSColor);

            if (FSaveData.Instance.Graphic >= 2)
            {
                clone.aniEvent.SetShaderColor("_RimColor", CloneRimColor);
                clone.aniEvent.SetShaderFloat("_RimMin", CloneRimMin);

                clone.aniEvent.SetShaderColor("_OutlineColor", CloneOutlineColor);
                clone.aniEvent.SetShaderFloat("_Outline", CloneOutlineWidth);
            }

            Utility.SetLayer(clone.gameObject, (int)eLayer.PlayerClone, true);
            m_listClone.Add(clone);
        }
    }

	public void InitClone( Unit owner, string folder ) {
		m_cloneOwner = owner;
		m_clone = true;
		IsUsing = false;

		m_attackPower = owner.attackPower;

		mChangedSuperArmorId = SetSuperArmor( eSuperArmor.None );

		m_rigidBody = GetComponent<Rigidbody>();
        if ( m_rigidBody != null ) {
            DestroyImmediate( m_rigidBody );
        }

		MainCollider = GetComponent<UnitCollider>();
		if ( MainCollider != null ) {
			MainCollider.SetTrigger( true );
		}
		else {
			mCapsuleCol = GetComponent<CapsuleCollider>();
            if( mCapsuleCol ) {
                mCapsuleCol.isTrigger = false;
            }
		}

		UnitCollider[] arrUnitCollider = GetComponentsInChildren<UnitCollider>();
		for ( int i = 0; i < arrUnitCollider.Length; i++ ) {
			arrUnitCollider[i].SetTrigger( true );
			ListCollider.Add( arrUnitCollider[i] );
		}

		m_listCmpt.Clear();
		m_listCmpt.AddRange( GetComponentsInChildren<CmptBase>() );
        for ( int i = 0; i < m_listCmpt.Count; i++ ) {
            Destroy( m_listCmpt[i] );
        }
		m_listCmpt.Clear();

		ActionBase[] actions = GetComponentsInChildren<ActionBase>();
        for ( int i = 0; i < actions.Length; i++ ) {
            Destroy( actions[i] );
        }

		m_afterImg = GetComponent<AfterImageComponent>();
        if ( m_afterImg != null ) {
            m_afterImg.SetParent( this );
        }

		m_costumeUnit = GetComponent<CostumeUnit>();

		m_folder = m_cloneOwner.folder;
		m_aniEvent = GetComponentInChildren<AniEvent>();
		if ( m_aniEvent != null ) {
			m_aniEvent.Init( this, m_folder, null );

			m_aniEvent.OnAttack = OnAttack;
			m_aniEvent.OnFire = OnFire;
			m_aniEvent.OnStepForward = OnStepForward;
			m_aniEvent.OnChangeColor = OnChangeColor;
			m_aniEvent.OnJump = OnJump;
		}

		m_actionSystem = gameObject.AddComponent<ActionSystem>();
		m_actionSystem.Init( this );
		m_actionSystem.AddAction( gameObject.AddComponent<ActionCloneHomingAttack>(), 0, null );
		m_actionSystem.AddAction( gameObject.AddComponent<ActionCloneAttack>(), 0, null );

		m_contactNormal = Vector3.zero;

		m_buffStats = new UnitBuffStats( this );

		m_attacker = null;
		m_mainTarget = null;
		m_evadedTarget = null;

		isFloating = false;

		m_pause = false;
		m_pauseFrame = false;

		string[] fileName = Utility.Split( name, ' ' );
		fileName[0] = fileName[0].Replace( "(Clone)", "" );

		TextAsset aniEventFile = ResourceMgr.Instance.LoadFromAssetBundle( "unit", string.Format( "Unit/{0}/{1}/{2}.bytes", folder, fileName[0], fileName[0] ) ) as TextAsset;
		TextAsset aniSndEventFile = ResourceMgr.Instance.LoadFromAssetBundle( "unit", string.Format( "Unit/{0}/{1}/{2}_snd.bytes", folder, fileName[0], fileName[0] ) ) as TextAsset;

		gameObject.name = string.Format( "{0}_C{1}", gameObject.name, m_cloneOwner.m_listClone.Count );

		m_aniEvent.GetBones();
		m_aniEvent.LoadEvent( aniEventFile, aniSndEventFile );

		Deactivate();

		transform.localScale = Vector3.one * 1.1f;

		m_maxHp = 1.0f;
		m_curHp = 1.0f;

		m_charType = eCharacterType.Summons;
	}

	public void ShowClone( int index, Vector3 pos, Quaternion rot, bool followOwner = false ) {
		if ( index < 0 || index >= m_listClone.Count ) {
			Debug.LogError( index + "번 클론은 존재하지 않습니다." );
			return;
		}

		Unit clone = m_listClone[index];
		if ( clone.IsActivate() ) {
			return;
		}

		clone.m_originalSpeed = originalSpeed;
		clone.m_curSpeed = originalSpeed;
		clone.showCloneIndex = index;

		clone.Activate();
		clone.SetInitialPosition( pos, rot );
		clone.SetKinematicRigidBody();
		clone.SetFollowOwner( followOwner );
	}

	public Unit GetClone(int index)
    {
        if (index < 0 || index >= m_listClone.Count)
        {
            Debug.LogError(index + "번 클론은 존재하지 않습니다.");
            return null;
        }

        return m_listClone[index];
    }

    public Unit GetActiveClone()
    {
        return m_listClone.Find(x => x.IsActivate());
    }

    public Unit GetDeactivateClone(int startIndex)
    {
        if (startIndex < 0 || startIndex >= m_listClone.Count)
        {
            return null;
        }

        for(int i = startIndex; i < m_listClone.Count; i++)
        {
            if(m_listClone[i].IsActivate() || m_listClone[i].IsUsing)
            {
                continue;
            }

            m_listClone[i].IsUsing = true;
            return m_listClone[i];
        }

        return null;
    }

	public int GetDeactivateCloneIndex( int startIndex ) {
		if( startIndex < 0 || startIndex >= m_listClone.Count ) {
			return -1;
		}

		for( int i = startIndex; i < m_listClone.Count; i++ ) {
			if( m_listClone[i].IsActivate() || m_listClone[i].IsUsing ) {
				continue;
			}

			m_listClone[i].IsUsing = true;
			return i;
		}

		return -1;
	}

	public void SetCloneAttackPowerRate(int index, float rate)
    {
        if (index < 0 || index >= m_listClone.Count)
        {
            Debug.LogError(index + "번 클론은 존재하지 않습니다.");
            return;
        }

        m_listClone[index].m_attackPower = (m_attackPower + (m_attackPower * IncreaseSummonsAttackPowerRate)) * rate;
    }

    public void SetCloneShader(int index, Shader shader)
    {
        if (index < 0 || index >= m_listClone.Count)
        {
            Debug.LogError(index + "번 클론은 존재하지 않습니다.");
            return;
        }

        m_listClone[index].aniEvent.SetShader(shader);
    }

    public void AddAIToClone(int index, string aiFileName)
    {
        if (index < 0 || index >= m_listClone.Count)
        {
            Debug.LogError(index + "번 클론은 존재하지 않습니다.");
            return;
        }

        Unit clone = m_listClone[index];
        if(clone.mAI != null)
        {
            clone.StartBT();
            return;
        }

        clone.aniEvent.RestoreOriginalColor();

        clone.m_rigidBody = clone.gameObject.AddComponent<Rigidbody>();

        clone.m_cmptJump = clone.gameObject.AddComponent<CmptJump>();
        clone.m_cmptJump.m_jumpPower = clone.m_cloneOwner.cmptJump.m_jumpPower;

        clone.m_cmptMovement = clone.gameObject.AddComponent<CmptMovement>();
        clone.m_cmptRotate = clone.gameObject.AddComponent<CmptRotateByDirection>();

        clone.MainCollider = clone.gameObject.AddComponent<UnitCollider>();
        clone.MainCollider.SetRadius(clone.m_cloneOwner.MainCollider.radius);
        clone.MainCollider.SetHeight(clone.m_cloneOwner.MainCollider.height);

        ActionComboAttack actionOwnerComboAttack = clone.m_cloneOwner.actionSystem.GetAction<ActionComboAttack>(eActionCommand.Attack01);
        ActionComboAttack actionComboAttack = clone.gameObject.AddComponent<ActionComboAttack>();
        actionComboAttack.attackAnimations = actionOwnerComboAttack.attackAnimations;
        clone.m_actionSystem.AddAction(actionComboAttack, 0, null);

        clone.actionSystem.AddAction(clone.gameObject.AddComponent<ActionMoveToTarget>(), 0, null);
        clone.actionSystem.AddAction(clone.gameObject.AddComponent<ActionWait>(), 0, null);

        clone.actionSystem.AddAction(clone.gameObject.AddComponent<ActionIdle>(), 0, null);

        clone.AddAIController(aiFileName);
        clone.StartBT();
    }

    public float PlayCloneAni(int index, eAnimation ani, bool immediate = true)
    {
        if (index < 0 || index >= m_listClone.Count)
        {
            Debug.LogError(index + "번 클론은 존재하지 않습니다.");
            return 0.0f;
        }

        m_listClone[index].aniEvent.Resume();

        float length = m_listClone[index].aniEvent.GetCutFrameLength(ani);
        if (length == 0.0f)
        {
            if (immediate)
                length = m_listClone[index].PlayAniImmediate(ani);
            else
                length = m_listClone[index].PlayAni(ani);
        }
        else
        {
            if(immediate)
                m_listClone[index].PlayAniImmediate(ani);
            else
                m_listClone[index].PlayAni(ani);
        }

        return length;
    }

    public float PlayAllCloneAni(eAnimation ani)
    {
        float aniLength = 0.0f;
        for (int i = 0; i < m_listClone.Count; i++)
            aniLength = m_listClone[i].PlayAniImmediate(ani);

        return aniLength;
    }

    public void HideClone(int index)
    {
        if (index < 0 || index >= m_listClone.Count)
        {
            Debug.LogError(index + "번 클론은 존재하지 않습니다.");
            return;
        }

        if (m_listClone[index].aniEvent)
        {
            m_listClone[index].aniEvent.StopEffects();
        }

        m_listClone[index].LockFollowAni = false;
        m_listClone[index].StopPauseFrame();
        m_listClone[index].StopBT();
        m_listClone[index].DeactivateClone();
    }

    public virtual void HideAllClone()
    {
        for (int i = 0; i < m_listClone.Count; i++)
        {
            HideClone(i);
        }
    }

    public void DeactivateClone()
    {
        IsShow = false;
        IsUsing = false;

        if (!isClone && m_actionSystem != null && m_actionSystem.HasNoAction() == false)
            m_actionSystem.CancelCurrentAction();

        if (mUIHpBar)
            mUIHpBar.gameObject.SetActive(false);

        if (m_grade != eGrade.Boss && mUIBuffDebuffIcon)
            mUIBuffDebuffIcon.End();

        StopAllCoroutines();
        gameObject.SetActive(false);
    }

    public int GetActivateCloneCount()
    {
        int count = 1;
        for (int i = 0; i < m_listClone.Count; i++)
        {
            if (m_listClone[i].IsActivate() == true)
                ++count;
        }

        return count;
    }

	public void ChangeCloneWeapon(int index, long weaponUID)
	{
		if (index < 0 || index >= m_listClone.Count)
		{
			Debug.LogError(index + "번 클론은 존재하지 않습니다.");
			return;
		}

		if (m_listClone[index].costumeUnit == null)
		{
			return;
		}

		m_listClone[index].costumeUnit.ShowWeaponByWeaponUID(weaponUID);
	}

	public void ChangeCloneWeaponByWeaponIndex(int index, int weaponIndex, string changeWeaponName = "")
	{
		if (index < 0 || index >= m_listClone.Count)
		{
			Debug.LogError(index + "번 클론은 존재하지 않습니다.");
			return;
		}

		if (m_listClone[index].costumeUnit == null)
		{
			return;
		}

		m_listClone[index].costumeUnit.ShowWeaponByIndex(weaponIndex, changeWeaponName);
	}

	public void SetClone()
	{
		m_clone = true;
	}

	public void SetFollowOwner( bool set ) {
		IsFollowOwner = set;

		if ( mCapsuleCol ) {
			mCapsuleCol.isTrigger = set;
		}
	}

	public bool HasFollowOwnerClone()
	{
		for(int i = 0; i < m_listClone.Count; ++i)
		{
			if(m_listClone[i].IsFollowOwner)
			{
				return true;
			}
		}

		return false;
	}

	public void DelayedPlayAni(float delay, eAnimation aniType, int layer = 0, bool backToIdle = false)
	{
        Utility.StopCoroutine(this, ref mCrAni);
		mCrAni = StartCoroutine(PlayAni(delay, aniType, layer, backToIdle));
	}

	public void DelayedPlayAniImmediate(float delay, eAnimation aniType, float normalizeTime = 0.0f, bool backToIdle = false)
	{
        Utility.StopCoroutine(this, ref mCrAni);
		mCrAni = StartCoroutine(PlayAniImmediate(delay, aniType, normalizeTime, backToIdle));
	}

	private void CheckCloneAI()
    {
        if (isClone && mAI != null && m_cloneOwner && m_cloneOwner.Input)
        {
            if (!m_cloneOwner.actionSystem.IsCurrentHitAction() && m_cloneOwner.Input.isPause && !mAI.IsStop)
            {
                mAI.StopBT();
                m_aniEvent.PlayAni(eAnimation.Idle01);
            }
            else if (!m_cloneOwner.Input.isPause && mAI.IsStop)
            {
                mAI.StartBT();
            }
        }
    }

	private IEnumerator PlayAni( float delay, eAnimation aniType, int layer = 0, bool backToIdle = false ) {
		yield return new WaitForSeconds( delay * ( showCloneIndex + 1 ) );

        transform.SetPositionAndRotation( m_cloneOwner.transform.position, m_cloneOwner.transform.rotation );
        PlayAni( aniType, layer, backToIdle );

        LockFollowAni = false;
	}

	private IEnumerator PlayAniImmediate( float delay, eAnimation aniType, float normalizeTime = 0.0f, bool backToIdle = false ) {
		yield return new WaitForSeconds( delay * ( showCloneIndex + 1 ) );

        transform.SetPositionAndRotation( m_cloneOwner.transform.position, m_cloneOwner.transform.rotation );
        PlayAniImmediate( aniType, normalizeTime, backToIdle );

		LockFollowAni = false;
	}
}
