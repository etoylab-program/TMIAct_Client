
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CmptJump : CmptBase
{
    [Header("Property")]
    public float m_jumpPower = 15.0f;
    public float m_shortJumpPowerRatio = 0.5f;
    public float m_shortJumpOnHitRatio = 0.07f;
    public float m_slowFallingRatio = 0.015f;
    public float m_slowFallingDuration = 1.0f;
    public float m_fastFallGravityRatio = 5.0f;


    private bool m_jumping = false;
    private Vector3 m_jumpPos;
    private bool m_slowFalling = false;
    private float m_gravityAcc = 0.0f;
    private bool m_holding = false;
    private float m_holdingDuration = 0.0f;

    private float m_originalJumpPower;
    private float m_originalShortJumpPowerRatio;
    private float m_originalslowFallingDurationn;

    public bool isHolding { get { return m_holding; } }
    public float highest { get; private set; }
    public float originalJumpPower { get { return m_originalJumpPower; } }


    protected override void Awake()
    {
        base.Awake();

        m_originalJumpPower = m_jumpPower;
        m_originalShortJumpPowerRatio = m_shortJumpPowerRatio;
        m_originalslowFallingDurationn = m_slowFallingDuration;
    }

    private void Init(bool enableSlowFalling, float slowFallingDuration)
    {
        m_jumping = true;
        m_slowFalling = enableSlowFalling;
        m_holding = false;

        StopCoroutine("StopHolding");

        if (slowFallingDuration == 0.0f)
            m_slowFallingDuration = m_originalslowFallingDurationn;
        else
            m_slowFallingDuration = slowFallingDuration;

        InitPos();

        m_owner.SetFloatingRigidBody();
        StopCoroutine("RestoreGravity");
    }

    public void InitPos()
    {
        m_gravityAcc = 1.0f;
        m_jumpPos = Vector3.zero;
    }

    public void StartJump(bool enableSlowFalling, float slowFallingDuration = 0.0f)
    {
        Init(enableSlowFalling, slowFallingDuration);
        m_jumpPos.y += m_jumpPower;
    }

    public void StartJump(float jumpPower, bool enableSlowFalling = false, float slowFallingDuration = 0.0f)
    {
        m_jumpPower = jumpPower;
        StartJump(enableSlowFalling, slowFallingDuration);
    }

    public void StartShortJump(float shortJumpPowerRatio, bool enableSlowFalling, float slowFallingDuration = 0.0f)
    {
        Init(enableSlowFalling, slowFallingDuration);
        m_jumpPos.y += m_jumpPower * shortJumpPowerRatio;
    }

    public void StartBounding(float shortJumpPowerRatio)
    {
        if (m_jumping == true)
            Holding(0.0f);
        else
        {
            Init(false, 0.0f);

            float ratio = m_shortJumpPowerRatio;
            if (shortJumpPowerRatio > 0.0f)
                ratio = shortJumpPowerRatio;

            m_jumpPos.y += m_jumpPower * ratio;
        }
    }

    public void SetFastFall()
    {
        m_gravityAcc = m_fastFallGravityRatio;

        m_slowFalling = false;
        StopCoroutine("RestoreGravity");
    }

    public void EnableSlowFalling(bool enable, float slowFallingDuration = 0.0f)
    {
        m_slowFalling = enable;

        if (slowFallingDuration == 0.0f)
            m_slowFallingDuration = m_originalslowFallingDurationn;
        else
            m_slowFallingDuration = slowFallingDuration;

        StartSlowFalling();
    }

    public void UpdateJump()
    {
        if (m_owner.isGrounded == true)
        {
            m_jumping = false;
            m_jumpPower = m_originalJumpPower;

            return;
        }

        if (m_holding == true)
            return;

        m_jumpPos.y += (Physics.gravity.y * m_gravityAcc) * m_owner.fixedDeltaTime;
        if (m_owner.isFalling == false && m_jumpPos.y < 0.0f)
        {
            //m_owner.isFalling = true;
            //Debug.Log(m_owner.name + " Jump Highest : " + m_owner.transform.position.y);
            m_owner.SetFallingRigidBody(false);

            if (m_slowFalling == true)
                StartSlowFalling();
        }
        else if (m_jumpPos.y >= 0.0f)
        {
            highest = 1.0f - (m_jumpPos.y / m_jumpPower);
            //Debug.Log(m_owner.tableName + " Jump Highest : " + highest);
        }

        m_owner.rigidBody.MovePosition(m_owner.transform.position + (m_jumpPos * m_owner.fixedDeltaTime));
    }

    private void StartSlowFalling()
    {
        m_gravityAcc = m_slowFallingRatio;

        StopCoroutine("RestoreGravity");
        StartCoroutine("RestoreGravity");

        m_jumpPos.y = 0.0f;
    }

    private IEnumerator RestoreGravity()
    {
        yield return new WaitForSeconds(m_slowFallingDuration);
        m_gravityAcc = 1.0f;
    }

    public void Holding(float duration)
    {
        m_holding = true;
        m_holdingDuration = duration;

        StopCoroutine("StopHolding");
        StartCoroutine("StopHolding");
    }

    private IEnumerator StopHolding()
    {
        yield return new WaitForSeconds(m_holdingDuration);
        m_holding = false;
    }

    public void Holding()
    {
        m_holding = true;
    }

    public void EndHolding()
    {
        m_holding = false;
    }
}
