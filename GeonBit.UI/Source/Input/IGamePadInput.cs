using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GeonBit.UI.Source.Input
{
    /// <summary>
    /// Define the interface GeonBit.UI uses to get gamepad or gamepad-like input from users.
    /// </summary>
    public interface IGamePadInput
    {
        /// <summary>
        /// Update input (called every frame).
        /// </summary>
        /// <param name="gameTime">Update frame game time.</param>
        void Update(GameTime gameTime);

        /// <summary>
        /// Check if a given gamepad button is down.
        /// </summary>
        /// <param name="button">GamePad button to check.</param>
        /// <return>True if given gamepad button is down.</return>
        bool GamePadButtonDown(Buttons button = Buttons.A);

        /// <summary>
        /// Check if a given gamepad button was pressed in current frame.
        /// </summary>
        /// <param name="button">GamePad button to check.</param>
        /// <return>True if given gamepad button was pressed in this frame.</return>
        bool GamePadButtonPressed(Buttons button = Buttons.A);

        /// <summary>
        /// Check if a given gamepad button was just clicked (eg released after being pressed down)
        /// </summary>
        /// <param name="button">GamePad button to check.</param>
        /// <return>True if given gamepad button is clicked.</return>
        bool GamePadButtonClick(Buttons button = Buttons.A);

        /// <summary>
        /// Return if any of gamepad buttons is down.
        /// </summary>
        /// <returns>True if any gamepad button is currently down.</returns>
        bool AnyGamePadButtonDown();

        /// <summary>
        /// Return if any gamepad button was pressed in current frame.
        /// </summary>
        /// <returns>True if any gamepad button was pressed in current frame.</returns>
        bool AnyGamePadButtonPressed();

        /// <summary>
        /// Check if a given gamepad button was released in current frame.
        /// </summary>
        /// <param name="button">GamePad button to check.</param>
        /// <return>True if given gamepad button was released in this frame.</return>
        bool GamePadButtonReleased(Buttons button = Buttons.A);

        /// <summary>
        /// Return if any gamepad button was released this frame.
        /// </summary>
        /// <returns>True if any gamepad button was released.</returns>
        bool AnyGamePadButtonReleased();
    }
}
