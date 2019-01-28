using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public Text game_text;
    public string human = "人类胜利";
    public string bird = "鸽子成功回家";
    public float timer = 0;
    public bool ge_win = false;
    public bool human_win = false;

    public AudioSource audiosource;
    public AudioClip human_win_clip;
    public AudioClip bird_win_clip;
    public bool is_played = false;
    private void Awake() {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        audiosource = this.GetComponent<AudioSource>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (human_win) {
            if(is_played == false) {
                audiosource.clip = human_win_clip;
                audiosource.volume = 0.2f;
                audiosource.Play();
                is_played = true;
            }
          
            timer += Time.deltaTime;
            if (timer > 2) {
                game_text.text = human;
                game_text.enabled = true;
            }
        }else if (ge_win) {
            if (is_played == false) {
                audiosource.clip = bird_win_clip;
                audiosource.volume = 2;
                audiosource.Play();
                audiosource.loop = true;
                is_played = true;
            }
            timer += Time.deltaTime;
            if (timer > 2) {
                game_text.text = bird;
                game_text.enabled = true;
            }
        }
    }
}
