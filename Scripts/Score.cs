using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Pong
{
    public class Score
    {
        private  SoundEffect soundWin;
        public bool IsLeftHoldLastestPoint = false;
        public int leftScore, rightScore, maxScore;

        public Score(int max)
        {
            leftScore = 0;
            rightScore = 0;
            maxScore = max;
        }

        public void LoadContent(Game game)
        {
            soundWin = game.Content.Load<SoundEffect>("Sound/win");
        }

        public void Check(Pong game)
        {
            if (leftScore == maxScore || rightScore == maxScore)
            {
                soundWin.Play();
                game.gameState = GameState.End;
            }
        }

        public void Reset()
        {
            leftScore = 0;
            rightScore = 0; 
            IsLeftHoldLastestPoint = true;
        }
    }
}