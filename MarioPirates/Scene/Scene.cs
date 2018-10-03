﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace MarioPirates
{
    using Event;

    internal sealed class Scene
    {
        public static Scene Instance { get; } = new Scene();

        private List<LevelDataObject> levelDataObjects = new List<LevelDataObject>();

        private List<GameObject> gameObjects = new List<GameObject>();
        private List<GameObject> gameObjectsStatic = new List<GameObject>();
        private List<GameObject> gameObjectToDestory = new List<GameObject>();

        private Scene() { }

        public void Reset()
        {
            gameObjects.Clear();
            gameObjectsStatic.Clear();
            gameObjectToDestory.Clear();

            EventManager.Instance.Subscribe(e => gameObjectToDestory.AddIfNotExist(((GameObjectDestroyEvent)e).Object), EventEnum.GameObjectDestroy);

            var bg = new Background();
            AddGameObject(bg);

            var mario = new Mario(600, 200);
            AddGameObject(mario, false);

            var hiddenBlock = new UsedBlock(100, 0);
            hiddenBlock.SetHidden(true);
            AddGameObject(hiddenBlock);

            var brickBlock = new BrickBlock(100, 150);
            AddGameObject(brickBlock);

            var brokenBlock = new BrokenBlock(100, 200);
            AddGameObject(brokenBlock);

            var usedBlock = new UsedBlock(100, 250);
            AddGameObject(usedBlock);

            var orangeBlock = new OrangeBlock(100, 100);
            AddGameObject(orangeBlock);

            var questionBlock = new QuestionBlock(100, 50);
            AddGameObject(questionBlock);

            var pipe = new Pipe(200, 200);
            AddGameObject(pipe);

            var flower = new Flower(400, 100);
            AddGameObject(flower);

            var coin = new Coin(600, 100);
            AddGameObject(coin);

            var star = new Star(500, 100);
            AddGameObject(star);

            var redMush = new RedMushroom(200, 100);
            AddGameObject(redMush);

            var greenMush = new GreenMushroom(300, 100);
            AddGameObject(greenMush);

            var koopa = new Koopa(700, 100);
            AddGameObject(koopa, false);

            var goomba = new Goomba(0, 400);
            AddGameObject(goomba, false);
            goomba.RigidBody.Velocity = new Vector2(100f, 0f);
        }

        public void AddGameObject(GameObject o, bool isStatic = true) =>
            (isStatic ? gameObjectsStatic : gameObjects).Add(o);

        public void Update(float dt)
        {
            Physics.Simulate(dt, in gameObjects, in gameObjectsStatic);
            gameObjectToDestory.ForEach(o => { gameObjectsStatic.Remove(o); gameObjects.Remove(o); });
            gameObjectToDestory.Clear();
        }

        public void Draw(SpriteBatch spriteBatch, Dictionary<string, Texture2D> textures)
        {
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            gameObjectsStatic.ForEach(o => o.Draw(spriteBatch, textures));
            gameObjects.ForEach(o => o.Draw(spriteBatch, textures));
            spriteBatch.End();
        }
    }
}
