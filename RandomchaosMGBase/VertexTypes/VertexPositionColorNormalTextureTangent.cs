using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RandomchaosMGBase.VertexTypes
{
    public struct VertexPositionColorNormalTextureTangent : IVertexType
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 TexCoord;
        public Vector3 Tangent;
        public Vector4 Color;

        public VertexPositionColorNormalTextureTangent(Vector3 position, Vector3 normal, Vector3 tangent, Vector2 texcoord, Color color)
        {
            Position = position;
            Normal = normal;
            TexCoord = texcoord;
            Tangent = tangent;
            Color = color.ToVector4();
        }

        static public VertexElement[] VertexElements = new VertexElement[]
        {
                new VertexElement(0,VertexElementFormat.Vector3,VertexElementUsage.Position,0),
                new VertexElement(4*3,VertexElementFormat.Vector3,VertexElementUsage.Normal,0),
                new VertexElement(4*6,VertexElementFormat.Vector2 ,VertexElementUsage.TextureCoordinate,0),
                new VertexElement(4*8,VertexElementFormat.Vector3,VertexElementUsage.Tangent,0),
                new VertexElement(4*11,VertexElementFormat.Vector4,VertexElementUsage.Color,0)
        };

        public static int SizeInBytes = (3 + 3 + 2 + 3 + 4) * 4;

        #region IVertexType Members

        public static VertexDeclaration VertDec
        {
            get
            {
                return new VertexDeclaration
                        (
                        new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                        new VertexElement(4 * 3, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
                        new VertexElement(4 * 6, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
                        new VertexElement(4 * 8, VertexElementFormat.Vector3, VertexElementUsage.Tangent, 0),
                        new VertexElement(4 * 11, VertexElementFormat.Vector4, VertexElementUsage.Color, 0)
                        );
            }
        }

        public VertexDeclaration VertexDeclaration
        {
            get
            {
                return new VertexDeclaration
                        (
                        new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                        new VertexElement(4 * 3, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
                        new VertexElement(4 * 6, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
                        new VertexElement(4 * 8, VertexElementFormat.Vector3, VertexElementUsage.Tangent, 0),
                        new VertexElement(4 * 11, VertexElementFormat.Vector4, VertexElementUsage.Color, 0)
                        );
            }
        }
        #endregion
    }
}
