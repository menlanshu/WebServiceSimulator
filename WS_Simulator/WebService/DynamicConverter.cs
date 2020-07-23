namespace WebServiceStudio
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Globalization;

    internal class DynamicConverter : TypeConverter
    {
        private static readonly Hashtable converterTable = null;
        private static readonly string typeNotFoundMessage = "ProxyPropertiesType {0} specified in WebServiceStudio.exe.options is not found";

        static DynamicConverter()
        {
            converterTable = new Hashtable
            {
                [typeof(bool)] = Activator.CreateInstance(typeof(BooleanConverter)),
                [typeof(byte)] = Activator.CreateInstance(typeof(ByteConverter)),
                [typeof(sbyte)] = Activator.CreateInstance(typeof(SByteConverter)),
                [typeof(char)] = Activator.CreateInstance(typeof(CharConverter)),
                [typeof(double)] = Activator.CreateInstance(typeof(DoubleConverter)),
                [typeof(string)] = Activator.CreateInstance(typeof(StringConverter)),
                [typeof(int)] = Activator.CreateInstance(typeof(Int32Converter)),
                [typeof(short)] = Activator.CreateInstance(typeof(Int16Converter)),
                [typeof(long)] = Activator.CreateInstance(typeof(Int64Converter)),
                [typeof(float)] = Activator.CreateInstance(typeof(SingleConverter)),
                [typeof(ushort)] = Activator.CreateInstance(typeof(UInt16Converter)),
                [typeof(uint)] = Activator.CreateInstance(typeof(UInt32Converter)),
                [typeof(ulong)] = Activator.CreateInstance(typeof(UInt64Converter)),
                [typeof(object)] = Activator.CreateInstance(typeof(ExpandableObjectConverter)),
                [typeof(void)] = Activator.CreateInstance(typeof(TypeConverter)),
                [typeof(CultureInfo)] = Activator.CreateInstance(typeof(CultureInfoConverter)),
                [typeof(DateTime)] = Activator.CreateInstance(typeof(DateTimeConverter)),
                [typeof(decimal)] = Activator.CreateInstance(typeof(DecimalConverter)),
                [typeof(TimeSpan)] = Activator.CreateInstance(typeof(TimeSpanConverter)),
                [typeof(Guid)] = Activator.CreateInstance(typeof(GuidConverter))
            };
            foreach (CustomHandler handler in Configuration.MasterConfig.TypeConverters)
            {
                Type type = Type.GetType(handler.TypeName);
                if (type == null)
                {
                    //MainForm.ShowMessage(typeof(DynamicConverter), MessageType.Warning, string.Format(typeNotFoundMessage, handler.TypeName));
                }
                else
                {
                    Type type2 = Type.GetType(handler.Handler);
                    if (type2 == null)
                    {
                       //MainForm.ShowMessage(typeof(DynamicConverter), MessageType.Warning, string.Format(typeNotFoundMessage, handler.Handler));
                    }
                    else
                    {
                        converterTable[type] = Activator.CreateInstance(type2);
                    }
                }
            }
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return GetConverter(context).CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return GetConverter(context).CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return GetConverter(context).ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            return GetConverter(context).ConvertTo(context, culture, value, destinationType);
        }

        public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
        {
            return GetConverter(context).CreateInstance(context, propertyValues);
        }

        private static Type GetContainedType(ITypeDescriptorContext context)
        {
            if (context != null)
            {
                if (context.Instance is TreeNodeProperty instance)
                {
                    return instance.Type;
                }
            }
            return null;
        }

        private static TypeConverter GetConverter(ITypeDescriptorContext context)
        {
            object obj2 = null;
            Type containedType = GetContainedType(context);
            if (containedType != null)
            {
                obj2 = converterTable[containedType];
            }
            if (obj2 == null)
            {
                if ((containedType != null) && containedType.IsEnum)
                {
                    obj2 = converterTable[containedType] = new EnumConverter(containedType);
                }
                else
                {
                    obj2 = converterTable[typeof(object)];
                }
            }
            return (obj2 as TypeConverter);
        }

        public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
        {
            return GetConverter(context).GetCreateInstanceSupported(context);
        }

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            return GetConverter(context).GetProperties(context, value, attributes);
        }

        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return GetConverter(context).GetPropertiesSupported(context);
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return GetConverter(context).GetStandardValues(context);
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return GetConverter(context).GetStandardValuesExclusive(context);
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return GetConverter(context).GetStandardValuesSupported(context);
        }

        public static bool IsConverterDefined(Type type)
        {
            return (converterTable[type] != null);
        }

        public override bool IsValid(ITypeDescriptorContext context, object value)
        {
            return GetConverter(context).IsValid(context, value);
        }
    }
}

