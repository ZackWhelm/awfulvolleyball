using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUD : MonoBehaviour
{
	[SerializeField] TMP_Text timeText;
	[field: SerializeField, Space] public GameObject SpectatingObj { get; private set; }

	public static HUD Instance { get; private set; }
	private void Awake()
	{
		Instance = this;
	}

	private void OnEnable()
	{

	}

	public static void SetLevelName(int holeIndex)
	{

	}

	public static void SetTimerText(float time)
	{
		Instance.timeText.text = $"{(int)(time / 60f):00}:{time % 60:00.00}";
	}

	public static void SetStrokeCount(int count)
	{

	}

	public static void SetPuttCooldown(float fill)
	{

	}

	public static void ForceHideAll()
	{

	}
}
