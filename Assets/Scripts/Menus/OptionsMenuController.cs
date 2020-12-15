using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class OptionsMenuController : MonoBehaviour
{
    [SerializeField] private GameSettings _gameSettings = default;
    [SerializeField] private Slider _soundEffects = default;
    [SerializeField] private Slider _wind = default;

    void Start()
    {
        _soundEffects.value = _gameSettings.SoundEffectsVolume;
        _wind.value = _gameSettings.WindVolume;
    }

    void Update()
    {
        if (_gameSettings.SoundEffectsVolume != _soundEffects.value)
        {
            _gameSettings.SoundEffectsVolume = _soundEffects.value;
        }

        if (_gameSettings.SoundEffectsVolume != _wind.value)
        {
            _gameSettings.WindVolume = _wind.value;
        }
    }

}
