using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : SingletonMono<AudioManager>
{
	[SerializeField] AudioMixer audioMixer;
	[SerializeField] AudioMixerGroup bgmMixerGroup;
	[SerializeField] AudioMixerGroup sfxMixerGroup;

	const int MAX_COUNT_BGM = 2;
	const int MAX_COUNT_SFX = 20;
	const string BGM_PATH = "Audios/BGMs/";
	const string SFX_PATH = "Audios/SFXs/";

	AudioSource[] _asBGM;
	List<AudioSource> _sfxAudioSources;
	Dictionary<string, AudioClip> _clipCache = new();

	int _playingChannel = -1;
	float _bgmVolume = 1f;
	float _sfxVolume = 1f;
	bool _muted = false;

	Coroutine _coChangeBGM;

	protected override void Awake()
	{
		base.Awake();
		ReadyAudio();
	}

	void ReadyAudio()
	{
		_asBGM = new AudioSource[MAX_COUNT_BGM];
		for (int i = 0; i < MAX_COUNT_BGM; i++)
		{
			_asBGM[i] = gameObject.AddComponent<AudioSource>();
			_asBGM[i].playOnAwake = false;
			_asBGM[i].loop = true;
			_asBGM[i].outputAudioMixerGroup = bgmMixerGroup;
		}

		_sfxAudioSources = new List<AudioSource>();
		for (int i = 0; i < MAX_COUNT_SFX; i++)
		{
			GameObject go = new GameObject("SfxChannel");
			go.transform.SetParent(transform, false);

			AudioSource source = go.AddComponent<AudioSource>();
			source.playOnAwake = false;
			source.loop = false;
			source.outputAudioMixerGroup = sfxMixerGroup;
			_sfxAudioSources.Add(source);
		}
	}

	AudioClip LoadClip(string path, string clipName)
	{
		string key = path + clipName;
		if (_clipCache.TryGetValue(key, out AudioClip cached))
			return cached;

		AudioClip clip = Resources.Load<AudioClip>(key);
		if (clip == null)
		{
			Debug.LogWarning($"AudioManager: clip not found at Resources/{key}");
			return null;
		}

		_clipCache[key] = clip;
		return clip;
	}

	#region BGM

	public void PlayBGM(string clipName, float fadeTime = 1f)
	{
		AudioClip clip = LoadClip(BGM_PATH, clipName);
		if (clip == null) return;

		if (_playingChannel == -1)
		{
			PlayBGMImmediate(clip, 0);
			return;
		}

		if (_asBGM[_playingChannel].clip == clip) return;

		if (_coChangeBGM != null) StopCoroutine(_coChangeBGM);
		_coChangeBGM = StartCoroutine(CoChangeBGM(clip, fadeTime));
	}

	void PlayBGMImmediate(AudioClip clip, int channel)
	{
		_playingChannel = channel;
		_asBGM[channel].clip = clip;
		_asBGM[channel].volume = _muted ? 0f : _bgmVolume;
		_asBGM[channel].Play();
	}

	IEnumerator CoChangeBGM(AudioClip clip, float fadeTime)
	{
		int fromCh = _playingChannel;
		int toCh = 1 - _playingChannel;

		_asBGM[toCh].clip = clip;
		_asBGM[toCh].volume = 0f;
		_asBGM[toCh].Play();
		_playingChannel = toCh;

		float targetVolume = _muted ? 0f : _bgmVolume;
		float fromStart = _asBGM[fromCh].volume;
		float t = 0f;

		while (t < fadeTime)
		{
			t += Time.deltaTime;
			float ratio = t / fadeTime;
			_asBGM[fromCh].volume = Mathf.Lerp(fromStart, 0f, ratio);
			_asBGM[toCh].volume = Mathf.Lerp(0f, targetVolume, ratio);
			yield return null;
		}

		_asBGM[fromCh].Stop();
		_asBGM[fromCh].volume = 0f;
		_asBGM[toCh].volume = targetVolume;
	}

	public void StopBGM()
	{
		if (_playingChannel == -1) return;

		if (_coChangeBGM != null)
		{
			StopCoroutine(_coChangeBGM);
			_coChangeBGM = null;
		}

		_asBGM[_playingChannel].Stop();
		_playingChannel = -1;
	}

	public void SetBgmVolume(float volume)
	{
		_bgmVolume = Mathf.Clamp01(volume);
		if (!_muted && _playingChannel != -1)
			_asBGM[_playingChannel].volume = _bgmVolume;
	}

	#endregion

	#region SFX

	AudioSource GetReadySfxSource()
	{
		foreach (AudioSource source in _sfxAudioSources)
			if (!source.isPlaying) return source;
		return null;
	}

	public void PlaySFX(string clipName, float delay = 0f)
	{
		AudioClip clip = LoadClip(SFX_PATH, clipName);
		if (clip == null) return;
		StartCoroutine(CoPlaySFX(clip, delay, Vector3.zero, false));
	}

	public void PlaySFX3D(string clipName, Vector3 position, float delay = 0f)
	{
		AudioClip clip = LoadClip(SFX_PATH, clipName);
		if (clip == null) return;
		StartCoroutine(CoPlaySFX(clip, delay, position, true));
	}

	IEnumerator CoPlaySFX(AudioClip clip, float delay, Vector3 position, bool is3D)
	{
		if (delay > 0f) yield return new WaitForSeconds(delay);

		AudioSource source = GetReadySfxSource();
		if (source == null) yield break;

		source.Stop();
		source.clip = clip;
		source.volume = _muted ? 0f : _sfxVolume;
		source.spatialBlend = is3D ? 1f : 0f;
		source.transform.position = position;
		source.Play();
	}

	public void StopSFX()
	{
		foreach (AudioSource source in _sfxAudioSources)
			source.Stop();
	}

	public void SetSfxVolume(float volume)
	{
		_sfxVolume = Mathf.Clamp01(volume);
	}

	#endregion

	#region Mute / Mixer

	public void SetMuted(bool muted)
	{
		_muted = muted;

		if (_playingChannel != -1)
			_asBGM[_playingChannel].volume = muted ? 0f : _bgmVolume;
	}

	// 씬 전환에 맞춰 그 씬의 BGM("bgm_씬이름")을 재생하고, AudioMixer 스냅샷도 같이 전환해요.
	// 해당 이름의 스냅샷이 없으면 "Default"로, bgm_씬이름 파일이 없으면 경고만 남기고 조용히 넘어가요.
	public void ChangeAudioMixSnapshot(ESceneType scene, float transitionTime = 1f)
	{
		PlayBGM("bgm_" + scene, transitionTime);

		if (audioMixer == null) return;

		AudioMixerSnapshot snapshot = audioMixer.FindSnapshot(scene.ToString())
			?? audioMixer.FindSnapshot("Default");

		if (snapshot == null) return;

		audioMixer.TransitionToSnapshots(new[] { snapshot }, new[] { 1f }, transitionTime);
	}

	#endregion
}
