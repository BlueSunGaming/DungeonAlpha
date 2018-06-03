using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace LogicSpawn.RPGMaker
{
    public class Rmh_GameInfo
    {
        public string GameTitle;
        public string GameCompany;
        public List<Rm_CreditsInfo> CreditsInfo;
        public Cursors Cursors;
        public MinimapOptions MinimapOptions;

        public Rmh_GameInfo()
        {
            GameTitle = "My Game";
            GameCompany = "My Company";
            CreditsInfo = new List<Rm_CreditsInfo>()
                              {
                                  new Rm_CreditsInfo()
                                      {
                                          Type = CreditsType.Text,
                                          Text = "Thanks for playing."
                                      }
                              };
            Cursors = new Cursors();
            Cursors.DefaultCursorPath = "";
            Cursors.NpcCursorPath = "";
            Cursors.InteractCursorPath = "";
            Cursors.ItemCursorPath = "";
            Cursors.EnemyCursorPath = "";
            Cursors.HarvestCursorPath = "";
            Cursors.DraggableCursorPath = "";

            MinimapOptions = new MinimapOptions();
            MinimapOptions.PlayerIconPath = "";
            MinimapOptions.NpcIconPath = "";
            MinimapOptions.InteractIconPath = "";
            MinimapOptions.HarvestIconPath = "";
            MinimapOptions.EnemyIconPath = "";
        }
    }

    public class Cursors
    {
        public string DefaultCursorPath;
        [JsonIgnore]
        public Texture2D _default ;
        [JsonIgnore]
        public Texture2D Default
        {
            get { return _default ?? (_default = Resources.Load(DefaultCursorPath) as Texture2D); }
            set { _default = value; }
        }

        public string NpcCursorPath;
        [JsonIgnore]
        public Texture2D _npc ;
        [JsonIgnore]
        public Texture2D NPC
        {
            get { return _npc ?? (_npc = Resources.Load(NpcCursorPath) as Texture2D); }
            set { _npc = value; }
        }

        public string InteractCursorPath;
        [JsonIgnore]
        public Texture2D _interact ;
        [JsonIgnore]
        public Texture2D Interact
        {
            get { return _interact ?? (_interact = Resources.Load(InteractCursorPath) as Texture2D); }
            set { _interact = value; }
        }

        public string ItemCursorPath;
        [JsonIgnore]
        public Texture2D _item ;
        [JsonIgnore]
        public Texture2D Item
        {
            get { return _item ?? (_item = Resources.Load(ItemCursorPath) as Texture2D); }
            set { _item = value; }
        }

        public string EnemyCursorPath;
        [JsonIgnore]
        public Texture2D _enemy ;
        [JsonIgnore]
        public Texture2D Enemy
        {
            get { return _enemy ?? (_enemy = Resources.Load(EnemyCursorPath) as Texture2D); }
            set { _enemy = value; }
        }

        public string HarvestCursorPath;
        [JsonIgnore]
        public Texture2D _harvest ;
        [JsonIgnore]
        public Texture2D Harvest
        {
            get { return _harvest ?? (_harvest = Resources.Load(HarvestCursorPath) as Texture2D); }
            set { _harvest = value; }
        }


        public string DraggableCursorPath;
        [JsonIgnore]
        public Texture2D _draggable ;
        [JsonIgnore]
        public Texture2D Draggable
        {
            get { return _draggable ?? (_draggable = Resources.Load(DraggableCursorPath) as Texture2D); }
            set { _draggable = value; }
        }
    }    
    
    public class MinimapOptions
    {

        public bool RotateMinimapWithPlayer;
        public bool ShowMinimap;

        public string PlayerIconPath;
        [JsonIgnore]
        public Texture2D _player;
        [JsonIgnore]
        public Texture2D Player
        {
            get { return _player ?? (_npc = Resources.Load(PlayerIconPath) as Texture2D); }
            set { _player = value; }
        }
        [JsonIgnore]
        public Sprite PlayerSprite
        {
            get { return Resources.Load<Sprite>(PlayerIconPath); }
        }

        public string NpcIconPath;
        [JsonIgnore]
        public Texture2D _npc;
        [JsonIgnore]
        public Texture2D NPC
        {
            get { return _npc ?? (_npc = Resources.Load(NpcIconPath) as Texture2D); }
            set { _npc = value; }
        }
        [JsonIgnore]
        public Sprite NpcSprite
        {
            get { return Resources.Load<Sprite>(NpcIconPath); }
        }


        public string InteractIconPath;
        [JsonIgnore]
        public Texture2D _interact;
        [JsonIgnore]
        public Texture2D Interact
        {
            get { return _interact ?? (_interact = Resources.Load(InteractIconPath) as Texture2D); }
            set { _interact = value; }
        }
        [JsonIgnore]
        public Sprite InteractSprite
        {
            get { return Resources.Load<Sprite>(InteractIconPath); }
        }

        public string EnemyIconPath;
        [JsonIgnore]
        public Texture2D _enemy;
        [JsonIgnore]
        public Texture2D Enemy
        {
            get { return _enemy ?? (_enemy = Resources.Load(EnemyIconPath) as Texture2D); }
            set { _enemy = value; }
        }
        [JsonIgnore]
        public Sprite EnemySprite
        {
            get { return Resources.Load<Sprite>(EnemyIconPath); }
        }

        public string HarvestIconPath;
        [JsonIgnore]
        public Texture2D _harvest;
        [JsonIgnore]
        public Texture2D Harvest
        {
            get { return _harvest ?? (_harvest = Resources.Load(HarvestIconPath) as Texture2D); }
            set { _harvest = value; }
        }
        [JsonIgnore]
        public Sprite HarvestSprite
        {
            get { return Resources.Load<Sprite>(HarvestIconPath); }
        }

        public MinimapOptions()
        {
            ShowMinimap = true;
            RotateMinimapWithPlayer = false;
        }
    }
}