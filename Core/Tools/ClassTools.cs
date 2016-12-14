using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Microsoft.CSharp.RuntimeBinder;
using Omu.ValueInjecter;
using Omu.ValueInjecter.Utils;

namespace Core.Tools
{
    public static class ClassTools
    {
        public static object Create(string typeName)
        {
            return Create(typeName, null);
        }

        public static object Create(string typeName, object[] arguments)
        {
            var type = Type.GetType(typeName);
            return arguments != null ? Activator.CreateInstance(type, arguments) : Activator.CreateInstance(type);
        }

        public static T CopyToNew<T>(this object source, params object[] additionalSources) where T : new()
        {
            var sources = new List<object> { source };
            sources.AddRange(additionalSources);
            return Create<T>(sources.ToArray());
        }

        public static T Create<T>(params object[] source) where T : new()
        {
            var t = new T();
            foreach (var item in source)
                t.InjectFrom(item);
            return t;
        }

        #region fields

        public static T GetFieldValue<T>(this object obj, string fieldName)
        {
            AssertTools.Assert(obj != null, "Object cannot be null.");
            var type = obj.GetType();
            var field = GetFieldInfo(type, fieldName);
            AssertTools.Assert(field != null, String.Format("Field {0} not found on type {1}.", fieldName, type.FullName));
            return (T)field.GetValue(obj);
        }

        public static void SetFieldValue<T>(this object obj, string fieldName, T value)
        {
            AssertTools.Assert(obj != null, "Object cannot be null.");
            var type = obj.GetType();
            var field = GetFieldInfo(type, fieldName);
            AssertTools.Assert(field != null, String.Format("Field {0} not found on type {1}.", fieldName, type.FullName));
            field.SetValue(obj, value);
        }

        private static FieldInfo GetFieldInfo(Type type, string fieldName, bool searchInBaseTypes = true)
        {
            var fieldInfo = type.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (fieldInfo != null) return fieldInfo;
            return searchInBaseTypes && type.BaseType != null
                ? GetFieldInfo(type.BaseType, fieldName)
                : null;
        }

        #endregion

        #region properties

        public static T GetProperyValue<T>(this object obj, string propertyName)
        {
            AssertTools.Assert(obj != null, "Object cannot be null.");
            var type = obj.GetType();
            var property = GetPropertyInfo(type, propertyName);
            AssertTools.Assert(property != null, String.Format("Property {0} not found on type {1}.", propertyName, type.FullName));
            return (T)property.GetValue(obj, null);
        }

        public static void SetProperyValue<T>(this object obj, string propertyName, T value)
        {
            AssertTools.Assert(obj != null, "Object cannot be null.");
            var type = obj.GetType();
            var property = GetPropertyInfo(type, propertyName);
            AssertTools.Assert(property != null, String.Format("Property {0} not found on type {1}.", propertyName, type.FullName));
            property.SetValue(obj, value, null);
        }

        public static PropertyInfo GetPropertyInfo(Type type, string propertyName, bool searchInBaseTypes = true, bool instance = true)
        {
            var flags = BindingFlags.Public | BindingFlags.NonPublic;
            flags |= instance ? BindingFlags.Instance : BindingFlags.Static;
            var propertyInfo = type.GetProperty(propertyName, flags);
            if (propertyInfo != null) return propertyInfo;
            return searchInBaseTypes && type.BaseType != null
                ? GetPropertyInfo(type.BaseType, propertyName)
                : null;
        }

        public static string GetPropertyName<T>(Expression<Func<T, object>> expression)
        {
            var body = expression.Body as MemberExpression ??
                       ((UnaryExpression)expression.Body).Operand as MemberExpression;
            return body.Member.Name;
        }

        public static T GetStaticProperyValue<T>(this Type type, string propertyName)
        {
            AssertTools.Assert(type != null, "Type cannot be null.");
            var property = GetPropertyInfo(type, propertyName, true, false);
            AssertTools.Assert(property != null, String.Format("Static property {0} not found on type {1}.", propertyName, type.FullName));
            return (T)property.GetValue(null, null);
        }
        #endregion

        #region fields and properties

        public static void SetFieldOrPropertyValue<T>(this object obj, string fieldOrPropertyName, T value)
        {
            AssertTools.Assert(obj != null, "Object cannot be null.");
            var type = obj.GetType();
            var field = GetFieldInfo(type, fieldOrPropertyName);
            if (field != null)
            {
                field.SetValue(obj, value);
                return;
            }
            var property = GetPropertyInfo(type, fieldOrPropertyName);
            if (property != null)
            {
                property.SetValue(obj, value, null);
                return;
            }
            throw new Exception(string.Format("Field or property {0} not found on type {1}.", fieldOrPropertyName, type.FullName));
        }

        public static Dictionary<string, object> GetFieldsAndProperties(this object obj)
        {
            return obj.GetProps().ToDictionary(prop => prop.Name, prop => prop.GetValue(obj, null));
        }

