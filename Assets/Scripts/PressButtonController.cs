using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// 물고기의 상태(색깔로 표현)
public enum eFishState
{
	RandomChangeableStateStart,
	Green, Orange, Red, Violet,
	RandomChangeableStateEnd,

	Gray
}

// 유저가 쓴 스킬의 상태
public enum eSkillState
{
	None,

	Power,

	SnatchStateStart,
	SnatchLeft,	SnatchRight,
	SnatchStateEnd
}

// 게임의 핵심 조작 버튼
public class PressButtonController : MonoBehaviour
{
	public UIController UIController = null;

	public Scrollbar TensionScrollbar = null; // 텐션게이지
	public Scrollbar PowerScrollbar = null; // 파워게이지

	// 물고기의 상태를 나타내는 이미지(UI 상에 색깔로 표시 : 맨 위, 아래)
	public Image FishStateUpperImage = null;
	public Image FishStateLowerImage = null;

	public Scrollbar FishHPScrollbar = null; // 물고기의 HP게이지
	public Text DistanceText = null; // 물고기와의 거리(UI)
	public Text LineLengthText = null; // 낚싯줄의 길이(UI)

	public GameObject PowerImage = null; // 파워게이지가 다 찼을 때 나타나는 화살표(윗방향)
	// 일정확률로 낚아채기가 가능한 상태가 되었을 때 나타나는 화살표(좌, 우)
	public GameObject SnatchLeftImage = null;
	public GameObject SnatchRightImage = null;



	private eFishState m_CurrentFishState = eFishState.Green; // 현재 물고기의 상태

	// 물고기의 상태 유지시간(최소 ~ 최대)
	public float FishStateDurationMin = 2.0f;
	public float FishStateDurationMax = 10.0f;

	// 낚아채기 가능한 상태의 유지시간
	public float VioletDurationMin = 2.0f;
	public float VioletDurationMax = 3.0f;

	private bool m_ButtonDown = false; // 조작버튼이 눌려졌는지의 여부
	private bool m_PointerEnter = false; // 포인터(손가락)이 버튼 안쪽에 들어왔는지 여부

	// 버튼을 누르거나(wind) 떼었을(unwind) 때 각 상태별 텐션 증감률
	public float GreenWindSpeed = 0.02f;
	public float GreenUnwindSpeed = -0.2f;
	public float OrangeWindSpeed = 0.25f;
	public float OrangeUnwindSpeed = -0.28f;
	public float RedWindSpeed = 0.3f;
	public float RedUnwindSpeed = -0.3f;
	public float VioletWindSpeed = 0.0f;
	public float VioletUnwindSpeed = -0.0f;
	public float GrayWindSpeed = 0.01f;
	public float GrayUnwindSpeed = -0.2f;

	public float HighValue = 0.9f; // 파워게이지가 증가할 수 있는 최소한의 tension값
	public float IncreasingPowerSpeed = 0.25f; // tension이 high값 이상일 때 시간당(초) 파워증가율
	public float UsingPowerSpeed = 0.4f; // 파워스킬 사용시 시간당 파워사용률

	// 물고기의 HP
	public float FishHPMin = 200.0f;
	public float FishHPMax = 400.0f;
	private float m_CurrentFishHPMax = 100.0f; // 현재 물고기의 최대 HP
	private float m_CurrentFishHP = 100.0f; // 현재 물고기의 HP

	// 줄을 당길 때 물고기에게 가해지는 데미지
	public float DamageSpeedMin = 10.0f;
	public float DamageSpeedMax = 20.0f;
	private float m_CurrentDamage = 10.0f; // 현재 데미지

	private eSkillState m_SkillState = eSkillState.None; // 현재 사용한 스킬(파워, 낚아채기 등)

	// 낚싯줄의 길이
	public float LineLengthMin = 150.0f;
	public float LineLengthMax = 200.0f;
	private float m_CurrentLineLength = 150.0f; // 현재 낚싯줄의 길이

	// 초기 물고기와의 거리
	public float InitialDistanceMin = 80.0f;
	public float InitialDistanceMax = 100.0f;
	private float m_CurrentDistance = 80.0f; // 현재 물고기와의 거리

