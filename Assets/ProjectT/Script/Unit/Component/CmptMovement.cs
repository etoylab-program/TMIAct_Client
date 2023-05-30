
using UnityEngine;
using System.Collections;


public class CmptMovement : CmptBase {
	private Vector3 m_v = Vector3.zero;


	public void UpdatePosition( Vector3 dir, float speed, bool freezeYPos ) {
		if( m_owner.rigidBody == null || m_owner.rigidBody.isKinematic == true ) {
			if( m_owner.holdPositionRef > 0 ) {
				dir = Vector3.zero;
			}

			m_owner.transform.localPosition += Vector3.Normalize( dir ) * speed * m_owner.fixedDeltaTime;
		}
		else {
			if( m_owner.holdPositionRef <= 0 ) {
				m_v = Vector3.Normalize( dir ) * speed * m_owner.fixedDeltaTime;

				if( freezeYPos == true )
					m_v.y = 0.0f;
			}
			else {
				m_v = Vector3.zero;
			}

			m_owner.rigidBody.MovePosition( m_owner.rigidBody.position + m_v );
		}
	}
}
