
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EffectManager : FMonoSingleton<EffectManager>
{
    public enum eType
    {
        None = 0,

        // 몬스터 최대 개수만큼 생성 될 이펙트들 (주로 몬스터 히트 이펙트로 사용)
        Each_Monster_Normal_Hit,
        Each_Monster_Critical_Hit,
        Each_Monster_DamageOnly_Hit,

        // 하나만 생성 될 이펙트들
        Common,
        Common_Show_CenterOfEnemies, // 적들 중간에 플레이될 이펙트에 사용
    }

    public enum eParticleFindType
    {
        None = 0,
        Active,
        Deactive
    }

    public class sParticleInfo
    {
        public string           Name;
        public ParticleSystem   Particle;
        public Coroutine        CrStop;
    }

    public class sEffInfo
    {
        public string           name;
        public int              id;
        public eType            type;
        public float            originalPSDuration;

        public string           attach;
        public bool             follow;
        public Vector3          originalPos;
        public Vector3          originalRot;
        public Vector3          addedPos;
        public Vector3          addedRot;

        public string           sndName;
        public int              sndMixerIndex;

        public AnimationClip    CameraAni;
    }


    private List<sEffInfo>      mListEffInfo            = new List<sEffInfo>();
    private List<sParticleInfo> mListParticle           = new List<sParticleInfo>();
    private List<int>           mListCheckPlayerId      = new List<int>();
    private List<int>           mListCheckSupporterId   = new List<int>();
	private WaitForFixedUpdate	mWaitForFixedUpdate		= new WaitForFixedUpdate();


	public void LoadEffs()
    {
        if (AppMgr.Instance.SceneType != AppMgr.eSceneType.None && 
            AppMgr.Instance.SceneType != AppMgr.eSceneType.Stage && 
            AppMgr.Instance.SceneType != AppMgr.eSceneType.Training)
        {
            return;
        }

        switch(World.Instance.StageType)
        {
            case eSTAGETYPE.STAGE_SPECIAL:
                {
                    mListEffInfo.Clear();

                    if (World.Instance.StageData.Chapter == (int)eSTAGE_SPECIAL_TYPE.BIKE)
                    {
                        AddRRaceEffect(20, 0, eType.Each_Monster_Normal_Hit, "Effect/Character/prf_fx_race_coin_get.prefab");
                        AddRRaceEffect(11, 1, eType.Each_Monster_Normal_Hit, "Effect/Background/prf_fx_envobject_explosion_racebridge_barricade.prefab");
                        AddRRaceEffect(3, 2, eType.Each_Monster_Normal_Hit, "Effect/Character/prf_fx_race_heart_get.prefab");
                    }
                    else if(World.Instance.StageData.Chapter == (int)eSTAGE_SPECIAL_TYPE.THROW)
                    {
                        AddRRaceEffect(10, 0, eType.Each_Monster_Normal_Hit, "Effect/Monster/prf_fx_monster_shiranui_hit_01.prefab");
                        AddRRaceEffect(10, 1, eType.Each_Monster_Normal_Hit, "Effect/Monster/prf_fx_monster_shiranui_hit_01.prefab");
                        AddRRaceEffect(10, 2, eType.Each_Monster_Normal_Hit, "Effect/Background/prf_fx_minigame_ball_01_hit.prefab");
                        AddRRaceEffect(10, 3, eType.Each_Monster_Normal_Hit, "Effect/Background/prf_fx_minigame_ball_02_hit.prefab");
                    }
                }
                break;

            default:
                {
                    SetCheckList();

                    //씬 변경시 이펙트는 날아가지만, 리스트는 남아있어서 클리어 (임시조치)
                    mListParticle.Clear();
                    mListEffInfo.Clear();

                    List<GameClientTable.CommonEffect.Param> listHitEffParam = GameInfo.Instance.GameClientTable.FindAllCommonEffect(x => x.ID > 0);
					if(listHitEffParam == null || listHitEffParam.Count <= 0)
					{
						return;
					}

					int totalCount = CalcParticleTotalCount(listHitEffParam);
					mListParticle.Capacity = totalCount;

					// 필요한 파티클만 먼저 생성 (중복X)
					for (int i = 0; i < listHitEffParam.Count; i++)
                    {
                        GameClientTable.CommonEffect.Param param = listHitEffParam[i];
                        if (param == null || string.IsNullOrEmpty(param.EffectPrefab))
                        {
                            continue;
                        }

						if((FSaveData.Instance.Graphic == 0 && (param.Type == 2 || param.Type == 3)) ||
						   (FSaveData.Instance.Graphic == 1 && param.Type == 3)) // 하 옵션은 타입 2,3번 스킵, 중 옵션은 타입 3번 스킵
						{
							continue;
						}

                        sParticleInfo find = mListParticle.Find(x => x.Name.CompareTo(param.EffectPrefab) == 0);
                        if (find != null)
                        {
                            continue;
                        }

                        if(param.UnitType > 0)
                        {
                            if(param.UnitType == (int)eCharacterType.Character && mListCheckPlayerId.Count > 0)
                            {
                                if(mListCheckPlayerId.Find(x => x == param.UnitTableId) == 0)
                                {
                                    continue;
                                }
                            }
                            else if(param.UnitType == (int)eCharacterType.Supporter)
                            {
                                if (mListCheckSupporterId.Count <= 0 || 
                                    (mListCheckSupporterId.Count > 0 && mListCheckSupporterId.Find(x => x == param.UnitTableId) == 0))
                                {
                                    continue;
                                }
                            }
                        }

                        int effCount = 1; // ((eType)param.Type <= eType.Each_Monster_DamageOnly_Hit) ? World.Instance.EnemyMgr.maxMonsterCountInSpawnGroup : 1;

                        if((eType)param.Type <= eType.Each_Monster_DamageOnly_Hit)
                        {
                            if(World.Instance.EnemyMgr.MaxSpawnMonsterCount > 0)
                            {
                                effCount = Mathf.Min(World.Instance.EnemyMgr.MaxSpawnMonsterCount, World.Instance.EnemyMgr.maxMonsterCountInSpawnGroup);
                            }
                            else
                            {
                                effCount = World.Instance.EnemyMgr.maxMonsterCountInSpawnGroup;
                            }
                        }

                        // PVP땐 같은 이펙트(예를 들면 서포터)가 있을 수 있어서 2개씩 생성
                        if (effCount == 1 && World.Instance.StageType == eSTAGETYPE.STAGE_PVP)
                        {
                            effCount = 2;
                        }

                        for (int j = 0; j < effCount; j++)
                        {
                            sParticleInfo info = new sParticleInfo();

                            info.Name = param.EffectPrefab;
                            info.CrStop = null;

                            info.Particle = ResourceMgr.Instance.CreateFromAssetBundle<ParticleSystem>("effect", param.EffectPrefab + ".prefab");
                            if (info.Particle == null)
                            {
                                continue;
                            }

                            info.Particle.gameObject.SetActive(false);
                            mListParticle.Add(info);
                        }
                    }

                    Log.Show("생성한 총 공용 파티클 개수는 " + mListParticle.Count, Log.ColorType.Blue);

					totalCount = CalcEffInfoTotalCount(listHitEffParam);
					mListEffInfo.Capacity = totalCount;

					// 파티클 정보는 모두 생성
					for (int i = 0; i < listHitEffParam.Count; i++)
                    {
                        GameClientTable.CommonEffect.Param param = listHitEffParam[i];
                        if (param == null || string.IsNullOrEmpty(param.EffectPrefab))
                        {
                            continue;
                        }

                        int effCount = ((eType)param.Type <= eType.Each_Monster_DamageOnly_Hit) ? World.Instance.EnemyMgr.maxMonsterCountInSpawnGroup : 1;
                        for (int j = 0; j < effCount; j++)
                        {
							if ((FSaveData.Instance.Graphic == 0 && (param.Type == 2 || param.Type == 3)) ||
								(FSaveData.Instance.Graphic == 1 && param.Type == 3)) // 하 옵션은 타입 2,3번 스킵, 중 옵션은 타입 3번 스킵
							{
								continue;
							}

							sEffInfo effInfo = new sEffInfo();
                            effInfo.name = param.EffectPrefab;

                            sParticleInfo particleInfo = mListParticle.Find(x => x.Name.CompareTo(effInfo.name) == 0);
                            if (particleInfo == null)
                            {
                                continue;
                            }

                            effInfo.id = param.ID;
                            effInfo.type = (eType)param.Type;

                            effInfo.originalPSDuration = particleInfo.Particle.main.duration;

                            effInfo.attach = param.Attach;
                            effInfo.follow = param.Follow == 1 ? true : false;
                            effInfo.originalPos = particleInfo.Particle.transform.position;
                            effInfo.originalRot = particleInfo.Particle.transform.eulerAngles;
                            effInfo.addedPos = new Vector3(param.PosX, param.PosY, param.PosZ);
                            effInfo.addedRot = new Vector3(param.RotX, param.RotY, param.RotZ);

                            if (!string.IsNullOrEmpty(param.Sound))
                            {
                                effInfo.sndName = Utility.GetNameFromPath(param.Sound);
                                effInfo.sndMixerIndex = param.MixerIndex;
                                SoundManager.Instance.AddAudioClip(effInfo.sndName, param.Sound, FSaveData.Instance.GetSEVolume());
                            }

                            if (!string.IsNullOrEmpty(param.CameraAni))
                            {
                                System.Text.StringBuilder sb = new System.Text.StringBuilder("Animation_Char/");
                                sb.Append("Camera/");
                                sb.Append(param.CameraAni);
                                sb.Append(".anim");

                                effInfo.CameraAni = ResourceMgr.Instance.LoadFromAssetBundle("animation_char", sb.ToString()) as AnimationClip;
                            }

                            mListEffInfo.Add(effInfo);
                        }
                    }
                }
                break;
        }


        Log.Show("생성한 총 이펙트 정보는 " + mListEffInfo.Count, Log.ColorType.Blue);
    }

    public void Release()
    {
        for(int i = 0; i< mListParticle.Count; i++)
        {
            sParticleInfo info = mListParticle[i];
            if(info == null)
            {
                continue;
            }

            if (info.Particle)
            {
                DestroyImmediate(info.Particle);
                info.Particle = null;
            }

            Utility.StopCoroutine(this, ref info.CrStop);
        }

        mListParticle.Clear();

        for (int i = 0; i < mListEffInfo.Count; i++)
        {
            sEffInfo info = mListEffInfo[i];
            if (info == null)
            {
                continue;
            }

            info.CameraAni = null;
            info = null;
        }

        mListEffInfo.Clear();
    }

	public sEffInfo GetEffectInfoOrNull(int tableId, eType type)
	{
		sEffInfo effInfo = null;
		for (int i = 0; i < mListEffInfo.Count; i++)
		{
			if (mListEffInfo[i].id == tableId && mListEffInfo[i].type == type)
			{
				effInfo = mListEffInfo[i];
				break;
			}
		}

		if (effInfo == null)
		{
			Debug.LogError(tableId + "번에 " + type.ToString() + "타입 이펙트정보가 없습니다.");
			return null;
		}

		return effInfo;
	}

	public ParticleSystem GetEffectOrNull( int tableId, eType type, eParticleFindType particleFindType = eParticleFindType.Deactive ) {
		sEffInfo effInfo = null;

		for( int i = 0; i < mListEffInfo.Count; i++ ) {
			if( mListEffInfo[i].id == tableId && mListEffInfo[i].type == type ) {
				effInfo = mListEffInfo[i];
				break;
			}
		}

		if( effInfo == null ) {
			Debug.LogError( tableId + "번에 " + type.ToString() + "타입 이펙트정보가 없습니다." );
			return null;
		}

		sParticleInfo particleInfo = GetParticleInfoOrNull( effInfo.name, particleFindType );
		if( particleInfo == null ) {
			return null;
		}

		return particleInfo.Particle;
	}

	public float Play( Unit parent, int tableId, eType type, float endTime = 0.0f, bool skipRegistStop = false, bool keepOnDie = false ) {
		// 하 옵션에 2, 3번 타입, 중 옵션에 3번 타입은 이펙트 로드 스킵했으므로 1번 타입으로 가져온다.
		if ( ( FSaveData.Instance.Graphic == 0 && ( type == eType.Each_Monster_Critical_Hit || type == eType.Each_Monster_DamageOnly_Hit ) ) ||
		     ( FSaveData.Instance.Graphic == 1 && type == eType.Each_Monster_DamageOnly_Hit ) ) {
			type = eType.Each_Monster_Normal_Hit;
		}

		sEffInfo effInfo = null;
		for ( int i = 0; i < mListEffInfo.Count; i++ ) {
            if ( mListEffInfo[i] == null ) {
                continue;
			}

			if ( mListEffInfo[i].id == tableId && mListEffInfo[i].type == type ) {
				effInfo = mListEffInfo[i];
				break;
			}
		}

		if ( effInfo == null ) {
			for ( int i = 0; i < mListEffInfo.Count; i++ ) {
                if ( mListEffInfo == null ) {
                    continue;
				}

				if ( mListEffInfo[i].id == tableId && mListEffInfo[i].type == eType.Each_Monster_Normal_Hit ) {
					effInfo = mListEffInfo[i];
					break;
				}
			}

			if ( effInfo == null ) {
				Debug.LogError( tableId + "번에 " + type.ToString() + "타입 이펙트 정보가 없습니다." );
				return 0.0f;
			}
		}

		sParticleInfo particleInfo = GetParticleInfoOrNull( effInfo.name, eParticleFindType.Deactive );
		if ( particleInfo == null || particleInfo.Particle == null ) {
			return 0.0f;
		}

		if ( particleInfo.CrStop != null ) {
			Utility.StopCoroutine( this, ref particleInfo.CrStop );
		}

		if ( endTime > 0.0f ) {
			ParticleSystem.MainModule main = particleInfo.Particle.main;
			main.duration = endTime;
		}

		particleInfo.Particle.gameObject.SetActive( true );

		Transform bone = null;
		if ( effInfo.attach.CompareTo( "UI" ) == 0 ) {
			bone = World.Instance.StageType == eSTAGETYPE.STAGE_PVP ? World.Instance.UIPVP.transform : World.Instance.UIPlay.transform;
		}
		else if ( parent != null && parent.aniEvent != null ) {
			bone = parent.aniEvent ? parent.aniEvent.GetBoneByName( effInfo.attach ) : null;
		}

		Vector3 boneRot = Vector3.zero;

		if ( effInfo.follow ) {
			if ( bone != null ) {
				particleInfo.Particle.transform.SetParent( bone );
				Utility.InitTransform( particleInfo.Particle.gameObject );

				particleInfo.Particle.transform.localPosition = effInfo.addedPos;
			}
			else {
				particleInfo.Particle.transform.SetParent( parent.transform );
				Utility.InitTransform( particleInfo.Particle.gameObject );

				particleInfo.Particle.transform.localPosition = effInfo.addedPos;
			}

			particleInfo.Particle.transform.localRotation = Quaternion.Euler( effInfo.addedRot.x, effInfo.addedRot.y, effInfo.addedRot.z );
		}
		else {
			if ( bone != null ) {
				particleInfo.Particle.transform.position = bone.position + effInfo.addedPos;
			}
			else if ( parent ) {
				particleInfo.Particle.transform.localPosition = parent.transform.position + effInfo.originalPos + effInfo.addedPos;
			}

            if ( parent ) {
                Vector3 parentRot = parent.transform.localRotation.eulerAngles;
                particleInfo.Particle.transform.rotation = Quaternion.Euler( parentRot.x + boneRot.x + effInfo.originalRot.x + effInfo.addedRot.x,
                                                                             parentRot.y + boneRot.y + effInfo.originalRot.y + effInfo.addedRot.y,
                                                                             parentRot.z + boneRot.z + effInfo.originalRot.z + effInfo.addedRot.z );
            }
		}

		if ( effInfo.CameraAni && World.Instance.InGameCamera ) {
			World.Instance.InGameCamera.PlayAnimation( effInfo.CameraAni, null );
		}

		particleInfo.Particle.Play();

		if ( !string.IsNullOrEmpty( effInfo.sndName ) )
			SoundManager.Instance.PlaySnd( (SoundManager.eSoundType)effInfo.sndMixerIndex, effInfo.sndName );

		if ( !skipRegistStop ) {
			particleInfo.CrStop = StartCoroutine( Stop( parent, particleInfo, effInfo, keepOnDie ) );
		}

        if ( World.Instance.StageType != eSTAGETYPE.STAGE_PVP ) {
            World.Instance.UIPlay.m_screenEffect.PlayConnectCommonEffect( tableId );
        }

		return particleInfo.Particle.main.duration;
	}

	public float Play(Vector3 pos, int tableId, eType type)
    {
		// 하 옵션에 2, 3번 타입, 중 옵션에 3번 타입은 이펙트 로드 스킵했으므로 1번 타입으로 가져온다.
		if ((FSaveData.Instance.Graphic == 0 && (type == eType.Each_Monster_Critical_Hit || type == eType.Each_Monster_DamageOnly_Hit)) ||
		   (FSaveData.Instance.Graphic == 1 && type == eType.Each_Monster_DamageOnly_Hit))
		{
			type = eType.Each_Monster_Normal_Hit;
		}

		//sEffInfo effInfo = mListEffInfo.Find(x => x.id == tableId && x.type == type);// && !x.ps.gameObject.activeSelf);
		sEffInfo effInfo = null;
        for (int i = 0; i < mListEffInfo.Count; i++)
        {
            if (mListEffInfo[i].id == tableId && mListEffInfo[i].type == type)
            {
                effInfo = mListEffInfo[i];
                break;
            }
        }

        if (effInfo == null)
        {
            Debug.LogError(tableId + "번에 " + type.ToString() + "타입 이펙트 정보가 없습니다.");
            return 0.0f;
        }

        sParticleInfo particleInfo = GetParticleInfoOrNull(effInfo.name, eParticleFindType.Deactive);
        if (particleInfo == null)
        {
            return 0.0f;
        }

        if (effInfo.CameraAni)
        {
            World.Instance.InGameCamera.PlayAnimation(effInfo.CameraAni, null);
        }

        particleInfo.Particle.gameObject.SetActive(true);
        particleInfo.Particle.transform.position = pos;
        particleInfo.Particle.Play();

        if (!string.IsNullOrEmpty(effInfo.sndName))
        {
            SoundManager.Instance.PlaySnd((SoundManager.eSoundType)effInfo.sndMixerIndex, effInfo.sndName);
        }

        if (particleInfo.CrStop != null)
        {
            Utility.StopCoroutine(this, ref particleInfo.CrStop);
        }

        particleInfo.CrStop = StartCoroutine(Stop(null, particleInfo, effInfo));
        World.Instance.UIPlay.m_screenEffect.PlayConnectCommonEffect(tableId);

        return particleInfo.Particle.main.duration;
    }

    public float Play(int tableId, eType type)
    {
		// 하 옵션에 2, 3번 타입, 중 옵션에 3번 타입은 이펙트 로드 스킵했으므로 1번 타입으로 가져온다.
		if ((FSaveData.Instance.Graphic == 0 && (type == eType.Each_Monster_Critical_Hit || type == eType.Each_Monster_DamageOnly_Hit)) ||
		   (FSaveData.Instance.Graphic == 1 && type == eType.Each_Monster_DamageOnly_Hit))
		{
			type = eType.Each_Monster_Normal_Hit;
		}

		//sEffInfo effInfo = mListEffInfo.Find(x => x.id == tableId && x.type == type);// && !x.ps.gameObject.activeSelf);
		sEffInfo effInfo = null;
        for (int i = 0; i < mListEffInfo.Count; i++)
        {
            if (mListEffInfo[i].id == tableId && mListEffInfo[i].type == type)
            {
                effInfo = mListEffInfo[i];
                break;
            }
        }

        if (effInfo == null)
        {
            Debug.LogError(tableId + "번에 " + type.ToString() + "타입 이펙트 정보가 없습니다.");
            return 0.0f;
        }

        sParticleInfo particleInfo = GetParticleInfoOrNull(effInfo.name, eParticleFindType.Deactive);
        if (particleInfo == null)
        {
            return 0.0f;
        }

        if (effInfo.CameraAni)
        {
            World.Instance.InGameCamera.PlayAnimation(effInfo.CameraAni, null);
        }

        particleInfo.Particle.gameObject.SetActive(true);
        particleInfo.Particle.Play();

        if (!string.IsNullOrEmpty(effInfo.sndName))
            SoundManager.Instance.PlaySnd((SoundManager.eSoundType)effInfo.sndMixerIndex, effInfo.sndName);

        if (particleInfo.CrStop != null)
        {
            Utility.StopCoroutine(this, ref particleInfo.CrStop);
        }

        particleInfo.CrStop = StartCoroutine(Stop(null, particleInfo, effInfo));
        World.Instance.UIPlay.m_screenEffect.PlayConnectCommonEffect(tableId);

        return particleInfo.Particle.main.duration;
    }

    public float Play( GameObject parent, int tableId, eType type, float endTime = 0.0f )
    {
		// 하 옵션에 2, 3번 타입, 중 옵션에 3번 타입은 이펙트 로드 스킵했으므로 1번 타입으로 가져온다.
		if ((FSaveData.Instance.Graphic == 0 && (type == eType.Each_Monster_Critical_Hit || type == eType.Each_Monster_DamageOnly_Hit)) ||
		   (FSaveData.Instance.Graphic == 1 && type == eType.Each_Monster_DamageOnly_Hit))
		{
			type = eType.Each_Monster_Normal_Hit;
		}

		sEffInfo effInfo = mListEffInfo.Find(x => x.id == tableId && x.type == type);// && !x.ps.gameObject.activeSelf);
        if (effInfo == null)
        {
            Debug.LogError(tableId + "번에 " + type.ToString() + "타입 이펙트 정보가 없습니다.");
            return 0.0f;
        }

        sParticleInfo particleInfo = GetParticleInfoOrNull(effInfo.name, eParticleFindType.Deactive);
        if (particleInfo == null)
        {
            return 0.0f;
        }

        if (effInfo.CameraAni)
        {
            World.Instance.InGameCamera.PlayAnimation(effInfo.CameraAni, null);
        }

        particleInfo.Particle.gameObject.transform.SetParent(parent.transform);
        Utility.InitTransform(particleInfo.Particle.gameObject);

        particleInfo.Particle.gameObject.SetActive(true);
        particleInfo.Particle.Play();

        if (!string.IsNullOrEmpty(effInfo.sndName))
        {
            SoundManager.Instance.PlaySnd((SoundManager.eSoundType)effInfo.sndMixerIndex, effInfo.sndName);
        }

        if (particleInfo.CrStop != null)
        {
            Utility.StopCoroutine(this, ref particleInfo.CrStop);
        }

        if (!particleInfo.Particle.main.loop)
        {
            particleInfo.CrStop = StartCoroutine(Stop(null, particleInfo, effInfo));
        }

        World.Instance.UIPlay.m_screenEffect.PlayConnectCommonEffect(tableId);
        return particleInfo.Particle.main.duration;
    }

    public float Play(Unit startUnit, Unit target, int tableId, eType type, int endEffectId, eType endEffectType)
    {
		// 하 옵션에 2, 3번 타입, 중 옵션에 3번 타입은 이펙트 로드 스킵했으므로 1번 타입으로 가져온다.
		if ((FSaveData.Instance.Graphic == 0 && (type == eType.Each_Monster_Critical_Hit || type == eType.Each_Monster_DamageOnly_Hit)) ||
		   (FSaveData.Instance.Graphic == 1 && type == eType.Each_Monster_DamageOnly_Hit))
		{
			type = eType.Each_Monster_Normal_Hit;
		}

		sEffInfo effInfo = mListEffInfo.Find(x => x.id == tableId && x.type == type);// && !x.ps.gameObject.activeSelf);
        if (effInfo == null)
        {
            Debug.LogError(tableId + "번에 " + type.ToString() + "타입 이펙트 정보가 없습니다.");
            return 0.0f;
        }

        sParticleInfo particleInfo = GetParticleInfoOrNull(effInfo.name, eParticleFindType.Deactive);
        if (particleInfo == null)
        {
            return 0.0f;
        }

        if (effInfo.CameraAni)
        {
            World.Instance.InGameCamera.PlayAnimation(effInfo.CameraAni, null);
        }

        particleInfo.Particle.gameObject.SetActive(true);
        particleInfo.Particle.Play();

        if (!string.IsNullOrEmpty(effInfo.sndName))
        {
            SoundManager.Instance.PlaySnd((SoundManager.eSoundType)effInfo.sndMixerIndex, effInfo.sndName);
        }

        World.Instance.UIPlay.m_screenEffect.PlayConnectCommonEffect(tableId);

        if(particleInfo.CrStop != null)
        {
            Utility.StopCoroutine(this, ref particleInfo.CrStop);
        }

        particleInfo.CrStop = StartCoroutine(UpdateMove(effInfo, particleInfo, startUnit, target, endEffectId, endEffectType));
        return 0.5f; // UpdateMove Duration 값
    }

    public void StopAll(string exceptName = "")
    {
        for(int i = 0; i < mListParticle.Count; i++)
        {
            sParticleInfo info = mListParticle[i];
            if (info == null || info.Particle == null || (!string.IsNullOrEmpty(exceptName) && info.Name == exceptName))
            {
                continue;
            }

            if(info.CrStop != null)
            {
                Utility.StopCoroutine(this, ref info.CrStop);
            }

            if (info.Particle.gameObject.activeSelf)
            {
                info.Particle.gameObject.SetActive(false);
            }
        }
    }

    public void StopAllByParent(Transform parent)
    {
        for (int i = 0; i < mListParticle.Count; i++)
        {
            sParticleInfo info = mListParticle[i];
            if (info == null || info.Particle == null || info.Particle.transform.parent != parent)
            {
                continue;
            }

            if (info.CrStop != null)
            {
                Utility.StopCoroutine(this, ref info.CrStop);
            }

            if (info.Particle.gameObject.activeSelf)
            {
                info.Particle.gameObject.SetActive(false);
            }
        }
    }

    // AniEvent에 Keep 속성을 갖는 이펙트 처리를 위한 함수 
    public void RegisterStopEff(AniEvent.sEffect eff, Transform parent)
    {
        if (eff.ps == null)
        {
            return;
        }

        if (eff.cr != null)
        {
            Utility.StopCoroutine(this, ref eff.cr);
        }

        eff.cr = StartCoroutine(StartStopEff(eff, parent));
    }

    public void RegisterStopEff(ParticleSystem ps, Transform parent, System.Action endCallback = null)
    {
        if (ps == null)
        {
            return;
        }

        StartCoroutine(StartStopEff(ps, parent, endCallback));
    }

    public void RegisterStopEffByDuration(ParticleSystem ps, Transform parent, float duration, System.Action endCallback = null)
    {
        if (ps == null)
        {
            return;
        }

        StartCoroutine(StartStopEffByDuration(ps, parent, duration, endCallback));
    }

    public void RegisterStopEffAtOwnerDie(ParticleSystem ps, Unit owner)
    {
        StartCoroutine(UpdateStopEffAtOwnerDie(ps, owner));
    }

    private IEnumerator UpdateStopEffAtOwnerDie(ParticleSystem ps, Unit owner)
    {
        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (owner.curHp > 0.0f)
        {
            if(!owner.IsShowMesh && ps.gameObject.activeSelf)
            {
                ps.gameObject.SetActive(false);
            }
            else if(owner.IsShowMesh && !ps.gameObject.activeSelf)
            {
                ps.gameObject.SetActive(true);
            }

            yield return mWaitForFixedUpdate;
        }

        ps.gameObject.SetActive(false);
    }

    public void StopEffImmediate(int effId, eType type, Transform parent)
    {
		// 하 옵션에 2, 3번 타입, 중 옵션에 3번 타입은 이펙트 로드 스킵했으므로 1번 타입으로 가져온다.
		if ((FSaveData.Instance.Graphic == 0 && (type == eType.Each_Monster_Critical_Hit || type == eType.Each_Monster_DamageOnly_Hit)) ||
		   (FSaveData.Instance.Graphic == 1 && type == eType.Each_Monster_DamageOnly_Hit))
		{
			type = eType.Each_Monster_Normal_Hit;
		}

		sEffInfo effInfo = mListEffInfo.Find(x => x.id == effId && x.type == type);// && x.ps.gameObject.activeSelf);
        if (effInfo == null)
        {
            Debug.LogError(effId + "번에 " + type.ToString() + "타입 이펙트 정보가 없습니다.");
            return;
        }

        sParticleInfo particleInfo = GetParticleInfoOrNull(effInfo.name, eParticleFindType.Active);
        if(particleInfo == null)
        {
            return;
        }

        particleInfo.Particle.transform.SetParent(parent);
        particleInfo.Particle.gameObject.SetActive(false);

        if(effInfo.CameraAni)
        {
            World.Instance.InGameCamera.StopAni();
        }

        ParticleSystem.MainModule main = particleInfo.Particle.main;
        main.duration = effInfo.originalPSDuration;
    }

    public void StopEffImmediate(ParticleSystem ps, Transform parent, System.Action endCallback = null)
    {
        if (ps)
        {
            ps.transform.SetParent(parent);
            ps.gameObject.SetActive(false);
        }

        endCallback?.Invoke();
    }

    public void Detach(Unit parent, int tableId, eType type)
    {
		// 하 옵션에 2, 3번 타입, 중 옵션에 3번 타입은 이펙트 로드 스킵했으므로 1번 타입으로 가져온다.
		if ((FSaveData.Instance.Graphic == 0 && (type == eType.Each_Monster_Critical_Hit || type == eType.Each_Monster_DamageOnly_Hit)) ||
		   (FSaveData.Instance.Graphic == 1 && type == eType.Each_Monster_DamageOnly_Hit))
		{
			type = eType.Each_Monster_Normal_Hit;
		}

		sEffInfo effInfo = mListEffInfo.Find(x => x.id == tableId && x.type == type);// && !x.ps.gameObject.activeSelf);
        if (effInfo == null)
        {
            effInfo = mListEffInfo.Find(x => x.id == tableId && x.type == eType.Each_Monster_Normal_Hit);
            if (effInfo == null)
            {
                Debug.LogError(tableId + "번에 " + type.ToString() + "타입 이펙트 정보가 없습니다.");
                return;
            }
        }

        sParticleInfo particleInfo = GetParticleInfoOrNull(effInfo.name, eParticleFindType.Deactive);
        if (particleInfo == null)
        {
            return;
        }

        particleInfo.Particle.transform.SetParent(null);
    }

    public void ChangeDuration(int tableId, eType type, float duration)
    {
		// 하 옵션에 2, 3번 타입, 중 옵션에 3번 타입은 이펙트 로드 스킵했으므로 1번 타입으로 가져온다.
		if ((FSaveData.Instance.Graphic == 0 && (type == eType.Each_Monster_Critical_Hit || type == eType.Each_Monster_DamageOnly_Hit)) ||
		   (FSaveData.Instance.Graphic == 1 && type == eType.Each_Monster_DamageOnly_Hit))
		{
			type = eType.Each_Monster_Normal_Hit;
		}

		sEffInfo effInfo = mListEffInfo.Find(x => x.id == tableId && x.type == type);// && !x.ps.gameObject.activeSelf);
        if (effInfo == null)
        {
            effInfo = mListEffInfo.Find(x => x.id == tableId && x.type == eType.Each_Monster_Normal_Hit);
            if (effInfo == null)
            {
                Debug.Log(tableId + "번에 " + type.ToString() + "타입 이펙트 정보가 없습니다.");
                return;
            }
        }

        sParticleInfo particleInfo = GetParticleInfoOrNull(effInfo.name, eParticleFindType.Deactive);
        if (particleInfo == null)
        {
            return;
        }

        ParticleSystem.MainModule main = particleInfo.Particle.main;
        main.duration = duration;
    }

    private void Awake()
    {
        DontDestroyOnLoad();
    }

    private void AddRRaceEffect(int effectCnt, int id, eType effectType, string effectPath)
    {
        for (int i = 0; i < effectCnt; i++)
        {
            sParticleInfo info = new sParticleInfo();

            info.Name = effectPath;
            info.Particle = ResourceMgr.Instance.CreateFromAssetBundle<ParticleSystem>("effect", effectPath);
            info.Particle.gameObject.SetActive(false);

            mListParticle.Add(info);
        }

        for (int i = 0; i < effectCnt; i++)
        {
            sEffInfo effInfo = new sEffInfo();

            effInfo.name = effectPath;
            effInfo.id = id;
            effInfo.type = effectType;

            sParticleInfo particleInfo = GetParticleInfoOrNull(effInfo.name, eParticleFindType.None);
            if (particleInfo == null)
            {
                continue;
            }

            effInfo.originalPSDuration = particleInfo.Particle.main.duration;

            mListEffInfo.Add(effInfo);
        }
    }

	private void SetCheckList() {
		mListCheckPlayerId.Clear();
		mListCheckSupporterId.Clear();

		if( World.Instance.StageType == eSTAGETYPE.STAGE_PVP ) {
			WorldPVP worldPVP = World.Instance as WorldPVP;
			if( worldPVP ) {
				for( int i = 0; i < worldPVP.PlayerChars.Length; i++ ) {
					Player player = worldPVP.PlayerChars[i];
					if( player == null ) {
						continue;
					}

					mListCheckPlayerId.Add( player.tableId );

					for( int j = 0; j < player.ListEquipCard.Count; j++ ) {
						mListCheckSupporterId.Add( player.ListEquipCard[j].TableID );
					}
				}

				for( int i = 0; i < worldPVP.OpponentChars.Length; i++ ) {
					Player player = worldPVP.OpponentChars[i];
					if( player == null ) {
						continue;
					}

					mListCheckPlayerId.Add( player.tableId );

					for( int j = 0; j < player.ListEquipCard.Count; j++ ) {
						mListCheckSupporterId.Add( player.ListEquipCard[j].TableID );
					}
				}
			}
		}
		else if( World.Instance.StageType == eSTAGETYPE.STAGE_TOWER ) {
			for( int i = 0; i < World.Instance.ListPlayer.Count; i++ ) {
				Player player = World.Instance.ListPlayer[i];
				if( player == null ) {
					continue;
				}

				mListCheckPlayerId.Add( player.tableId );

				for( int j = 0; j < player.ListEquipCard.Count; j++ ) {
					mListCheckSupporterId.Add( player.ListEquipCard[j].TableID );
				}
			}
		}
		else {
            if( World.Instance.ListPlayer.Count > 0 ) {
                for( int i = 0; i < World.Instance.ListPlayer.Count; i++ ) {
                    mListCheckPlayerId.Add( World.Instance.ListPlayer[i].tableId );

                    for( int j = 0; j < World.Instance.ListPlayer[i].ListEquipCard.Count; j++ ) {
                        mListCheckSupporterId.Add( World.Instance.ListPlayer[i].ListEquipCard[j].TableID );
                    }
                }
            }
            else {
                mListCheckPlayerId.Add( 1 );
                mListCheckPlayerId.Add( 2 );
                mListCheckPlayerId.Add( 3 );
            }
		}
	}

	List<sParticleInfo> listParticleInfos = new List<sParticleInfo>();
    private sParticleInfo GetParticleInfoOrNull(string name, eParticleFindType findType)
    {
        if (findType == eParticleFindType.None)
        {
            sParticleInfo particleInfo = null;
            for (int i = 0; i < mListParticle.Count; i++)
            {
                if (mListParticle[i].Name.CompareTo(name) == 0)
                {
                    particleInfo = mListParticle[i];
                    break;
                }
            }

            if (particleInfo == null)
            {
                Debug.LogError(name + " 파티클이 없습니다.");
            }

            return particleInfo;
        }
        else if (findType == eParticleFindType.Active)
        {
            sParticleInfo particleInfo = null;
            for (int i = 0; i < mListParticle.Count; i++)
            {
                if (mListParticle[i].Name.Equals(name) && mListParticle[i].Particle.gameObject.activeSelf)
                {
                    particleInfo = mListParticle[i];
                    break;
                }
            }

            if (particleInfo == null)
            {
                Debug.LogWarning(name + " 파티클들 중에 활성화된 파티클이 없습니다.");
            }

            return particleInfo;
        }
        else if (findType == eParticleFindType.Deactive)
        {
            listParticleInfos.Clear();
            for (int i = 0; i < mListParticle.Count; i++)
            {
                if (mListParticle[i].Name.Equals(name))
                    listParticleInfos.Add(mListParticle[i]);
            }

            if (listParticleInfos == null || listParticleInfos.Count <= 0)
            {
                Debug.LogError(name + " 파티클이 없습니다.");
            }
            else
            {
                //sParticleInfo particleInfo = listParticleInfos.Find(x => !x.Particle.gameObject.activeSelf);
                sParticleInfo particleInfo = null;
                for (int i = 0; i < listParticleInfos.Count; i++)
                {
                    if (!listParticleInfos[i].Particle.gameObject.activeSelf)
                    {
                        particleInfo = listParticleInfos[i];
                        break;
                    }
                }

                if (particleInfo == null)
                {
                    Debug.LogWarning(name + " 파티클들 중에 비활성화된 파티클이 없어서 활성화된거 하나 빼옴");

                    int rand = Random.Range(0, listParticleInfos.Count);
                    return listParticleInfos[rand];
                }
                else
                {
                    return particleInfo;
                }
            }
        }

        return null;
    }

    private IEnumerator UpdateMove(sEffInfo effInfo, sParticleInfo particleInfo, Unit startUnit, Unit target, int endEffectId, eType endEffectType)
    {
        particleInfo.Particle.transform.position = startUnit.MainCollider.GetCenterPos();

		int rand = Random.Range(0, 3);
		Vector3 r1 = Vector3.zero;
		if(rand == 1)
		{
			r1 = startUnit.transform.right;
		}
		else
		{
			r1 = -startUnit.transform.right;
		}

		float f1 = Utility.GetRandom(0.3f, 1.0f, 10.0f);
		float f2 = Utility.GetRandom(0.3f, 1.0f, 10.0f);

		Vector3 r = (Vector3.up * f1) + (r1 * f2);

		Vector3 v1 = ((startUnit.transform.position - target.transform.position) + r).normalized;
        Vector3 v2 = Vector3.zero;
        Vector3 v = Vector3.zero;

        Vector3 targetCenterPos = Vector3.zero;

        float time = 0.0f;
        float t = 0.0f;

        //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
        while (true)
        {
            if (!particleInfo.Particle.gameObject.activeSelf)
            {
                particleInfo.Particle.gameObject.SetActive(true);
                particleInfo.Particle.Play();
            }

            targetCenterPos = target.GetCenterPos();

            time += Time.fixedDeltaTime / 0.5f;
            t = Mathf.SmoothStep(0.0f, 1.0f, time);

            v2 = (targetCenterPos - particleInfo.Particle.transform.position).normalized;
            v = ((v1 * (1.0f - t)) + (v2 * t)).normalized;

            particleInfo.Particle.transform.rotation = Quaternion.LookRotation(targetCenterPos);
            particleInfo.Particle.transform.position += (v * 25.0f * Time.fixedDeltaTime);

            if (t >= 1.0f && Vector3.Distance(particleInfo.Particle.transform.position, targetCenterPos) <= target.MainCollider.radius)
            {
                break;
            }

            yield return mWaitForFixedUpdate;
        }

		StopEffImmediate(particleInfo.Particle, null);
		//StopEffImmediate(effInfo.id, effInfo.type, null);
        particleInfo.CrStop = null;

        if (endEffectId > 0)
        {
            Play(target, endEffectId, endEffectType);
        }
    }

    private IEnumerator Stop(Unit parent, sParticleInfo particleInfo, sEffInfo effInfo, bool keepOnDie = false)
    {
        if (effInfo == null || particleInfo.Particle == null || particleInfo.Particle.main.loop)
        {
            yield break;
        }

        if (parent)
        {
            float checkTime = 0.0f;
            bool end = false;

            //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
            while (!end)
            {
                checkTime += Time.deltaTime;
                if (World.Instance.IsEndGame || checkTime >= particleInfo.Particle.main.duration)
                {
                    end = true;
                }
                else if(!keepOnDie && (parent.curHp <= 0.0f || !parent.IsActivate()))
                {
                    end = true;
                }

                yield return mWaitForFixedUpdate;
            }
        }
        else
        {
            yield return new WaitForSeconds(particleInfo.Particle.main.duration);
        }

        particleInfo.Particle.transform.SetParent(null);
        particleInfo.Particle.gameObject.SetActive(false);
        particleInfo.CrStop = null;

        ParticleSystem.MainModule main = particleInfo.Particle.main;
        main.duration = effInfo.originalPSDuration;
    }

    private IEnumerator StartStopEff(AniEvent.sEffect eff, Transform parent)
    {
        Unit unit = parent.GetComponentInParent<Unit>();
        if (unit)
        {
            float checkTime = 0.0f;

            //WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
            while (checkTime < eff.ps.main.duration)
            {
                checkTime += ((unit && unit.IsActivate()) ? unit.fixedDeltaTime : Time.fixedDeltaTime);
                yield return mWaitForFixedUpdate;
            }
        }
        else
        {
            yield return new WaitForSeconds(eff.ps.main.duration);
        }

        if (eff != null && eff.ps)
        {
            eff.ps.transform.SetParent(parent);
            eff.ps.gameObject.SetActive(false);

            eff.cr = null;
        }
        else
        {
            eff.cr = null;
        }
    }

    private IEnumerator StartStopEff(ParticleSystem ps, Transform parent, System.Action endCallback = null)
    {
        yield return new WaitForSeconds(ps.main.duration);
        StopEffImmediate(ps, parent, endCallback);
    }

    private IEnumerator StartStopEffByDuration(ParticleSystem ps, Transform parent, float duration, System.Action endCallback = null)
    {
        yield return new WaitForSeconds(duration);
        StopEffImmediate(ps, parent, endCallback);
    }

	private int CalcParticleTotalCount(List<GameClientTable.CommonEffect.Param> listHitEffParam)
	{
		int count = 0;

		// 필요한 파티클만 먼저 생성 (중복X)
		for (int i = 0; i < listHitEffParam.Count; i++)
		{
			GameClientTable.CommonEffect.Param param = listHitEffParam[i];
			if (param == null || string.IsNullOrEmpty(param.EffectPrefab))
			{
				continue;
			}

			if ((FSaveData.Instance.Graphic == 0 && (param.Type == 2 || param.Type == 3)) ||
			    (FSaveData.Instance.Graphic == 1 && param.Type == 3)) // 하 옵션은 타입 2,3번 스킵, 중 옵션은 타입 3번 스킵
			{
				continue;
			}

			sParticleInfo find = FindParticleInfoOrNull(param.EffectPrefab);
			if (find != null)
			{
				continue;
			}

			if (param.UnitType > 0)
			{
				if (param.UnitType == (int)eCharacterType.Character && mListCheckPlayerId.Count > 0)
				{
					if (mListCheckPlayerId.Find(x => x == param.UnitTableId) == 0)
					{
						continue;
					}
				}
				else if (param.UnitType == (int)eCharacterType.Supporter)
				{
					if (mListCheckSupporterId.Count <= 0 ||
						(mListCheckSupporterId.Count > 0 && mListCheckSupporterId.Find(x => x == param.UnitTableId) == 0))
					{
						continue;
					}
				}
			}

			int effCount = 1; // ((eType)param.Type <= eType.Each_Monster_DamageOnly_Hit) ? World.Instance.EnemyMgr.maxMonsterCountInSpawnGroup : 1;

			if ((eType)param.Type <= eType.Each_Monster_DamageOnly_Hit)
			{
				if (World.Instance.EnemyMgr.MaxSpawnMonsterCount > 0)
				{
					effCount = Mathf.Min(World.Instance.EnemyMgr.MaxSpawnMonsterCount, World.Instance.EnemyMgr.maxMonsterCountInSpawnGroup);
				}
				else
				{
					effCount = World.Instance.EnemyMgr.maxMonsterCountInSpawnGroup;
				}
			}

			// PVP땐 같은 이펙트(예를 들면 서포터)가 있을 수 있어서 2개씩 생성
			if (effCount == 1 && World.Instance.StageType == eSTAGETYPE.STAGE_PVP)
			{
				effCount = 2;
			}

			for (int j = 0; j < effCount; j++)
			{
				++count;
			}
		}

		return count;
	}

	private int CalcEffInfoTotalCount(List<GameClientTable.CommonEffect.Param> listHitEffParam)
	{
		int count = 0;

		// 파티클 정보는 모두 생성
		for (int i = 0; i < listHitEffParam.Count; i++)
		{
			GameClientTable.CommonEffect.Param param = listHitEffParam[i];
			if (param == null || string.IsNullOrEmpty(param.EffectPrefab))
			{
				continue;
			}

			int effCount = ((eType)param.Type <= eType.Each_Monster_DamageOnly_Hit) ? World.Instance.EnemyMgr.maxMonsterCountInSpawnGroup : 1;
			for (int j = 0; j < effCount; j++)
			{
				if ((FSaveData.Instance.Graphic == 0 && (param.Type == 2 || param.Type == 3)) ||
					(FSaveData.Instance.Graphic == 1 && param.Type == 3)) // 하 옵션은 타입 2,3번 스킵, 중 옵션은 타입 3번 스킵
				{
					continue;
				}

				sParticleInfo particleInfo = FindParticleInfoOrNull(param.EffectPrefab);
				if (particleInfo == null)
				{
					continue;
				}

				++count;
			}
		}

		return count;
	}

	private sParticleInfo FindParticleInfoOrNull(string name)
	{
		for(int i = 0; i < mListParticle.Count; i++)
		{
			if(mListParticle[i].Name.CompareTo(name) == 0)
			{
				return mListParticle[i];
			}
		}

		return null;	
	}
}
