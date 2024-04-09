using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;

namespace ContextMenuCustomApp.Service.Menu
{
    public static class EnumItemUtil
    {
        public static IReadOnlyList<EnumItem> GetEnumItems<T>() where T : struct
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
            }

            return Enum.GetValues(typeof(T))
                .Cast<T>()
                .Select(x => new EnumItem
                {
                    Label = x.GetType().GetField(x.ToString()).GetCustomAttributes(false).OfType<DescriptionAttribute>().FirstOrDefault()?.Description ?? x.ToString(), 
                    Value = Convert.ToInt32(x)
                })
                .ToImmutableList();
        }
    }
}