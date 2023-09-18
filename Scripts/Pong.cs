using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.VisualBasic;

namespace Pong;

public class Pong : Game
{
    private GraphicsDeviceManager graphics;
    private SpriteBatch sprites;
    private RenderTarget2D renderTarget;
    private Rectangle renderRect;
    private Texture2D texture;
    public GameState gameState;

    private Score score;

    private Ball ball;
    private Paddle[] paddles;

    private SpriteFont font;
    private SpriteFont smallfont;
    
    private double timer = 0;
    private const double interval = 0.8d;
    
    public Pong()
    {
        graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        IsFixedTimeStep = true;
        Window.AllowUserResizing = true;
        TargetElapsedTime = TimeSpan.FromSeconds(1d/30d);
        Window.ClientSizeChanged += OnClientSizeChanged;
    }

    protected override void Initialize()
    {
        renderTarget = new RenderTarget2D(this.GraphicsDevice, 640, 480); 
        this.graphics.PreferredBackBufferWidth = 800;
        this.graphics.PreferredBackBufferHeight = 600;
        this.graphics.ApplyChanges();
        OnClientSizeChanged(null, null);

        gameState = GameState.Idle;

        score = new Score(3);

        ball = new Ball(this.renderTarget);
        ball.Reset(score);

        Paddle paddle1 = new Paddle(this.renderTarget);
        paddle1.Info = new Rectangle(paddle1.Info.Width * 5, (renderTarget.Height - paddle1.Info.Height)/2, paddle1.Info.Width, paddle1.Info.Height);
        Paddle paddle2 = new Paddle(this.renderTarget);
        paddle2.Info = new Rectangle(renderTarget.Width - paddle2.Info.Width * 6, (renderTarget.Height - paddle2.Info.Height)/2, paddle2.Info.Width, paddle2.Info.Height);
        paddles = new Paddle[]{paddle1, paddle2};

        base.Initialize();
    }

    protected override void LoadContent()
    {
        sprites = new SpriteBatch(GraphicsDevice);

        texture = new Texture2D(GraphicsDevice, 1, 1);
        Color[] data = new Color[]{Color.White};
        texture.SetData(data);

        font = Content.Load<SpriteFont>("Font/font");
        smallfont = Content.Load<SpriteFont>("Font/smallfont");

        ball.LoadContent(this);
        score.LoadContent(this);
        paddles[0].LoadContent(this);
        paddles[1].LoadContent(this);
    }

