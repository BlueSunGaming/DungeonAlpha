using System;
using LogicSpawn.RPGMaker;
using LogicSpawn.RPGMaker.API;
using LogicSpawn.RPGMaker.Core;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Testing
{
    public class LootItemMono : MonoBehaviour
    {
        public LootItem LootItem;
        public GameObject Canvas;
        public LootItemModel Model;
        public Text ItemText;

        public GameObject BookCanvas;
        public Text UseBookText;

        private Transform _transform;
        private Transform _playerTransform;

        private Vector3 _readPos;
        private bool _readingBook;
        private bool _isBook;

        void OnEnable()
        {
            _transform = transform;
            _playerTransform = GetObject.PlayerMonoGameObject.transform;
        }

        void Awake()
        {
            Canvas.GetComponent<Canvas>().worldCamera = GetObject.RPGCamera.GetComponent<Camera>();
            BookCanvas.GetComponent<Canvas>().worldCamera = GetObject.RPGCamera.GetComponent<Camera>();
            Model = Canvas.GetComponent<LootItemModel>();
        }

        public void Init()
        {
            Model.Item = LootItem.Item;
            _isBook = false;
            //Set book settings
            var book = Model.Item as Book;
            if( book != null)
            {
                _isBook = true;
                switch(book.BookType)
                {
                    case BookType.Text:
                        UseBookText.text = "[Read Book]";
                        break;
                    case BookType.Picture:
                        UseBookText.text = "[View Book]";
                        break;
                    case BookType.Audio:
                        UseBookText.text = "[Listen]";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                BookCanvas.SetActive(false);
            }

            //Set normal settings
            SetLootText();
        }

        void SetLootText()
        {
            var lootText = LootItem.Gold != 0 ? LootItem.Gold + "g" : LootItem.Item.Name;
            if (LootItem.Item != null)
            {
                var stackable = LootItem.Item as IStackable;
                if (stackable != null && stackable.CurrentStacks > 0)
                {
                    lootText += " x" + stackable.CurrentStacks;
                }

                var color = RPG.Items.GetRarityColorById(LootItem.Item.RarityID);
                lootText = RPG.UI.FormatString(color, lootText);
            }

            ItemText.text = lootText;
        }

        public void Update()
        {
            if (_readingBook)
            {
                if (Vector3.Distance(_readPos, _playerTransform.position) > 4)
                {
                    _readingBook = false;
                    BookHandler.Instance.CloseBook();
                }
            }

            SetLootText();
            if (LootItem == null)
            {
                Destroy(gameObject);
                return;
            }

            if (Canvas == null)
            {
                return;
            }

            Canvas.transform.rotation = GetObject.RPGCamera.transform.rotation;
            BookCanvas.transform.rotation = GetObject.RPGCamera.transform.rotation;

            var showLootInfo = Vector3.Distance(_transform.position, _playerTransform.position) < Rm_RPGHandler.Instance.Items.LootShowDistance;
            Canvas.SetActive(showLootInfo);
            BookCanvas.SetActive(_isBook && showLootInfo);

        }

        public void Loot()
        {
            var looted = false;

            if(Vector3.Distance(GetObject.PlayerMonoGameObject.transform.position, transform.position) > 
                Rm_RPGHandler.Instance.Items.MaxLootRange)
            {
                Debug.Log("Item is too far away to loot.");
                return;
            }

            if (LootItem.Gold != 0)
            {
                GetObject.PlayerMono.Player.Inventory.AddGold(LootItem.Gold);
                Debug.Log("You looted: " + LootItem.Gold + " gold.");
                looted = true;
            }
            else
            {
                if (Rm_RPGHandler.Instance.Items.AutoEquipOnLoot && (LootItem.Item is Apparel || LootItem.Item is Weapon))
                {
                    if (Rm_RPGHandler.Instance.Items.AutoEquipAlways)
                    {
                        var result = GetObject.PlayerCharacter.Equipment.EquipItem(LootItem.Item);
                        if (result == EquipResult.Success) looted = true;
                    }
                    else if (Rm_RPGHandler.Instance.Items.AutoEquipIfSlotEmpty)
                    {
                        if (LootItem.Item is Weapon)
                        {
                            var slot = GetObject.PlayerCharacter.Equipment.GetSlot("Weapon");
                            if (slot.Item == null)
                            {
                                var result = GetObject.PlayerCharacter.Equipment.EquipItem(LootItem.Item);
                                if (result == EquipResult.Success) looted = true;
                            }
                        }
                        else
                        {
                            var apparel = LootItem.Item as Apparel;
                            var slot = GetObject.PlayerCharacter.Equipment.GetSlot(apparel.apparelSlotID);
                            if (slot.Item == null)
                            {
                                var result = GetObject.PlayerCharacter.Equipment.EquipItem(LootItem.Item);
                                if (result == EquipResult.Success) looted = true;
                            }
                        }
                    }
                    else if (Rm_RPGHandler.Instance.Items.AutoEquipBasedOnLevel)
                    {
                        if (LootItem.Item is Weapon)
                        {
                            var slot = GetObject.PlayerCharacter.Equipment.GetSlot("Weapon");
                            var offHandslot = GetObject.PlayerCharacter.Equipment.GetSlot("OffHand");
                            var wep = LootItem.Item as Weapon;
                            if (slot.Item == null || (slot.Item as Weapon).RequiredLevel < wep.RequiredLevel || (Rm_RPGHandler.Instance.Items.AllowDualWield && Rm_RPGHandler.Instance.Items.EnableOffHandSlot && offHandslot.Item == null))
                            {
                                var result = GetObject.PlayerCharacter.Equipment.EquipItem(LootItem.Item);
                                if (result == EquipResult.Success) looted = true;
                            }
                        }
                        else
                        {
                            var apparel = LootItem.Item as Apparel;
                            var slot = GetObject.PlayerCharacter.Equipment.GetSlot(apparel.apparelSlotID);
                            if (slot.Item == null || (slot.Item as Apparel).RequiredLevel < apparel.RequiredLevel)
                            {
                                var result = GetObject.PlayerCharacter.Equipment.EquipItem(LootItem.Item);
                                if (result == EquipResult.Success) looted = true;
                            }
                        }
                    }
                }

                if (!looted)
                {
                    looted = GetObject.PlayerMono.Player.Inventory.AddItem(LootItem.Item);

                    if (looted)
                    {
                        Debug.Log("You looted: " + LootItem.Item.Name);
                    }
                }


            }

            if (looted)
            {
                _readingBook = false;
                BookHandler.Instance.CloseBook();
                TooltipUI.Clear();
                Destroy(gameObject);
            }
        }

        public void UseBook()
        {
            _readingBook = true;
            _readPos = GetObject.PlayerMonoGameObject.transform.position;
            BookHandler.Instance.CloseBook();
            BookHandler.Instance.ReadBook(Model.Item as Book);
        }
    }
}