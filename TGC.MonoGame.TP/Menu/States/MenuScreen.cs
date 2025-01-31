﻿using TGC.MonoGame.TP.Menu.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using TGC.MonoGame.TP.Cameras;
using TGC.MonoGame.TP.Skydome;
using TGC.MonoGame.TP.Geometries;

namespace TGC.MonoGame.TP.Menu.States
{
    public class MenuScreen : Screen
    {
        public const int ST_PRESENTACION = 0;
        public const int ST_MENU = 1;
        public const int ST_CONTROLS = 2;
        public int status = ST_PRESENTACION;

        private Texture2D background;
        private Texture2D BGInstructions;
        private Texture2D BGControls;

        private List<Button> _ButtonsMainMenu;

        private Texture2D buttonTexture;
        private SpriteFont buttonFont;
        private SpriteFont montserratFont;

        public StaticCamera menuCam;
        //private Ship Patrol { get; set; }
        private Model ShipModel { get; set; }
        private Effect ShipEffect { get; set; }

        public Texture2D ShipTexture;
        public Texture2D ShipAoTexture;
        public Texture2D ShipNormalTexture;

        Matrix shipMatrix;
        private float time;

        Button menuButton;
        Button newGameButton;
        //Button godGameButton;
        Button controlsButton;
        Button quitGameButton;
        Button goBackButton;

        private Model ModelWater { get; set; }
        private Effect WaterEffect { get; set; }
        public Texture2D WaterTexture;
        public Texture2D WaterFoamTexture;
        public Texture2D WaterNormalTexture;

        private SkyDome Skydome { get; set; }
        private Model SkyDomeModel { get; set; }
        private Effect SkyDomeEffect { get; set; }
        public Texture2D SkyDomeTexture;

        // Iluminacion
        private Effect LightEffect { get; set; }
        private Matrix LightBoxWorld { get; set; } = Matrix.Identity;

        private CubePrimitive lightBox;

        private Vector2 MenuPos;
        private Vector2 JugarPos;
        private Vector2 VolverPos;
        private Vector2 ControlesPos;
        private Vector2 SalirPos;

        public int buttonWidth = 160;
        public int buttonHeight = 40;


