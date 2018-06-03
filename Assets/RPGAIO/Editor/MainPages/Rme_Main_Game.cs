using System;
using System.Collections.Generic;
using System.Linq;
using LogicSpawn.RPGMaker.Core;
using LogicSpawn.RPGMaker.Editor.New;
using UnityEditor;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Editor
{
    public static class Rme_Main_Game
    {
        private static Rmh_Customise Customise
        {
            get { return Rm_RPGHandler.Instance.Customise; }
        }
        public static void Options(Rect fullArea, Rect leftArea, Rect mainArea)
        {
            GUI.Box(fullArea, "", "backgroundBox");
            GUILayout.BeginArea(fullArea);
            RPGMakerGUI.Title("Game Options");

            RPGMakerGUI.SubTitle("General");

            Customise.GameHasAchievements = RPGMakerGUI.Toggle("Enable Achievements?",Customise.GameHasAchievements,
                                                                         GUILayout.Width(300));
            Customise.AchievementUnlockedSound.Audio = RPGMakerGUI.AudioClipSelector("Achievement Unlocked Sound:",Customise.AchievementUnlockedSound.Audio,ref Customise.AchievementUnlockedSound.AudioPath);
            Customise.LoadingScreen.Image = RPGMakerGUI.ImageSelector("Loading Screen:", Customise.LoadingScreen.Image, ref Customise.LoadingScreen.ImagePath);
            Customise.SubtitleDelayBetweenLines = RPGMakerGUI.FloatField("Delay Between Subtitle Lines:", Customise.SubtitleDelayBetweenLines);

            RPGMakerGUI.SubTitle("Enable Popups");
            RPGMakerGUI.Toggle("Exp Gained", ref Customise.EnableExpGainedPopup);
            RPGMakerGUI.Toggle("Level Reached", ref Customise.EnableLevelReachedPopup);
            RPGMakerGUI.Toggle("Skill Exp Gained", ref Customise.EnableSkillExpGainedPopup);

            RPGMakerGUI.SubTitle("Tooltip");
            RPGMakerGUI.Toggle("Follow Mouse Position", ref Customise.TooltipFollowsCursor);

            GUILayout.EndArea();
        }

        private static Rmh_Audio Audio
        {
            get { return Rm_RPGHandler.Instance.Audio; }
        }
        private static Vector2 playlistScrollPos;
        private static bool showPlaylist = true;
        private static bool showBattlePlaylist = true;
        private static bool showScenePlaylists = true;
        public static void GlobalPlaylist(Rect fullArea, Rect leftArea, Rect mainArea)
        {
            GUI.Box(fullArea, "", "backgroundBox");
            GUILayout.BeginArea(fullArea);
            playlistScrollPos = GUILayout.BeginScrollView(playlistScrollPos);
            RPGMakerGUI.Title("Global Music Playlist");
            RPGMakerGUI.SubTitle("Audio Options");
            Audio.PlayThroughSceneSwitch = RPGMakerGUI.Toggle("Persist Through Scenes?", Audio.PlayThroughSceneSwitch,
                                                                         GUILayout.Width(300));

            Audio.LoadAllAudioOnLoad = RPGMakerGUI.Toggle("Load All Audio On Load?", Audio.LoadAllAudioOnLoad,GUILayout.Width(300));
            Audio.FadeOutMusic = RPGMakerGUI.Toggle("Fade Out Tracks?", Audio.FadeOutMusic,GUILayout.Width(300));
            Audio.FadeOutTime = RPGMakerGUI.FloatField("Fade Out Time:", Audio.FadeOutTime, 1,GUILayout.Width(300));
            Audio.FadeInMusic = RPGMakerGUI.Toggle("Fade In Tracks?", Audio.FadeInMusic, GUILayout.Width(300));
            Audio.FadeInTime = RPGMakerGUI.FloatField("Fade In Time:", Audio.FadeInTime, 1, GUILayout.Width(300));
            RPGMakerGUI.Toggle("Shuffle Playlist?", ref Audio.ShufflePlaylist, GUILayout.Width(300));
            var result = RPGMakerGUI.FoldoutToolBar(ref showPlaylist, "Global Playlist", new[] {"+ Music"});
            if(showPlaylist)
            {
                if (result == 0)
                {
                    Audio.GlobalPlaylist.Add(new AudioContainer());
                }
                GUILayout.Space(5);
                for (int index = 0; index < Audio.GlobalPlaylist.Count; index++)
                {
                    var music = Audio.GlobalPlaylist[index];
                    GUILayout.BeginHorizontal(GUILayout.Height(30));
                    GUILayout.Space(5);

                    music.Audio = RPGMakerGUI.AudioClipSelector("Music File:", music.Audio, ref music.AudioPath);

                    if (GUILayout.Button(RPGMakerGUI.DelIcon, "genericButton",GUILayout.Width(25),GUILayout.Height(25)))
                    {
                        Audio.GlobalPlaylist.Remove(music);
                        index--;
                    }
                    GUILayout.Space(5);
                    GUILayout.EndHorizontal();
                    GUILayout.Space(5);
                }

                if(Audio.GlobalPlaylist.Count == 0)
                {
                    EditorGUILayout.HelpBox("Click +Music to add bg music to be played when scene music is not found.",MessageType.Info);
                }
                RPGMakerGUI.EndFoldout();
            }

            if (RPGMakerGUI.Toggle("Unique scene music?", ref Audio.PlayUniqueMusicForScenes))
            {
                var battleResult = RPGMakerGUI.FoldoutToolBar(ref showScenePlaylists, "Scene Playlists", new[] { "+ Scene" });
                if (showScenePlaylists)
                {
                    if (battleResult == 0)
                    {
                        Audio.ScenePlaylists.Add(new SceneMusic());
                    }
                    GUILayout.Space(5);
                    for (int index = 0; index < Audio.ScenePlaylists.Count; index++)
                    {
                        var sceneMusic = Audio.ScenePlaylists[index];
                        GUILayout.BeginHorizontal(GUILayout.Height(30));
                        GUILayout.Space(5);

                        sceneMusic.Music.Audio = RPGMakerGUI.AudioClipSelector("Music File:", sceneMusic.Music.Audio, ref sceneMusic.Music.AudioPath);
                        sceneMusic.SceneName = RPGMakerGUI.SceneSelector("Scene:", ref sceneMusic.SceneName);

                        if (GUILayout.Button(RPGMakerGUI.DelIcon, "genericButton", GUILayout.Width(25), GUILayout.Height(25)))
                        {
                            Audio.ScenePlaylists.Remove(sceneMusic);
                            index--;
                        }
                        GUILayout.Space(5);
                        GUILayout.EndHorizontal();
                        GUILayout.Space(5);
                    }

                    if (Audio.ScenePlaylists.Count == 0)
                    {
                        EditorGUILayout.HelpBox("Click +Music to add music to be played during battles.", MessageType.Info);
                    }
                    RPGMakerGUI.EndFoldout();
                }
            }

            if(RPGMakerGUI.Toggle("Unique battle music?", ref Audio.PlayUniqueMusicForBattles))
            {
                var battleResult = RPGMakerGUI.FoldoutToolBar(ref showBattlePlaylist, "Battle Playlist", new[] { "+ Music" });
                if (showBattlePlaylist)
                {
                    if (battleResult == 0)
                    {
                        Audio.BattlePlaylist.Add(new AudioContainer());
                    }
                    GUILayout.Space(5);
                    for (int index = 0; index < Audio.BattlePlaylist.Count; index++)
                    {
                        var music = Audio.BattlePlaylist[index];
                        GUILayout.BeginHorizontal(GUILayout.Height(30));
                        GUILayout.Space(5);

                        music.Audio = RPGMakerGUI.AudioClipSelector("Music File:", music.Audio, ref music.AudioPath);

                        if (GUILayout.Button(RPGMakerGUI.DelIcon, "genericButton", GUILayout.Width(25), GUILayout.Height(25)))
                        {
                            Audio.BattlePlaylist.Remove(music);
                            index--;
                        }
                        GUILayout.Space(5);
                        GUILayout.EndHorizontal();
                        GUILayout.Space(5);
                    }

                    if (Audio.BattlePlaylist.Count == 0)
                    {
                        EditorGUILayout.HelpBox("Click +Music to add music to be played during battles.", MessageType.Info);
                    }
                    RPGMakerGUI.EndFoldout();
                }
            }

            if (RPGMakerGUI.Toggle("Unique music on Player Death?", ref Audio.PlayUniqueMusicForDeath))
            {
                Audio.DeathMusic.Audio = RPGMakerGUI.AudioClipSelector("- Music File:", Audio.DeathMusic.Audio, ref Audio.DeathMusic.AudioPath);
            }

            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

         public static Rect PadRect(Rect rect, int left, int top)
         {
             return new Rect(rect.x + left, rect.y + top, rect.width - (left * 2), rect.height - (top * 2));
         }
        private static Rect leftAreaB;
        private static Rect mainAreaAlt;
        private static WorldArea selectedWorldArea;
        private static Location selectedWorldAreaLocation;
        private static Vector2 worldAreaScrollPos;
        private static List<WorldArea> worldAreaList
        {
            get { return Rm_RPGHandler.Instance.Customise.WorldMapLocations; }
        }
        public static void WorldMap(Rect fullArea, Rect leftArea, Rect mainArea)
        {
            leftAreaB = new Rect(leftArea.xMax + 5, leftArea.y, leftArea.width, leftArea.height);
            mainAreaAlt = new Rect(leftAreaB.xMax + 5, leftArea.y, mainArea.width - (leftAreaB.width + 5),
                                    leftArea.height);

            GUI.Box(leftArea, "", "backgroundBox");
            GUI.Box(leftAreaB, "", "backgroundBox");
            GUI.Box(mainAreaAlt, "", "backgroundBox");



            GUILayout.BeginArea(PadRect(leftArea, 0, 0));
            RPGMakerGUI.ListArea(worldAreaList, ref selectedWorldArea, Rm_ListAreaType.WorldAreas, false, false);
            GUILayout.EndArea();

            GUILayout.BeginArea(leftAreaB);
            if (selectedWorldArea != null)
            {
                var rect = RPGMakerGUI.ListArea(selectedWorldArea.Locations, ref selectedWorldAreaLocation, Rm_ListAreaType.Location, false, false, Rme_ListButtonsToShow.AllExceptHelp, true, selectedWorldArea.ID);
            }
            GUILayout.EndArea();

            GUILayout.BeginArea(mainAreaAlt);
            worldAreaScrollPos = GUILayout.BeginScrollView(worldAreaScrollPos);
            RPGMakerGUI.Title("World Areas (Not Finished)");
            if(selectedWorldArea != null)
            {
                selectedWorldArea.Name = RPGMakerGUI.TextField("Name: ", selectedWorldArea.Name);
                selectedWorldArea.ImageContainer.Image = RPGMakerGUI.ImageSelector("Image:", selectedWorldArea.ImageContainer.Image, ref selectedWorldArea.ImageContainer.ImagePath);
                if (GUILayout.Button("Open Interaction Node Tree", "genericButton", GUILayout.MaxHeight(30)))
                {
                    var trees = Rm_RPGHandler.Instance.Nodes.WorldMapNodeBank.NodeTrees;
                    var existingTree = trees.FirstOrDefault(t => t.ID == selectedWorldArea.ID);
                    if (existingTree == null)
                    {
                        existingTree = NodeWindow.GetNewTree(NodeTreeType.WorldMap);
                        existingTree.ID = selectedWorldArea.ID;

                        Debug.Log(existingTree.ID + ":::" + existingTree.Name);
                        existingTree.Name = selectedWorldArea.Name;
                        trees.Add(existingTree);
                    }

                    WorldMapNodeWindow.ShowWindow(selectedWorldArea.ID);
                }
            }
            
            if (selectedWorldArea != null)
            {
                RPGMakerGUI.SubTitle("Location Info:");
                if (selectedWorldAreaLocation != null)
                {
                    var loc = selectedWorldAreaLocation;
                    loc.Name = RPGMakerGUI.TextField("Name:", loc.Name);
                    loc.ImageContainer.Image = RPGMakerGUI.ImageSelector("Name:", loc.ImageContainer.Image, ref loc.ImageContainer.ImagePath);
                    loc.Description = RPGMakerGUI.TextField("Description:", loc.Description);
                    RPGMakerGUI.SceneSelector("Scene:", ref loc.SceneName);
                    if(RPGMakerGUI.Toggle("Use Custom Spawn Location?",ref loc.UseCustomLocation))
                    {
                        loc.CustomSpawnLocation = EditorGUILayout.Vector3Field("Location:", loc.CustomSpawnLocation);
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("Select a location in the world area to begin editing it.", MessageType.Info);
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Add or select a new field to customise world areas.", MessageType.Info);
            }
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }
    }
}