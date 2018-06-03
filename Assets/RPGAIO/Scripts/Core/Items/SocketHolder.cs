using Newtonsoft.Json;

namespace LogicSpawn.RPGMaker.Core
{
    public class SocketHolder
    {
        public Socket[] Sockets ;

        [JsonIgnore]
        public int Count
        {
            get { return Sockets.Length; }
        }

        public SocketHolder()
        {
            Sockets = new Socket[0];
        }
    }
}