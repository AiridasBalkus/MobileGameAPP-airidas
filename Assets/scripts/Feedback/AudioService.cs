using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// All game audio, clips loaded from Resources/MinoUW/Audio
public class AudioService : MonoBehaviour
{
    const string SoundKey = "sound";
    const string ClipPath = "MinoUW/Audio/";
    const int PoolSize = 4;
    const float MusicVolume = 0.35f;
    const float AmbienceVolume = 0.2f;

    public static AudioService Instance { get; private set; }

    public static bool SoundOn
    {
        get => PlayerPrefs.GetInt(SoundKey, 1) == 1;
        set { PlayerPrefs.SetInt(SoundKey, value ? 1 : 0); PlayerPrefs.Save(); ApplyVolume(); }
    }

    AudioClip _swipe, _coin, _hit, _click, _newBest, _music, _ambience, _upgrade, _runStart;
    AudioSource _musicSource, _ambienceSource, _uiSource;
    readonly AudioSource[] _pool = new AudioSource[PoolSize];
    int _next;
    UnityAction _onClick;
    readonly List<Button> _buttons = new List<Button>(32);

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Create()
    {
        if (Instance) return;
        var go = new GameObject("AudioService");
        DontDestroyOnLoad(go);
        go.AddComponent<AudioService>();
    }

    void Awake()
    {
        if (Instance && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        _swipe = Load("SFX_Swipe");
        _coin = Load("SFX_Coin");
        _hit = Load("SFX_Hit");
        _click = Load("SFX_Click");
        _newBest = Load("SFX_NewBest");
        _music = Load("MUS_Underwater_Loop");
        _ambience = Load("AMB_Underwater_Loop");
        _upgrade = Load("SFX_Upgrade");
        _runStart = Load("SFX_RunStart");

        _musicSource = NewSource();
        _musicSource.clip = _music;
        _musicSource.loop = true;
        _musicSource.volume = MusicVolume;
        _ambienceSource = NewSource();
        _ambienceSource.clip = _ambience;
        _ambienceSource.loop = true;
        _ambienceSource.volume = AmbienceVolume;
        for (int i = 0; i < PoolSize; i++) _pool[i] = NewSource();
        _uiSource = NewSource();
        _uiSource.ignoreListenerPause = true;

        _onClick = Click;
        ApplyVolume();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        if (_music) _musicSource.Play();
        if (_ambience) _ambienceSource.Play();
    }

    void OnDestroy()
    {
        if (Instance != this) return;
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Instance = null;
    }

    AudioSource NewSource()
    {
        var s = gameObject.AddComponent<AudioSource>();
        s.playOnAwake = false;
        s.spatialBlend = 0f;
        return s;
    }

    static AudioClip Load(string name)
    {
        var clip = Resources.Load<AudioClip>(ClipPath + name);
        if (!clip) Debug.LogWarning("[Audio] missing Resources/" + ClipPath + name);
        return clip;
    }

    static void ApplyVolume() => AudioListener.volume = SoundOn ? 1f : 0f;

    public void Swipe() => Play(_swipe, 0.6f, true);
    public void Coin() => Play(_coin, 0.8f, true);
    public void Hit() => Play(_hit, 1f, false);
    public void NewBest() => Play(_newBest, 1f, false);
    public void Upgrade() => Play(_upgrade, 0.8f, false);
    public void RunStart() => Play(_runStart, 0.7f, false);
    public void Click() { if (_click) _uiSource.PlayOneShot(_click, 0.6f); }

    void Play(AudioClip clip, float volume, bool varyPitch)
    {
        if (!clip) return;
        var s = _pool[_next];
        _next = (_next + 1) % PoolSize;
        s.pitch = varyPitch ? Random.Range(0.95f, 1.05f) : 1f;
        s.PlayOneShot(clip, volume);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        foreach (var root in scene.GetRootGameObjects())
        {
            root.GetComponentsInChildren(true, _buttons);
            for (int i = 0; i < _buttons.Count; i++)
            {
                _buttons[i].onClick.RemoveListener(_onClick);
                _buttons[i].onClick.AddListener(_onClick);
            }
        }
        _buttons.Clear();
    }
}
