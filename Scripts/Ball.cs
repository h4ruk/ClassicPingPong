using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace Pong
{
    public class Ball
    {
        private Rectangle info;
        public Rectangle Info {get => info; set => info = value;}

        private Point velocity;
        public Point Velocity {get => velocity; set => velocity = value;}

        private int speed;
        public int Speed {get => speed; set => speed = value;}

        private const int maxspeed = 10;
        private Random random;
        private Texture2D texture;
        private SpriteBatch sprites;
        private SoundEffect soundScore;
        private RenderTarget2D renderTarget;

        public Ball(RenderTarget2D renderTarget)
        {
            this.info = new Rectangle(0, 0, 8, 8); 
            this.speed = 7;
            this.velocity = new Point();
            this.random = new Random();
            this.renderTarget = renderTarget;
        }

        public void LoadContent(Game game)
        {
            this.sprites = new SpriteBatch(game.GraphicsDevice);
            this.texture = new Texture2D(game.GraphicsDevice, 1, 1);
            this.texture.SetData(new Color[]{Color.White});
            this.soundScore = game.Content.Load<SoundEffect>("Sound/score");
        }

        public void Reset(Score score)
        {
            //put the ball in center
            info.X = (renderTarget.Width - info.Width)/2;
            info.Y = (renderTarget.Height - info.Height)/2;

            //random velocity
            velocity.X = score.IsLeftHoldLastestPoint ? speed : -speed;
            velocity.Y = random.Next(-speed, speed);
        }

        public void Update(Score score)
        {
            Move(score);
        }

        private void Move(Score score)
        {
            //check velocity
            velocity.X = Math.Clamp(velocity.X, -maxspeed, maxspeed);
            velocity.Y = Math.Clamp(velocity.Y, -maxspeed, maxspeed);

            //update position
            info.X += velocity.X;
            info.Y += velocity.Y;

            //hit up (bottom) bound
            if (info.Y < 0 || info.Y + info.Height > renderTarget.Height)
                velocity.Y = -velocity.Y;

            //hit left bound => right win a point
            if (info.X < 0)
            {
                Reset(score);
                soundScore.Play();
                score.rightScore++;
                score.IsLeftHoldLastestPoint = false;
            }

            //hit right bound => left win a point
            if (info.X + info.Width > renderTarget.Width)
            {
                Reset(score);
                soundScore.Play();
                score.leftScore++;
                score.IsLeftHoldLastestPoint = true;
            }
        }

        public void Draw()
        {
            sprites.Begin();
            sprites.Draw(texture, info, Color.White);
            sprites.End();
        }
    }
}