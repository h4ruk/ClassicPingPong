using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Pong
{
    public static class InputManager
    {
        public static void ChangeGameState(Pong game, Keys conditionKey, GameState desiredState)
        {
            if(Keyboard.GetState().IsKeyDown(conditionKey))
                game.gameState = desiredState;
        }

        public static int Pos_NevKeyPress(Keys negativeKey , Keys positiveKey)
        {
            if(Keyboard.GetState().IsKeyDown(negativeKey)) return -1;
            else if(Keyboard.GetState().IsKeyDown(positiveKey)) return 1;
            else return 0;
        }
    }
}