        public static void PopulateIntoFieldsAndProperties(object obj, Dictionary<string, object> data)
        {
            foreach (var keyValuePair in data)
                obj.SetFieldOrPropertyValue(keyValuePair.Key, keyValuePair.Value);
        }

        public static T PopulateIntoFieldsAndProperties<T>(Dictionary<string, object> data) where T : new()
        {
            var obj = new T();
            PopulateIntoFieldsAndProperties(obj, data);
            return obj;
        }

        #endregion

        #region methods

        public static void BaseMethodExecute(this object obj, string methodName, object[] parameters = null, Type[] types = null)
        {
            AssertTools.Assert(obj != null, "Object cannot be null.");
            var baseType = obj.GetType().BaseType;
            AssertTools.Assert(baseType != null, String.Format("Type {0} has no basetype.", obj.GetType().FullName));
            var method = GetMethodInfo(baseType, methodName, types);
            AssertTools.Assert(method != null, String.Format("Method {0} not found on type {1}.", methodName, baseType.FullName));
            method.Invoke(obj, parameters);
        }

        public static T BaseMethodExecute<T>(this object obj, string methodName, object[] parameters = null, Type[] types = null)
        {
            AssertTools.Assert(obj != null, "Object cannot be null.");
            var baseType = obj.GetType().BaseType;
            AssertTools.Assert(baseType != null, String.Format("Type {0} has no basetype.", obj.GetType().FullName));
            var method = GetMethodInfo(baseType, methodName, types);
            AssertTools.Assert(method != null, String.Format("Method {0} not found on type {1}.", methodName, baseType.FullName));
            return (T)method.Invoke(obj, parameters);
        }

        private static MethodInfo GetMethodInfo(Type type, string methodName, Type[] types, bool searchInBaseTypes = true)
        {
            var propertyInfo = types == null
                ? type.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                : type.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, Type.DefaultBinder, types, null);
            if (propertyInfo != null) return propertyInfo;
            return searchInBaseTypes && type.BaseType != null
                ? GetMethodInfo(type.BaseType, methodName, types)
                : null;
        }

        public static T StaticBaseMethodExecute<T>(Type type, string methodName, object[] parameters, Type[] types = null)
        {
            var basetype = type.BaseType;
            AssertTools.Assert(basetype != null, String.Format("Type {0} has no basetype.", type.FullName));
            var method = GetStaticMethodInfo(basetype, methodName, types);
            AssertTools.Assert(method != null, String.Format("Static method {0} not found on type {1}.", methodName, type.FullName));
            return (T)method.Invoke(null, parameters);
        }

