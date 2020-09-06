using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RandomchaosMGFire
{
    public class GPUOldSchoolFireBox : GPUOldSchoolFire
    {
        protected Quaternion rotation = Quaternion.Identity;
        protected Matrix World;
        protected Matrix View = Matrix.Identity;
        protected Matrix Projection;

        protected SpriteFont font;

        public GPUOldSchoolFireBox(Game game) : base(game) { }
        public GPUOldSchoolFireBox(Game game, Point resolution) : base(game,resolution) { }

        protected override void LoadContent()
        {
            base.LoadContent();

            font = Game.Content.Load<SpriteFont>("Fonts/font2");
            thisBox = new BoundingBox(-Vector3.One * .5f, Vector3.One * .5f);

            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.Pi / 4, (float)Resolution.X / (float)Resolution.Y, .1f, 1000f);
        }
        

        public override void Update(GameTime gameTime)
        {
            if (!Enabled)
                return;

            base.Update(gameTime);

            boxCol = new Color(maxFuel / 255f, maxFuel / 255f, oxygen, 1);

            World = Matrix.CreateScale(Vector3.One) *
                      Matrix.CreateFromQuaternion(rotation) *
                      Matrix.CreateTranslation(new Vector3(0, 0, -4f));

            Vector3 axis = Vector3.Transform(Vector3.Up + Vector3.Forward + Vector3.Left, Matrix.CreateFromQuaternion(rotation));
            rotation = Quaternion.Normalize(Quaternion.CreateFromAxisAngle(axis, .05f) * rotation);


        }

        public override void GenerateFireMap(int mapWidth, int mapHeight)
        {
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            spriteBatch.DrawString(font, "Fire!", new Vector2((Resolution.X / 2) - font.MeasureString("Fire!").X / 2, (Resolution.Y / 2) - font.MeasureString("Fire!").Y / 2), Color.Black);
            spriteBatch.End();

            DrawBox(thisBox);
        }

        public override void Draw(GameTime gameTime)
        {

            if (!Enabled)
                return;

            effect.Parameters["fireMap"].SetValue(fireMap);

            effect.Parameters["oxygen"].SetValue(oxygen);
            effect.Parameters["paletteMap"].SetValue(paletteTexture);
            effect.Parameters["width"].SetValue((float)Resolution.X);
            effect.Parameters["height"].SetValue((float)Resolution.Y);

            // Capture fireMap data.
            GraphicsDevice.SetRenderTarget(rt);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            effect.CurrentTechnique = effect.Techniques["UpdateFire"];
            effect.CurrentTechnique.Passes[0].Apply();

            spriteBatch.Draw(fireMap, new Rectangle(0, 0, Resolution.X, Resolution.Y), Color.White);
            spriteBatch.End();

            // Create fireMap data
            GenerateFireMap(0, 0);

            GraphicsDevice.SetRenderTarget(null);

            // Get the data from the rt and pack it back into the map.
            rt.GetData<Color>(rtc);
            GraphicsDevice.Textures[1] = null;
            fireMap.SetData<Color>(rtc);

            // Re draw with color passing in the firemap data
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);

            effect.CurrentTechnique = effect.Techniques["ColorFire"];
            effect.CurrentTechnique.Passes[0].Apply();

            spriteBatch.Draw(fireMap, new Rectangle(0, 0, GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight), Color.White);

            spriteBatch.End();

            // Debug
            if (ShowDebug)
                RenderDebugTextures(paletteTexture, fireMap);
        }
        
        protected VertexPositionColor[] points;
        protected Color boxCol = Color.White;
        protected short[] index;
        protected BasicEffect lineShader;
        protected BoundingBox thisBox;

        protected void DrawBox(BoundingBox box)
        {
            BuildBox(box, boxCol);
            DrawLines(12);
        }
        protected void BuildBox(BoundingBox box, Color lineColor)
        {
            points = new VertexPositionColor[8];

            Vector3[] corners = box.GetCorners();

            points[0] = new VertexPositionColor(corners[1], lineColor); // Front Top Right
            points[1] = new VertexPositionColor(corners[0], lineColor); // Front Top Left
            points[2] = new VertexPositionColor(corners[2], lineColor); // Front Bottom Right
            points[3] = new VertexPositionColor(corners[3], lineColor); // Front Bottom Left
            points[4] = new VertexPositionColor(corners[5], lineColor); // Back Top Right
            points[5] = new VertexPositionColor(corners[4], lineColor); // Back Top Left
            points[6] = new VertexPositionColor(corners[6], lineColor); // Back Bottom Right
            points[7] = new VertexPositionColor(corners[7], lineColor); // Bakc Bottom Left

            index = new short[] {
                0, 1, 0, 2, 1, 3, 2, 3,
                4, 5, 4, 6, 5, 7, 6, 7,
                0, 4, 1, 5, 2, 6, 3, 7
                };
        }
        protected void DrawLines(int primativeCount)
        {
            if (lineShader == null)
                lineShader = new BasicEffect(GraphicsDevice);

            lineShader.World = World;
            lineShader.View = View;
            lineShader.Projection = Projection;
            lineShader.VertexColorEnabled = true;

            lineShader.CurrentTechnique.Passes[0].Apply();
            GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.LineList, points, 0, points.Length, index, 0, primativeCount);
        }
    }
}
