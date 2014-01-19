using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Lorlandia.Physics;

namespace Lorlandia.Terrain
{
    class HightMapTerrain
    {
        Texture2D hightMap;
        Texture2D texture;
        VertexBuffer vertexBuffer;
        IndexBuffer indexBuffer;
        Vector3[,] vertex_grid;
        QuadGridColision collision;

        public Vector3[,] VertexCoords
        {
            get { return vertex_grid; }
            private set {}
        }

        Effect effect;
        GraphicsDevice device;
        Matrix world;
        float[,] heightData;

        int terrain_width;
        int terrain_height;
        int triangle_count;

        public HightMapTerrain(Texture2D height_map, Texture2D texture, Effect effect, GraphicsDevice device)
        {
            this.hightMap = height_map;
            this.texture = texture;
            terrain_width = hightMap.Width;
            terrain_height = hightMap.Height;
            world = Matrix.CreateTranslation(-terrain_width/2.0f, 0.0f, -terrain_height/2.0f);
            this.effect = effect;
            this.device = device;
            vertex_grid = new Vector3[terrain_width, terrain_height];
            collision = new QuadGridColision();
        }

        public void Collision(ref Vector3 position)
        {
            Matrix inverse_world = Matrix.Invert(world);
            position = Vector3.Transform(position, inverse_world);
            position =collision.GetCurrentHeight(vertex_grid, position);
            position = Vector3.Transform(position, world);
        }

        public void SetUpGeometry()
        {
            LoadHeightData();

            VertexPositionNormalTexture[] vertices = new VertexPositionNormalTexture[heightData.Length];
            for (int i = 0; i < terrain_width; i++)
            {
                for (int j = 0; j < terrain_height; j++)
                {
                    vertices[i * terrain_width + j] = new VertexPositionNormalTexture(new Vector3(i, heightData[i, j], j), Vector3.Zero, new Vector2(((float)i*6)/terrain_width, ((float)j*6)/terrain_height));
                    vertex_grid[i, j] = new Vector3(i, heightData[i, j], j);
                }
            }
            ushort[] indices;
            LoadIndices(out indices);
            triangle_count = indices.Length / 3;
            CalculateNormals(vertices, indices);
            vertexBuffer = new VertexBuffer(device, VertexPositionNormalTexture.VertexDeclaration, vertices.Length, BufferUsage.WriteOnly);
            indexBuffer = new IndexBuffer(device, typeof(ushort), indices.Length, BufferUsage.WriteOnly);
            vertexBuffer.SetData<VertexPositionNormalTexture>(vertices);
            indexBuffer.SetData<ushort>(indices);
        }

        void LoadIndices(out ushort[] indices)
        {
            indices = new ushort[(terrain_height - 1) * (terrain_width - 1) * 6];
            int counter = 0;
            for (int i = 0; i < terrain_height - 1; i++)
            {
                for (int j = 0; j < terrain_width - 1; j++)
                {
                    ushort bottom_left = (ushort)(i * terrain_width + j);
                    ushort bottom_right = (ushort)(i * terrain_width + (j + 1));
                    ushort top_left = (ushort)((i + 1) * terrain_width + j);
                    ushort top_right = (ushort)((i + 1) * terrain_width + (j + 1));
                    
                    indices[counter++] = top_left;
                    indices[counter++] = top_right;
                    indices[counter++] = bottom_left;

                    indices[counter++] = bottom_right;
                    indices[counter++] = bottom_left;
                    indices[counter++] = top_right;
                }
            }
        }

        void LoadHeightData()
        {
            heightData = new float[terrain_width, terrain_height];
            Color[] colours = new Color[terrain_width * terrain_height];
            hightMap.GetData<Color>(colours);
            for (int i = 0; i < terrain_height; i++)
            {
                for (int j = 0; j < terrain_width; j++)
                {
                    heightData[j, i] = colours[i * terrain_width + j].R*0.1f;
                }
            }
        }

        void CalculateNormals(VertexPositionNormalTexture[] vertices, ushort[] indices)
        {
            for (int i = 0; i < indices.Length / 3; i++)
            {
                int index0 = i * 3;
                int index1 = i * 3+1;
                int index2 = i * 3+2;

                Vector3 edgeA = vertices[indices[index2]].Position - vertices[indices[index0]].Position;
                Vector3 edgeB = vertices[indices[index1]].Position - vertices[indices[index0]].Position;
                Vector3 triangle_normal = Vector3.Cross(edgeA, edgeB);

                vertices[indices[index0]].Normal += triangle_normal;
                vertices[indices[index1]].Normal += triangle_normal;
                vertices[indices[index2]].Normal += triangle_normal;
            }

            for (int j = 0; j < vertices.Length; j++) vertices[j].Normal.Normalize();
        }

        public void Draw(Matrix view, Matrix projection)
        {
            effect.CurrentTechnique = effect.Techniques["Textured"];
            effect.Parameters["World"].SetValue(world);
            effect.Parameters["View"].SetValue(view);
            effect.Parameters["Projection"].SetValue(projection);
            effect.Parameters["xTexture"].SetValue(texture);
            device.SetVertexBuffer(vertexBuffer);
            device.Indices = indexBuffer;
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, terrain_width * terrain_height, 0, triangle_count);
            }
        }
    }
}
