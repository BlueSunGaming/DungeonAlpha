using System.Collections.Generic;
using System.Linq;
using LogicSpawn.RPGMaker.Core;
using Newtonsoft.Json;

namespace LogicSpawn.RPGMaker.Core
{
    public class NPCVendorRepository
    {
        public List<VendorShop> AllVendors;

        public NPCVendorRepository()
        {
            AllVendors = new List<VendorShop>();
        }

        public VendorShop Get(string vendorId)
        {
            return AllVendors.First(v => v.ID == vendorId);
        }
    }
}