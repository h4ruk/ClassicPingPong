using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace Pong
{
    public class Paddle
    {
        private int speed;
        public int Speed {get => speed; set => speed = value;}

        private bool control;
        public bool Control {set => control = value;}

        private Rectangle info;
        public Rectangle Info {get => info; set => info = value;}

        private Point velocity;
        public Point Velocity {get => velocity; set => velocity = value;}

        private Texture2D texture;
        private SpriteBatch sprites;
        private SoundEffect soundHit;
        private RenderTarget2D renderTarget;

        public Paddle(RenderTarget2D renderTarget)
        {
            this.control = false;
            this.speed = 5;
            this.info = new Rectangle(0, 0, 5, 60);
            this.velocity = new Point();
            this.renderTarget = renderTarget;
        }

        public void LoadContent(Game game)
        {
            this.soundHit = game.Content.Load<SoundEffect>("Sound/hit");
            this.sprites = new SpriteBatch(game.GraphicsDevice);
            this.texture = new Texture2D(game.GraphicsDevice, 1, 1);
            this.texture.SetData<Color>(new Color[]{Color.White});
        }

        public void Update(Ball ball)
        {
            if (control)
                Input();
            else
                MoveTo(ball);
            CheckBound();
            OnCollision(ball);
        }

        private void MoveTo(Ball ball)
        {
            //determine direction
            if (ball.Info.Y > info.Y + 3*info.Height/4)
                velocity.Y = speed;
            else if (ball.Info.Y < info.Y + info.Height/4)
                velocity.Y = -speed;
            else 
                velocity.Y = 0;
            
            //move to direction
            info.Y += velocity.Y;
        }

        private void Input()
        {
            velocity.Y = InputManager.Pos_NevKeyPress(Keys.Up, Keys.Down) * speed;
            info.Y += velocity.Y;
        }

        private void CheckBound()
        {
            //check top bound
            if (info.Y < 0)
                info.Y = 0;

            //check bot bound
            if (info.Y + info.Height > renderTarget.Height)
                info.Y = renderTarget.Height - info.Height;
        }

        public void Reset(int x, int y)
        {
            control = false;
            info.X = x;
            info.Y = y;
        }

        public void OnCollision(Ball ball)
        {
            if(ball.Info.X < Info.X + Info.Width &&
               ball.Info.X + ball.Info.Width > Info.X &&
               ball.Info.Y < Info.Y + Info.Height &&
               ball.Info.Y + ball.Info.Height > Info.Y
            )
            {
                soundHit.Play();
                ball.Velocity = new Point(-ball.Velocity.X + velocity.X, ball.Velocity.Y + velocity.Y);
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