using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TapButtonController : MonoBehaviour
{
	public BonusScrollbarController BonusScrollbarController = null;
	public BonusGageHit Great = null;
	public BonusGageHit Perfect = null;
	public Text BonusHitText = null;

	public Scrollbar TensionScrollbar = null;
	public Scrollbar PowerScrollbar = null;

	public Button PressButton = null;

	public GameObject FishState = null;


	// TAP버튼을 누를 때 실행된다.
	public void OnTap()
	{
		if(BonusScrollbarController == null)
		{
			return;
		}
		
		BonusScrollbarController.StopMove();
		
		if(Great == null)
		{
			return;
		}
		
		if(Perfect == null)
		{
			return;
		}

		BonusHitText.gameObject.SetActive(true);
		
		if(Perfect.Hit)
		{
			BonusHitText.text = "Perfect";
		}
		else if(Great.Hit)
		{
			BonusHitText.text = "Great";
		}

		StartCoroutine(ShowGages());
	}

	// 2초 후 게임에서 보일 게이지나 버튼등을 나타나게 한다.
	IEnumerator ShowGages()
	{
		yield return new WaitForSeconds(2.0f);

		gameObject.SetActive(false);

		if(BonusScrollbarController != null)
		{
			BonusScrollbarController.gameObject.SetActive(false);
		}

		if(BonusHitText != null)
		{
			BonusHitText.gameObject.SetActive(false);
		}

		if(TensionScrollbar != null)
		{
			TensionScrollbar.gameObject.SetActive(true);
		}

		if(PowerScrollbar != null)
		{
			PowerScrollbar.gameObject.SetActive(true);
		}

		if(PressButton != null)
		{
			PressButton.gameObject.SetActive(true);
		}

		if(FishState != null)
		{
			FishState.SetActive(true);
		}
	}
}