
using System.Reflection;

namespace ArsoftTestServer.Data 
{

    public delegate object Getter_t<T>(T obj);
    public delegate void Setter_t<T>(T obj, object value);

    internal class LinqHelper
    {


        private static object FlexibleChangeType(object objVal, System.Type targetType)
        {
            System.Reflection.TypeInfo ti = System.Reflection.IntrospectionExtensions.GetTypeInfo(targetType);
            bool typeIsNullable = (ti.IsGenericType && object.ReferenceEquals(targetType.GetGenericTypeDefinition(), typeof(System.Nullable<>)));
            bool typeCanBeAssignedNull = !ti.IsValueType || typeIsNullable;

            if (objVal == null || object.ReferenceEquals(objVal, System.DBNull.Value))
            {
                if (typeCanBeAssignedNull)
                    return null;
                else
                    throw new System.ArgumentNullException("objVal ([DataSource] => SetProperty => FlexibleChangeType => you're trying to assign NULL to a type that NULL cannot be assigned to...)");
            } // End if (objVal == null || object.ReferenceEquals(objVal, System.DBNull.Value))

            // Get base-type
            System.Type tThisType = objVal.GetType();

            if (typeIsNullable)
            {
                targetType = System.Nullable.GetUnderlyingType(targetType);
            } // End if (typeIsNullable) 


            if (object.ReferenceEquals(tThisType, targetType))
                return objVal;

            // Convert Guid => string
            if (object.ReferenceEquals(targetType, typeof(string)) && object.ReferenceEquals(tThisType, typeof(System.Guid)))
            {
                return objVal.ToString();
            } // End if (object.ReferenceEquals(targetType, typeof(string)) && object.ReferenceEquals(tThisType, typeof(System.Guid)))

            // Convert string => System.Net.IPAddress
            if (object.ReferenceEquals(targetType, typeof(System.Net.IPAddress)) && object.ReferenceEquals(tThisType, typeof(string)))
            {
                return System.Net.IPAddress.Parse(objVal.ToString());
            } // End if (object.ReferenceEquals(targetType, typeof(System.Net.IPAddress)) && object.ReferenceEquals(tThisType, typeof(string)))

            // Convert string => TimeSpan
            if (object.ReferenceEquals(targetType, typeof(System.TimeSpan)) && object.ReferenceEquals(tThisType, typeof(string)))
            {
                // https://stackoverflow.com/questions/11719055/why-does-timespan-parseexact-not-work
                // This is grotesque... ParseExact ignores the 12/24 hour convention...
                // return System.TimeSpan.ParseExact(objVal.ToString(), "HH':'mm':'ss", System.Globalization.CultureInfo.InvariantCulture); // Exception 
                // return System.TimeSpan.ParseExact(objVal.ToString(), "hh\\:mm\\:ss", System.Globalization.CultureInfo.InvariantCulture); // This works, bc of lowercase ?
                // return System.TimeSpan.ParseExact(objVal.ToString(), "hh':'mm':'ss", System.Globalization.CultureInfo.InvariantCulture); // Yep, lowercase - no 24 notation...
                return System.TimeSpan.Parse(objVal.ToString());
            } // End if (object.ReferenceEquals(targetType, typeof(System.TimeSpan)) && object.ReferenceEquals(tThisType, typeof(string))) 

            // Convert string => DateTime
            if (object.ReferenceEquals(targetType, typeof(System.DateTime)) && object.ReferenceEquals(tThisType, typeof(string)))
            {
                return System.DateTime.Parse(objVal.ToString(), System.Globalization.CultureInfo.InvariantCulture);
            } // End if (object.ReferenceEquals(targetType, typeof(System.DateTime)) && object.ReferenceEquals(tThisType, typeof(string)))

            // Convert string => Guid 
            if (object.ReferenceEquals(targetType, typeof(System.Guid)) && object.ReferenceEquals(tThisType, typeof(string)))
            {
                return new System.Guid(objVal.ToString());
            } // End else if (object.ReferenceEquals(targetType, typeof(System.Guid)) && object.ReferenceEquals(tThisType, typeof(string))) 

            return System.Convert.ChangeType(objVal, targetType);
        } // End Function FlexibleChangeType


