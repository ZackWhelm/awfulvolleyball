using UnityEngine;

public class CountdownBridge : MonoBehaviour
{
	public string countdownSound = "countdownLowSFX";
	public string countdownFinish = "countdownHighSFX";

	public void PlayCountdownSound()
	{
		AudioManager.Play(countdownSound, AudioManager.MixerTarget.SFX);
	}

	public void FinishCountdown()
	{
		AudioManager.Play(countdownFinish, AudioManager.MixerTarget.SFX);
	}
}
