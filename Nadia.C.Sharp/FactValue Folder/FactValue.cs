using System;
using System.Collections.Generic;


namespace Nadia.C.Sharp.FactValueFolder
{
    public abstract class FactValue<T>
    {

        public static FactDefiStringValue<T> ParseDefiString(string s)
        {
            return new FactDefiStringValue<T>(s);
        }

        public static FactStringValue<T> Parse(string s)
        {
            return new FactStringValue<T>(s);
        }

        public static FactIntegerValue<T> Parse(int i)
        {
            return new FactIntegerValue<T>(i);
        }

        public static FactDateValue<T> Parse(DateTime cal)
        {
            return new FactDateValue<T>(cal);
        }

        public static FactDoubleValue<T> Parse(double d)
        {
            return new FactDoubleValue<T>(d);
        }


        public static FactBooleanValue<T> Parse(bool b)
        {
            return new FactBooleanValue<T>(b);
        }


        public static FactListValue<T> ParseList(T l)
        {
            return new FactListValue<T>(l);
        }

        public static FactURLValue<T> ParseURL(String url)
        {
            return new FactURLValue<T>(url);
        }

        public static FactHashValue<T> ParseHash(String hash)
        {
            return new FactHashValue<T>(hash);
        }

        public static FactUUIDValue<T> ParseUUID(String uuid)
        {
            return new FactUUIDValue<T>(uuid);
        }


        public abstract FactValueType GetFactValueType();
        public abstract void  SetDefaultValue(T str);
        public abstract T GetValue();
        public abstract T GetDefaultValue();

    }
}
