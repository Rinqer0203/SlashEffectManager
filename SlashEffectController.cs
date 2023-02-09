using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SlashEffectController : MonoBehaviour
{
	[SerializeField] private ParticleSystem particle = null;
	private SlashEffectManager.SlashEffectPool _callback;

    public void Play(SlashEffectManager.SlashEffectPool callback)
	{
		_callback = callback;
		particle.Play();
	}

	public void OnParticleSystemStopped()
    {
		_callback.Return(this);
    }
}
