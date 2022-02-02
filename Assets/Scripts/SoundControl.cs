using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SoundControl : MonoBehaviour
{
    public AudioSource music; // This will be accesed from outside

    [SerializeField] private Slider volumeSlider;
    [SerializeField] private TextMeshProUGUI volumeLabel;

    // Start is called before the first frame update
    void Start()
    {
        music = GetComponent<AudioSource>();
        volumeSlider.value = DataSaver.Instance.volume;
        UpdateMusic();
    }

    // Update is called once per frame
    void Update()
    {   
        // Volume is updated according to slider value
        if (volumeSlider.value != DataSaver.Instance.volume)
        {
            DataSaver.Instance.volume = volumeSlider.value;
            UpdateMusic();
        }        
    }

    private void UpdateMusic()
    {
        music.volume = DataSaver.Instance.volume;
        volumeLabel.text = "" + (int)(DataSaver.Instance.volume * 100);
    }
}
