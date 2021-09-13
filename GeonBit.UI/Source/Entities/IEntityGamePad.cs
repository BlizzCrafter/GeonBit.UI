using GeonBit.UI.Entities;

namespace GeonBit.UI.Source.Entities
{
    /// <summary>
    /// Makes original GeonBit.UI functionalities available in custom gamepad classes.
    /// </summary>
    public interface IEntityGamePad
    {
        /// <summary>
        /// Calls the DoOnClick() event internally.
        /// </summary>
        void TriggerOnButtonClick();

        /// <summary>
        /// Calls the DoOnMouseEnter() event internally.
        /// </summary>
        void TriggerOnSelect();

        /// <summary>
        /// Calls the DoOnMouseLeave() event internally.
        /// </summary>
        void TriggerOnDeSelect();

        /// <summary>
        /// Calls the OnMouseDown() event internally.
        /// </summary>
        void TriggerOnButtonDown();

        /// <summary>
        /// Calls the DoWhileMouseDown() event internally.
        /// </summary>
        void TriggerDoWhileButtonDown();

        /// <summary>
        /// Calls the DoWhileMouseHover() event internally.
        /// </summary>
        void TriggerDoWhileHover();

        /// <summary>
        /// Calls the DoOnMouseReleased() event internally.
        /// </summary>
        void TriggerOnButtonReleased();

        /// <summary>
        /// Calls the DoOnValueChange() event internally.
        /// </summary>
        void TriggerOnValueChanged();

        /// <summary>
        /// Calls the OnMouseWheelScroll() event internally.
        /// </summary>
        void TriggerOnScroll(PanelDirection direction, bool thumbstickEvent);

        /// <summary>
        /// Triggers when the layout of the RootGrid changed.
        /// </summary>
        /// <param name="rootGrid"></param>
        void TriggerOnLayoutChange(PanelGrid rootGrid);
    }
}
