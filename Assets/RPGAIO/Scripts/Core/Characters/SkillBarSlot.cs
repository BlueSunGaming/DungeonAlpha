using System;
using System.Linq;
using LogicSpawn.RPGMaker.API;
using LogicSpawn.RPGMaker.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    public class SkillBarSlot
    {
        private SkillHandler _skillHandler;
        public bool IsItem;
        public ItemType ItemType;
        public string SkillId;
        public string ItemInventoryRefId;


        public ImageContainer _imageContainer;

        [JsonIgnore]
        public Texture2D Image
        {
            get { return _imageContainer.Image; }
        }

        [JsonIgnore]
        public SkillHandler SkillHandler
        {
            get { return _skillHandler ?? (_skillHandler = GetObject.PlayerCharacter.SkillHandler); }
        }

        [JsonIgnore]
        public bool InUse
        {
            get { return !string.IsNullOrEmpty(SkillId) || !string.IsNullOrEmpty(ItemInventoryRefId); }
        }

        [JsonIgnore]
        public bool Available
        {
            get { return IsItem ? Item != null : Skill != null; }
        }

        [JsonIgnore]
        public bool Usable
        {
            get
            {
                if (!Available || !InUse) return false;

                if(IsItem)
                {   
                    return new[] {ItemType.Apparel, ItemType.Weapon, ItemType.Socket, ItemType.Book, ItemType.Consumable}.Any(s => s == Item.ItemType) ||
                           Item.RunEventOnUse && !Item.EventHasRun;
                }
                else
                {
                    //return Skill.Unlocked && Skill.CanCast(GetObject.PlayerCharacter);
                    return true;
                }
            }
        }

        [JsonIgnore]
        public Item Item
        {
            get
            {
               if(ItemInventoryRefId == null) return null;

                var isInInventory = GetObject.PlayerMono.Player.Inventory.GetReferencedItem(ItemInventoryRefId);
                var isEquipped = (ItemType == ItemType.Apparel || ItemType == ItemType.Weapon) ? GetObject.PlayerMono.Player.Equipment.GetReferencedItem(ItemInventoryRefId) : null;

                if (isInInventory != null) return isInInventory;
                if (isEquipped != null) return isEquipped;

                return null;
            }
        }

        [JsonIgnore]
        public Skill Skill
        {
            get
            {
                if (SkillId == null) return null;

                return SkillHandler.AvailableSkills.FirstOrDefault(s => s.ID == SkillId);
            }
        }

        public SkillBarSlot(SkillHandler skillHandler)
        {
            _skillHandler = skillHandler;
            _imageContainer = new ImageContainer();
        }

        public void ChangeSlotTo(Item item)
        {
            if (item.ItemType == ItemType.Consumable || item.ItemType == ItemType.Weapon || item.ItemType == ItemType.Apparel)
            {
                EmptySlot();
                SetImage(item.ImagePath);
                IsItem = true;
                ItemInventoryRefId = item.InventoryRefID;
                ItemType = item.ItemType;

            }
        }

        public void ChangeSlotTo(Skill skill)
        {
            EmptySlot();
            SetImage(skill.Image.ImagePath);
            IsItem = false;
            SkillId = skill.ID;
        }

        private void SetImage(string path)
        {
            _imageContainer = new ImageContainer {ImagePath = path};
        }


        public void Use()
        {
            if(IsItem)
            {
                var isEquipped = (ItemType == ItemType.Apparel || ItemType == ItemType.Weapon) ? GetObject.PlayerMono.Player.Equipment.GetReferencedItem(ItemInventoryRefId) : null;

                if (isEquipped != null)
                {
                    var slot = GetObject.PlayerCharacter.Equipment.EquippedItems.FirstOrDefault(s => s.Item == Item);
                    if(slot != null)
                    {
                        GetObject.PlayerCharacter.Equipment.UnEquipItem(slot,isEquipped);    
                    }
                    return;
                }

                GetObject.PlayerCharacter.Inventory.UseItemByRef(ItemInventoryRefId);    
            }
            else
            {
                if(Skill != null)
                    GetObject.PlayerController.UseRefSkill(Skill);
            }
        }

        public void EmptySlot()
        {
            SkillId = null;
            ItemInventoryRefId = null;
            _imageContainer = new ImageContainer();

            var index = Array.IndexOf(RPG.GetPlayerCharacter.SkillHandler.Slots, this);

            SkillBarUI.Instance.SkillButtons[index].SkillImage.sprite = null;
            SkillBarUI.Instance.SkillButtons[index].SkillImage.color = Color.clear;
        }

    }
}