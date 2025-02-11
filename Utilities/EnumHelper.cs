
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace ProductManagement.Utilities
{
    public static class EnumHelper
    {
        public static string GetDescription<T>(T value) where T : struct, IConvertible
        {
            var enumType = value.GetType();
            var field = enumType.GetField(value.ToString());
            if (field == null)
            {
                return Enum.GetName(enumType, value);
            }
            var attributes = field.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes.Length == 0)
            {
                return Enum.GetName(enumType, value);
            }

            return ((DescriptionAttribute)attributes[0]).Description;
        }

        public static string GetDescription<T>(int value) where T : struct, IConvertible
        {
            return GetDescription<T>((T)Enum.Parse(typeof(T), value.ToString()));
        }

    
        public static int GetEnumFromDescription<T>(string description) where T : struct, IConvertible
        {
            // Get the type of the enum
            var enumType = typeof(T);

            // Loop through each field in the enum type
            foreach (var field in enumType.GetFields())
            {
                // Get the Description attribute of the enum field
                var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));

                // If a Description attribute exists and matches the provided description, return the integer value of the enum
                if (attribute != null && attribute.Description == description)
                {
                    // Cast the enum value to an integer and return it
                    return (int)field.GetValue(null);
                }
            }
            // Return -1 or throw an exception if no match is found
            throw new ArgumentException($"No matching enum value found for description: {description}");
        }
    }




}
