using System;

namespace Battleborn.Windows
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (BattlebornGame game = new BattlebornGame(600, 800, "Battleborn"))
            {
                game.Run();
            }
        }
    }
#endif
}