        public MenuScreen(Game game, GraphicsDeviceManager graphics, ContentManager content): base(game, graphics, content)
        {

            menuCam = new StaticCamera(graphics.GraphicsDevice.Viewport.AspectRatio, new Vector3(0,100,0), new Vector3(0, 0, 1), new Vector3(0, 1, 0));
            menuCam.FarPlane = 50000;

            ShipModel = game.Content.Load<Model>(TGCGame.ContentFolder3D + "Botes/PatrolGeo");
            ShipEffect = game.Content.Load<Effect>(TGCGame.ContentFolderEffects + "ShipsShader");
            ShipTexture = game.Content.Load<Texture2D>(TGCGame.ContentFolderTextures + "Botes/T_Patrol_Ship_1K_BaseColor");
            ShipAoTexture = game.Content.Load<Texture2D>(TGCGame.ContentFolderTextures + "Botes/T_Patrol_Ship_1K_OcclusionRoughnessMetallic");
            ShipNormalTexture = game.Content.Load<Texture2D>(TGCGame.ContentFolderTextures + "Botes/T_Patrol_Ship_1K_Normal");

            ShipEffect.Parameters["ambientColor"]?.SetValue(new Vector3(0.6f, 0.6f, 0.6f));
            ShipEffect.Parameters["specularColor"]?.SetValue(new Vector3(0.98f, 0.98f, 0.98f));
            ShipEffect.Parameters["KAmbient"]?.SetValue(0.6f);
            ShipEffect.Parameters["KDiffuse"]?.SetValue(5f);
            ShipEffect.Parameters["KSpecular"]?.SetValue(.3f);
            ShipEffect.Parameters["shininess"]?.SetValue(10f);

            shipMatrix = Matrix.Identity * Matrix.CreateRotationY(4.1f) * Matrix.CreateTranslation(-1200, -200, 2800);

            ModelWater = game.Content.Load<Model>(TGCGame.ContentFolder3D + "Island/waterAltaGeo");
            WaterEffect = game.Content.Load<Effect>(TGCGame.ContentFolderEffects + "WaterShader");
            WaterTexture = game.Content.Load<Texture2D>(TGCGame.ContentFolderTextures + "Island/TexturesCom_WaterPlain0012_1_seamless_S");
            WaterFoamTexture = game.Content.Load<Texture2D>(TGCGame.ContentFolderTextures + "Island/53_sea water foam texture-seamless");
            WaterNormalTexture = game.Content.Load<Texture2D>(TGCGame.ContentFolderTextures + "Island/normalAgua");
            WaterEffect.Parameters["ambientColor"]?.SetValue(new Vector3(0f, 0.17f, 0.37f));
            WaterEffect.Parameters["diffuseColor"]?.SetValue(new Vector3(0f, 0.25f, 0.48f));
            WaterEffect.Parameters["specularColor"]?.SetValue(new Vector3(0.95f, 0.95f, 0.95f));
            WaterEffect.Parameters["KAmbient"]?.SetValue(0.5f);
            WaterEffect.Parameters["KFoam"]?.SetValue(0.15f);
            WaterEffect.Parameters["KDiffuse"]?.SetValue(0.8f);
            WaterEffect.Parameters["KSpecular"]?.SetValue(0.42f);
            WaterEffect.Parameters["shininess"]?.SetValue(15f);
            WaterEffect.Parameters["KReflection"]?.SetValue(0.52f);

            SkyDomeModel = game.Content.Load<Model>(TGCGame.ContentFolder3D + "Skydome/SkyDome");
            SkyDomeTexture = game.Content.Load<Texture2D>(TGCGame.ContentFolderTextures + "Menu/lightningpano-deck");
            SkyDomeEffect = game.Content.Load<Effect>(TGCGame.ContentFolderEffects + "SkyDomeMenu");
            Skydome = new SkyDome(SkyDomeModel, SkyDomeTexture, SkyDomeEffect, 200);

            lightBox = new CubePrimitive(game.GraphicsDevice, 150, Color.Yellow);
            var lightPosition = new Vector3(300f, 500f, 100f);

            LightBoxWorld = Matrix.CreateTranslation(lightPosition);

            WaterEffect.Parameters["lightPosition"]?.SetValue(lightPosition);
            WaterEffect.Parameters["eyePosition"]?.SetValue(menuCam.Position);

            background = _content.Load<Texture2D>("Textures/Menu/Presentacion_IM");
            BGInstructions = _content.Load<Texture2D>("Textures/Menu/MenuBG");
            BGControls = _content.Load<Texture2D>("Textures/Menu/controlesBG");

            buttonTexture = _content.Load<Texture2D>("Textures/Menu/Boton");
            buttonFont = _content.Load<SpriteFont>("Fonts/Font");
            montserratFont = _content.Load<SpriteFont>("Fonts/monserrat22");

            MenuPos = new Vector2((_game.GraphicsDevice.Viewport.Width / 2) - buttonWidth / 2, _game.GraphicsDevice.Viewport.Height - 150);

            menuButton = new Button(buttonTexture, montserratFont, true)
            {
                Position = MenuPos,
                Text = "Menu",
            };

            menuButton.Click += MenuButton_Click;

            JugarPos = new Vector2(50, 50);

            newGameButton = new Button(buttonTexture, montserratFont, true)
            {
                Position = JugarPos,
                Text = "Jugar",
            };

            newGameButton.Click += NewGameButton_Click;
            newGameButton.visible = false;

            ControlesPos = new Vector2(50 + buttonWidth + 50, 50);

            controlsButton = new Button(buttonTexture, montserratFont, true)
            {
                Position = ControlesPos,
                Text = "Controles",
            };

            controlsButton.Click += controlsButton_Click;
            controlsButton.visible = false;

            VolverPos = new Vector2(_game.GraphicsDevice.Viewport.Width - 2 * buttonWidth - 100, _game.GraphicsDevice.Viewport.Height - buttonHeight - 50);

            goBackButton = new Button(buttonTexture, montserratFont, false)
            {
                Position = VolverPos,
                Text = "Volver",
            };
            
            goBackButton.Click += goBackButton_Click;
            goBackButton.visible = false;


            SalirPos = new Vector2(_game.GraphicsDevice.Viewport.Width - buttonWidth - 50, _game.GraphicsDevice.Viewport.Height - buttonHeight - 50);

            quitGameButton = new Button(buttonTexture, montserratFont, true)
            {
                Position = SalirPos,
                Text = "Salir",
            };

            quitGameButton.Click += QuitGameButton_Click;
            quitGameButton.visible = false;


            _ButtonsMainMenu = new List<Button>()
            {
                newGameButton,
                controlsButton,
                quitGameButton,
                goBackButton,
                menuButton,
            };
        }

