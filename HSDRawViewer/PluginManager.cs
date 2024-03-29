﻿using HSDRawViewer.ContextMenus;
using HSDRawViewer.GUI;
using HSDRawViewer.GUI.Plugins;
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
        private static Dictionary<Type, ContextMenuStrip> typeToContextMenu;
        private static CommonContextMenu commonContextMenu;

        private static Dictionary<Type, Type> typeToEditor;

        private static bool Initialized = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static ContextMenuStrip GetContextMenuFromType(Type t)
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
        public static PluginBase GetEditorFromType(Type t)
        {
            if (typeToEditor.ContainsKey(t))
            {
                return (PluginBase)Activator.CreateInstance(typeToEditor[t]);
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

            typeToContextMenu = new Dictionary<Type, ContextMenuStrip>();
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
                         where typeof(PluginBase).IsAssignableFrom(assemblyType)
                         select assemblyType).ToArray();

            foreach (var t in types)
            {
                if (t != typeof(PluginBase))
                {
                    SupportedTypes suppTypes = (SupportedTypes)Attribute.GetCustomAttribute(t, typeof(SupportedTypes));

                    if (suppTypes != null)
                    {
                        foreach (var v in suppTypes.Types)
                        {
                            typeToEditor.Add(v, t);
                        }
                    }
                }
            }
        }
    }
}
