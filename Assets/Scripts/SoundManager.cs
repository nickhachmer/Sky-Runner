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

    [SerializeField] private AudioSource _audioSource = default;
    [SerializeField] private SoundsDatabase _soundsDatabase = default;

    public void PlaySoundEffect(SoundEffects soundEffect)
    {
        switch (soundEffect) {
            case SoundEffects.Jump:
                _audioSource.PlayOneShot(_soundsDatabase.Jump);
                break;
            case SoundEffects.Dash:
                _audioSource.PlayOneShot(_soundsDatabase.Dash);
                break;
            case SoundEffects.WallJump:
                _audioSource.PlayOneShot(_soundsDatabase.WallJump); break;
            case SoundEffects.OnWall:
                _audioSource.PlayOneShot(_soundsDatabase.OnWall); 
                break;
            case SoundEffects.Orb:
                _audioSource.loop = true;
                if (!_audioSource.isPlaying) _audioSource.PlayOneShot(_soundsDatabase.Orb);
                break;
            case SoundEffects.MovingFast:
                _audioSource.PlayOneShot(_soundsDatabase.MovingFast);
                break;
        }
    }

}
