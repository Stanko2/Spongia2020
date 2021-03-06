﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

public class MainMenu : MonoBehaviour
{
    public Animation Animation;
    public Animation SettingsAnim;
    public Animation MultiplayerAnim;
    public Animation HowToPlayAnim;
    public Dropdown dropdown;
    Vector2Int[] resolutions; 
    Vector2Int resolution;
    public TextMeshProUGUI MainText;
    public AudioMixer audioMixer;
    public Slider MusicSlider;
    public Slider SFXSlider;
    public Text MessageText;
    public Toggle fullscreen;
    [Header("Start Anim Settings")] 
    public string[] texts;
    public Animation textShow;
    public TextMeshProUGUI showText;
    public Animation backgroundShow;

    private string _message;
    public string Message {
        get { return _message; }
        set {
            _message = value;
            MessageText.text = _message;
            MessageText.GetComponent<Animation>().Play();
        }
    }

    enum action { Howtoplay, singleplayer, settings, quit, none };
    private action Action = action.settings;

    // Start is called before the first frame update
    void Start()
    {
        SaveData saveData = GameSaver.LoadGame();
        if (saveData != null)
        {
            LoadData(saveData);
        }
        Vector2Int[] resolutions = { new Vector2Int(3840, 2160), new Vector2Int(2560, 1440), new Vector2Int(1920, 1080), new Vector2Int(1280, 720), new Vector2Int(854,480)};
        resolution = new Vector2Int(Display.main.systemWidth, Display.main.systemHeight);
        Animation.Play("Show");
        int height = Display.main.systemHeight;
        int width = Display.main.systemWidth;
        foreach (Vector2 res in resolutions)
        {
            if (res.x <= width && res.y <= height)
            {
                dropdown.options.Add(new Dropdown.OptionData(res.x + "×" + res.y));
            }
        }
    }

    private void ChangeVolume()
    {
        audioMixer.SetFloat("SFX", SFXSlider.value);
        audioMixer.SetFloat("Music", MusicSlider.value);
    }

    public void SinglePlayer()
    {
        Action = action.singleplayer;
        Animation.Play("Hide");
    }

    public void HowToplay()
    {
        Action = action.Howtoplay;
        Animation.Play("Hide");
    }



    public void Exit()
    {
        Action = action.quit;
        Animation.Play("Hide");
    }

    public void Settings()
    {
        Action = action.settings;
        Animation.Play("Hide");
        MainText.SetText("Settings");
    }
    
    public void BackSettings()
    {
        SettingsAnim.Play("Hide");
        Animation.Play("Show");
        Screen.SetResolution(resolution.x, resolution.y, fullscreen.isOn);
        MainText.SetText("Pandemic 3000");
        ChangeVolume();
        SaveData saveData = GetSaveData();
        GameSaver.Savegame(saveData);
    }
    
    public void BackHowToPlay()
    {
        HowToPlayAnim.Play("Hide");
        Animation.Play("Show");
        MainText.SetText("Pandemic 3000");
        Action = action.none;
    }

    public void OnHideAnimEnd()
    {
        switch (Action)
        {
            case action.singleplayer:
                // MultiplayerAnim.Play("Show");
                // MainText.SetText("Create Game");
                StartCoroutine(GameStart());
                break;
            case action.Howtoplay:
                HowToPlayAnim.Play("Show");
                MainText.SetText("How To Play");
                break;
            case action.settings:
                SettingsAnim.Play("Show");
                break;
            case action.quit:
                Application.Quit();
                break;
            default:
                break;
        }
        Action = action.none;
    }

    private IEnumerator GameStart()
    {
        backgroundShow.Play("ShowBackground");
        yield return new WaitWhile(() => backgroundShow.isPlaying);
        foreach (var text in texts)
        {
            showText.SetText(text);
            textShow.Play("ShowText");
            yield return new WaitWhile(()=>textShow.isPlaying);
        }
        SceneManager.LoadScene(1);
    }

    public void dropDownChanged()
    {
        Dropdown.OptionData optionData = dropdown.options[dropdown.value];
        string options = optionData.text;
        string[] option = options.Split('×');
        resolution.x = int.Parse(option[0]);
        resolution.y = int.Parse(option[1]);
    }

    private SaveData GetSaveData()
    {
        SaveData saveData = new SaveData();
        saveData.MusicVol = MusicSlider.value;
        saveData.SFXVol = SFXSlider.value;
        saveData.resX = resolution.x;
        saveData.resY = resolution.y;
        saveData.fullscreen = fullscreen.isOn;
        return saveData;
    }

    private void LoadData(SaveData data)
    {
        SFXSlider.value = data.SFXVol;
        MusicSlider.value = data.MusicVol;
        ChangeVolume();
        Screen.SetResolution(data.resX, data.resY, data.fullscreen);
        dropdown.value = dropdown.options.IndexOf(new Dropdown.OptionData(data.resX + "×" + data.resY));
    }
}