	public float IncreasingDistanceSpeed = 5.0f; // 줄을 풀 때 물고기와 멀어지는 속도(시간당)
	public float DecreasingDistanceSpeed = 2.0f; // 줄을 감을 때 물고기와 가까워지는 속도
	public float SkillDecreasingDistanceSpeed = 5.0f; // 스킬(파워, 낚아채기)을 사용했을 때 물고기와 가까워지는 속도
	public float HPZeroDecreasingDistanceSpeed = 25.0f; // 물고기의 체력이 0일 때 줄을 감았을 때 물고기와 가까워지는 속도

	private bool m_GetEnduringBonus = true; // 물고기 상태가 Green상태일 때 연속적인 시간으로 줄을 감을 때(버튼을 누를 때) 연속 보너스를 얻었는지의 여부
	public float EnduringTime = 2.0f; // 줄을 감을 때 적용되는 연속보너스의 시간 간격
	public float EnduringPowerBonusRate = 0.15f; // 연속보너스 얻었을 때 충전되는 파워게이지


	void OnEnable()
	{
		if(TensionScrollbar != null)
		{
			TensionScrollbar.value = 0.5f;
		}

		if(PowerScrollbar != null)
		{
			PowerScrollbar.value = 0.0f;
		}

		m_CurrentFishState = eFishState.Red;
		ChangeFishStateColor();

		m_ButtonDown = false;
		m_CurrentFishHPMax = m_CurrentFishHP = Random.Range(FishHPMin, FishHPMax);
		m_CurrentDamage = Random.Range(DamageSpeedMin, DamageSpeedMax);
		FishHPScrollbar.value = 1.0f;

		m_CurrentLineLength = Random.Range(LineLengthMin, LineLengthMax);
		LineLengthText.text = m_CurrentLineLength.ToString();
		m_CurrentDistance = Random.Range(InitialDistanceMin, InitialDistanceMax);
		DistanceText.text = m_CurrentDistance.ToString();

		m_SkillState = eSkillState.None;

		m_GetEnduringBonus = true;

		StartCoroutine(ChangeFishState());
		StartCoroutine(AddTension());
		StartCoroutine(DecreaseFishHP());
		StartCoroutine(CheckResult());
	}

	// 물고기의 상태를 변화시킨다. 일정시간이 지나면 다음 상태로 변화됨.
	IEnumerator ChangeFishState()
	{
		while(true)
		{
			// 물고기 HP가 0이면 상태변화는 하지 않는다.
			if(FishHPScrollbar.value <= 0.0f)
			{
				yield return null;

				continue;
			}

			// 스킬을 사용했다면 물고기는 잠시동안 상태변화하지 않는다.
			if(m_SkillState == eSkillState.Power
			   || m_SkillState == eSkillState.SnatchLeft || m_SkillState == eSkillState.SnatchRight)
			{
				yield return null;

				continue;
			}

			float duration = 0.0f;

			// 낚아채기일 때의 상태유지시간(대기시간)
			if(m_CurrentFishState == eFishState.Violet)
			{
				duration = Random.Range(VioletDurationMin, VioletDurationMax);
			}
			else // 그 외의 상태유지시간
			{
				duration = Random.Range(FishStateDurationMin, FishStateDurationMax);
			}

			// 다음 상태까지 대기시간을 갖는다.
			yield return new WaitForSeconds(duration);

			// 대기시간동안 물고기의 체력이 0이 된 경우도 발생하기 때문에 아래의 명령문은 무시한다.
			if(FishHPScrollbar.value <= 0.0f)
			{
				yield return null;
				
				continue;
			}

			// 마찬가지로 대기시간동안 유저가 스킬을 사용할 경우도 발생하기 때문에 아래의 명령문은 무시한다.
			if(m_SkillState == eSkillState.Power
			   || m_SkillState == eSkillState.SnatchLeft || m_SkillState == eSkillState.SnatchRight)
			{
				yield return null;
				
				continue;
			}

			// 나타났던 화살표는 다시 숨겨준다.
			if(m_CurrentFishState == eFishState.Violet)
			{
				HideAllArrows();
			}

			// 다음상태로 전이(현재 상태와 다른 상태로)
			int currentStateNumber = (int)m_CurrentFishState;
			int nextStateNumber = Random.Range((int)eFishState.RandomChangeableStateStart + 1, (int)eFishState.RandomChangeableStateEnd);

			while(currentStateNumber == nextStateNumber)
			{
				nextStateNumber = Random.Range((int)eFishState.RandomChangeableStateStart + 1, (int)eFishState.RandomChangeableStateEnd);
			}

			m_CurrentFishState = (eFishState)nextStateNumber;

			ChangeFishStateColor();

			ShowSnatchArrow();
		}
	}

