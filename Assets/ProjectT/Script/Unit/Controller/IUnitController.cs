
using UnityEngine;


public abstract class IUnitController : MonoBehaviour
{
    public enum eInputKind
    {
        Default = 0,
        Joystick,
        AI,
    }

    [System.Flags]
    public enum ELockBtnFlag
    {
        NONE        = 0,
        ATTACK      = 1 << 1,
        DASH        = 1 << 2,
        WEAPON      = 1 << 3,
        USKILL      = 1 << 4,
        SUPPORTER   = 1 << 5,
    }


    public bool LockPause { get; set; } = false;

    protected bool m_pause = false;
    protected Vector3 m_dir = Vector3.zero;
    protected eInputKind m_inputKind = eInputKind.Default;

    public bool isPause { get { return m_pause; } }
    public eInputKind inputKind { get { return m_inputKind; } }


    public abstract bool Pause(bool pause);
    public abstract bool HasDirection();
    public abstract Vector3 GetRawDirection();
    public abstract Vector3 GetDirection();
    
    public virtual void ResetBeforeDir()
    {
    }

    public virtual void LockDirection(bool lockDirection)
    {
    }

    public virtual void LockBtnFlag(ELockBtnFlag flag)
    {
    }
}
