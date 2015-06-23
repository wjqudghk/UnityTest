using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StartButtonController : MonoBehaviour
{
	public TapButtonController TapButtonController = null;
	public BonusScrollbarController BonusScrollbarController = null;


	// Start 버튼을 누를 때 실행된다.
	public void OnStart()
	{
		gameObject.SetActive(false);
		
		if(TapButtonController == null)
		{
			return;
		}
		
		TapButtonController.gameObject.SetActive(true);
		
		if(BonusScrollbarController == null)
		{
			return;
		}
		
		BonusScrollbarController.gameObject.SetActive(true);
	}
}