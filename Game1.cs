using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace IWKS_3400_Lab4
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>

    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        SpriteFont Font1;
        Vector2 FontPos;

        Boolean done = false;  // if true, we will display the victory/consolation message

        public string victory;  // used to hold the congratulations/better luck next time message

        //  Paddles 
        Paddle computer_paddle;
        Paddle player_paddle;

        // Ball
        Ball ball;

        // Sound Effects
        SoundEffect ballhit;
        SoundEffect killshothit;
        SoundEffect paddlemiss;
    
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //  changing the back buffer size changes the window size (when in windowed mode)
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //Load the SoundEffect resources
            // Load the SoundEffect resource
            ballhit = Content.Load<SoundEffect>("ballhit");
            killshothit = Content.Load<SoundEffect>("killshot");
            paddlemiss = Content.Load<SoundEffect>("miss");

            Font1 = Content.Load<SpriteFont>("Courier New");

            computer_paddle = new Paddle(Content.Load<Texture2D>("left_paddle"), new Vector2(20f, 268f), new Vector2(24f, 64f),
                graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            player_paddle = new Paddle(Content.Load<Texture2D>("right_paddle"), new Vector2(756f, 268f), new Vector2(24f, 64f),
                graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            ball = new Ball(Content.Load<Texture2D>("small_ball"), new Vector2(384f, 284f), new Vector2(32f, 32f),
                graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);

            //  set the speed the objects will move
            // the ball always starts in the middle and moves toward the player
            ball.Reset();
            computer_paddle.velocity = new Vector2(0, 2);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            //  Free the previously alocated resources
            computer_paddle.texture.Dispose();
            player_paddle.texture.Dispose();
            ball.texture.Dispose();

        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // this check allows to freeze the game to display the victory/consolation message
            if (done == false)
            {
                // Move the ball and computer paddle
                // All the Move calls do is adjust velocity; position will be changed after
                // velocity adjustments.

                ball.Move(player_paddle, computer_paddle);
                computer_paddle.Move(ball);

                if (ball.collision_occured && ball.playit)
                {
                    ballhit.Play();
                    ball.playit = false;
                }

                //TODO: play sounds for paddle miss and kill shots
                // This will require ball and paddle to tell us when to do this

                // Now adjust postion
                ball.position += ball.velocity;
                computer_paddle.position += computer_paddle.velocity;

                // Move the player paddle
                // Change the player paddle position using the left thumbstick, mouse or keyboard 

                //Thumbstick
                // Vector2 LeftThumb = GamePad.GetState(PlayerIndex.One).ThumbSticks.Left;
                // player_paddle.position += new Vector2(LeftThumb.X, -LeftThumb.Y) * 5;

                //  Change the player paddle position using the keyboard
                KeyboardState keyboardState = Keyboard.GetState();
                if (keyboardState.IsKeyDown(Keys.Up))
                    if (player_paddle.position.Y > 0)  // don't run off the edge
                        player_paddle.position += new Vector2(0, -8);
                if (keyboardState.IsKeyDown(Keys.Down))
                    if (player_paddle.position.Y < (graphics.PreferredBackBufferHeight - player_paddle.size.Y)) // don't run off the edge
                        player_paddle.position += new Vector2(0, 8);

                //  Make the player paddle follow the mouse, but only in Y

                //if (player_paddle.position.Y < Mouse.GetState().Y)
                //    player_paddle.position += new Vector2(0, 5);
                //if (player_paddle.position.Y > Mouse.GetState().Y)
                //    player_paddle.position += new Vector2(0, -5);

            }

            if (ball.scorePlayer > ball.scoreFinal)
            {
                victory = "Congratulations!  You Win!  Your Score: " + ball.scorePlayer + "     Computer Score: " + ball.scoreComputer;
                done = true;
            }
            else if (ball.scoreComputer > ball.scoreFinal)
            {
                victory = "Better luck next time!  Your Score: " + ball.scorePlayer + "     Computer Score: " + ball.scoreComputer;
                done = true;
            }

            if (done == false)
            {
                base.Update(gameTime);
            }

        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Draw the sprites
            spriteBatch.Begin();

            // Draw running score string
            spriteBatch.DrawString(Font1, "Computer: " + ball.scoreComputer, new Vector2(5, 10), Color.Yellow);
            spriteBatch.DrawString(Font1, "Player: " + ball.scorePlayer,
                new Vector2(graphics.GraphicsDevice.Viewport.Width - Font1.MeasureString("Player: " + ball.scorePlayer).X - 5, 10), Color.Yellow);

            if (done) //draw victory/consolation message
            {
                FontPos = new Vector2((graphics.GraphicsDevice.Viewport.Width / 2) - 300,
                    (graphics.GraphicsDevice.Viewport.Height / 2) - 50);
                spriteBatch.DrawString(Font1, victory, FontPos, Color.Yellow);
            }
            //Draw the other sprites
            computer_paddle.Draw(spriteBatch);
            player_paddle.Draw(spriteBatch);
            ball.Draw(spriteBatch);

            spriteBatch.End();

            if (done == false)
            {
                base.Draw(gameTime);
            }

        }
    }
}
//-------------------------------------------here is Vinh Game1.cs ----------------------------------
/*
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace IWKS_3400_Lab4
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>

    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        SpriteFont Font1;
        Vector2 FontPos;

        Boolean done = false;  // if true, we will display the victory/consolation message

        public string victory;  // used to hold the congratulations/better luck next time message

        //  Paddles 
        Paddle computer_paddle;
        Paddle player_paddle;

        // Ball
        Ball ball;
        
         //Blocks
        Block blockL;
        Block blockU;
        Block blockR;
        Block blockD;
        Block blockGhost;
        int[] blockArray = { 1, 2, 3, 4 }; //1=up, 2=down, 3=left, 4=right
        static Random rnd = new Random();

        // Sound Effects
        SoundEffect ballhit;
        SoundEffect killshothit;
        SoundEffect paddlemiss;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //  changing the back buffer size changes the window size (when in windowed mode)
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //Load the SoundEffect resources
            // Load the SoundEffect resource
            ballhit = Content.Load<SoundEffect>("ballhit");
            killshothit = Content.Load<SoundEffect>("killshot");
            paddlemiss = Content.Load<SoundEffect>("miss");

            Font1 = Content.Load<SpriteFont>("Courier New");

            computer_paddle = new Paddle(Content.Load<Texture2D>("right_paddle"), new Vector2(350f, 268f), new Vector2(24f, 64f),
                graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            player_paddle = new Paddle(Content.Load<Texture2D>("left_paddle"), new Vector2(426f, 268f), new Vector2(24f, 64f),
                graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            ball = new Ball(Content.Load<Texture2D>("small_ball"), new Vector2(600f, 100f), new Vector2(32f, 32f),
                graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            blockL = new Block(Content.Load<Texture2D>("Left Arrow"), new Vector2(500f, -150f), new Vector2(200f, 600f),
                graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            blockR = new Block(Content.Load<Texture2D>("Right Arrow"), new Vector2(600f, -150f), new Vector2(200f, 600f),
               graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            blockU = new Block(Content.Load<Texture2D>("Up Arrow125"), new Vector2(700f, -500f), new Vector2(200f, 600f),
                          graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            blockD = new Block(Content.Load<Texture2D>("Down Arrow 125"), new Vector2(800f, -500f), new Vector2(200f, 600f),
              graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight); 

            //  set the speed the objects will move
            // the ball always starts in the middle and moves toward the player
            ball.Reset();
            computer_paddle.velocity = new Vector2(0, 2);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            //  Free the previously alocated resources
            computer_paddle.texture.Dispose();
            player_paddle.texture.Dispose();
            ball.texture.Dispose();
            blockL.texture.Dispose();
            blockR.texture.Dispose();
            blockD.texture.Dispose();
            blockU.texture.Dispose();

        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // this check allows to freeze the game to display the victory/consolation message
            if (done == false)
            {
                // Move the ball and computer paddle
                // All the Move calls do is adjust velocity; position will be changed after
                // velocity adjustments.

                ball.Move(player_paddle, computer_paddle);
                computer_paddle.Move(ball);

                if (ball.collision_occured && ball.playit)
                {
                    ballhit.Play();
                    ball.playit = false;
                }
                 //check if the ball hit the right side to begin droping blocks
                /*   begin block randomization loop
                 *   outer loop - counts to 4, or greater depending on difficulty setting
                 *   inner loop randomly picks a number 1-4 from the array to determine which block to use
                 *   trying to make the variable g equal to where the first block will be placed before being dropped, then incrementing
                 *   g to the value where the next block will be placed.
                 *   still working on this part....
                if (ball.position.X + ball.size.X >= graphics.PreferredBackBufferWidth)
                {
                    //begin a loop to randomly pick which blocks will drop
                    for (int c = -150; c < ; c-=200)
                    {
                        for(int g = )
                        int key = rnd.Next(0, blockArray.Length);
                        if(key == 1)
                        {

                        }
                    }
                    blockL.move(ball);
                } */

                //TODO: play sounds for paddle miss and kill shots
                // This will require ball and paddle to tell us when to do this

                // Now adjust postion
                ball.position += ball.velocity;
                computer_paddle.position += computer_paddle.velocity;

                // Move the player paddle
                // Change the player paddle position using the left thumbstick, mouse or keyboard 

                //Thumbstick
                // Vector2 LeftThumb = GamePad.GetState(PlayerIndex.One).ThumbSticks.Left;
                // player_paddle.position += new Vector2(LeftThumb.X, -LeftThumb.Y) * 5;

                //  Change the player paddle position using the keyboard
                KeyboardState keyboardState = Keyboard.GetState();
                if (keyboardState.IsKeyDown(Keys.Up))
                    if (player_paddle.position.Y > 0)  // don't run off the edge
                        player_paddle.position += new Vector2(0, -8);
                if (keyboardState.IsKeyDown(Keys.Down))
                    if (player_paddle.position.Y < (graphics.PreferredBackBufferHeight - player_paddle.size.Y)) // don't run off the edge
                        player_paddle.position += new Vector2(0, 8);

                //  Make the player paddle follow the mouse, but only in Y

                //if (player_paddle.position.Y < Mouse.GetState().Y)
                //    player_paddle.position += new Vector2(0, 5);
                //if (player_paddle.position.Y > Mouse.GetState().Y)
                //    player_paddle.position += new Vector2(0, -5);

            }

            if (ball.scorePlayer > ball.scoreFinal)
            {
                victory = "Congratulations!  You Win!  Your Score: " + ball.scorePlayer + "     Computer Score: " + ball.scoreComputer;
                done = true;
            }
            else if (ball.scoreComputer > ball.scoreFinal)
            {
                victory = "Better luck next time!  Your Score: " + ball.scorePlayer + "     Computer Score: " + ball.scoreComputer;
                done = true;
            }

            if (done == false)
            {
                base.Update(gameTime);
            }

        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Draw the sprites
            spriteBatch.Begin();

            // Draw running score string
            spriteBatch.DrawString(Font1, "Computer: " + ball.scoreComputer, new Vector2(5, 10), Color.Yellow);
            spriteBatch.DrawString(Font1, "Player: " + ball.scorePlayer,
                new Vector2(graphics.GraphicsDevice.Viewport.Width - Font1.MeasureString("Player: " + ball.scorePlayer).X - 5, 10), Color.Yellow);

            if (done) //draw victory/consolation message
            {
                FontPos = new Vector2((graphics.GraphicsDevice.Viewport.Width / 2) - 300,
                    (graphics.GraphicsDevice.Viewport.Height / 2) - 50);
                spriteBatch.DrawString(Font1, victory, FontPos, Color.Yellow);
            }
            //Draw the other sprites
            computer_paddle.Draw(spriteBatch);
            player_paddle.Draw(spriteBatch);
            ball.Draw(spriteBatch);
            blockL.Draw(spriteBatch);
            blockR.Draw(spriteBatch);
            blockD.Draw(spriteBatch);
            blockU.Draw(spriteBatch);


            spriteBatch.End();

            if (done == false)
            {
                base.Draw(gameTime);
            }

        }
    }
}

*/