	// 물고기의 상태별로 UI색상 변경시킨다.
	void ChangeFishStateColor()
	{
		Color changedColor = Color.white;

		switch(m_CurrentFishState)
		{
		case eFishState.Green :
			changedColor = Color.green;
			break;
		case eFishState.Orange :
			changedColor = new Color(1.0f, 0.5f, 0.0f);
			break;
		case eFishState.Red :
			changedColor = Color.red;
			break;
		case eFishState.Violet :
			changedColor = new Color(0.7f, 0.0f, 1.0f);
			break;
		case eFishState.Gray :
			changedColor = Color.gray;
			break;
		}

		FishStateUpperImage.color = FishStateLowerImage.color = changedColor;
	}

	// 낚아채기 화살표 표시
	void ShowSnatchArrow()
	{
		if(m_CurrentFishState != eFishState.Violet)
		{
			return;
		}

		float random = Random.Range(0, 2);

		HidePowerArrow();

		if(random < 1)
		{
			SnatchLeftImage.SetActive(true);
		}
		else
		{
			SnatchRightImage.SetActive(true);
		}
	}

	// 텐션게이지를 관리한다.
	IEnumerator AddTension()
	{
		while(true)
		{
			// 물고기의 체력이 0일 때에는 낚싯줄을 조금 빨리 감을 수 있게 하기 위하여
			if(FishHPScrollbar.value <= 0.0f)
			{
				float dt = Time.deltaTime;

				if(m_ButtonDown)
				{
					m_CurrentDistance -= HPZeroDecreasingDistanceSpeed * dt;
				}

				if(m_SkillState == eSkillState.Power)
				{
					m_CurrentDistance -= SkillDecreasingDistanceSpeed * dt;
				}

				DistanceText.text = m_CurrentDistance.ToString();
			}

			// 스킬 사용중일 때에는 줄을 감는 속도를 향상시키고, 버튼을 누르지 않아도 줄이 풀리지 않는다.
			if(m_SkillState == eSkillState.Power
			   || m_SkillState == eSkillState.SnatchLeft || m_SkillState == eSkillState.SnatchRight)
			{
				float dt = Time.deltaTime;

				m_CurrentDistance -= SkillDecreasingDistanceSpeed * dt;

				if(m_ButtonDown && m_PointerEnter)
				{
					m_CurrentDistance -= DecreasingDistanceSpeed * dt;
				}

				DistanceText.text = m_CurrentDistance.ToString();

				yield return null;

				continue;
			}

			// 버튼을 누르면 텐션을 증가시키고, 떼면 감소시킨다.
			if(m_ButtonDown)
			{
				IncreaseTension();
			}
			else
			{
				DecreaseTension();
			}

			// 상태에 따라 파워 게이지를 증가시킨다.
			IncreasePower();

			// 물고기가 Green 상태일 때 게이지 연속 보너스
			if(m_GetEnduringBonus && m_CurrentFishState == eFishState.Green)
			{
				StartCoroutine(IncreasePowerInGreenState());
			}

			// 파워게이지가 다 찼지만 낚아채기가 나오면 잠시 파워스킬 화살표는 숨겨둔다.
			if(PowerScrollbar.value >= 1.0f && m_CurrentFishState != eFishState.Violet)
			{
				PowerImage.SetActive(true);

				HideSnatchArrows();
			}

			yield return null;
		}
	}

	// 물고기 상태에 따라 각각 다른 값으로 텐션을 증가시킨다.
	void IncreaseTension()
	{
		float speed = 0.0f;
		
		switch(m_CurrentFishState)
		{
		case eFishState.Green :
			speed = GreenWindSpeed;
			break;
		case eFishState.Orange :
			speed = OrangeWindSpeed;
			break;
		case eFishState.Red :
			speed = RedWindSpeed;
			break;
		case eFishState.Violet :
			speed = VioletWindSpeed;
			break;
		case eFishState.Gray :
			speed = GrayWindSpeed;
			break;
		}

		m_CurrentDistance -= DecreasingDistanceSpeed * Time.deltaTime;
		DistanceText.text = m_CurrentDistance.ToString();

		AddTension(speed);
	}

