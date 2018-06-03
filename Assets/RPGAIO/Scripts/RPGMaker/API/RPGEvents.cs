using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Core.Interaction;
using LogicSpawn.RPGMaker.Core;
using LogicSpawn.RPGMaker.Generic;

namespace LogicSpawn.RPGMaker.API
{
    public class RPGEvents
    {
        //Minimap Zoom
        public event EventHandler MinimapZoom;

        public void OnMinimapZoom()
        {
            var handler = MinimapZoom;
            if (handler != null)
            {
                handler(this, null);
            }
        }

        //Quest Accept/Complete
        public event EventHandler QuestStatusUpdate;

        public void OnQuestStatusUpdate()
        {
            var handler = QuestStatusUpdate;
            if (handler != null)
            {
                handler(this, null);
            }
        }

        //StartHarvesting
        public event EventHandler<StartHarvestingEventArgs> StartHarvesting;

        public void OnStartHarvesting(StartHarvestingEventArgs e)
   		{
            var handler = StartHarvesting;
            if (handler != null)
            {
                handler(this, e);
            }
   		}

        public class StartHarvestingEventArgs : EventArgs
        {
            public InteractableHarvestable Harvestable;
        }
        //StopHarvesting
        public event EventHandler<StopHarvestingEventArgs> StopHarvesting;

        public void OnStopHarvesting(StopHarvestingEventArgs e)
   		{
            var handler = StopHarvesting;
            if (handler != null)
            {
                handler(this, e);
            }
   		}

        public class StopHarvestingEventArgs : EventArgs
        {
        }
        //OpenVendor
        public event EventHandler<OpenVendorEventArgs> OpenVendor;

        public void OnOpenVendor(OpenVendorEventArgs e)
   		{
            var handler = OpenVendor;
            if (handler != null)
            {
                handler(this, e);
            }
   		}

        public class OpenVendorEventArgs : EventArgs
        {
            public string VendorShop { get; set; }
        }

        //OpenCrafting
        public event EventHandler<OpenCraftingEventArgs> OpenCrafting;

        public void OnOpenCrafting(OpenCraftingEventArgs e)
   		{
            var handler = OpenCrafting;
            if (handler != null)
            {
                handler(this, e);
            }
   		}

        public class OpenCraftingEventArgs : EventArgs
        {
        }

        //MenuOpened
        public event EventHandler<OpenMenuEventArgs> MenuOpened;

        public void OnMenuOpened(OpenMenuEventArgs e)
   		{
            var handler = MenuOpened;
            if (handler != null)
            {
                handler(this, e);
            }
   		}

        public class OpenMenuEventArgs : EventArgs
        {
        }
        //InventoryUpdated
        public event EventHandler<InventoryUpdateEventArgs> InventoryUpdated;

        public void OnInventoryUpdate(InventoryUpdateEventArgs e)
   		{
            if (!GetObject.InGame) return;

            var handler = InventoryUpdated;
            if (handler != null)
            {
                handler(this, e);
            }
   		}

        public class InventoryUpdateEventArgs : EventArgs
        {
        }

        //EquippedItem
        public event EventHandler<EquippedItemEventArgs> EquippedItem;

        public void OnEquippedItem(EquippedItemEventArgs e)
   		{
            var handler = EquippedItem;
            if (handler != null)
            {
                handler(this, e);
            }
   		}

        public class EquippedItemEventArgs : EventArgs
        {
            public Item Item { get; set; }
        }
        //UnEquippedItem
        public event EventHandler<UnEquippedItemEventArgs> UnEquippedItem;

        public void OnUnEquippedItem(UnEquippedItemEventArgs e)
   		{
            var handler = UnEquippedItem;
            if (handler != null)
            {
                handler(this, e);
            }
   		}

        public class UnEquippedItemEventArgs : EventArgs
        {
            public Item Item { get; set; }
        }

        //Gained Exp
        public event EventHandler<GainedExpEventArgs> GainedExp;

   		public void OnGainedExp(GainedExpEventArgs e)
   		{
            var handler = GainedExp;
            if (handler != null)
            {
                handler(this, e);
            }
   		}

        public class GainedExpEventArgs : EventArgs
        {
            public long ExpGained { get; set; }
            public bool Leveled { get; set; }
        }

        //Updated Player Stats
        public event EventHandler<UpdatedPlayerStatsArgs> UpdatedPlayerStats;

        public void OnUpdatedPlayerStats(UpdatedPlayerStatsArgs e)
   		{
            var handler = UpdatedPlayerStats;
            if (handler != null)
            {
                handler(this, e);
            }
   		}

        public class UpdatedPlayerStatsArgs : EventArgs
        {
        }

        //Gained Skill Exp
        public event EventHandler<GainedSkillExpEventArgs> GainedSkillExp;

        public void OnGainedSkillExp(GainedSkillExpEventArgs e)
   		{
            var handler = GainedSkillExp;
            if (handler != null)
            {
                handler(this, e);
            }
   		}

        public class GainedSkillExpEventArgs : EventArgs
        {
            public long ExpGained { get; set; }
        }

        //Read Book
        public event EventHandler<ReadBookEventArgs> ReadBook;

        public void OnReadBook(ReadBookEventArgs e)
   		{
            var handler = ReadBook;
            if (handler != null)
            {
                handler(this, e);
            }
   		}

        public class ReadBookEventArgs : EventArgs
        {
            public Book Book { get; set; }
            public AudioBase NullableAudio { get; set; }
        }

        //Closed Book
        public event EventHandler<ClosedBookEventArgs> ClosedBook;

        public void OnClosedBook(ClosedBookEventArgs e)
   		{
            var handler = ClosedBook;
            if (handler != null)
            {
                handler(this, e);
            }
   		}

        public class ClosedBookEventArgs : EventArgs
        {
            public Book Book { get; set; }
        }
    }
}
