using GeonBit.UI.Entities;
using Microsoft.Xna.Framework;

namespace GeonBit.UI.Source.Input
{
    /// <summary>
    /// Use the GamePadSetup class to define some basic rules for your GamePad supported GeonBit.UI GUI.
    /// </summary>
    public static class GamePadSetup
    {
        /// <summary>
        /// The default Color of an entity.
        /// </summary>
        public static Color DefaultColor = Color.White;
        /// <summary>
        /// The selected Color of an entity.
        /// </summary>
        public static Color SelectedColor = Color.LightPink;

        /// <summary>
        /// The default Skin of an entity.
        /// </summary>
        public static PanelSkin DefaultSkin = PanelSkin.Simple;
        /// <summary>
        /// The selected Skin of an entity.
        /// </summary>
        public static PanelSkin SelectedSkin = PanelSkin.Fancy;

        /// <summary>
        /// How long does it take before a MessageBox should be confirmable?
        /// Don't make it to short to avoid that the MessageBox would close immediately.
        /// </summary>
        public static double MessageBoxTimeOut = 500;

        /// <summary>
        /// How long does it take before a clicked panel content should call the OnClick(); event?
        /// Don't make it to short to avoid that the clicked panel content triggers right after a panel was selected.
        /// </summary>
        public static double PanelContentClickedTimeOut = 100;
    }
}
