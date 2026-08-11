using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace HSDRawViewer.GUI.PropertyGrid
{
    public class ListConverter<T> : CollectionConverter
    {
        public override PropertyDescriptorCollection GetProperties(
            ITypeDescriptorContext context,
            object value,
            Attribute[] attributes)
        {
            if (value is not IList<T> list)
                return base.GetProperties(context, value, attributes);

            var properties = new List<PropertyDescriptor>();

            for (int i = 0; i < list.Count; i++)
            {
                properties.Add(new ListItemPropertyDescriptor<T>(list, i));
            }

            return new PropertyDescriptorCollection(properties.ToArray());
        }

        public override bool GetPropertiesSupported(
            ITypeDescriptorContext context)
        {
            return true;
        }
    }

    public class ListItemPropertyDescriptor<T> : PropertyDescriptor
    {
        private readonly IList<T> _list;
        private readonly int _index;

        public ListItemPropertyDescriptor(IList<T> list, int index)
            : base($"[{index}]", null)
        {
            _list = list;
            _index = index;
        }

        public override Type ComponentType => typeof(IList<T>);

        public override Type PropertyType => typeof(T);

        public override bool IsReadOnly => false;

        public override bool CanResetValue(object component) => false;

        public override object GetValue(object component)
            => _list[_index];

        public override void SetValue(object component, object value)
        {
            _list[_index] = (T)value;
            OnValueChanged(component, EventArgs.Empty);
        }

        public override void ResetValue(object component) { }

        public override bool ShouldSerializeValue(object component) => false;
    }
}
