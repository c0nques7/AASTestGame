using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class LanguageControl : MonoBehaviour {

	//[Header("JSON File Names")]
	//public string mainScreenLanguage = "MainScreenLanguage"; 
	//public string optionsScreenLanguage = "OptionsScreenLanguages";

	public Text[] main_titles;
	public Text[] buttons_options;
	public Text[] options_video;
	public Text[] options_audio;
	public Text[] options_game;

	public WS_MAIN_LANGUAGE ws_main_language;
	public WS_OPTIONS_LANGUAGE ws_options_language;

	private MenuControl _menuControl;
	private OptionsControl _optionsControl;

    //Editor
    [HideInInspector]
    public int currentTab;
    [HideInInspector]
    public int currentTabTwo;
    [HideInInspector]
    public int previousTab = -1;
    [HideInInspector]
    public int previousTabTwo = -1;

    // Use this for initialization
    void Start () {
		ws_main_language = new WS_MAIN_LANGUAGE ();
		ws_options_language = new WS_OPTIONS_LANGUAGE ();
		_menuControl = this.GetComponent<MenuControl> ();
		_optionsControl = this.GetComponent<OptionsControl> ();

	}
	
	// Update is called once per frame
	void Update () {
	}
   
	// SET ALL CHANGES
	public void SetLanguageInGame(){
		SetMainLanguage ();
		SetOptionsLanguage ();
	}// END

	// MAIN MENU
	public void SetMainLanguage(){
		if (_optionsControl != null && _optionsControl._gameConfig != null && ws_main_language != null) {
			if (_optionsControl._gameConfig.language == 0) { // ENGLISH
                //Debug.Log ("Change To English!");
                if (_menuControl.inGame == false)
                {
                    main_titles[0].text = ws_main_language.play_en;
                    main_titles[1].text = ws_main_language.options_en;
                    _menuControl.menu[0].SetText(ws_main_language.continue_en);
                    _menuControl.menu[1].SetText(ws_main_language.play_en);
                    _menuControl.menu[2].SetText(ws_main_language.options_en);
                    _menuControl.menu[3].SetText(ws_main_language.credits_en);
                    _menuControl.menu[4].SetText(ws_main_language.exit_en);

                }
                else
                {
                    _menuControl.menu[0].SetText(ws_main_language.resume_en);
                    _menuControl.menu[1].SetText(ws_main_language.options_en);
                    _menuControl.menu[2].SetText(ws_main_language.exit_en);
                }
			} else if (_optionsControl._gameConfig.language == 1) { // PTBR
                //Debug.Log ("Change To PTBR");
                if (_menuControl.inGame == false) // not Pause Menu
                {
                    main_titles[0].text = ws_main_language.play_ptbr;
                    main_titles[1].text = ws_main_language.options_ptbr;
                    _menuControl.menu[0].SetText(ws_main_language.continue_ptbr);
                    _menuControl.menu[1].SetText(ws_main_language.play_ptbr);
                    _menuControl.menu[2].SetText(ws_main_language.options_ptbr);
                    _menuControl.menu[3].SetText(ws_main_language.credits_ptbr);
                    _menuControl.menu[4].SetText(ws_main_language.exit_ptbr);

                }
                else // In Pause Menu
                {
                    _menuControl.menu[0].SetText(ws_main_language.resume_ptbr);
                    _menuControl.menu[1].SetText(ws_main_language.options_ptbr);
                    _menuControl.menu[2].SetText(ws_main_language.exit_ptbr);
                }

            }
		}
	}// END

	// OPTIONS LANGUAGE
	public void SetOptionsLanguage(){
		if (_optionsControl != null && _optionsControl._gameConfig != null && ws_main_language != null) {
			if (_optionsControl._gameConfig.language == 0) { // ENGLISH
				// BUTTONS
				buttons_options[0].text = ws_options_language.video_en;
				buttons_options[1].text = ws_options_language.audio_en;
				buttons_options[2].text = ws_options_language.game_en;
				buttons_options[3].text = ws_options_language.apply_en;
				buttons_options[4].text = ws_options_language.return_en;

				// VIDEO
				options_video[0].text = ws_options_language.displayMode_en;
				options_video[1].text = ws_options_language.targetDisplay_en;
				options_video[2].text = ws_options_language.resolution_en;
				options_video[3].text = ws_options_language.graphicsQuality_en;
				options_video[4].text = ws_options_language.antialiasing_en;
				options_video[5].text = ws_options_language.vsync_en;


				// AUDIO
				options_audio [0].text = ws_options_language.masterVolume_en;
				options_audio [1].text = ws_options_language.musicVolume_en;
				options_audio [2].text = ws_options_language.effectsVolume_en;
				options_audio [3].text = ws_options_language.voiceVolume_en;
				options_audio [4].text = ws_options_language.micVolume_en;
				options_audio [5].text = ws_options_language.soundBackground_en;

				// GAME
				options_game [0].text = ws_options_language.horizontalSensitivy_en;
				options_game [1].text = ws_options_language.vericalSensitivy_en;
				options_game [2].text = ws_options_language.difficulty_en;
				options_game [3].text = ws_options_language.language_en;
				options_game [4].text = ws_options_language.forward_en;
				options_game [5].text = ws_options_language.back_en;
				options_game [6].text = ws_options_language.left_en;
				options_game [7].text = ws_options_language.right_en;
				options_game [8].text = ws_options_language.crouch_en;
				options_game [9].text = ws_options_language.jump_en;
				options_game [10].text = ws_options_language.tips_en;

			} else if (_optionsControl._gameConfig.language == 1) { // PTBR
				// BUTTONS
				buttons_options[0].text = ws_options_language.video_ptbr;
				buttons_options[1].text = ws_options_language.audio_ptbr;
				buttons_options[2].text = ws_options_language.game_ptbr;
				buttons_options[3].text = ws_options_language.apply_ptbr;
				buttons_options[4].text = ws_options_language.return_ptbr;

				// VIDEO
				options_video[0].text = ws_options_language.displayMode_ptbr;
				options_video[1].text = ws_options_language.targetDisplay_ptbr;
				options_video[2].text = ws_options_language.resolution_ptbr;
				options_video[3].text = ws_options_language.graphicsQuality_ptbr;
				options_video[4].text = ws_options_language.antialiasing_ptbr;
				options_video[5].text = ws_options_language.vsync_ptbr;

				// AUDIO
				options_audio [0].text = ws_options_language.masterVolume_ptbr;
				options_audio [1].text = ws_options_language.musicVolume_ptbr;
				options_audio [2].text = ws_options_language.effectsVolume_ptbr;
				options_audio [3].text = ws_options_language.voiceVolume_ptbr;
				options_audio [4].text = ws_options_language.micVolume_ptbr;
				options_audio [5].text = ws_options_language.soundBackground_ptbr;

				// GAME
				options_game [0].text = ws_options_language.horizontalSensitivy_ptbr;
				options_game [1].text = ws_options_language.vericalSensitivy_ptbr;
				options_game [2].text = ws_options_language.difficulty_ptbr;
				options_game [3].text = ws_options_language.language_ptbr;
				options_game [4].text = ws_options_language.forward_ptbr;
				options_game [5].text = ws_options_language.back_ptbr;
				options_game [6].text = ws_options_language.left_ptbr;
				options_game [7].text = ws_options_language.right_ptbr;
				options_game [8].text = ws_options_language.crouch_ptbr;
				options_game [9].text = ws_options_language.jump_ptbr;
				options_game [10].text = ws_options_language.tips_ptbr;
			}
		}// END
	}
}
