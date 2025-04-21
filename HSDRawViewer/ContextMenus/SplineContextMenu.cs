﻿using HSDRaw.Common;
using HSDRaw.Common.Animation;
using HSDRaw.Tools;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace HSDRawViewer.ContextMenus
{
    public class SplineContextMenu : CommonContextMenu
    {
        public override Type[] SupportedTypes { get; } = new Type[] { typeof(HSD_Spline) };

        public SplineContextMenu() : base()
        {
            ToolStripMenuItem importobj = new("Import OBJ");
            importobj.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is HSD_Spline spline)
                {
                    string f = Tools.FileIO.OpenFile("Wavefront OBJ(*.obj)|*.obj");

                    if (f != null)
                    {
                        SplineOBJ obj = new();
                        obj.Open(f);

                        if (obj.Polys.Count == 0)
                        {
                            MessageBox.Show("No splines found!", "OBJ Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        if (obj.Polys.Count > 1)
                        {
                            MessageBox.Show($"Multiple splines found\nUsing {obj.Polys[0].Name}", "OBJ Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }

                        Poly poly = obj.Polys[0];

                        // connect the lines to form one line
                        List<HSD_Vector3> lpoints = new();
                        foreach (int i in poly.GetConnectedLine())
                        {
                            lpoints.Add(obj.Vertices[i]);
                        }

                        // calculate total length
                        float total_length = 0;
                        for (int i = 1; i < lpoints.Count; i++)
                        {
                            float length = (lpoints[i] - lpoints[i - 1]).Length;
                            total_length += length;
                        }

                        // calculate lengths
                        float[] lengths = new float[lpoints.Count];
                        float t = 0;
                        for (int i = 0; i < lpoints.Count; i++)
                        {
                            if (i == 0)
                            {
                                lengths[0] = 0;
                            }
                            else
                            {
                                float length = (lpoints[i] - lpoints[i - 1]).Length;
                                t += length;
                                lengths[i] = t / total_length;
                            }
                        }

                        // dump to spline
                        spline.TotalLength = total_length;
                        spline.Points = lpoints.ToArray();
                        spline.Lengths = new HSDRaw.HSDFloatArray() { Array = lengths };

                        return;
                    }
                }
            };
            Items.Add(importobj);


            ToolStripMenuItem Exportobj = new("Export OBJ");
            Exportobj.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is HSD_Spline spline)
                {
                    string f = Tools.FileIO.SaveFile("Wavefront OBJ(*.obj)|*.obj");

                    if (f != null)
                    {
                        using FileStream stream = new(f, FileMode.Create);
                        using StreamWriter s = new(stream);
                        HSD_Vector3[] points = spline.Points;

                        foreach (HSD_Vector3 p in points)
                        {
                            s.WriteLine($"v {p.X} {p.Y} {p.Z}");
                        }

                        int i = 1;
                        s.WriteLine($"o spline");
                        s.Write($"l ");
                        foreach (HSD_Vector3 p in points)
                        {
                            s.Write($"{i++} ");
                        }
                    }
                }
            };
            Items.Add(Exportobj);


            ToolStripMenuItem anim = new("Generate Anim Joint");
            anim.Click += (sender, args) =>
            {
                if (MainForm.SelectedDataNode.Accessor is HSD_Spline spline)
                {
                    string f = Tools.FileIO.SaveFile(ApplicationSettings.HSDFileFilter);

                    if (f != null)
                    {
                        HSDRaw.HSDRawFile file = new();

                        file.Roots.Add(new HSDRaw.HSDRootNode()
                        {
                            Name = "_animjoint",
                            Data = GenerateAnimJoint(spline)
                        });

                        file.Save(f);
                    }
                }
            };
            Items.Add(anim);

        }

        private static HSD_AnimJoint GenerateAnimJoint(HSD_Spline spline)
        {
            List<FOBJKey> x = new();
            List<FOBJKey> y = new();
            List<FOBJKey> z = new();
            List<FOBJKey> rx = new();
            List<FOBJKey> ry = new();
            List<FOBJKey> rz = new();

            HSD_Vector3[] points = spline.Points;
            for (int i = 1; i < points.Length; i++)
            {
                Vector3 p1 = new(points[i - 1].X, points[i - 1].Y, points[i - 1].Z);
                Vector3 p2 = new(points[i].X, points[i].Y, points[i].Z);
                Vector3 direction = (p2 - p1).Normalized();
                Vector3 rotation = ConvertDirectionToEulerAngles(direction);
                rotation.Y -= (float)Math.PI / 2;

                float frame = i * 10;

                x.Add(new FOBJKey()
                {
                    Frame = frame,
                    Value = p1.X,
                    InterpolationType = GXInterpolationType.HSD_A_OP_LIN
                });
                y.Add(new FOBJKey()
                {
                    Frame = frame,
                    Value = p1.Y,
                    InterpolationType = GXInterpolationType.HSD_A_OP_LIN
                });
                z.Add(new FOBJKey()
                {
                    Frame = frame,
                    Value = p1.Z,
                    InterpolationType = GXInterpolationType.HSD_A_OP_LIN
                });
                rx.Add(new FOBJKey()
                {
                    Frame = frame,
                    Value = rotation.X,
                    InterpolationType = GXInterpolationType.HSD_A_OP_LIN
                });
                ry.Add(new FOBJKey()
                {
                    Frame = frame,
                    Value = rotation.Y,
                    InterpolationType = GXInterpolationType.HSD_A_OP_LIN
                });
                rz.Add(new FOBJKey()
                {
                    Frame = frame,
                    Value = rotation.Z,
                    InterpolationType = GXInterpolationType.HSD_A_OP_LIN
                });

            }

            // generate anim joint
            HSD_AnimJoint joint = new();
            joint.AOBJ = new HSD_AOBJ()
            {
                EndFrame = (points.Length - 1) * 10
            };

            HSD_FOBJDesc prev = null;
            foreach (Tuple<List<FOBJKey>, JointTrackType> v in new Tuple<List<FOBJKey>, JointTrackType>[]
            {
                new(x, JointTrackType.HSD_A_J_TRAX),
                new(y, JointTrackType.HSD_A_J_TRAY),
                new(z, JointTrackType.HSD_A_J_TRAZ),
                new(rx, JointTrackType.HSD_A_J_ROTX),
                new(ry, JointTrackType.HSD_A_J_ROTY),
                new(rz, JointTrackType.HSD_A_J_ROTZ),
            }
            )
            {
                HSD_FOBJDesc desc = new();
                desc.SetKeys(v.Item1, (byte)v.Item2);

                if (prev != null)
                {
                    prev.Next = desc;
                }
                else
                {
                    joint.AOBJ.FObjDesc = desc;
                }
                prev = desc;
            }

            return joint;
        }

        public static Vector3 ConvertDirectionToEulerAngles(Vector3 direction)
        {
            // Ensure the direction vector is normalized
            direction.Normalize();

            // Calculate the pitch (rotation around the X-axis)
            float pitch = (float)Math.Asin(-direction.Y);

            // Calculate the yaw (rotation around the Y-axis)
            float yaw = (float)Math.Atan2(direction.X, direction.Z);

            // Adjust the angle to be between 0 and TwoPi
            while (yaw < 0)
                yaw += MathHelper.TwoPi;

            while (yaw > MathHelper.TwoPi)
                yaw -= MathHelper.TwoPi;

            return new Vector3(pitch, yaw, 0f);
        }

        /// <summary>
        /// 
        /// </summary>
        private class SplineOBJ
        {
            public List<HSD_Vector3> Vertices = new();
            public List<Poly> Polys = new();

            /// <summary>
            /// 
            /// </summary>
            /// <param name="filePath"></param>
            public void Open(string filePath)
            {
                using FileStream stream = new(filePath, FileMode.Open);
                using StreamReader s = new(stream);
                Poly poly = null;

                while (!s.EndOfStream)
                {
                    string[] line = s.ReadLine()?.Split(' ');

                    if (line == null || line.Length == 0)
                        continue;

                    switch (line[0])
                    {
                        case "o":
                            poly = new Poly()
                            {
                                Name = line.Length > 1 ? line[1] : string.Empty
                            };
                            Polys.Add(poly);
                            break;
                        case "v":
                            {
                                if (line.Length >= 4 &&
                                    float.TryParse(line[1], out float x) &&
                                    float.TryParse(line[2], out float y) &&
                                    float.TryParse(line[3], out float z))
                                {
                                    Vertices.Add(new HSD_Vector3(x, y, z));
                                }
                            }
                            break;
                        case "l":
                            if (poly != null)
                            {
                                PolyLine polyLine = new();
                                for (int i = 1; i < line.Length; i++)
                                {
                                    if (int.TryParse(line[i], out int index))
                                    {
                                        // Adjust indices to be zero-based
                                        polyLine.Indices.Add(index - 1);
                                    }
                                }
                                poly.Lines.Add(polyLine);
                            }
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private class Poly
        {
            public string Name;
            public List<PolyLine> Lines = new();

            public IEnumerable<int> GetConnectedLine()
            {
                if (Lines.Count == 0)
                {
                    yield break;
                }

                HashSet<int> visitedIndices = new();
                Queue<int> indexQueue = new();

                // Start with the first line
                PolyLine firstLine = Lines[0];

                foreach (int index in firstLine.Indices)
                {
                    visitedIndices.Add(index);
                    indexQueue.Enqueue(index);
                    yield return index;
                }

                while (indexQueue.Count > 0)
                {
                    int currentIndex = indexQueue.Dequeue();

                    foreach (PolyLine line in Lines)
                    {
                        if (line.Indices.Contains(currentIndex))
                        {
                            foreach (int newIndex in line.Indices)
                            {
                                if (!visitedIndices.Contains(newIndex))
                                {
                                    visitedIndices.Add(newIndex);
                                    indexQueue.Enqueue(newIndex);
                                    yield return newIndex;
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private class PolyLine
        {
            public List<int> Indices = new();
        }
    }
}
