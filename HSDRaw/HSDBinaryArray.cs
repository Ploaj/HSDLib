using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace HSDRaw
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class HSDBinaryArray<T> : ICustomTypeDescriptor
    {
        private readonly Func<int, T> _getter;
        private readonly Action<int, T> _setter;

        public int Length { get; }

        public HSDBinaryArray(
            int length,
            Func<int, T> getter,
            Action<int, T> setter)
        {
            Length = length;
            _getter = getter;
            _setter = setter;
        }

        public T this[int index]
        {
            get
            {
                if ((uint)index >= Length)
                    throw new IndexOutOfRangeException();

                return _getter(index);
            }
            set
            {
                if ((uint)index >= Length)
                    throw new IndexOutOfRangeException();

                _setter(index, value);
            }
        }

        public override string ToString()
        {
            return $"[{Length}]";
        }


        // ICustomTypeDescriptor

        public AttributeCollection GetAttributes()
            => AttributeCollection.Empty;

        public string GetClassName()
            => typeof(HSDBinaryArray<T>).Name;

        public string GetComponentName()
            => null;

        public TypeConverter GetConverter()
            => new ExpandableObjectConverter();

        public EventDescriptor GetDefaultEvent()
            => null;

        public PropertyDescriptor GetDefaultProperty()
            => null;

        public object GetEditor(Type editorBaseType)
            => null;

        public EventDescriptorCollection GetEvents()
            => EventDescriptorCollection.Empty;

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
            => EventDescriptorCollection.Empty;

        public object GetPropertyOwner(PropertyDescriptor pd)
            => this;

        public PropertyDescriptorCollection GetProperties()
            => GetProperties(null);

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            var properties = Enumerable.Range(0, Length)
                .Select(i => new HSDBinaryArrayPropertyDescriptor<T>(this, i))
                .ToArray();

            return new PropertyDescriptorCollection(properties);
        }
    }

    public class HSDBinaryArrayPropertyDescriptor<T> : PropertyDescriptor
    {
        private readonly HSDBinaryArray<T> _array;
        private readonly int _index;

        public HSDBinaryArrayPropertyDescriptor(
            HSDBinaryArray<T> array,
            int index)
            : base($"[{index}]", null)
        {
            _array = array;
            _index = index;
        }

        public override Type ComponentType
            => typeof(HSDBinaryArray<T>);

        public override Type PropertyType
            => typeof(T);

        public override bool IsReadOnly
            => false;


        public override object GetValue(object component)
        {
            return _array[_index];
        }


        public override void SetValue(object component, object value)
        {
            _array[_index] = (T)value;

            OnValueChanged(component, EventArgs.Empty);
        }


        public override bool CanResetValue(object component)
            => false;

        public override void ResetValue(object component)
        {
        }

        public override bool ShouldSerializeValue(object component)
            => false;
    }

}
