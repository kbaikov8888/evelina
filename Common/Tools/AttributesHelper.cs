using DynamicData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Tools;

public static class AttributesHelper
{
    public static T? GetAttribute<T>(this MemberDescriptor descriptor) where T : Attribute
    {
        return descriptor.GetAttributes<T>().FirstOrDefault();
    }

    public static List<T> GetAttributes<T>(this MemberDescriptor descriptor) where T : Attribute
    {
        return descriptor.Attributes.OfType<T>().ToList();
    }

    private static Type? GetType(object? obj)
    {
        return obj is IReflectableType reflectable ? reflectable.GetTypeInfo() : obj?.GetType();
    }

    public static T? GetAttribute<T>(this object? obj) where T : Attribute
    {
        return obj.GetAttributes<T>().FirstOrDefault();
    }

    public static List<T> GetAttributes<T>(this object? obj) where T : Attribute
    {
        return GetType(obj)?.GetCustomAttributes(typeof(T), true) is { } atrCollection ?
            atrCollection.Cast<T>().ToList() :
            new List<T>();
    }

    public static List<(string Description, object? Value)> GetInfos(this object obj)
    {
        var props = obj.GetProperties();

        var res = new List<(string Description, object? value)>();

        foreach (var prop in props)
        {
            var descr = prop.Description;
            if (string.IsNullOrEmpty(descr)) continue;

            var value = prop.GetValue(obj);

            res.Add((descr, value));
        }

        return res;
    }

    public static List<PropertyDescriptor> GetProperties(this object? obj)
    {
        if (obj == null)
            return new List<PropertyDescriptor>();
        var typeConverter = TypeDescriptor.GetConverter(obj);
        if (typeConverter.GetPropertiesSupported())
            return typeConverter.GetProperties(obj)?.Cast<PropertyDescriptor>().ToList() ?? new List<PropertyDescriptor>();
        return obj switch
        {
            ICustomTypeDescriptor customTypeDescriptor => customTypeDescriptor.GetProperties()
                .Cast<PropertyDescriptor>()
                .ToList(),
            ICustomTypeProvider typeProvider => TypeDescriptor.GetProperties(typeProvider.GetCustomType())
                .Cast<PropertyDescriptor>()
                .ToList(),
            _ => TypeDescriptor.GetProperties(obj.GetType()).Cast<PropertyDescriptor>().ToList()
        };
    }
}