using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using RandomchaosMGBase;
using RandomchaosMGBase.BaseClasses.Animation;

namespace CommunityPost13853
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D Explosion1;
        Rectangle Explosion1SourceRect;
        SpriteSheetAnimationPlayer Explosion1AnimationPlayer;

        Texture2D Explosion2;
        Rectangle Explosion2SourceRect;
        SpriteSheetAnimationPlayer Explosion2AnimationPlayer;

        Texture2D Explosion3;
        Rectangle Explosion3SourceRect;
        SpriteSheetAnimationPlayer Explosion3AnimationPlayer;

        Texture2D Explosion4;
        Rectangle Explosion4SourceRect;
        SpriteSheetAnimationPlayer Explosion4AnimationPlayer;

        Texture2D Ship;
        Rectangle ShipSourceRect;
        SpriteSheetAnimationPlayer ShipAnimationPlayer;

        int bangSize = 256;

        float Speed = 0;
        float Acceleration = .2f;
        float Deceleration = .05f;
        float MaxSpeed = 8;
        float AnimationLerpValue;
        float shipRotation = 0;
        Vector3 ShipPos;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            ShipPos = new Vector3(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2,0);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

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

            // TODO: use this.Content to load your game content here
            // Textures are from here: https://blogs.unity3d.com/2016/11/28/free-vfx-image-sequences-flipbooks/
            SpriteAnimationClipGenerator sacg = null;
            Explosion1 = Content.Load<Texture2D>("Textures/Explosion00_5x5");
            sacg = new SpriteAnimationClipGenerator(new Vector2(Explosion1.Width, Explosion1.Height), new Vector2(5, 5));
            Dictionary<string, SpriteSheetAnimationClip> exp1Clips = new Dictionary<string, SpriteSheetAnimationClip>()
            {
                { "Boom", sacg.Generate("Boom", new Vector2(0, 0), new Vector2(4, 4), new TimeSpan(0, 0, 0, 1, 0), true) }
            };
            Explosion1AnimationPlayer = new SpriteSheetAnimationPlayer(exp1Clips);
            Explosion1AnimationPlayer.StartClip("Boom");

            Explosion2 = Content.Load<Texture2D>("Textures/Explosion01_5x5");
            sacg = new SpriteAnimationClipGenerator(new Vector2(Explosion2.Width, Explosion2.Height), new Vector2(5, 5));
            Dictionary<string, SpriteSheetAnimationClip> exp2Clips = new Dictionary<string, SpriteSheetAnimationClip>()
            {
                { "Boom", sacg.Generate("Boom", new Vector2(0, 0), new Vector2(4, 4), new TimeSpan(0, 0, 0, 1, 0), true) }
            };
            Explosion2AnimationPlayer = new SpriteSheetAnimationPlayer(exp2Clips);
            Explosion2AnimationPlayer.StartClip("Boom");

            Explosion3 = Content.Load<Texture2D>("Textures/Explosion01-light_5x5");
            sacg = new SpriteAnimationClipGenerator(new Vector2(Explosion3.Width, Explosion3.Height), new Vector2(5, 5));
            Dictionary<string, SpriteSheetAnimationClip> exp3Clips = new Dictionary<string, SpriteSheetAnimationClip>()
            {
                { "Boom", sacg.Generate("Boom", new Vector2(0, 0), new Vector2(4, 4), new TimeSpan(0, 0, 0, 1, 0), true) }
            };
            Explosion3AnimationPlayer = new SpriteSheetAnimationPlayer(exp3Clips);
            Explosion3AnimationPlayer.StartClip("Boom");

            Explosion4 = Content.Load<Texture2D>("Textures/Explosion02HD_5x5");
            sacg = new SpriteAnimationClipGenerator(new Vector2(Explosion4.Width, Explosion4.Height), new Vector2(5, 5));
            Dictionary<string, SpriteSheetAnimationClip> exp4Clips = new Dictionary<string, SpriteSheetAnimationClip>()
            {
                { "Boom", sacg.Generate("Boom", new Vector2(0, 0), new Vector2(4, 4), new TimeSpan(0, 0, 0, 1, 0), true) }
            };
            Explosion4AnimationPlayer = new SpriteSheetAnimationPlayer(exp4Clips);
            Explosion4AnimationPlayer.StartClip("Boom");

            Ship = Content.Load<Texture2D>("Textures/ship1Anim");
            sacg = new SpriteAnimationClipGenerator(new Vector2(Ship.Width, Ship.Height), new Vector2(31, 1));
            Dictionary<string, SpriteSheetAnimationClip> shipClips = new Dictionary<string, SpriteSheetAnimationClip>()
            {
                { "turnLeft", sacg.Generate("turnLeft", new Vector2(14, 0), new Vector2(0, 0), new TimeSpan(0, 0, 0, 1, 500), false) },
                { "idle", sacg.Generate("idle", new Vector2(15, 0), new Vector2(15, 0), new TimeSpan(0, 0, 0, 1, 0), true) },
                { "turnRight", sacg.Generate("turnRight", new Vector2(16, 0), new Vector2(30, 0), new TimeSpan(0, 0, 0, 1, 500), false) }
            };
            ShipAnimationPlayer = new SpriteSheetAnimationPlayer(shipClips);
            ShipAnimationPlayer.StartClip("idle");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            Explosion1AnimationPlayer.Update(gameTime.ElapsedGameTime);
            Explosion2AnimationPlayer.Update(gameTime.ElapsedGameTime);
            Explosion3AnimationPlayer.Update(gameTime.ElapsedGameTime);
            Explosion4AnimationPlayer.Update(gameTime.ElapsedGameTime);
            ShipAnimationPlayer.Update(AnimationLerpValue);

            base.Update(gameTime);

            float rotateSpeed = .2f;
            float rotMod = .1f;

            float animLerpSpeed = .05f;
            

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                //if (Speed != 0)
                {
                    ShipAnimationPlayer.StartClip("turnLeft");
                    AnimationLerpValue = Math.Min(1, AnimationLerpValue + animLerpSpeed);
                    rotMod = ShipAnimationPlayer.ClipLerpValue;
                }
                shipRotation -= rotateSpeed * rotMod;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                //if (Speed != 0)
                {
                    ShipAnimationPlayer.StartClip("turnRight");
                    AnimationLerpValue = Math.Min(1, AnimationLerpValue + animLerpSpeed);
                    rotMod = ShipAnimationPlayer.ClipLerpValue;
                }
                shipRotation += rotateSpeed * rotMod;
            }
            else
            {
                AnimationLerpValue = MathHelper.Max(0, AnimationLerpValue - animLerpSpeed);
            }



            if (Keyboard.GetState().IsKeyDown(Keys.W))
                Speed -= Acceleration;

            if (Keyboard.GetState().IsKeyDown(Keys.S))
                Speed += Acceleration;

            Speed = Math.Max(-MaxSpeed, Speed);

            ShipPos += GameComponentHelper.Translate3D(Vector3.Up * Speed, Quaternion.CreateFromRotationMatrix(Matrix.CreateRotationZ(shipRotation)));
            

            Speed = Math.Min(0, Speed + Deceleration);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            ShipSourceRect = new Rectangle((int)ShipAnimationPlayer.CurrentCell.X, (int)ShipAnimationPlayer.CurrentCell.Y, 64, 64);
            spriteBatch.Draw(Ship, new Rectangle((int)ShipPos.X, (int)ShipPos.Y, 32, 32), ShipSourceRect, Color.White, shipRotation, new Vector2(64, 64) / 2, SpriteEffects.None, 0);

            Explosion1SourceRect = new Rectangle((int)Explosion1AnimationPlayer.CurrentCell.X, (int)Explosion1AnimationPlayer.CurrentCell.Y, 204, 204);
            spriteBatch.Draw(Explosion1, new Rectangle(0, 0, bangSize, bangSize), Explosion1SourceRect, Color.White);

            Explosion2SourceRect = new Rectangle((int)Explosion2AnimationPlayer.CurrentCell.X, (int)Explosion2AnimationPlayer.CurrentCell.Y, 204, 204);
            spriteBatch.Draw(Explosion2, new Rectangle(GraphicsDevice.Viewport.Width- bangSize, 0, bangSize, bangSize), Explosion2SourceRect, Color.White);

            Explosion3SourceRect = new Rectangle((int)Explosion3AnimationPlayer.CurrentCell.X, (int)Explosion3AnimationPlayer.CurrentCell.Y, 204, 204);
            spriteBatch.Draw(Explosion3, new Rectangle(0, GraphicsDevice.Viewport.Height- bangSize, bangSize, bangSize), Explosion3SourceRect, Color.White);

            Explosion4SourceRect = new Rectangle((int)Explosion4AnimationPlayer.CurrentCell.X, (int)Explosion4AnimationPlayer.CurrentCell.Y, 409, 409);
            spriteBatch.Draw(Explosion4, new Rectangle(GraphicsDevice.Viewport.Width- bangSize, GraphicsDevice.Viewport.Height- bangSize, bangSize, bangSize), Explosion4SourceRect, Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
