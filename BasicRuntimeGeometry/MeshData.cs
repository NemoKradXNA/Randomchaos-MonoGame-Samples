using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicRuntimeGeometry
{
    public class MeshData
    {
        public List<Vector3> Vertices = new List<Vector3>();
        public List<Vector3> Normals = new List<Vector3>();
        public List<Vector3> Tangents = new List<Vector3>();
        public List<Vector2> TextCoords = new List<Vector2>();
        public List<Color> Colors = new List<Color>();
        public List<int> Indicies = new List<int>();

        public void Combine(MeshData data)
        {
            int tcnt = Vertices.Count;
            foreach (int t in data.Indicies)
            {
                Indicies.Add(t + tcnt);
            }
            Tangents.AddRange(data.Tangents);
            Normals.AddRange(data.Normals);
            Vertices.AddRange(data.Vertices);
            TextCoords.AddRange(data.TextCoords);
            Colors.AddRange(data.Colors);
            Tangents.AddRange(data.Tangents);
        }
    }
}
