using HSDRaw;
using HSDRaw.AirRide.Gr.Data;
using HSDRaw.Common;
using HSDRawViewer.Rendering;
using System.Drawing;
using System.Linq;

namespace HSDRawViewer.GUI.Plugins.AirRide.GrEditors
{
    /// <summary>
    /// 
    /// </summary>
    public class GrPositionEditor
    {
        public AirRideGrDataPositionProxy[] _items { get; set; }

        private readonly KAR_grPositionList _list;
        private readonly HSD_JOBJ _joint;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="joint"></param>
        /// <param name="list"></param>
        public GrPositionEditor(HSD_JOBJ joint, KAR_grPositionList list)
        {
            _list = list;
            _joint = joint;

            if (list == null)
                return;

            if (list.JointIndices != null)
            {
                System.Collections.Generic.List<HSD_JOBJ> joints = joint.TreeList;
                _items = list.JointIndices.Array.Select(e => new AirRideGrDataPositionProxy(joints[e])).ToArray();
            }
            else
            if (list.PositionData != null)
            {
                _items = list.PositionData.Array.Select(e => new AirRideGrDataPositionProxy(e)).ToArray();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Update()
        {
            _list.Count = _items.Length;
            _list.JointIndices = null;
            _list.PositionData = null;

            foreach (AirRideGrDataPositionProxy v in _items)
            {
                // joint index
                if (v._joint != null)
                {
                    if (_list.JointIndices == null)
                        _list.JointIndices = new HSDIntArray() { _s = new HSDStruct() };

                    int index = _joint.TreeList.IndexOf(v._joint);

                    if (index == -1)
                    {
                        _joint.AddChild(v._joint);
                        _list.JointIndices.Add(_joint.TreeList.Count - 1);
                    }
                    else
                    {
                        _list.JointIndices.Add(index);
                    }
                }
                else
                // position data
                if (v._data != null)
                {
                    if (_list.PositionData == null)
                        _list.PositionData = new HSDArrayAccessor<KAR_grPositionData>();

                    _list.PositionData.Add(v._data);
                }
            }

            return;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selected"></param>
        public void Render(object selected)
        {
            if (_items == null)
                return;

            foreach (AirRideGrDataPositionProxy p in _items)
            {
                DrawShape.DrawBox(selected != null && p == selected ? Color.Yellow : Color.Red, p.X, p.Y, p.Z, 1f);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public class GrAreaPositionEditor
        {
            public AirRideGrDataAreaPositionProxy[] _items { get; set; }

            private readonly KAR_grAreaPositionList _list;
            private readonly HSD_JOBJ _joint;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="joint"></param>
            /// <param name="list"></param>
            public GrAreaPositionEditor(HSD_JOBJ joint, KAR_grAreaPositionList list)
            {
                _list = list;
                _joint = joint;

                if (list == null)
                    return;

                if (list.JointIndices != null)
                {
                    System.Collections.Generic.List<HSD_JOBJ> joints = joint.TreeList;
                    _items = list.JointIndices.Array.Select(e => new AirRideGrDataAreaPositionProxy(joints[e])).ToArray();
                }
                else
                if (list.AreaPosition != null)
                {
                    _items = list.AreaPosition.Array.Select(e => new AirRideGrDataAreaPositionProxy(e)).ToArray();
                }
            }

            /// <summary>
            /// 
            /// </summary>
            public void Update()
            {
                _list.Count = _items.Length;
                _list.JointIndices = null;
                _list.AreaPosition = null;

                foreach (AirRideGrDataAreaPositionProxy v in _items)
                {
                    // joint index
                    if (v._joint != null)
                    {
                        if (_list.JointIndices == null)
                            _list.JointIndices = new HSDIntArray() { _s = new HSDStruct() };

                        int index = _joint.TreeList.IndexOf(v._joint);

                        if (index == -1)
                        {
                            _joint.AddChild(v._joint);
                            _list.JointIndices.Add(_joint.TreeList.Count - 1);
                        }
                        else
                        {
                            _list.JointIndices.Add(index);
                        }
                    }
                    else
                    // position data
                    if (v._areadata != null)
                    {
                        if (_list.AreaPosition == null)
                            _list.AreaPosition = new HSDArrayAccessor<KAR_grAreaPositionData>();

                        _list.AreaPosition.Add(v._areadata);
                    }
                }

                return;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="selected"></param>
            public void Render(object selected)
            {
                if (_items == null)
                    return;

                foreach (AirRideGrDataAreaPositionProxy p in _items)
                {
                    DrawShape.DrawBox(selected != null && p == selected ? Color.Yellow : Color.Red, p.X, p.Y, p.Z, 1f);
                }
            }

        }
    }
}