using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    private AudioSource _audioSound;
    private void Awake()
    {
        Instance = this;
        _audioSound = GetComponent<AudioSource>();
    }

    [Header("Click")]
    public AudioClip click1, click2, click3;


    public void PlaySound_Click() {
        if (Config.isSound)
        {
            _audioSound.PlayOneShot(click2);
        }
    }

    [Header("Block Click")]
    public AudioClip block_click;
    public void PlaySound_BlockClick()
    {
        if (Config.isSound)
        {
            _audioSound.PlayOneShot(block_click);
        }
    }
    
    [Header("Combo")]
    public List<AudioClip> combo_sounds;
    public void PlaySound_ComboMatch(int combo)
    {
      
        if (Config.isSound)
        {
            _audioSound.PlayOneShot(combo_sounds[combo - 1]);
        }
    }

    [Header("Block MoveFinish")]
    public AudioClip blockMoveFinish;
    public void PlaySound_BlockMoveFinish()
    {
        if (Config.isSound)
        {
            _audioSound.PlayOneShot(blockMoveFinish);
        }
    }


    [Header("Block Cash")]
    public AudioClip blockCash;
    public void PlaySound_Cash()
    {
        if (Config.isSound)
        {
            _audioSound.PlayOneShot(blockCash);
        }
    }

    [Header("Block Clear")]
    public AudioClip blockClear;
    public void PlaySound_Clear()
    {
        if (Config.isSound)
        {
            _audioSound.PlayOneShot(blockClear);
        }
    }

    [Header("Block Free Block")]
    public AudioClip free_block;
    public void PlaySound_FreeBlock()
    {
        if (Config.isSound)
        {
            _audioSound.PlayOneShot(free_block);
        }
    }

    [Header("Block Game Over")]
    public AudioClip game_over;
    public void PlaySound_GameOver()
    {
        if (Config.isSound)
        {
            _audioSound.PlayOneShot(game_over);
        }
    }

    [Header("no_more_move")]
    public AudioClip no_more_move;
    public void PlaySound_NoMoreMove()
    {
        if (Config.isSound)
        {
            _audioSound.PlayOneShot(no_more_move);
        }
    }

    [Header("sfx_popup")]
    public AudioClip sfx_popup;
    public void PlaySound_Popup()
    {
        if (Config.isSound)
        {
            _audioSound.PlayOneShot(sfx_popup);
        }
    }

    [Header("sfx_wind")]
    public AudioClip sfx_wind;
    public void PlaySound_Wind()
    {
        if (Config.isSound)
        {
            _audioSound.PlayOneShot(sfx_wind);
        }
    }

    [Header("showBoard")]
    public AudioClip showBoard;
    public void PlaySound_ShowBoard()
    {
        if (Config.isSound)
        {
            _audioSound.PlayOneShot(showBoard);
        }
    }

    [Header("win")]
    public AudioClip win;
    public void PlaySound_Win()
    {
        if (Config.isSound)
        {
            _audioSound.PlayOneShot(win);
        }
    }

    [Header("showView")]
    public AudioClip showView;
    public void PlaySound_ShowView()
    {
        if (Config.isSound)
        {
            _audioSound.PlayOneShot(showView);
        }
    }

    [Header("hideView")]
    public AudioClip hideView;
    public void PlaySound_HideView()
    {
        if (Config.isSound)
        {
            _audioSound.PlayOneShot(hideView);
        }
    }

    [Header("win Star Pop")]
    public AudioClip winstarPop;
    public void PlaySound_WinStarPop()
    {
        if (Config.isSound)
        {
            _audioSound.PlayOneShot(winstarPop);
        }
    }

    [Header("notification")]
    public AudioClip notification;
    public void PlaySound_Notification()
    {
        if (Config.isSound)
        {
            _audioSound.PlayOneShot(notification);
        }
    }

    [Header("reward")]
    public AudioClip reward;
    public void PlaySound_Reward()
    {
        if (Config.isSound)
        {
            _audioSound.PlayOneShot(reward);
        }
    }

    [Header("Get Star")]
    public AudioClip getStar;
    public void PlaySound_GetStar()
    {
        if (Config.isSound)
        {
            _audioSound.PlayOneShot(getStar);
        }
    }
      
    [Header("Get Coin")]
    public AudioClip getCoin;
    public void PlaySound_GetCoin()
    {
        if (Config.isSound)
        {
            _audioSound.PlayOneShot(getCoin);
        }
    }
    
    public void Stop()
    {
        if (Config.isSound)
        {
            _audioSound.Stop();
        }
    }
}
