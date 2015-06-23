using UnityEngine;
using System.Collections;

// 대어보너스를 위한 클래스, 콜리더박스에 닿으면 Great나 Perfect중 하나의 보너스가 주어진다.
public class BonusGageHit : MonoBehaviour
{
	private bool m_Hit = false;
	public bool Hit
	{
		get { return m_Hit; }
	}


	void OnTriggerEnter2D(Collider2D other)
	{
		//Debug.Log(other.name);

		m_Hit = true;
	}

	void OnTriggerExit2D(Collider2D other)
	{
		//Debug.Log(other.name);

		m_Hit = false;
	}
}