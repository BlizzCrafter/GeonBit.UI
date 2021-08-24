using GeonBit.UI.Entities;
using Microsoft.Xna.Framework;

namespace GeonBit.UI.Source.Input
{
    /// <summary>
    /// The SelectionMode defines which panel type is currently selected in a PanelGrid.
    /// </summary>
    public enum SelectionMode
    {
        /// <summary>
        /// None is selected.
        /// </summary>
        None,
        /// <summary>
        /// A PanelRoot is selected.
        /// </summary>
        PanelRoot,
        /// <summary>
        /// A Panel is selected.
        /// </summary>
        Panel,
        /// <summary>
        /// The content of a Panel is selected.
        /// </summary>
        PanelContent
    }

    /// <summary>
    /// The HierachyIdentifier shows us in which hierachy we are currently standing inside a PanelGrid.
    /// </summary>
    public enum HierarchyIdentifier
    {
        /// <summary>
        /// The very first PanelGrid of our system.
        /// </summary>
        RootGrid,
        /// <summary>
        /// A PanelGrid gets hosted in on of the Panels of a RootGrid.
        /// </summary>
        PanelGrid,
        /// <summary>
        /// A Panel is a selectable element inside a PanelGrid.
        /// </summary>
        Panel,
        /// <summary>
        /// The PanelContents are selectables inside a Panel.
        /// </summary>
        PanelContent
    }

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
        /// Don't make it to short to avoid a bug where the MessageBox would close immediately.
        /// </summary>
        public static double MessageBoxTimeOut = 500;

        /// <summary>
        /// Get a gamepad implementation related identifier in the PanelGrid-System.
        /// </summary>
        /// <param name="identifier">A base identifier.</param>
        /// <returns>The real identifier.</returns>
        public static string GetIdentifier(HierarchyIdentifier identifier)
        {
            return $"__{identifier}";
        }

        /// <summary>
        /// Get a trimmed gamepad implementation related identifier in the PanelGrid-System.
        /// </summary>
        /// <param name="identifier">A real identifier.</param>
        /// <returns>The base identifier.</returns>
        public static string GetIdentifier(string identifier)
        {
            return identifier.TrimStart('_');
        }
    }
}
