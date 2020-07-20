using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RandomchaosMGBase.BaseClasses
{
    public class Base3DQuad : Base3DObject
    {
        VertexPositionNormalTexture[] corners;
        VertexBuffer vb;
        short[] ib;
        //VertexDeclaration vertDec;

        public Base3DQuad(Game game, string effect) : base(game, string.Empty, effect)
        {
            corners = new VertexPositionNormalTexture[4];
            corners[0].Position = new Vector3(0, 0, 0);
            corners[0].TextureCoordinate = Vector2.Zero;
        }

        public override void Initialize()
        {
            base.Initialize();

            //vertDec = VertexPositionNormalTexture.VertexDeclaration;

            corners = new VertexPositionNormalTexture[]
                    {
                        new VertexPositionNormalTexture(
                            new Vector3(.5f,-.5f,0),
                            new Vector3(0,0,-1),
                            new Vector2(1,1)),
                        new VertexPositionNormalTexture(
                            new Vector3(-.5f,-.5f,0),
                            new Vector3(0,0,-1),
                            new Vector2(0,1)),
                        new VertexPositionNormalTexture(
                            new Vector3(-.5f,.5f,0),
                            new Vector3(0,0,-1),
                            new Vector2(0,0)),
                        new VertexPositionNormalTexture(
                            new Vector3(.5f,.5f,0),
                            new Vector3(0,0,-1),
                            new Vector2(1,0))
                    };

            ib = new short[] { 0, 1, 2, 2, 3, 0 };
            vb = new VertexBuffer(Game.GraphicsDevice, typeof(VertexPositionNormalTexture), corners.Length, BufferUsage.None);

            vb.SetData(corners);
            
        }

        public override void SetEffect(GameTime gameTime, Effect effect)
        {
            meshWorld = World;
            meshWVP = meshWorld * camera.View * camera.Projection;

            base.SetEffect(gameTime, effect);
        }

        public override void Draw(GameTime gameTime, Effect effect)
        {
            if (!Enabled)
                return;

            Game.GraphicsDevice.BlendState = BlendState.Opaque;
            Game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            Game.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            Game.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;

            Game.GraphicsDevice.SetVertexBuffer(vb);

            SetEffect(gameTime, effect);

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                Game.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, corners, 0, 4, ib, 0, 2);
            }
        }
    }
}
