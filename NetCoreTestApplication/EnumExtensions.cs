
namespace NetCoreTestApplication
{


    public static class EnumExtensions
    {

        public static T[] GetValues<T>()
        {
            T[] a = (T[])System.Enum.GetValues(typeof(T));
            return a;
        }


        public static string GetEnumInsert<T>()
        {
            System.Type t = typeof(T).GetEnumUnderlyingType();
            T[] a = GetValues<T>();

            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            for (int i = 0; i < a.Length; ++i)
            {
                string name = a[i].ToString();
                object value = System.Convert.ChangeType(a[i], t);

                sb.AppendLine(value.ToString() + "\t" + name);
            } // Next i 

            string retValue = sb.ToString();
            sb.Clear();
            sb = null;
            return retValue;
        }


    }


}
