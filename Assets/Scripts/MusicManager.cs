using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;
    private AudioSource _audioMusic;

    public List<AudioClip> listBgMusic;
    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);
        _audioMusic = GetComponent<AudioSource>();

    }

    public void PlayMusicBackground()
    {
        if (!Config.isMusic) return;
        if (_audioMusic.clip == null)
        {
            var k = Random.Range(0, listBgMusic.Count);
            _audioMusic.clip = listBgMusic[k];
            _audioMusic.loop = true;
            _audioMusic.Play();
        }
        else
        {
            _audioMusic.Play();
        }
    }

    public void StopMusicBackground() {
        _audioMusic.Stop();
    }
}
