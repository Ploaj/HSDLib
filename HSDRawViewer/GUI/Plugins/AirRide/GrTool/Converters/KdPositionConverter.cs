using HSDRawViewer.IO.AirRide.DataFormat;
using HSDRawViewer.Rendering;
using IONET.Core.Model;
using IONET.Core.Skeleton;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;

namespace HSDRawViewer.GUI.Plugins.AirRide.GrTool.Converters
{
    public class KdPositionConverter
    {

        public static IOBone GenerateIOBone(string name, KdPosition p)
        {
            var r = p.ToQuaternion();
            var bone = new IOBone()
            {
                Name = name,
                Translation = new Vector3(p.Position.X, p.Position.Y, p.Position.Z),
                Rotation = new Quaternion(r.X, r.Y, r.Z, r.W),
                Scale = Vector3.One,
            };

            return bone;
        }

        public static KdPosition FromIOBone(IOBone b)
        {
            var p = b.WorldTransform.Translation;
            var f = Vector3.TransformNormal(Vector3.UnitZ, b.WorldTransform);
            var u = Vector3.TransformNormal(Vector3.UnitY, b.WorldTransform);

            return new KdPosition()
            {
                Position = new KdVector(p.X, p.Y, p.Z),
                Forward = new KdVector(f.X, f.Y, f.Z),
                Up = new KdVector(u.X, u.Y, u.Z),
            };
        }

        public static (IOBone, IOMesh) GenerateIOBoneMesh(string name, KdPositionArea p)
        {
            OpenTK.Mathematics.Vector3 forward = new OpenTK.Mathematics.Vector3(
                p.Forward.X,
                p.Forward.Y,
                p.Forward.Z);

            var tkr = Math3D.FromForwardUp(
                forward,
                OpenTK.Mathematics.Vector3.UnitY);

            Quaternion rotation = new Quaternion(tkr.X, tkr.Y, tkr.Z, tkr.W);

            Vector3 center = new Vector3(
                (p.P1.X + p.P2.X) * 0.5f,
                (p.P1.Y + p.P2.Y) * 0.5f,
                (p.P1.Z + p.P2.Z) * 0.5f);

            // Half-extents.
            Vector3 halfExtents = new Vector3(
                MathF.Abs(p.P1.X - p.P2.X),
                MathF.Abs(p.P1.Y - p.P2.Y),
                MathF.Abs(p.P1.Z - p.P2.Z)) * 0.5f;

            var bone = new IOBone()
            {
                Name = name,
                Translation = center,
                Rotation = rotation,
                Scale = halfExtents,
            };

            var mesh = new IOMesh()
            {
                Name = $"M{name}",
                ParentBone = bone,
            };

            // Unit cube in BONE LOCAL SPACE.
            //
            // The bone's scale turns this into the actual area dimensions.
            mesh.Vertices.AddRange(
                new IOVertex[]
                {
            // Front (+Z)
            new IOVertex() { Position = new Vector3(-1, -1,  1), Normal = new Vector3( 0,  0,  1) },
            new IOVertex() { Position = new Vector3( 1, -1,  1), Normal = new Vector3( 0,  0,  1) },
            new IOVertex() { Position = new Vector3( 1,  1,  1), Normal = new Vector3( 0,  0,  1) },
            new IOVertex() { Position = new Vector3(-1,  1,  1), Normal = new Vector3( 0,  0,  1) },

            // Back (-Z)
            new IOVertex() { Position = new Vector3( 1, -1, -1), Normal = new Vector3( 0,  0, -1) },
            new IOVertex() { Position = new Vector3(-1, -1, -1), Normal = new Vector3( 0,  0, -1) },
            new IOVertex() { Position = new Vector3(-1,  1, -1), Normal = new Vector3( 0,  0, -1) },
            new IOVertex() { Position = new Vector3( 1,  1, -1), Normal = new Vector3( 0,  0, -1) },

            // Right (+X)
            new IOVertex() { Position = new Vector3( 1, -1, -1), Normal = new Vector3( 1,  0,  0) },
            new IOVertex() { Position = new Vector3( 1, -1,  1), Normal = new Vector3( 1,  0,  0) },
            new IOVertex() { Position = new Vector3( 1,  1,  1), Normal = new Vector3( 1,  0,  0) },
            new IOVertex() { Position = new Vector3( 1,  1, -1), Normal = new Vector3( 1,  0,  0) },

            // Left (-X)
            new IOVertex() { Position = new Vector3(-1, -1,  1), Normal = new Vector3(-1,  0,  0) },
            new IOVertex() { Position = new Vector3(-1, -1, -1), Normal = new Vector3(-1,  0,  0) },
            new IOVertex() { Position = new Vector3(-1,  1, -1), Normal = new Vector3(-1,  0,  0) },
            new IOVertex() { Position = new Vector3(-1,  1,  1), Normal = new Vector3(-1,  0,  0) },

            // Top (+Y)
            new IOVertex() { Position = new Vector3(-1,  1,  1), Normal = new Vector3( 0,  1,  0) },
            new IOVertex() { Position = new Vector3( 1,  1,  1), Normal = new Vector3( 0,  1,  0) },
            new IOVertex() { Position = new Vector3( 1,  1, -1), Normal = new Vector3( 0,  1,  0) },
            new IOVertex() { Position = new Vector3(-1,  1, -1), Normal = new Vector3( 0,  1,  0) },

            // Bottom (-Y)
            new IOVertex() { Position = new Vector3(-1, -1, -1), Normal = new Vector3( 0, -1,  0) },
            new IOVertex() { Position = new Vector3( 1, -1, -1), Normal = new Vector3( 0, -1,  0) },
            new IOVertex() { Position = new Vector3( 1, -1,  1), Normal = new Vector3( 0, -1,  0) },
            new IOVertex() { Position = new Vector3(-1, -1,  1), Normal = new Vector3( 0, -1,  0) },
            });

            var poly = new IOPolygon()
            {
                PrimitiveType = IOPrimitive.TRIANGLE,
                Indicies = new List<int>()
        {
            // Front
             0,  1,  2,
             0,  2,  3,

            // Back
             4,  5,  6,
             4,  6,  7,

            // Right
             8,  9, 10,
             8, 10, 11,

            // Left
            12, 13, 14,
            12, 14, 15,

            // Top
            16, 17, 18,
            16, 18, 19,

            // Bottom
            20, 21, 22,
            20, 22, 23,
        }
            };

            mesh.Polygons.Add(poly);

            return (bone, mesh);
        }
    }
}
