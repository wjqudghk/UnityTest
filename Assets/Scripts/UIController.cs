using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// 게임의 결과 상태
public enum eResultState
{
	None, Fail, Success
}

public class UIController : MonoBehaviour
{
	private bool m_Initial = true;

	public Button StartButton = null;
	public Button TapButton = null;

	public Scrollbar BonusScrollbar = null;
	public Text BonusHitText = null;

	public Scrollbar TensionScrollbar = null;
	public Scrollbar PowerScrollbar = null;

	public Button PressButton = null;

	public GameObject FishState = null;

	public GameObject PowerImage = null;
	public GameObject SnatchLeftImage = null;
	public GameObject SnatchRightImage = null;

	public Text ResultText = null;
	private eResultState m_ResultState = eResultState.None;


	void Start()
	{
		StartCoroutine(CheckInit());
		StartCoroutine(CheckResult());
	}

	// 초기화 상태인지 체크한다. 초기화를 해야 할 상태이면 한 번 초기화 시켜준다.
	IEnumerator CheckInit()
	{
		while(true)
		{
			if(!m_Initial)
			{
				yield return new WaitForSeconds(0.1f);

				continue;
			}

			Init();

			m_Initial = false;
		}
	}

	// 게임 시작 상태로 초기화
	private void Init()
	{
		if(StartButton != null)
		{
			StartButton.gameObject.SetActive(true);
		}

		if(TapButton != null)
		{
			TapButton.gameObject.SetActive(false);
		}
		
		if(BonusScrollbar != null)
		{
			BonusScrollbar.gameObject.SetActive(false);
		}

		if(BonusHitText != null)
		{
			BonusHitText.text = "";
		}

		if(TensionScrollbar != null)
		{
			TensionScrollbar.gameObject.SetActive(false);
		}

		if(PowerScrollbar != null)
		{
			PowerScrollbar.gameObject.SetActive(false);
		}

		if(PressButton != null)
		{
			PressButton.gameObject.SetActive(false);
		}

		if(FishState != null)
		{
			FishState.SetActive(false);
		}

		if(PowerImage != null)
		{
			PowerImage.SetActive(false);
		}

		if(SnatchLeftImage != null)
		{
			SnatchLeftImage.SetActive(false);
		}

		if(SnatchRightImage != null)
		{
			SnatchRightImage.SetActive(false);
		}

		if(ResultText != null)
		{
			ResultText.gameObject.SetActive(false);
		}
	}

	// 결과(성공 / 실패)만 놔두고 나머지는 숨겨준다.
	private void HideAllWithoutResultText()
	{
		if(StartButton != null)
		{
			StartButton.gameObject.SetActive(false);
		}
		
		if(TapButton != null)
		{
			TapButton.gameObject.SetActive(false);
		}
		
		if(BonusScrollbar != null)
		{
			BonusScrollbar.gameObject.SetActive(false);
		}
		
		if(BonusHitText != null)
		{
			BonusHitText.text = "";
		}
		
		if(TensionScrollbar != null)
		{
			TensionScrollbar.gameObject.SetActive(false);
		}
		
		if(PowerScrollbar != null)
		{
			PowerScrollbar.gameObject.SetActive(false);
		}
		
		if(PressButton != null)
		{
			PressButton.gameObject.SetActive(false);
		}
		
		if(FishState != null)
		{
			FishState.SetActive(false);
		}
		
		if(PowerImage != null)
		{
			PowerImage.SetActive(false);
		}
		
		if(SnatchLeftImage != null)
		{
			SnatchLeftImage.SetActive(false);
		}
		
		if(SnatchRightImage != null)
		{
			SnatchRightImage.SetActive(false);
		}
	}

	// 현재의 결과 상태를 체크하여 결과를 알려준다.
	IEnumerator CheckResult()
	{
		while(true)
		{
			if(m_ResultState == eResultState.None)
			{
				yield return null;

				continue;
			}

			ResultText.gameObject.SetActive(true);

			if(m_ResultState == eResultState.Fail)
			{
				ResultText.text = "Fail";
			}
			else
			{
				ResultText.text = "Success";
			}

			HideAllWithoutResultText();

			yield return new WaitForSeconds(2.0f);

			ResultText.gameObject.SetActive(false);

			m_ResultState = eResultState.None;

			m_Initial = true;
		}
	}

	// 성공 또는 실패 등의 상태로 바꿔준다.
	public void ChangeResult(eResultState resultState)
	{
		m_ResultState = resultState;
	}
}