        private void DrawModel(Model geometry, Matrix transform, Effect effect, Camera cam)
        {
            foreach (var mesh in geometry.Meshes)
            {
                effect.Parameters["World"]?.SetValue(transform);
                effect.Parameters["InverseTransposeWorld"]?.SetValue(Matrix.Transpose(Matrix.Invert(transform)));
                effect.Parameters["WorldViewProjection"]?.SetValue(transform * cam.View * cam.Projection);


                foreach (var meshPart in mesh.MeshParts)
                    meshPart.Effect = effect;
                mesh.Draw();
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            spriteBatch.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.CornflowerBlue, 1f, 0);
            spriteBatch.Begin();


            switch (status)
            {

                case ST_PRESENTACION:
                    spriteBatch.Draw(background, new Rectangle(0,0,_game.GraphicsDevice.Viewport.Width, _game.GraphicsDevice.Viewport.Height), Color.White);
                    menuButton.Position = new Vector2((_game.GraphicsDevice.Viewport.Width / 2) - buttonWidth / 2, _game.GraphicsDevice.Viewport.Height - 150);
                    break;
                case ST_MENU:
                    spriteBatch.GraphicsDevice.BlendState = BlendState.Opaque;
                    ShipEffect.Parameters["baseTexture"]?.SetValue(ShipTexture);
                    ShipEffect.Parameters["aoTexture"]?.SetValue(ShipAoTexture);
                    ShipEffect.Parameters["normalTexture"]?.SetValue(ShipNormalTexture);
                    DrawModel(ShipModel, shipMatrix, ShipEffect, menuCam);

                    WaterEffect.Parameters["baseTexture"]?.SetValue(WaterTexture);
                    WaterEffect.Parameters["foamTexture"]?.SetValue(WaterFoamTexture);
                    WaterEffect.Parameters["normalTexture"]?.SetValue(WaterNormalTexture);
                    WaterEffect.Parameters["Time"]?.SetValue(time);
                    WaterEffect.Parameters["eyePosition"]?.SetValue(menuCam.Position);

                    for (int i = -20; i < 20; i++)
                        for (int j = -20; j < 20; j++)
                        {
                            Matrix MatrixWater = Matrix.Identity * Matrix.CreateScale(10f, 0f, 10f) * Matrix.CreateTranslation(i * 200, -50, j * -200);
                            WaterEffect.Parameters["World"].SetValue(MatrixWater);
                            WaterEffect.Parameters["View"].SetValue(menuCam.View);
                            WaterEffect.Parameters["Projection"].SetValue(menuCam.Projection);
                            WaterEffect.Parameters["InverseTransposeWorld"]?.SetValue(Matrix.Transpose(Matrix.Invert(MatrixWater)));

                            DrawModel(ModelWater, MatrixWater, WaterEffect, menuCam);
                        }

                    Skydome.Draw(menuCam.View, menuCam.Projection, menuCam.Position);
                    SkyDomeEffect.Parameters["Time"]?.SetValue(time);

                    lightBox.Draw(LightBoxWorld, menuCam.View, menuCam.Projection);

                    newGameButton.visible = true;
                    quitGameButton.visible = true;
                    controlsButton.visible = true;
                    quitGameButton.Position = new Vector2(_game.GraphicsDevice.Viewport.Width - buttonWidth - 50, _game.GraphicsDevice.Viewport.Height - buttonHeight - 50);
                    break;
                case ST_CONTROLS:
                    spriteBatch.Draw(BGControls, new Rectangle(0, 0, _game.GraphicsDevice.Viewport.Width, _game.GraphicsDevice.Viewport.Height), Color.White);
                    goBackButton.visible = true;
                    quitGameButton.visible = true;
                    quitGameButton.Position = new Vector2(_game.GraphicsDevice.Viewport.Width - buttonWidth - 50, _game.GraphicsDevice.Viewport.Height - buttonHeight - 50);
                    goBackButton.Position = new Vector2(_game.GraphicsDevice.Viewport.Width - 2 * buttonWidth - 100, _game.GraphicsDevice.Viewport.Height - buttonHeight - 50);


                    spriteBatch.DrawString(montserratFont, $"Movimiento del barco:\n" +
                                $"\n" +
                                $"      W: Mover hacia adelante\n" +
                                $"      A: Rotar hacia la izquierda\n" +
                                $"      S: Mover hacia atras\n" +
                                $"      D: Rotar hacia la derecha\n\n\n" +
                                $"Extras:\n" +
                                $"\n" +
                                $"      R: Night Vision\n" +
                                $"      G: God Mode\n" +
                                $"\n", new Vector2(_game.GraphicsDevice.Viewport.Width / 3, 100), Color.White);

                    spriteBatch.DrawString(montserratFont, $"Movilidad de la mira:\n" +
                                $"\n" +
                                $"      Apuntar: Mouse\n" +
                                $"      Disparar: Boton Izquierdo del Mouse\n" +
                                $"      Vista: Boton Derecho del Mouse\n\n\n" +
                                $"Sonido FX:\n" +
                                $"\n" +
                                $"      N: Bajar el volumen\n" +
                                $"      M: Subir el volumen\n" +
                                $"\n", new Vector2(3 * _game.GraphicsDevice.Viewport.Width / 5, 100), Color.White);

                    break;
            }



            foreach (Button button in _ButtonsMainMenu)
                button.Draw(gameTime, spriteBatch);

            spriteBatch.End();
        }

        private static BlendFunction GetAlphaBlendFunction(SpriteBatch spriteBatch)
        {
            return spriteBatch.GraphicsDevice.BlendState.AlphaBlendFunction;
        }

        private void MenuButton_Click(object sender, EventArgs e)
        {
            menuButton.visible = false;
            status = ST_MENU;
        }
        private void NewGameButton_Click(object sender, EventArgs e)
        {
            using (var game = new TGCGame())
                game.Run();
        }

        private void goBackButton_Click(object sender, EventArgs e)
        {
            goBackButton.visible = false;
            status = ST_MENU;
        }
        
        private void controlsButton_Click(object sender, EventArgs e)
        {
            controlsButton.visible = false;
            newGameButton.visible = false;
            status = ST_CONTROLS;
        }

        public override void Update(GameTime gameTime)
        {

            time += Convert.ToSingle(gameTime.ElapsedGameTime.TotalSeconds);

            shipMatrix *= Matrix.CreateTranslation(0, MathF.Sin(time) * 0.2f , 0); 
            foreach (var button in _ButtonsMainMenu)
                button.Update(gameTime);
        }

        private void QuitGameButton_Click(object sender, EventArgs e)
        {
            _game.Exit();
        }
    }
}
