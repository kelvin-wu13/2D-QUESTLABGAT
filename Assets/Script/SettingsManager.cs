using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Slider bgmVolumeSlider;
    [SerializeField] private AudioManager audioManager;

    private void Start()
    {
        // Load saved volume settings
        sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 0.5f);
        bgmVolumeSlider.value = PlayerPrefs.GetFloat("BGMVolume", 0.5f);
    }

    public void UpdateSFXVolume()
    {
        float volume = sfxVolumeSlider.value;
        audioManager.SetSFXVolume(volume);
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    public void UpdateBGMVolume()
    {
        float volume = bgmVolumeSlider.value;
        audioManager.SetBGMVolume(volume);
        PlayerPrefs.SetFloat("BGMVolume", volume);
    }
}