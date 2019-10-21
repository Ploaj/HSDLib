using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HSDRaw;
using HSDRaw.Common;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace HSDRawViewer.Rendering
{

    /// <summary>
    /// 
    /// </summary>
    public class JOBJManager
    {
        #region Settings

        public bool RenderBones { get; set; }

        public bool RenderObjects { get; set; }

        public bool RenderMaterials { get; set; }

        #endregion

        private HSD_JOBJ RootJOBJ { get; set; }

        // caches
        private class JOBJCache
        {
            public JOBJCache Parent;

            public Matrix4 LocalTransform;
            public Matrix4 WorldTransform;
            public Matrix4 BindTransform;
        }
        private Dictionary<HSD_JOBJ, JOBJCache> jobjToCache = new Dictionary<HSD_JOBJ, JOBJCache>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jobj"></param>
        /// <returns></returns>
        public Matrix4 GetWorldTransform(HSD_JOBJ jobj)
        {
            if (jobjToCache.ContainsKey(jobj))
                return jobjToCache[jobj].WorldTransform;
            else
                return Matrix4.Identity;
        }

        /// <summary>
        /// 
        /// </summary>
        public void ClearRenderingCache()
        {
            jobjToCache.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        public void SetJOBJ(HSD_JOBJ root)
        {
            ClearRenderingCache();
            RootJOBJ = root;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Render()
        {
            UpdateTransforms(RootJOBJ);
            
            foreach(var b in jobjToCache)
            {
                if(b.Value.Parent != null)
                    RenderBone(b.Value.WorldTransform, b.Value.Parent.WorldTransform);
                else
                    RenderBone(b.Value.WorldTransform, b.Value.WorldTransform);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void UpdateNoRender()
        {
            UpdateTransforms(RootJOBJ);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="parentTransform"></param>
        private void RenderBone(Matrix4 transform, Matrix4 parentTransform)
        {
            var bonePosition = Vector3.TransformPosition(Vector3.Zero, transform);
            var parentPosition = Vector3.TransformPosition(Vector3.Zero, parentTransform);

            GL.LineWidth(1f);
            GL.Begin(PrimitiveType.Lines);
            GL.Color3(0f, 1f, 0f);
            GL.Vertex3(parentPosition);
            GL.Color3(0f, 0f, 1f);
            GL.Vertex3(bonePosition);
            GL.End();

            GL.Color3(1f, 0f, 0f);
            GL.PointSize(5f);
            GL.Begin(PrimitiveType.Points);
            GL.Vertex3(bonePosition);
            GL.End();
        }

        /// <summary>
        /// Updates the transforms
        /// </summary>
        /// <param name="root"></param>
        private void UpdateTransforms(HSD_JOBJ root, JOBJCache parent = null)
        {
            if (root == null)
                return;

            var local = CreateLocalTransform(root);
            var world = local;
            if (parent != null)
                world = local * parent.WorldTransform;

            var cache = new JOBJCache()
            {
                LocalTransform = local,
                WorldTransform = world,
                BindTransform = world,
                Parent = parent
            };

            if(!jobjToCache.ContainsKey(root))
                jobjToCache.Add(root, cache);

            jobjToCache[root] = cache;

            foreach (var child in root.Children)
            {
                UpdateTransforms(child, cache);
            }
        }

        /// <summary>
        /// Creates a Matrix4 from a HSD_JOBJ
        /// </summary>
        /// <param name="jobj"></param>
        /// <returns></returns>
        private Matrix4 CreateLocalTransform(HSD_JOBJ jobj)
        {
            float TX = jobj.TX;
            float TY = jobj.TY;
            float TZ = jobj.TZ;
            float RX = jobj.RX;
            float RY = jobj.RY;
            float RZ = jobj.RZ;
            float SX = jobj.SX;
            float SY = jobj.SY;
            float SZ = jobj.SZ;
            
            Matrix4 Transform = Matrix4.CreateScale(SX, SY, SZ) *
                Matrix4.CreateFromQuaternion(Math3D.FromEulerAngles(RZ, RY, RX)) *
                Matrix4.CreateTranslation(TX, TY, TZ);

            return Transform;
        }

        /// <summary>
        /// Removes a JOBJ from the tree
        /// </summary>
        /// <param name="jobj"></param>
        public void RemoveJOBJ(HSD_JOBJ jobj)
        {
            // find parent or prev in list

            HSD_JOBJ next = null;
            HSD_JOBJ parent = null;

            foreach (var v in RootJOBJ.DepthFirstList)
            {
                if (v.Next == jobj)
                    next = v;
                if (v.Child == jobj)
                    parent = v;
            }

            if(next != null)
            {
                next.Next = jobj.Next;
            }
            if(parent != null)
            {
                parent.Child = jobj.Next;
            }

            ClearRenderingCache();
        }
    }
}
