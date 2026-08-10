using System.ArrayExtensions;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace System
{
    public static class ObjectExtensions
    {
        private static readonly MethodInfo CloneMethod = typeof(Object).GetMethod("MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance);

        public static bool IsPrimitive(this Type type)
        {
            if (type == typeof(String)) return true;
            return (type.IsValueType & type.IsPrimitive);
        }

        public static Object Copy(this Object originalObject)
        {
            return InternalCopy(originalObject, new Dictionary<Object, Object>(new ReferenceEqualityComparer()));
        }
        private static Object InternalCopy(Object originalObject, IDictionary<Object, Object> visited)
        {
            if (originalObject == null) return null;
            Type typeToReflect = originalObject.GetType();
            if (IsPrimitive(typeToReflect)) return originalObject;
            if (visited.ContainsKey(originalObject)) return visited[originalObject];
            if (typeof(Delegate).IsAssignableFrom(typeToReflect)) return null;
            object cloneObject = CloneMethod.Invoke(originalObject, null);
            if (typeToReflect.IsArray)
            {
                Type arrayType = typeToReflect.GetElementType();
                if (IsPrimitive(arrayType) == false)
                {
                    Array clonedArray = (Array)cloneObject;
                    clonedArray.ForEach((array, indices) => array.SetValue(InternalCopy(clonedArray.GetValue(indices), visited), indices));
                }

            }
            visited.Add(originalObject, cloneObject);
            CopyFields(originalObject, visited, cloneObject, typeToReflect);
            RecursiveCopyBaseTypePrivateFields(originalObject, visited, cloneObject, typeToReflect);
            return cloneObject;
        }

        private static void RecursiveCopyBaseTypePrivateFields(object originalObject, IDictionary<object, object> visited, object cloneObject, Type typeToReflect)
        {
            if (typeToReflect.BaseType != null)
            {
                RecursiveCopyBaseTypePrivateFields(originalObject, visited, cloneObject, typeToReflect.BaseType);
                CopyFields(originalObject, visited, cloneObject, typeToReflect.BaseType, BindingFlags.Instance | BindingFlags.NonPublic, info => info.IsPrivate);
            }
        }

        private static void CopyFields(object originalObject, IDictionary<object, object> visited, object cloneObject, Type typeToReflect, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy, Func<FieldInfo, bool> filter = null)
        {
            foreach (FieldInfo fieldInfo in typeToReflect.GetFields(bindingFlags))
            {
                if (filter != null && filter(fieldInfo) == false) continue;
                if (IsPrimitive(fieldInfo.FieldType)) continue;
                object originalFieldValue = fieldInfo.GetValue(originalObject);
                object clonedFieldValue = InternalCopy(originalFieldValue, visited);
                fieldInfo.SetValue(cloneObject, clonedFieldValue);
            }
        }
        public static T Copy<T>(this T original)
        {
            return (T)Copy((Object)original);
        }

        public static void CopyTo<T>(this T source, T destination)
        {
            if (source == null || destination == null)
                return;

            InternalCopyTo(source, destination, new HashSet<(object, object)>(new ReferencePairComparer()));
        }

        private static void InternalCopyTo(
            object source,
            object destination,
            ISet<(object, object)> visited)
        {
            if (source == null || destination == null)
                return;

            Type type = source.GetType();

            if (type != destination.GetType())
                throw new ArgumentException("Source and destination types must match.");

            if (IsPrimitive(type))
                return;

            if (!visited.Add((source, destination)))
                return;

            CopyFieldsTo(
                source,
                destination,
                type,
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy,
                null,
                visited);

            RecursiveCopyBaseTypePrivateFieldsTo(
                source,
                destination,
                type,
                visited);
        }

        private static void RecursiveCopyBaseTypePrivateFieldsTo(
            object source,
            object destination,
            Type type,
            ISet<(object, object)> visited)
        {
            if (type.BaseType == null)
                return;

            RecursiveCopyBaseTypePrivateFieldsTo(
                source,
                destination,
                type.BaseType,
                visited);

            CopyFieldsTo(
                source,
                destination,
                type.BaseType,
                BindingFlags.Instance | BindingFlags.NonPublic,
                f => f.IsPrivate,
                visited);
        }

        private static void CopyFieldsTo(
            object source,
            object destination,
            Type type,
            BindingFlags bindingFlags,
            Func<FieldInfo, bool> filter,
            ISet<(object, object)> visited)
        {
            foreach (FieldInfo field in type.GetFields(bindingFlags))
            {
                if (filter != null && !filter(field))
                    continue;

                if (IsPrimitive(field.FieldType))
                {
                    field.SetValue(destination, field.GetValue(source));
                    continue;
                }

                object sourceValue = field.GetValue(source);
                object destinationValue = field.GetValue(destination);

                if (sourceValue == null)
                {
                    field.SetValue(destination, null);
                }
                else if (destinationValue == null)
                {
                    // Destination doesn't have an existing object to mutate,
                    // so create a deep copy.
                    field.SetValue(
                        destination,
                        InternalCopy(sourceValue, new Dictionary<object, object>(
                            new ReferenceEqualityComparer())));
                }
                else if (sourceValue.GetType().IsValueType)
                {
                    // Structs need to be copied as a whole.
                    field.SetValue(destination, sourceValue);
                }
                else if (sourceValue is Array sourceArray &&
                         destinationValue is Array destinationArray)
                {
                    CopyArrayTo(sourceArray, destinationArray, visited);
                }
                else
                {
                    InternalCopyTo(sourceValue, destinationValue, visited);
                }
            }
        }

        private static void CopyArrayTo(
            Array source,
            Array destination,
            ISet<(object, object)> visited)
        {
            if (source.Length != destination.Length ||
                source.Rank != destination.Rank)
            {
                // Can't resize an existing array, so replace it.
                return;
            }

            foreach (int[] indices in GetArrayIndices(source))
            {
                object sourceValue = source.GetValue(indices);
                object destinationValue = destination.GetValue(indices);

                if (sourceValue == null)
                {
                    destination.SetValue(null, indices);
                }
                else if (IsPrimitive(sourceValue.GetType()))
                {
                    destination.SetValue(sourceValue, indices);
                }
                else if (destinationValue != null &&
                         sourceValue.GetType() == destinationValue.GetType())
                {
                    InternalCopyTo(sourceValue, destinationValue, visited);
                }
                else
                {
                    destination.SetValue(
                        InternalCopy(
                            sourceValue,
                            new Dictionary<object, object>(
                                new ReferenceEqualityComparer())),
                        indices);
                }
            }
        }

        private static IEnumerable<int[]> GetArrayIndices(Array array)
        {
            int[] lengths = new int[array.Rank];
            int[] indices = new int[array.Rank];

            for (int i = 0; i < array.Rank; i++)
                lengths[i] = array.GetLength(i);

            while (true)
            {
                yield return (int[])indices.Clone();

                int dimension = array.Rank - 1;

                while (dimension >= 0)
                {
                    indices[dimension]++;

                    if (indices[dimension] < lengths[dimension])
                        break;

                    indices[dimension] = 0;
                    dimension--;
                }

                if (dimension < 0)
                    break;
            }
        }

        private sealed class ReferencePairComparer : IEqualityComparer<(object, object)>
        {
            public bool Equals((object, object) x, (object, object) y)
            {
                return ReferenceEquals(x.Item1, y.Item1) &&
                       ReferenceEquals(x.Item2, y.Item2);
            }

            public int GetHashCode((object, object) obj)
            {
                return HashCode.Combine(
                    RuntimeHelpers.GetHashCode(obj.Item1),
                    RuntimeHelpers.GetHashCode(obj.Item2));
            }
        }
    }

    public class ReferenceEqualityComparer : EqualityComparer<Object>
    {
        public override bool Equals(object x, object y)
        {
            return ReferenceEquals(x, y);
        }
        public override int GetHashCode(object obj)
        {
            if (obj == null) return 0;
            return obj.GetHashCode();
        }
    }

    namespace ArrayExtensions
    {
        public static class ArrayExtensions
        {
            public static void ForEach(this Array array, Action<Array, int[]> action)
            {
                if (array.LongLength == 0) return;
                ArrayTraverse walker = new(array);
                do action(array, walker.Position);
                while (walker.Step());
            }
        }

        internal class ArrayTraverse
        {
            public int[] Position;
            private readonly int[] maxLengths;

            public ArrayTraverse(Array array)
            {
                maxLengths = new int[array.Rank];
                for (int i = 0; i < array.Rank; ++i)
                {
                    maxLengths[i] = array.GetLength(i) - 1;
                }
                Position = new int[array.Rank];
            }

            public bool Step()
            {
                for (int i = 0; i < Position.Length; ++i)
                {
                    if (Position[i] < maxLengths[i])
                    {
                        Position[i]++;
                        for (int j = 0; j < i; j++)
                        {
                            Position[j] = 0;
                        }
                        return true;
                    }
                }
                return false;
            }
        }
    }

}