	// 물고기 상태에 따라 각각 다른 값으로 텐션을 감소시킨다.
	void DecreaseTension()
	{
		float speed = 0.0f;

		switch(m_CurrentFishState)
		{
		case eFishState.Green :
			speed = GreenUnwindSpeed;
			break;
		case eFishState.Orange :
			speed = OrangeUnwindSpeed;
			break;
		case eFishState.Red :
			speed = RedUnwindSpeed;
			break;
		case eFishState.Violet :
			speed = VioletUnwindSpeed;
			break;
		case eFishState.Gray :
			speed = GrayUnwindSpeed;
			break;
		}

		m_CurrentDistance += IncreasingDistanceSpeed * Time.deltaTime;
		DistanceText.text = m_CurrentDistance.ToString();

		AddTension(speed);
	}

	// 텐션값이 high값 이상일 때, high값 이상에 머무른 시간만큼 파워게이지를 증가시켜준다.
	void IncreasePower()
	{
		// 파워스킬을 사용중이거나 낚아채기 상태일 때에는 증가시키지 않는다.
		if(m_SkillState == eSkillState.Power
		   || m_CurrentFishState == eFishState.Violet)
		{
			return;
		}

		if(TensionScrollbar.value < HighValue)
		{
			return;
		}

		float dt = Time.deltaTime;
		
		PowerScrollbar.value += dt * IncreasingPowerSpeed;
	}

	// 물고기 상태가 Green 일 때, 연속으로 줄을 감으면 얻는 보너스 EnduringTime이 지날 때마다 얻는다.
	IEnumerator IncreasePowerInGreenState()
	{
		m_GetEnduringBonus = false;

		yield return new WaitForSeconds(EnduringTime);

		if(!m_ButtonDown || m_CurrentFishState != eFishState.Green)
		{
			m_GetEnduringBonus = true;

			yield break;
		}

		PowerScrollbar.value += EnduringPowerBonusRate;

		m_GetEnduringBonus = true;
	}

	// 주어진 speed만큼 텐션을 증가시킨다.
	void AddTension(float speed)
	{
		float dt = Time.deltaTime;

		TensionScrollbar.value += dt * speed;
	}

	// 물고기의 HP를 감소시킨다.
	IEnumerator DecreaseFishHP()
	{
		while(true)
		{
			if(FishHPScrollbar.value <= 0.0f)
			{
				yield return null;

				continue;
			}

			// 버튼을 누르면(줄을 당기면) 물고기의 HP가 단다.
			if(m_ButtonDown)
			{
				float dt = Time.deltaTime;

				m_CurrentFishHP -= m_CurrentDamage * dt;

				FishHPScrollbar.value = m_CurrentFishHP / m_CurrentFishHPMax;
			}

			// 물고기의 HP가 0이면 물고기 상태는 Gray로 변화시킨다.
			if(FishHPScrollbar.value <= 0.0f)
			{
				if(m_CurrentFishState != eFishState.Gray)
				{
					m_CurrentFishState = eFishState.Gray;
					
					ChangeFishStateColor();

					HideSnatchArrows();
				}
			}

			yield return null;
		}
	}

	// 물고기와의 거리나 텐션 값에 따라 게임의 결과 여부를 체크한다.
	IEnumerator CheckResult()
	{
		while(true)
		{
			// 물고기와의 거리가 0이면 낚시에 성공, 낚싯줄보다 거리가 멀어지면 줄이 끊어져 실패
			if(m_CurrentDistance <= 0.0f)
			{
				UIController.ChangeResult(eResultState.Success);
				
				yield break;
			}
			else if(m_CurrentDistance > m_CurrentLineLength)
			{
				UIController.ChangeResult(eResultState.Fail);
			}

			// 텐션이 0이거나 최대이면 실패
			if(TensionScrollbar.value >= 1.0f
			   || TensionScrollbar.value <= 0.0f)
			{
				UIController.ChangeResult(eResultState.Fail);
			}

			yield return null;
		}
	}