        private static MethodInfo GetStaticMethodInfo(Type type, string methodName, Type[] types, bool searchInBaseTypes = true)
        {
            var propertyInfo = types == null
                ? type.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                : type.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static, Type.DefaultBinder, types, null);
            if (propertyInfo != null) return propertyInfo;
            return searchInBaseTypes && type.BaseType != null
                ? GetStaticMethodInfo(type.BaseType, methodName, types)
                : null;
        }

        #endregion

        /// <summary>
        /// Supports System.Nullable
        /// </summary>
        public static object ChangeType(IConvertible obj, Type destinationType)
        {
            if (destinationType.IsGenericType && (destinationType.GetGenericTypeDefinition() == typeof(Nullable<>)))
            {
                if (obj == null)
                    return null;
                return Convert.ChangeType(obj, Nullable.GetUnderlyingType(destinationType));
            }
            return Convert.ChangeType(obj, destinationType);
        }

        public static bool IsDerivedFromOpenGenericType(this Type type, Type openGenericType)
        {
            Contract.Requires(type != null);
            Contract.Requires(openGenericType != null);
            Contract.Requires(openGenericType.IsGenericTypeDefinition);
            return type.GetTypeHierarchy()
                       .Where(t => t.IsGenericType)
                       .Select(t => t.GetGenericTypeDefinition())
                       .Any(t => openGenericType.Equals(t));
        }

        public static IEnumerable<Type> GetTypeHierarchy(this Type type)
        {
            Contract.Requires(type != null);
            Type currentType = type;
            while (currentType != null)
            {
                yield return currentType;
                currentType = currentType.BaseType;
            }
        }

        public static object GetDefaultValue(this Type t)
        {
            if (!t.IsValueType || Nullable.GetUnderlyingType(t) != null)
                return null;
            return Activator.CreateInstance(t);
        }

        public static TDest GetIfPossible<TSource, TDest>(this TSource source, Func<TSource, TDest> selector)
        {
            try
            {
                return selector.Invoke(source);
            }
            catch (NullReferenceException)
            {
                return default(TDest);
            }
        }

        public static bool IsPrimitive(Type type)
        {
            if (GetPrimitiveTypes().Any(_ => _.IsAssignableFrom(type)))
                return true;

            var nut = Nullable.GetUnderlyingType(type);
            return nut != null && nut.IsEnum;
        }

        private static Type[] primitiveTypesCache;
        private static Type[] GetPrimitiveTypes()
        {
            if (primitiveTypesCache != null) return primitiveTypesCache;
            var types = new[]
            {
                typeof (Enum),
                typeof (String),
                typeof (Char),
                typeof (Guid),

                typeof (Boolean),
                typeof (Byte),
                typeof (Int16),
                typeof (Int32),
                typeof (Int64),
                typeof (Single),
                typeof (Double),
                typeof (Decimal),

                typeof (SByte),
                typeof (UInt16),
                typeof (UInt32),
                typeof (UInt64),

                typeof (DateTime),
                typeof (DateTimeOffset),
                typeof (TimeSpan),
            };

            var nullTypes = from t in types
                            where t.IsValueType
                            select typeof(Nullable<>).MakeGenericType(t);

            primitiveTypesCache = types.Concat(nullTypes).ToArray();
            return primitiveTypesCache;
        }

        #region convertibility

        public static MethodInfo GetMethod<TInstance>(Expression<Action<TInstance>> expr)
        {
            return ((MethodCallExpression)expr.Body).Method;
        }
        public static MethodInfo GetMethod(Expression<Action> expr)
        {
            return ((MethodCallExpression)expr.Body).Method;
        }

        public static bool IsImplicitlyCastableTo(this Type from, Type to)
        {
            AssertTools.Assert(from != null);
            AssertTools.Assert(to != null);

            // not strictly necessary, but speeds things up
            if (to.IsAssignableFrom(from))
            {
                return true;
            }

            try
            {
                // overload of GetMethod() from http://www.codeducky.org/10-utilities-c-developers-should-know-part-two/ 
                // that takes Expression<Action>
                GetMethod(() => AttemptImplicitCast<object, object>())
                    .GetGenericMethodDefinition()
                    .MakeGenericMethod(from, to)
                    .Invoke(null, new object[0]);
                return true;
            }
            catch (TargetInvocationException ex)
            {
                return !(
                    ex.InnerException is RuntimeBinderException
                    // if the code runs in an environment where this message is localized, we could attempt a known failure first and base the regex on it's message
                    && Regex.IsMatch(ex.InnerException.Message, @"^The best overloaded method match for 'System.Collections.Generic.List<.*>.Add(.*)' has some invalid arguments$")
                );
            }
        }

        private static void AttemptImplicitCast<TFrom, TTo>()
        {
            // based on the IL produced by:
            // dynamic list = new List<TTo>();
            // list.Add(default(TFrom));
            // We can't use the above code because it will mimic a cast in a generic method
            // which doesn't have the same semantics as a cast in a non-generic method

            var list = new List<TTo>(capacity: 1);
            var binder = Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(
                flags: CSharpBinderFlags.ResultDiscarded,
                name: "Add",
                typeArguments: null,
                context: typeof(ClassTools), // the current type
                argumentInfo: new[]
                {
                    CSharpArgumentInfo.Create(flags: CSharpArgumentInfoFlags.None, name: null),
                    CSharpArgumentInfo.Create(
                        flags: CSharpArgumentInfoFlags.UseCompileTimeType,
                        name: null
                    )
                }
            );
            var callSite = CallSite<Action<CallSite, object, TFrom>>.Create(binder);
            callSite.Target.Invoke(callSite, list, default(TFrom));
        }

        public static TTo ImplicitCast<TTo>(object value)
        {
            var type = value.GetType();
            var method = GetMethod(() => ImplicitCast<object, object>(null))
                   .GetGenericMethodDefinition()
                   .MakeGenericMethod(type, typeof(TTo));

            var result = method.Invoke(null, new[] { value });
            return (TTo)result;
        }

        public static TTo ImplicitCast<TFrom, TTo>(TFrom value)
        {
            // based on the IL produced by:
            // dynamic list = new List<TTo>();
            // list.Add(default(TFrom));
            // We can't use the above code because it will mimic a cast in a generic method
            // which doesn't have the same semantics as a cast in a non-generic method

            var list = new List<TTo>(capacity: 1);
            var binder = Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(
                flags: CSharpBinderFlags.ResultDiscarded,
                name: "Add",
                typeArguments: null,
                context: typeof(ClassTools), // the current type
                argumentInfo: new[]
                {
                    CSharpArgumentInfo.Create(flags: CSharpArgumentInfoFlags.None, name: null),
                    CSharpArgumentInfo.Create(
                        flags: CSharpArgumentInfoFlags.UseCompileTimeType,
                        name: null
                    )
                }
            );
            var callSite = CallSite<Action<CallSite, object, TFrom>>.Create(binder);
            callSite.Target.Invoke(callSite, list, value);
            return list[0];
        }

        #endregion
    }
}
