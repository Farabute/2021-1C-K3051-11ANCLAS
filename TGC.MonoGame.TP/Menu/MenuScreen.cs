﻿using System;
using System.Collections.Generic;
using System.Text;
using TGC.MonoGame.TP.Menu.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Input;


namespace TGC.MonoGame.TP.Menu
{
    public class MenuScreen : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private Screen _currentState;

        private Screen _nextState;

        public void ChangeState(Screen state)
        {
            _nextState = state;
        }

        public MenuScreen()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            
        }


        public Song menuMusic;

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            //Window.Title = "_.~\"~._.~\"~.__.~\"~._.~\"~._.~\"~._.~\"~.__.~\"~._.~\"~._.~\"~._.~\"~.__.~\"(_.~\"(_.~\"(_.~\"(_.~\"(    11 ANCLAS 11" +
            //   "    _.~\"(_.~\"(_.~\"(_.~\"(_.~\"(_.~\"~._.~\"~._.~\"~._.~\"~.__.~\"~._.~\"~._.~\"~._.~\"~._";

            IsMouseVisible = true;
            Window.AllowUserResizing = true;
            Window.IsBorderless = true;
            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;
            //graphics.IsFullScreen = true;
            graphics.ApplyChanges();
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


            _currentState = new States.MenuScreen(this, graphics, Content);


            menuMusic = Content.Load<Song>("Music/PirateOst.mp3");
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(menuMusic);
            MediaPlayer.Volume = (float)0.1;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            base.UnloadContent();
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (_nextState != null)
            {
                _currentState = _nextState;

                _nextState = null;
            }


            _currentState.Update(gameTime);

            var keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            base.Update(gameTime);
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {

            _currentState.Draw(gameTime, spriteBatch);


            base.Draw(gameTime);
        }

    }
    }
