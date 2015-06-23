using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// 보너스 게이지 움직임.
public class BonusScrollbarController : MonoBehaviour
{
	private Scrollbar m_TapBonusScrollbar = null;

	public RectTransform BonusHandle = null;

	public float LocalPosXMin = -50.0f;
	public float LocalPosXMax = 50.0f;

	public float MovingSpeedMin = 0.7f;
	public float MovingSpeedMax = 1.5f;
	private float m_CurrentMovingSpeed = 0.7f;

	private bool m_GageUp = true;
	private bool m_StopMove = false;


	void OnEnable()
	{
		if(m_TapBonusScrollbar == null)
		{
			m_TapBonusScrollbar = GetComponent<Scrollbar>();
		}

		m_TapBonusScrollbar.value = 0.0f;

		if(BonusHandle != null)
		{
			Vector3 randomLocalPos = BonusHandle.localPosition;
			randomLocalPos.x = Random.Range(LocalPosXMin, LocalPosXMax);
			BonusHandle.localPosition = randomLocalPos;
		}

		m_CurrentMovingSpeed = Random.Range(MovingSpeedMin, MovingSpeedMax);

		m_StopMove = false;
	}

	void Update()
	{
		if(m_StopMove)
		{
			return;
		}

		if(m_TapBonusScrollbar == null)
		{
			return;
		}

		float dt = Time.deltaTime;

		if(m_GageUp)
		{
			m_TapBonusScrollbar.value += m_CurrentMovingSpeed * dt;
		}
		else
		{
			m_TapBonusScrollbar.value -= m_CurrentMovingSpeed * dt;
		}

		if(m_TapBonusScrollbar.value <= 0.0f)
		{
			m_GageUp = true;
		}
		else if(m_TapBonusScrollbar.value >= 1.0f)
		{
			m_GageUp = false;
		}
	}

	// 움직이던 게이지 멈춤
	public void StopMove()
	{
		m_StopMove = true;
	}
}