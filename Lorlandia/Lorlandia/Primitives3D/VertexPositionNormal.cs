using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Lorlandia.Primitives3D
{
    public struct VertexPositionNormal:IVertexType
    {
        public Vector3 position;
        public Vector3 normal;

        public VertexPositionNormal(Vector3 position, Vector3 normal)
        {
            this.position = position;
            this.normal = normal;
        }

        public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration(new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0), new VertexElement(sizeof(float)*3, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0));

        VertexDeclaration IVertexType.VertexDeclaration
        {
            get { return VertexPositionNormal.VertexDeclaration; }
        }
    }
}
