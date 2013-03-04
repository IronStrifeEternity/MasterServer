namespace IronStrifeMasterServer
{
    using System.Net;
    public static class Globals
    {
        /// <summary>
        /// The port that the master server listens for clients on
        /// </summary>
        public static int listenPort = 11417;

        /// <summary>
        /// Returns the IP Address 66.61.116.111
        /// </summary>
        public static IPAddress LocalAddress
        {
            get
            {
                return IPAddress.Parse("66.61.116.111");
            }
        }
    }
}
