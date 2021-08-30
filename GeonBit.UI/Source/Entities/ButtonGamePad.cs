using GeonBit.UI.Entities;
using Microsoft.Xna.Framework;

namespace GeonBit.UI.Source.Entities
{
    /// <summary>
    /// A clickable gamepad supported button with label on it.
    /// </summary>
    [System.Serializable]
    public class ButtonGamePad : Button, IEntityGamePad
    {
        /// <summary>
        /// Static ctor.
        /// </summary>
        static ButtonGamePad()
        {
            Entity.MakeSerializable(typeof(ButtonGamePad));
        }

        /// <summary>
        /// Calls the DoOnClick event from the base entity.
        /// </summary>
        public void TriggerOnClick()
        {
            DoOnClick();
        }

        /// <summary>
        /// Create the gamepad supported button.
        /// </summary>
        /// <param name="text">Text for the button label.</param>
        /// <param name="skin">Button skin (texture to use).</param>
        /// <param name="anchor">Position anchor.</param>
        /// <param name="size">Button size (if not defined will use default size).</param>
        /// <param name="offset">Offset from anchor position.</param>
        public ButtonGamePad(
            string text, 
            ButtonSkin skin = ButtonSkin.Default, 
            Anchor anchor = Anchor.Auto, 
            Vector2? size = null,
            Vector2? offset = null)
            : base(text, skin, anchor, size, offset)
        {
            //We are doing clicking and event handling customly.
            ClickThrough = true;
        }
    }
}
