using System.Collections.Generic;
using LogicSpawn.RPGMaker.Core;
using UnityEngine;

namespace LogicSpawn.RPGMaker
{
    public class Rmh_Customise
    {
        //Game
        public bool SaveInMenu;
        public bool SaveOnSceneSwitch;

        public bool PersistGameObjectInfo;
        public bool SaveEnemyStatus;
        public bool SaveGameObjectPosition;
        public bool SaveGameObjectRotation;
        public bool SaveGameObjectDestroyed;
        public bool SaveGameObjectEnabled;

        public bool GameHasAchievements;
        public AudioContainer AchievementUnlockedSound;
        public List<WorldArea> WorldMapLocations;
        public ImageContainer LoadingScreen;

        //Subtitles
        public float SubtitleDelayBetweenLines;


        //Popups
        public bool EnableExpGainedPopup;
        public bool EnableSkillExpGainedPopup;
        public bool EnableLevelReachedPopup;

        //Tooltip
        public bool TooltipFollowsCursor;

        //camera
        public float TopDownHeight;
        public float TopDownDistance;
        public float CameraXOffset;
        public float CameraYOffset;
        public float CameraZOffset;

        public bool PressBothMouseButtonsToMove;
        public bool RotateCameraWithPlayer;
        public bool EnableOrbitPlayer;
        public ClickOption OrbitPlayerOption;
        public bool EnableClickToRotate;
        public ClickOption ClickToRotateOption;

        public Rmh_Customise()
        {
            SaveInMenu = true;
            SaveOnSceneSwitch = true;

            EnableExpGainedPopup = true;
            EnableLevelReachedPopup = true;

            PersistGameObjectInfo = true;
            SaveEnemyStatus = true;
            SaveGameObjectPosition = true;
            SaveGameObjectRotation = true;
            SaveGameObjectDestroyed = true;
            SaveGameObjectEnabled = true;
            WorldMapLocations = new List<WorldArea>();
            GameHasAchievements = true;
            AchievementUnlockedSound = new AudioContainer();
            LoadingScreen = new ImageContainer();

            TopDownHeight = 10;
            TopDownDistance = 10;
            CameraXOffset = 0;
            CameraYOffset = 0;
            CameraZOffset = 0;

            SubtitleDelayBetweenLines = 0.2f;

            PressBothMouseButtonsToMove = true;
            RotateCameraWithPlayer = true;
            EnableOrbitPlayer = true;
            OrbitPlayerOption = ClickOption.Left;
            EnableClickToRotate = true;
            ClickToRotateOption = ClickOption.Right;

            TooltipFollowsCursor = true;
        }
    }

    public class LayerMaskObject : ScriptableObject
    {
        public LayerMask Mask;

        public LayerMaskObject()
        {
            Mask = new LayerMask();
        }
    }
}