using AutoPause.Utilities;
using CustomUI.BeatSaber;
using CustomUI.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace AutoPause
{
    public class AutoPause : MonoBehaviour 
    {
        PlayerController playerController;
        StandardLevelGameplayManager gameplayManager;
        Vector3 previousLeftSaber;
        Vector3 previousRightSaber;
        Vector3 constantLeft;
        Vector3 constantRight;
        float previousLeftSaberSpeed;
        float previousRightSaberSpeed;
        float constantSpeedLeft;
        float constantSpeedRight;
        static RectTransform pauseCanvas;
        static TextMeshProUGUI text;
        public static SoundPlayer TrackNoise = new SoundPlayer(Properties.Resources.Click);
        public static string gameStateCheck;
        int flagvalue;
        float timevalue;
        Image _image;
        Image _image2;

        public static void Load()
        {
            gameStateCheck = BS_Utils.Gameplay.Gamemode.GameMode;
            if (gameStateCheck == "Standard" && Plugin.AutoPauseConfig.GetBool("Main", "Enabled", true, true) == true)
                new GameObject("AutoPause").AddComponent<AutoPause>();
        }

        private void Awake()
        {
            playerController = Resources.FindObjectsOfTypeAll<PlayerController>().FirstOrDefault();
            gameplayManager = Resources.FindObjectsOfTypeAll<StandardLevelGameplayManager>().FirstOrDefault();

        }

        private void Start()
        {
            pauseCanvas = Resources.FindObjectsOfTypeAll<Transform>().First(x => x.name == "PauseMenu").GetComponentsInChildren<RectTransform>(true).First(x => x.name == "Canvas");
            text = BeatSaberUI.CreateText(pauseCanvas, "", new Vector2(0f, 35f));
            text.fontSize = 5f;
            text.alignment = TextAlignmentOptions.Center;
            if (Plugin.AutoPauseConfig.GetBool("Main", "LowSense", false, true))
            {
                flagvalue = 3;
                timevalue = .3f;
            }
            if (Plugin.AutoPauseConfig.GetBool("Main", "SensiMain", true, true))
            {
                flagvalue = 2;
                timevalue = .25f;
            }
            if (Plugin.AutoPauseConfig.GetBool("Main", "SensiRed", false, true))
            {
                flagvalue = 1;
                timevalue = .15f;
            }

            CreateImage();
            InvokeRepeating("TrackingScan", 1.0f, timevalue);
        }

        private void Update()
        {
            constantLeft = playerController.leftSaber.handlePos;
            constantRight = playerController.rightSaber.handlePos;
            constantSpeedLeft = playerController.leftSaber.bladeSpeed;
            constantSpeedRight = playerController.rightSaber.bladeSpeed;
        }

        private void TrackingScan()
        {
            int LeftTrackingFlags = 0;
            int RightTrackingFlags = 0;

            if (previousLeftSaber == null || gameplayManager.gameState == StandardLevelGameplayManager.GameState.Paused)
            {
                //Avoiding unnecessary checking
                previousLeftSaber = constantRight;
                previousRightSaber = constantLeft;
                previousLeftSaberSpeed = constantSpeedRight;
                previousRightSaberSpeed = constantSpeedLeft;
                return;
            }


            //Left Saber Scanning
            if (constantSpeedLeft == previousLeftSaberSpeed)
            {
                Log.Info("Left Speed");
                LeftTrackingFlags += 1;
            }

            if (constantLeft == previousLeftSaber)
            {
                Log.Info("Left Frozen");
                LeftTrackingFlags += 2;
            }

            if (new Vector3(constantLeft.x, 0f, constantLeft.z).sqrMagnitude > 1.3f)
            {
                Log.Info("Left Playspace");
                LeftTrackingFlags += 2;
            }

            if (constantLeft.y < 0f)
            {
                Log.Info("Left Negative Y");
                LeftTrackingFlags += 2;
            }

            //Right Saber Scanning
            if (constantSpeedRight == previousRightSaberSpeed)
            {
                Log.Info("Right Speed");
                RightTrackingFlags += 1;
            }

            if (constantRight == previousRightSaber)
            {
                Log.Info("Left Frozen");
                RightTrackingFlags += 2;
            }

            if (new Vector3(constantRight.x, 0f, constantRight.z).sqrMagnitude > 1.3f)
            {
                Log.Info("Right Playspace");
                RightTrackingFlags += 2;
            }

            if (constantRight.y < 0f)
            {
                Log.Info("Right Negative Y");
                RightTrackingFlags += 2;
            }

            //Decision Making
            if (LeftTrackingFlags >= flagvalue || RightTrackingFlags >= flagvalue)
            {
                text.text = "Issue Detected From:";

                if (LeftTrackingFlags > flagvalue && LeftTrackingFlags == RightTrackingFlags)
                    text.text += " <color=red>Left</color> and <color=blue>Right</color> Saber";
                else
                {
                    if (LeftTrackingFlags > RightTrackingFlags)
                        text.text += " <color=red>Left Saber</color>";

                    if (RightTrackingFlags > LeftTrackingFlags)
                        text.text += " <color=blue>Right Saber</color>";
                }
                
                gameplayManager.HandlePauseTriggered();

                TrackNoise.Play();
                _image.enabled = true;
                _image2.enabled = true;

                Invoke("Reset", 5f);
            }

            previousLeftSaber = constantLeft;
            previousRightSaber = constantRight;
            previousLeftSaberSpeed = constantSpeedLeft;
            previousRightSaberSpeed = constantSpeedRight;
        }

        private void Reset()
        {
            text.text = "";
            _image.enabled = false;
            _image2.enabled = false;
        }
        private void Cleanup()
        {
            playerController = null;
            gameplayManager = null;
            Destroy(this);
        }

        public void OnActiveSceneChanged(Scene prevScene, Scene nextScene)
        {
            if (prevScene.name == "GameCore")
            {
                Log.Info("AutoPause Cleanup Initiated");
                Cleanup();
            }
        }

        public void CreateImage()
        {
            _image = new GameObject("CustomUIImage").AddComponent<Image>();
            _image.material = Sprites.NoGlowMat;
            _image.rectTransform.SetParent(pauseCanvas, false);
            _image.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            _image.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            _image.rectTransform.anchoredPosition = new Vector2(0f, 4.5f);
            _image.rectTransform.sizeDelta = new Vector2(16f, 16f);
            _image.rectTransform.localPosition = new Vector2(-30f, -5f);
            _image.sprite = UIUtilities.BlankSprite;

            SharedCoroutineStarter.instance.StartCoroutine(LoadScripts.LoadSpriteCoroutine("https://auros.red/persistantfiles/sadtracking1.png", (image) =>
            {
                _image.sprite = image;
                Log.Info("Image Created");
            }
            ));

            _image2 = new GameObject("CustomUIImage").AddComponent<Image>();
            _image2.material = Sprites.NoGlowMat;
            _image2.rectTransform.SetParent(pauseCanvas, false);
            _image2.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            _image2.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            _image2.rectTransform.anchoredPosition = new Vector2(0f, 4.5f);
            _image2.rectTransform.sizeDelta = new Vector2(16f, 16f);
            _image2.rectTransform.localPosition = new Vector2(30f, -5f);
            _image2.sprite = UIUtilities.BlankSprite;

            SharedCoroutineStarter.instance.StartCoroutine(LoadScripts.LoadSpriteCoroutine("https://auros.red/persistantfiles/sadtracking2.png", (image) =>
            {
                _image2.sprite = image;
                Log.Info("Image Created");
            }
            ));

            _image.enabled = false;
            _image2.enabled = false;
        }
    }
}