	// 버튼을 누르면 발생되는 콜백
	public void OnDown()
	{
		m_ButtonDown = true;
	}

	// 손을 떼면 발생되는 콜백
	public void OnUp()
	{
		m_ButtonDown = false;
	}

	// 드래그 하면 발생되는 콜백
	public void OnDrag()
	{
		// 스킬 사용중일 때에는 드래그 무시
		if(m_SkillState == eSkillState.Power
		   || m_SkillState == eSkillState.SnatchLeft || m_SkillState == eSkillState.SnatchRight)
		{
			return;
		}

		if(PowerImage.activeSelf)
		{
			float dy = Input.GetAxis("Mouse Y");

			if(dy >= 0.2f)
			{
				StartCoroutine(UsePower());
			}
		}
		else if(SnatchLeftImage.activeSelf)
		{
			float dx = Input.GetAxis("Mouse X");

			if(dx <= -0.2f)
			{
				StartCoroutine(UseSnatch(true));
			}
		}
		else if(SnatchRightImage.activeSelf)
		{
			float dx = Input.GetAxis("Mouse X");
			
			if(dx >= 0.2f)
			{
				StartCoroutine(UseSnatch(false));
			}
		}
	}

	// 포인터가 버튼안에 들어왔을 때 콜백
	public void OnPointerEnter()
	{
		m_PointerEnter = true;
	}

	// 포인터가 버튼밖으로 나갔을 때 콜백
	public void OnPointerExit()
	{
		m_PointerEnter = false;
	}

	// 파워스킬 사용
	IEnumerator UsePower()
	{
		m_SkillState = eSkillState.Power;

		HideAllArrows();

		m_CurrentFishState = eFishState.Gray;
		ChangeFishStateColor();

		while(m_SkillState == eSkillState.Power)
		{
			float dt = Time.deltaTime;
			PowerScrollbar.value -= UsingPowerSpeed * dt;

			// 파워게이지를 다 쓰면 다시 스킬당태 원상복구, 물고기 상태도 Gray에서 다른 상태로 변경
			if(PowerScrollbar.value <= 0.0f)
			{
				m_SkillState = eSkillState.None;

				if(FishHPScrollbar.value > 0.0f)
				{
					int nextStateNumber = Random.Range((int)eFishState.RandomChangeableStateStart + 1, (int)eFishState.Violet);
					m_CurrentFishState = (eFishState)nextStateNumber;
					ChangeFishStateColor();
				}
			}

			yield return null;
		}
	}

	// 낚아채기스킬 사용(좌, 우)
	IEnumerator UseSnatch(bool left)
	{
		if(left)
		{
			m_SkillState = eSkillState.SnatchLeft;
		}
		else
		{
			m_SkillState = eSkillState.SnatchRight;
		}

		HideAllArrows();

		m_CurrentFishState = eFishState.Gray;
		ChangeFishStateColor();

		float time = Random.Range(VioletDurationMin, VioletDurationMax);

		while(time > 0.0f)
		{
			float dt = Time.deltaTime;
			time -= dt;

			yield return null;
		}

		m_SkillState = eSkillState.None;
		
		int nextStateNumber = Random.Range((int)eFishState.RandomChangeableStateStart + 1, (int)eFishState.Violet);
		m_CurrentFishState = (eFishState)nextStateNumber;
		ChangeFishStateColor();
	}

	// 파워스킬 화살표이미지 숨김
	void HidePowerArrow()
	{
		if(PowerImage.activeSelf)
		{
			PowerImage.SetActive(false);
		}
	}

	// 낚아채기스킬 화살표이미지 숨김.
	void HideSnatchArrows()
	{
		if(SnatchLeftImage.activeSelf)
		{
			SnatchLeftImage.SetActive(false);
		}
		if(SnatchRightImage.activeSelf)
		{
			SnatchRightImage.SetActive(false);
		}
	}

	// 모든 스킬의 화살표이미지 숨김.
	void HideAllArrows()
	{
		if(PowerImage.activeSelf)
		{
			PowerImage.SetActive(false);
		}
		if(SnatchLeftImage.activeSelf)
		{
			SnatchLeftImage.SetActive(false);
		}
		if(SnatchRightImage.activeSelf)
		{
			SnatchRightImage.SetActive(false);
		}
	}
}