//using HSDRaw.AirRide.Gr.Data;
//using OpenTK.Graphics.OpenGL;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace HSDRawViewer.GUI.Plugins.AirRide.GrEditors
//{
//    public class GrZoneEditor : IGrEditor<KAR_ZoneCollisionJoint>
//    {
//        public KAR_ZoneCollisionJoint[] _items { get; set; }

//        public void Render(object selected)
//        {
//            if (_items == null)
//                return;

//            //var render_zones = new HashSet<KAR_ZoneCollisionJoint>();
//            //KAR_ZoneCollisionJoint _selectedBucket = (KAR_ZoneCollisionJoint)selected;

//            //if (_selectedBucket != null)
//            //{
//            //    for (int i = _selectedBucket.ZoneIndexStart; i < _selectedBucket.ZoneIndexStart + _selectedBucket.ZoneIndexCount; i++)
//            //    {
//            //        render_zones.Add(_zoneJoints[zoneIndices[i]]);
//            //    }
//            //}

//            // render collisions
//            GL.Begin(PrimitiveType.Triangles);
//            foreach (var j in _items)
//            {
//                //if (!render_zones.Contains(j))
//                //    continue;

//                if (arrayMemberEditor1.SelectedObject is KAR_ZoneCollisionJoint joint2 && joint2 != j)
//                    continue;

//                for (int i = j.ZoneFaceStart; i < j.ZoneFaceStart + j.ZoneFaceSize; i++)
//                {
//                    var t = _zoneTris[i];
//                    if (arrayMemberEditor1.SelectedObject is KAR_ZoneCollisionJoint joint && joint == j)
//                        GL.Color3(0f, 0f, 1f);
//                    else
//                        if (render_zones.Contains(j) || _selectedZoneTriangles.Contains(i))
//                        GL.Color3(1f, 1f, 0f);
//                    else
//                        GL.Color3(1f, 1f, 1f);

//                    GL.Vertex3(GXTranslator.toVector3(_zoneVertices[t.V1]));
//                    GL.Vertex3(GXTranslator.toVector3(_zoneVertices[t.V2]));
//                    GL.Vertex3(GXTranslator.toVector3(_zoneVertices[t.V3]));
//                }
//            }
//            GL.End();

//            GL.Begin(PrimitiveType.Lines);
//            foreach (var j in _items)
//            {
//                //if (!render_zones.Contains(j))
//                //    continue;

//                if (arrayMemberEditor1.SelectedObject is KAR_ZoneCollisionJoint joint2 && joint2 != j)
//                    continue;

//                for (int i = j.ZoneFaceStart; i < j.ZoneFaceStart + j.ZoneFaceSize; i++)
//                {
//                    GL.Color3(Color.Black);

//                    var t = _zoneTris[i];
//                    GL.Vertex3(GXTranslator.toVector3(_zoneVertices[t.V1]));
//                    GL.Vertex3(GXTranslator.toVector3(_zoneVertices[t.V2]));

//                    GL.Vertex3(GXTranslator.toVector3(_zoneVertices[t.V2]));
//                    GL.Vertex3(GXTranslator.toVector3(_zoneVertices[t.V3]));

//                    GL.Vertex3(GXTranslator.toVector3(_zoneVertices[t.V3]));
//                    GL.Vertex3(GXTranslator.toVector3(_zoneVertices[t.V1]));
//                }
//            }
//            GL.End();
//        }

//        public void Update()
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