        public static Setter_t<T> GetSetter<T>(string fieldName)
        {
            System.Type t = typeof(T);

            System.Reflection.FieldInfo fi = t.GetField(fieldName, System.Reflection.BindingFlags.IgnoreCase
                | System.Reflection.BindingFlags.Public
                | System.Reflection.BindingFlags.Instance
            );

            if (fi != null)
                return delegate(T obj, object value)
                {
                    fi.SetValue(obj, FlexibleChangeType(value, fi.FieldType));
                };



            System.Reflection.PropertyInfo pi = t.GetProperty(fieldName, System.Reflection.BindingFlags.IgnoreCase
                | System.Reflection.BindingFlags.Public
                | System.Reflection.BindingFlags.Instance
            );

            if (pi != null)
                return delegate(T obj, object value)
                {
                    pi.SetValue(obj, FlexibleChangeType(value, pi.PropertyType), null);
                };

            return null;
        } // End Function GetSetter 


        public static Getter_t<T> GetGetter<T>(string fieldName)
        {
            System.Type t = typeof(T);

          
            System.Reflection.FieldInfo fi = t.GetField(fieldName, System.Reflection.BindingFlags.IgnoreCase
                | System.Reflection.BindingFlags.Public
                | System.Reflection.BindingFlags.Instance
            );


            if (fi != null)
                return delegate(T obj)
                {
                    return fi.GetValue(obj);
                };


            System.Reflection.PropertyInfo pi = t.GetProperty(fieldName, System.Reflection.BindingFlags.IgnoreCase
                  | System.Reflection.BindingFlags.Public
                  | System.Reflection.BindingFlags.Instance
              );

            if (pi != null)
                return delegate(T obj)
                {
                    return pi.GetValue(obj, null);
                };

            return null;
        } // End Function GetGetter 



        // ------------------------------------



        public static System.Reflection.MemberInfo[] GetFieldsAndProperties(System.Type t)
        {
            System.Reflection.TypeInfo ti = System.Reflection.IntrospectionExtensions.GetTypeInfo(t);

            System.Reflection.FieldInfo[] fis = ti.GetFields();
            System.Reflection.PropertyInfo[] pis = ti.GetProperties();
            System.Reflection.MemberInfo[] mis = new System.Reflection.MemberInfo[fis.Length + pis.Length];
            System.Array.Copy(fis, mis, fis.Length);
            System.Array.Copy(pis, 0, mis, fis.Length, pis.Length);

            return mis;
        } // End Function GetFieldsAndProperties 


        public static Getter_t<T>[] GetGetters<T>()
        {
            System.Reflection.MemberInfo[] mis = GetFieldsAndProperties(typeof(T));

            string[] memberNames = new string[mis.Length];
            for (int i = 0; i < mis.Length; ++i)
            {
                memberNames[i] = mis[i].Name;
            } // Next i 

            return GetGetters<T>(memberNames);
        } // End Function GetGetters 


        public static Getter_t<T>[] GetGetters<T>(string[] fieldNames)
        {
            Getter_t<T>[] iisLogGetters = new Getter_t<T>[fieldNames.Length];

            for (int i = 0; i < fieldNames.Length; ++i)
            {
                iisLogGetters[i] = GetGetter<T>(fieldNames[i]);
            } // Next i 

            return iisLogGetters;
        } // End Function GetGetters 


        public static Setter_t<T>[] GetSetters<T>()
        {
            System.Reflection.MemberInfo[] mis = GetFieldsAndProperties(typeof(T));

            string[] memberNames = new string[mis.Length];
            for (int i = 0; i < mis.Length; ++i)
            {
                memberNames[i] = mis[i].Name;
            } // Next i 

            return GetSetters<T>(memberNames);
        } // End Function GetSetters 


        public static Setter_t<T>[] GetSetters<T>(string[] fieldNames)
        {
            Setter_t<T>[] iisLogSetters = new Setter_t<T>[fieldNames.Length];

            for (int i = 0; i < fieldNames.Length; ++i)
            {
                iisLogSetters[i] = GetSetter<T>(fieldNames[i]);
            } // Next i 

            return iisLogSetters;
        } // End Function GetSetters 


    } // End Class LinqHelper 


} // End Namespace LinkedServerSync 
