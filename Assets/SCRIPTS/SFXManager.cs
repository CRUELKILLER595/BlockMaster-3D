
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SFXManager : MonoBehaviour
{
    public static SFXManager instance;

    public AudioSource sfxSource;

    private const string SFXVolumeKey = "BM_SFXVolume";
    private const string SFXEnabledKey = "BM_SFXEnabled";

    public float CurrentVolume =>
        sfxSource != null ? sfxSource.volume : 1f;

    public bool IsSFXEnabled =>
        sfxSource != null && !sfxSource.mute;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        sfxSource = GetComponent<AudioSource>();

        sfxSource.volume =
            PlayerPrefs.GetFloat(SFXVolumeKey, sfxSource.volume);

        sfxSource.mute =
            PlayerPrefs.GetInt(SFXEnabledKey, 1) == 0;
    }

    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }

    public void PlaySound(AudioClip clip)
    {
        if (sfxSource == null || clip == null)
            return;

        sfxSource.PlayOneShot(clip);
    }

    public void SetSFXVolume(float volume)
    {
        if (sfxSource == null)
            return;

        sfxSource.volume = Mathf.Clamp01(volume);

        PlayerPrefs.SetFloat(SFXVolumeKey, sfxSource.volume);
        PlayerPrefs.Save();
    }

    public void SetSFXEnabled(bool enabled)
    {
        if (sfxSource == null)
            return;

        sfxSource.mute = !enabled;

        PlayerPrefs.SetInt(SFXEnabledKey, enabled ? 1 : 0);
        PlayerPrefs.Save();
    }
}
