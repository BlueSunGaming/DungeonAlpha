using LogicSpawn.RPGMaker.API;
using Newtonsoft.Json;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Game", "")]
    public class OpenVendorNode : SimpleNode
    {
        [JsonIgnore]
        public string VendorID
        {
            get { return (string) ValueOf("Vendor Shop"); }
        }

        public override string Name
        {
            get { return "Open Vendor"; }
        }

        public override string Description
        {
            get { return "Opens the vendor window for a given vendor shop."; }
        }

        public override string SubText
        {
            get { return ""; }
        }

        public override bool CanBeLinkedTo
        {
            get
            {
                return true;
            }
        }

        public override string NextNodeLinkLabel(int index)
        {
            return "Next";
        }

        protected override void SetupParameters()
        {
            Add("Vendor Shop", PropertyType.VendorShop, null, "");
        }

        protected override void Eval(NodeChain nodeChain)
        {
            var vendorShop = (string) ValueOf("Vendor Shop");
            RPG.Events.OnOpenVendor(new RPGEvents.OpenVendorEventArgs()
                                        {
                                            VendorShop = vendorShop
                                        });
        }
    }
}