using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lorlandia.Primitives3D
{
    public class GeometricPrimitive:IDisposable
    {
        protected GraphicsDevice device;

        VertexBuffer vertexBuffer;
        IndexBuffer indexBuffer;
        BasicEffect effect;
        protected Boolean quadrangle;

        public Color color = Color.White;

        protected List<VertexPositionNormal> vertices = new List<VertexPositionNormal>();
        List<ushort> indices = new List<ushort>();
        protected Matrix world;
        
        Vector3 _centre;

        public Vector3 Centre
        {
            get
            {
                return Vector3.Transform(_centre, world);
            }
            protected set
            {
                _centre = value;
            }
        }
        protected Int32 LastVertex
        {
            get { return vertices.Count-1; }
            private set { }
        }
        static RasterizerState Wire = new RasterizerState { FillMode = FillMode.WireFrame, CullMode = CullMode.None };

        public void InitializePrimitive()
        {
            vertexBuffer = new VertexBuffer(device, VertexPositionNormal.VertexDeclaration, vertices.Count, BufferUsage.WriteOnly);
            indexBuffer = new IndexBuffer(device, typeof(ushort), indices.Count, BufferUsage.WriteOnly);
            effect = new BasicEffect(device);
            vertexBuffer.SetData<VertexPositionNormal>(vertices.ToArray());
            indexBuffer.SetData<ushort>(indices.ToArray());
            effect.EnableDefaultLighting();
        }
        
        protected void AddVertex(Vector3 position, Vector3 normal)
        { 
            vertices.Add(new VertexPositionNormal(position, normal));
        }

        protected void AddIndex(int index)
        {
            if (index > ushort.MaxValue) throw new ArgumentOutOfRangeException("index");
            indices.Add((ushort)index);
        }

        public virtual void Update(Matrix world)
        {
            this.world = world;
        }

        public virtual void Draw(Matrix view, Matrix projection)
        {
            //this.world = world;
            effect.World = world;
            effect.View = view;
            effect.Projection = projection;
            effect.DiffuseColor = color.ToVector3();
            effect.Alpha = color.A / 255.0f;

            device.DepthStencilState = DepthStencilState.Default;
            device.BlendState = BlendState.AlphaBlend;
            device.RasterizerState = Wire;
            device.SetVertexBuffer(vertexBuffer);
            device.Indices = indexBuffer;
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                if (!quadrangle) device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertices.Count, 0, indices.Count / 3);
                else
                {
                    device.DrawIndexedPrimitives(PrimitiveType.LineStrip, 0, 0, vertices.Count, 0, indices.Count - 1);
                }

            }
            device.RasterizerState = RasterizerState.CullCounterClockwise;
            device.BlendState = BlendState.Opaque;            
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (vertexBuffer != null) vertexBuffer.Dispose();
                if (indexBuffer != null) indexBuffer.Dispose();
                if (effect != null) effect.Dispose();
                device.Dispose();
            }
        }

    }
}
