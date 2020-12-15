using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundEffects {
    Jump,
    Dash,
    WallJump,
    OnWall,
    Orb,
    MovingFast
}

public class SoundManager : MonoBehaviour
{
    [SerializeField] private GameSettings _gameSettings = default;
    [SerializeField] private AudioSource _soundEffects = default;
    [SerializeField] private AudioSource _windSound = default;
    [SerializeField] private SoundsDatabase _soundsDatabase = default;

    void Start()
    {
        _gameSettings.OnUpdateGameSettings += updateVolume;
        updateVolume();
    }

    public void PlaySoundEffect(SoundEffects soundEffect)
    {
        switch (soundEffect) {
            case SoundEffects.Jump:
                _soundEffects.PlayOneShot(_soundsDatabase.Jump);
                break;
            case SoundEffects.Dash:
                _soundEffects.PlayOneShot(_soundsDatabase.Dash);
                break;
            case SoundEffects.WallJump:
                _soundEffects.PlayOneShot(_soundsDatabase.WallJump); break;
            case SoundEffects.OnWall:
                _soundEffects.PlayOneShot(_soundsDatabase.OnWall); 
                break;
            case SoundEffects.Orb:
                _soundEffects.loop = true;
                if (!_soundEffects.isPlaying) _soundEffects.PlayOneShot(_soundsDatabase.Orb);
                break;
            case SoundEffects.MovingFast:
                _soundEffects.PlayOneShot(_soundsDatabase.MovingFast);
                break;
        }
    }

    private void updateVolume()
    {
        _soundEffects.volume = _gameSettings.SoundEffectsVolume;
        _windSound.volume = _gameSettings.WindVolume;
    }

}
