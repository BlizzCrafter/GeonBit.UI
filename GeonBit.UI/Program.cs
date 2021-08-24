using System;

namespace GeonBit.UI.Example
{
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (var game = new GamePad_Example())
                game.Run();
        }
    }
}
