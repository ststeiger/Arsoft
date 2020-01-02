
namespace NetCoreTestApplication
{


    public static class EnumExtensions
    {

        public static T[] GetValues<T>()
        {
            T[] a = (T[])System.Enum.GetValues(typeof(T));
            return a;
        }


        public static System.Collections.Generic.KeyValuePair<string, object>[] GetEnumKeyValues<T>()
        {
            System.Type t = typeof(T).GetEnumUnderlyingType();
            T[] a = GetValues<T>();

            System.Collections.Generic.KeyValuePair<string, object>[] ls = new System.Collections.Generic.KeyValuePair<string, object>[a.Length];
            
            for (int i = 0; i < a.Length; ++i)
            {
                string name = a[i].ToString();
                object value = System.Convert.ChangeType(a[i], t);

                ls[i] = new System.Collections.Generic.KeyValuePair<string, object>(name, value);
            } // Next i 

            return ls;
        }


        public static string GetEnumInsert<T>()
        {
            string retValue = null;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            foreach(System.Collections.Generic.KeyValuePair<string, object> kvp in GetEnumKeyValues<T>())
            {
                sb.Append(kvp.Value);
                sb.Append("\t");
                sb.AppendLine(kvp.Key);
            } // Next kvp 

            retValue = sb.ToString();
            sb.Clear();
            sb = null;
            return retValue;
        }


    }


}
