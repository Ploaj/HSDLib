using HSDRawViewer.ContextMenus;
using HSDRawViewer.GUI;
using HSDRawViewer.GUI.Plugins;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HSDRawViewer
{
    /// <summary>
    /// 
    /// </summary>
    public class PluginManager
    {
        private static Dictionary<Type, ContextMenu> typeToContextMenu;
        private static CommonContextMenu commonContextMenu;

        private static Dictionary<Type, Type> typeToEditor;

        private static bool Initialized = false;

        /// <summary>
        /// Gets the shared viewport control
        /// </summary>
        /// <returns></returns>
        public static ViewportControl GetCommonViewport()
        {
            if(MainForm.Instance != null)
            {
                return MainForm.Instance.Viewport.glViewport;
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static ContextMenu GetContextMenuFromType(Type t)
        {
            if (typeToContextMenu.ContainsKey(t))
            {
                return typeToContextMenu[t];
            }
            return commonContextMenu;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static EditorBase GetEditorFromType(Type t)
        {
            if (typeToEditor.ContainsKey(t))
            {
                return (EditorBase)Activator.CreateInstance(typeToEditor[t]);
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static bool HasEditor(Type t)
        {
            return typeToEditor.ContainsKey(t);
        }

        /// <summary>
        /// 
        /// </summary>
        public static void Init()
        {
            if (Initialized)
                return;

            typeToEditor = new Dictionary<Type, Type>();
            InitEditors();

            typeToContextMenu = new Dictionary<Type, ContextMenu>();
            InitContextMenus();
            commonContextMenu = new CommonContextMenu();

            Initialized = true;
        }

        private static void InitContextMenus()
        {
            var types = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                         from assemblyType in domainAssembly.GetTypes()
                         where typeof(CommonContextMenu).IsAssignableFrom(assemblyType)
                         select assemblyType).ToArray();

            foreach (var t in types)
            {
                if (t != typeof(CommonContextMenu))
                {
                    var ren = (CommonContextMenu)Activator.CreateInstance(t);

                    foreach (var v in ren.SupportedTypes)
                    {
                        typeToContextMenu.Add(v, ren);
                    }
                }
            }
        }

        private static void InitEditors()
        {
            var types = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                         from assemblyType in domainAssembly.GetTypes()
                         where typeof(EditorBase).IsAssignableFrom(assemblyType)
                         select assemblyType).ToArray();

            foreach (var t in types)
            {
                if (t != typeof(EditorBase))
                {
                    var ren = (EditorBase)Activator.CreateInstance(t);

                    foreach (var v in ren.SupportedTypes)
                    {
                        typeToEditor.Add(v, t);
                    }

                    if (ren is IDisposable dis)
                        dis.Dispose();
                }
            }
        }
    }
}