    protected override void Update(GameTime gameTime)
    {
        switch (gameState)
        {
            case GameState.Idle:
                ball.Reset(score);
                score.Reset();
                paddles[0].Reset(paddles[0].Info.Width * 5, (renderTarget.Height - paddles[0].Info.Height)/2);
                paddles[1].Reset(renderTarget.Width - paddles[1].Info.Width * 6, (renderTarget.Height - paddles[1].Info.Height)/2);
                InputManager.ChangeGameState(this, Keys.Enter, GameState.Start);
                break;
            case GameState.Start:
                if (InputManager.Pos_NevKeyPress(Keys.Left, Keys.Right) > 0) 
                {
                    paddles[1].Control = true;
                    gameState = GameState.Tutorial;
                }
                else if (InputManager.Pos_NevKeyPress(Keys.Left, Keys.Right) < 0) 
                {
                    paddles[0].Control = true;
                    gameState = GameState.Tutorial;
                }
                break;
            case GameState.Tutorial:
                if (InputManager.Pos_NevKeyPress(Keys.Up, Keys.Down) != 0) gameState = GameState.Play;
                break;
            case GameState.Play:
                ball.Update(score);    
                for (int i = 0; i < paddles.Length; i++)
                    paddles[i].Update(ball);
                score.Check(this);
                break;
            case GameState.End:
                InputManager.ChangeGameState(this, Keys.Enter, GameState.Idle);
                break;
            default:
                gameState = GameState.Idle;
                break;
        }
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.SetRenderTarget(renderTarget);
        GraphicsDevice.Clear(Color.Black);
        sprites.Begin();

        int smallfontSize = 20;
        int fontSize = 80;
        int spaceFont = fontSize*20/100;
        string text = "";
        timer += gameTime.ElapsedGameTime.TotalSeconds;
        switch (gameState)
        {
            case GameState.Start:
                //text area
                text = "press left (right) arrow to choose side";
                sprites.DrawString(
                    smallfont, 
                    text, 
                    new Vector2((renderTarget.Width - smallfontSize*text.Length/2)/2, renderTarget.Height/2), 
                    Color.White
                );

                //paddles area
                for (int i = 0; i < paddles.Length; i++)
                    paddles[i].Draw();
                break;
            case GameState.Tutorial:
                //text area
                text = "press up (down) arrow to move";
                sprites.DrawString(
                    smallfont, 
                    text, 
                    new Vector2((renderTarget.Width - smallfontSize*text.Length/2)/2, renderTarget.Height/2), 
                    Color.White
                );
                
                //play area
                for (int i = 0; i < renderTarget.Height; i += 30)
                {
                    Rectangle destination = new Rectangle(renderTarget.Width/2, i, 2, 10); 
                    sprites.Draw(texture, destination, Color.DimGray); 
                }

                //paddles area
                for (int i = 0; i < paddles.Length; i++)
                    paddles[i].Draw();
                break;
            case GameState.Play:
                //play area
                for (int i = 0; i < renderTarget.Height; i += 30)
                {
                    Rectangle destination = new Rectangle(renderTarget.Width/2, i, 2, 10); 
                    sprites.Draw(texture, destination, Color.DimGray); 
                }
                ball.Draw();
                paddles[0].Draw();
                paddles[1].Draw();

                //left score
                text = score.leftScore.ToString();
                sprites.DrawString(
                    font, 
                    text, 
                    new Vector2((renderTarget.Width - fontSize*text.Length)/4 - spaceFont*(text.Length - 1), (renderTarget.Height - fontSize)/2),
                    Color.DimGray
                );

                //right score
                text = score.rightScore.ToString();
                sprites.DrawString(
                    font, 
                    text, 
                    new Vector2((3*renderTarget.Width - fontSize*text.Length)/4 - spaceFont*(text.Length - 1), (renderTarget.Height - fontSize)/2),
                    Color.DimGray
                );
                break;
            case GameState.End:
                if (timer < interval)
                {
                    text = "press Enter to restart";
                    sprites.DrawString(
                        smallfont, 
                        text, 
                        new Vector2((renderTarget.Width - smallfontSize*text.Length/2)/2, renderTarget.Height/2 + fontSize), 
                        Color.DimGray
                    );
                }
                else if (timer > interval * 2) timer = 0;

                text = (score.leftScore > score.rightScore) ? "LEFT WIN" : "RIGHT WIN";
                sprites.DrawString(
                    font, 
                    text,
                    new Vector2((renderTarget.Width - fontSize*text.Length/3)/2 - spaceFont*(text.Length - 1), (renderTarget.Height - fontSize)/2), 
                    Color.White
                );
                break;
            default:
                if (timer < interval)
                {
                    text = "press Enter to play";
                    sprites.DrawString(
                        smallfont, 
                        text, 
                        new Vector2((renderTarget.Width - smallfontSize*text.Length/2)/2, renderTarget.Height/2 + fontSize), 
                        Color.DimGray
                    );
                }
                else if (timer > interval * 2) timer = 0;

                text = "PONG";
                sprites.DrawString(
                    font, 
                    text, 
                    new Vector2((renderTarget.Width - fontSize*text.Length/2)/2 - spaceFont*(text.Length - 1), (renderTarget.Height - fontSize)/2), 
                    Color.White
                );
                break;
        }
        sprites.End();
        GraphicsDevice.SetRenderTarget(null);

        GraphicsDevice.Clear(Color.CornflowerBlue);
        sprites.Begin();
        sprites.Draw(renderTarget, renderRect, Color.White);
        sprites.End();

        base.Draw(gameTime);
    }

    private void OnClientSizeChanged(object obj, EventArgs args)
    {
        int screenWidth = Window.ClientBounds.Width;
        int screenHeight = Window.ClientBounds.Height;

        float scaleWidth = screenWidth/renderTarget.Width;
        float scaleHeight = screenHeight/renderTarget.Height;

        int newWidth = (int)scaleHeight*renderTarget.Width;
        int newHeight = (int)scaleWidth*renderTarget.Height;
        if (newHeight <= screenHeight)
            newWidth = screenWidth;
        else 
            newHeight = screenHeight;

        int x = (screenWidth - newWidth)/2;
        int y = (screenHeight - newHeight)/2;

        renderRect = new Rectangle(x, y, newWidth, newHeight);
    }
}
