﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace MarioPirates
{
    using Controller;
    using Microsoft.Xna.Framework.Input;

    internal class MapEditor : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        public List<IController> Controllers { get; } = new List<IController>();

        public MapEditorState State { get; set; }

        public MapEditor()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = Constants.CONTENT_ROOT;
        }

        protected override void Initialize()
        {
            base.Initialize();

            graphics.IsFullScreen = false;

            spriteBatch = new SpriteBatch(GraphicsDevice);
            SpriteFactory.Ins.LoadContent(Content);
            AudioManager.Ins.LoadContent(Content);

            Reset();
        }

        protected override void UnloadContent()
        {
            Content.Unload();
        }

        protected override void Update(GameTime gameTime)
        {
            State.DoUpdate(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            State.DoDraw(spriteBatch);
        }

        private void Reset()
        {
            EventManager.Ins.Reset();
            Camera.Ins.Reset();
            Scene.Ins.Reset();

            State = new MapEditorStateNormal(this);

            EventManager.Ins.Subscribe(EventEnum.KeyDown, (s, e) =>
            {
                State.HandleKeyDown(e as KeyDownEventArgs);
            });

            Controllers.Clear();

            var keyboardController = new KeyboardController();
            keyboardController.SetKeyMapping(Keys.W, Keys.Up);
            keyboardController.SetKeyMapping(Keys.S, Keys.Down);
            keyboardController.SetKeyMapping(Keys.A, Keys.Left);
            keyboardController.SetKeyMapping(Keys.D, Keys.Right);
            keyboardController.EnableKeyEvent(InputState.Down, Keys.Escape, Keys.F4, Keys.Q);
            keyboardController.EnableKeyEvent(InputState.Hold, Keys.Up, Keys.Down, Keys.Left, Keys.Right);
            Controllers.Add(keyboardController);
        }
    }
}
