using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("#BGM")]
    public AudioClip[] bgmClip;
    public float bgmVolume;
    AudioSource bgmPlayer;
    AudioHighPassFilter bgmEffect;

    public enum Bgm
    {
        Camp, Stage0, Stage1, Stage2, Title, Boss, Death, Victory
    }

    [Header("#SFC")]
    public AudioClip[] sfxClip;
    public float sfxVolume;
    public int channels;
    AudioSource[] sfxPlayers;
    int channelIndex;

    public enum Sfx
    {
        Dead, Hit, LevelUp = 3, Lose, Melee, Range = 7, Select, Win, WarriorSkill = 11, WarriorAttack, PlayerHit, Dodge = 14, Fail, Kunai, Arrow, Healthy,
        ButtonChange, ButtonPress, Cancel, Equip, Unequip, Destroy, Gold, AcquireItem, HeartBeat, Success, Upgrade, ChestOpen, MenuSelect, MenuChange
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
        Init();
    }

    void Init()
    {
        //배경음 플레이어 초기화
        GameObject bgmObject = new("BgmPlayer");
        bgmObject.transform.parent = transform;
        bgmPlayer = bgmObject.AddComponent<AudioSource>();
        bgmPlayer.playOnAwake = false;
        bgmPlayer.loop = true;
        bgmPlayer.volume = bgmVolume;
        bgmPlayer.clip = null;

        //효과음 플레이어 초기화
        GameObject sfxObject = new("SfxPlayer");
        sfxObject.transform.parent = transform;
        sfxPlayers = new AudioSource[channels];

        for (int i = 0; i < channels; i++)
        {
            sfxPlayers[i] = sfxObject.AddComponent<AudioSource>();
            sfxPlayers[i].playOnAwake = false;
            sfxPlayers[i].bypassListenerEffects = true;
            sfxPlayers[i].volume = sfxVolume;
        }
    }

    public void PlayBgm(bool isPlay)
    {
        if (isPlay)
        {
            bgmPlayer.Play();

        }
        else
        {
            bgmPlayer.Stop();
        }
    }
    public void ChangeBGM(Bgm bgmType, float bgmVol, bool isLoop)
    {
        bgmPlayer.clip = bgmClip[(int)bgmType];
        bgmPlayer.volume = bgmVol;

        bgmPlayer.loop = isLoop;
    }

    public void PauseBGM(bool pause)
    {
        if (pause)
        {
            bgmPlayer.Pause();
        }
        else
        {
            bgmPlayer.UnPause();
        }
    }

    public void EffectBgm(bool isPlay)
    {
        Camera.main.GetComponent<AudioHighPassFilter>().enabled = isPlay;
    }

    public void PlaySfx(Sfx sfx)
    {
        for (int i = 0; i < sfxPlayers.Length; i++)
        {
            int loopIndex = (i + channelIndex) % channels;

            if (sfxPlayers[loopIndex].isPlaying)
                continue;

            int ranIndex = 0;
            if (sfx == Sfx.Hit || sfx == Sfx.Melee)
            {
                ranIndex = Random.Range(0, 2);
            }

            channelIndex = loopIndex;
            sfxPlayers[loopIndex].clip = sfxClip[(int)sfx + ranIndex];
            sfxPlayers[loopIndex].Play();
            break;
        }
    }

}
