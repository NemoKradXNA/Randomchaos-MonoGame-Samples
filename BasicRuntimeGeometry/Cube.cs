using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RandomchaosMGBase;
using RandomchaosMGBase.BaseClasses;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicRuntimeGeometry
{
    public class Cube : DrawableGameComponent
    {

        protected Base3DCamera _camera { get { return Game.Services.GetService<Base3DCamera>(); } }
        protected MeshData _meshData { get; set; }

        public Effect Effect { get; set; }
        string _effectAsset;
        VertexPositionNormalTexture[] vertexArray;

        public Texture2D Texture;
        string _texture;

        public Transform Transform { get; set; }

        public Cube(Game game, string texture, string effectAsset) : base(game)
        {
            Transform = new Transform();
            _texture = texture;
            _effectAsset = effectAsset;

            _meshData = new MeshData();

            _meshData.Vertices = new List<Vector3>()
            {
                new Vector3(-.5f, .5f, .5f), new Vector3(.5f, .5f, .5f), new Vector3(.5f, -.5f, .5f),new Vector3(-.5f, -.5f, .5f),
                new Vector3(-.5f, .5f, -.5f), new Vector3(.5f, .5f, -.5f), new Vector3(.5f, -.5f, -.5f), new Vector3(-.5f, -.5f, -.5f),
                new Vector3(-.5f, .5f, .5f), new Vector3(.5f, .5f, .5f), new Vector3(.5f, .5f, -.5f),new Vector3(-.5f, .5f, -.5f),
                new Vector3(-.5f, -.5f, .5f),new Vector3(.5f, -.5f, .5f),new Vector3(.5f, -.5f, -.5f),new Vector3(-.5f, -.5f, -.5f),
                new Vector3(-.5f, .5f, -.5f),new Vector3(-.5f, .5f, .5f),new Vector3(-.5f, -.5f, .5f),new Vector3(-.5f, -.5f, -.5f),
                new Vector3(.5f, .5f, -.5f),new Vector3(.5f, .5f, .5f),new Vector3(.5f, -.5f, .5f),new Vector3(.5f, -.5f, -.5f)
            };

            _meshData.Normals = new List<Vector3>()
            {
                Vector3.Backward,Vector3.Backward,Vector3.Backward,Vector3.Backward,
                Vector3.Forward,Vector3.Forward,Vector3.Forward,Vector3.Forward,
                Vector3.Up,Vector3.Up,Vector3.Up,Vector3.Up,
                Vector3.Down,Vector3.Down,Vector3.Down,Vector3.Down,
                Vector3.Left,Vector3.Left,Vector3.Left,Vector3.Left,
                Vector3.Right,Vector3.Right,Vector3.Right,Vector3.Right,
            };

            _meshData.TextCoords = new List<Vector2>()
            {
                new Vector2(0, 0),new Vector2(1, 0),new Vector2(1, 1),new Vector2(0, 1),
                new Vector2(1, 0),new Vector2(0, 0),new Vector2(0, 1),new Vector2(1, 1),
                new Vector2(0, 1),new Vector2(1, 1),new Vector2(1, 0),new Vector2(0, 0),
                new Vector2(0, 0),new Vector2(1, 0),new Vector2(1, 1),new Vector2(0, 1),
                new Vector2(0, 0),new Vector2(1, 0),new Vector2(1, 1),new Vector2(0, 1),
                new Vector2(1, 0),new Vector2(0, 0),new Vector2(0, 1),new Vector2(1, 1),
            };

            _meshData.Colors = new List<Color>();
            for (int v = 0; v < _meshData.Vertices.Count; v++)
                _meshData.Colors.Add(new Color(_meshData.Normals[v]));

            _meshData.Indicies = new List<int>()
            {
                0, 1, 2, 2, 3, 0, // Front
                4, 7, 6, 6, 5, 4, // Back
                8, 11, 10, 10, 9, 8, // Top
                12, 13, 14, 14, 15, 12, // Bottom
                16, 17, 18, 18, 19, 16, // Left
                20, 23, 22, 22, 21, 20, // Right
            };

            vertexArray = new VertexPositionNormalTexture[_meshData.Vertices.Count];

            

            for (int v = 0; v < _meshData.Vertices.Count; v++)
                vertexArray[v] = new VertexPositionNormalTexture(_meshData.Vertices[v], _meshData.Normals[v], _meshData.TextCoords[v]);
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            if (!string.IsNullOrEmpty(_effectAsset))
                Effect = Game.Content.Load<Effect>(_effectAsset);

            if (!string.IsNullOrEmpty(_texture))
                Texture = Game.Content.Load<Texture2D>(_texture);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Transform.Update();
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if (Effect.Parameters["world"] != null)
                Effect.Parameters["world"].SetValue(Transform.World);


            if (Effect.Parameters["wvp"] != null)
                Effect.Parameters["wvp"].SetValue(Transform.World * _camera.View * _camera.Projection);


            if (Effect.Parameters["textureMat"] != null)
            {
                if (Texture != null)
                    Effect.Parameters["textureMat"].SetValue(Texture);
            }

            if (vertexArray != null)
            {
                int pCnt = Effect.CurrentTechnique.Passes.Count;

                for (int p = 0; p < pCnt; p++)
                {
                    
                    Effect.CurrentTechnique.Passes[p].Apply();
                    Game.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertexArray, 0, _meshData.Vertices.Count, _meshData.Indicies.ToArray(), 0, _meshData.Indicies.Count / 3);

                }
            }
        }
    }
}
