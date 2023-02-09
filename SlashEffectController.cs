using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SlashEffectController : MonoBehaviour
{
	[SerializeField] private ParticleSystem _particle = null;
	private SlashEffectManager.SlashEffectPool _callback;

    public void Play(SlashEffectManager.SlashEffectPool callback)
	{
		_callback = callback;
		_particle.Play();
	}

	public void OnParticleSystemStopped()
    {
		_callback.Return(this);
    }
}
