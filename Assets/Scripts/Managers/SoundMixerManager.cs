using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundMixerManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider fxSlider;
    [SerializeField] private Slider musicSlider;

    private void Start()
    {
        if (PlayerPrefs.HasKey("MasterVolume"))
            LoadVolume();
        else
        {
            SetMasterVolume(masterSlider.value);
            SetFXVolume(fxSlider.value);
            SetMusicVolume(musicSlider.value);
        }
    }

    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log(volume) * 20f);
        PlayerPrefs.SetFloat("MasterVolume", volume);
    }

    public void SetFXVolume(float volume)
    {
        audioMixer.SetFloat("FXVolume", Mathf.Log(volume) * 20f);
        PlayerPrefs.SetFloat("FXVolume", volume);
    }

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log(volume) * 20f);
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    private void LoadVolume()
    {
        masterSlider.value = PlayerPrefs.GetFloat("MasterVolume");
        fxSlider.value = PlayerPrefs.GetFloat("FXVolume");
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
    }
}
