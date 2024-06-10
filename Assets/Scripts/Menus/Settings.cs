using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class Settings : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private ControlCamera controlCamera;
    [SerializeField] private AudioMixer audioMixer;

    private Resolution[] _resolutions;

    private void Start()
    {
        _resolutions = Screen.resolutions
            .Where(resolution => Mathf.Approximately((float)resolution.width / resolution.height, 16f / 9f))
            .Select(resolution => new Resolution { width = resolution.width, height = resolution.height })
            .Distinct()
            .ToArray();

        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;

        for (int i = 0; i < _resolutions.Length; i++)
        {
            string option = _resolutions[i].width + "x" + _resolutions[i].height;
            options.Add(option);

            if (_resolutions[i].width == Screen.currentResolution.width &&
                _resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        qualityDropdown.value = QualitySettings.GetQualityLevel();
        qualityDropdown.RefreshShownValue();

        fullscreenToggle.isOn = Screen.fullScreen;

        sensitivitySlider.value = controlCamera.GetSensitivity();
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = _resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetSensitive(float sensitivity)
    {
        controlCamera.SetSensitivity(sensitivity);
    }

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("Volume", volume);
    }

    [System.Serializable]
    public struct SaveData
    {
        public int qualityIndex;
        public bool isFullscreen;
        public int resolutionIndex;
        public float sensitivity;
        //public float volume;
    }

    public SaveData GetSaveData()
    {
        SaveData saveData;
        saveData.qualityIndex = QualitySettings.GetQualityLevel();
        saveData.isFullscreen = Screen.fullScreen;
        saveData.resolutionIndex = resolutionDropdown.value;
        saveData.sensitivity = sensitivitySlider.value;
        //saveData.volume = 0f;
        return saveData;
    }

    public void LoadSaveData(SaveData saveData)
    {
        SetQuality(saveData.qualityIndex);
        SetFullscreen(saveData.isFullscreen);
        SetResolution(saveData.resolutionIndex);
        SetSensitive(saveData.sensitivity);
        //SetVolume(saveData.volume);

        qualityDropdown.value = saveData.qualityIndex;
        qualityDropdown.RefreshShownValue();

        fullscreenToggle.isOn = saveData.isFullscreen;

        resolutionDropdown.value = saveData.resolutionIndex;
        resolutionDropdown.RefreshShownValue();

        sensitivitySlider.value = saveData.sensitivity;
        //volumeSlider.value = saveData.volume;
    }
}