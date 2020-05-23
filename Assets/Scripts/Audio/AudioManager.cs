using Boo.Lang;
using System;
using System.Linq;
using UnityEditor.Audio;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    private static AudioManager _instance;
    public static AudioManager instance { get { return _instance; } }

    void Awake() {

        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
            return;
        } 
        else _instance = this;

        DontDestroyOnLoad(gameObject);

        foreach (Sound s in sounds) 
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;

            s.source.playOnAwake = s.playOnAwake;
            if (s.source.playOnAwake) s.source.Play();
        }

    }
    
    public void Play (string name) { 
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null) {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        s.source.Play();
    }

    public void StopPlaying (string name) {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null) {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        s.source.Stop();
    }

    #region Mute/Change Audio

    public void ChangeSoundVolume(Slider slider) {
        AudioListener.volume = slider.value;
    }
    public void MuteSound() {
        Sound[] s = ExcludeFromSounds("Theme");
        for (int i = 0; i < s.Length; i++) {
            s[i].source.Stop();
            s[i].source.volume = 0;
        }
    }
    public void UnMuteSound() {
        Sound[] s = ExcludeFromSounds("Theme");
        for (int i = 0; i < s.Length; i++) {
            if (s[i].source == null) continue; 
            s[i].source.volume = s[i].volume;
        }
    }

    public void ChangeMusicVolume(Slider slider) {
        GetAudioSource("Theme").volume = slider.value;
    }
    public void MuteMusic() {
        GetAudioSource("Theme").Pause();
    }
    public void UnMuteMusic() {
        GetAudioSource("Theme").UnPause();
    }

    public Sound[] ExcludeFromSounds(string startName) {
        Sound[] newSounds = sounds;
        Sound[] withoutName = newSounds.Where((sound, index) => !sound.name.StartsWith(startName)).ToArray();

        return withoutName;
    }


    #endregion

    public AudioSource GetAudioSource(string name) {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null) {
            Debug.LogWarning("Sound: " + name + " not found!");
            return null;
        }
        return s.source;
    }

}
