namespace GeonBit.UI.Source.Entities
{
    /// <summary>
    /// Makes original GeonBit.UI functionalities available in custom gamepad classes.
    /// </summary>
    public interface IEntityGamePad
    {
        /// <summary>
        /// Calls the DoOnClick() event from the base entity.
        /// </summary>
        void TriggerOnClick();
    }
}
