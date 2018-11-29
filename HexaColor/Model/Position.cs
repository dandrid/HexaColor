using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexaColor.Model
{
    [TypeConverter(typeof(PositionConverter))]
    public class Position
    {
        public Position() { }
        public Position(string data)
        {
            string[] parts = data.Split(';');
            rowCooridnate = int.Parse(parts[0]);
            columnCooridnate = int.Parse(parts[1]);
        }
        public Position(int rowCooridnate, int columnCooridnate)
        {
            this.rowCooridnate = rowCooridnate;
            this.columnCooridnate = columnCooridnate;
        }
        public int rowCooridnate { get; set; }
        public int columnCooridnate { get; set; }
       
        public override string ToString()
        {
            return rowCooridnate + ";" + columnCooridnate;
        }
        
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj is Position)
            {
                var another = (Position)obj;
                return rowCooridnate == another.rowCooridnate && columnCooridnate == another.columnCooridnate;
            }
            return false;
        }

        public override int GetHashCode()
        {
            var hashCode = 484048395;
            hashCode = hashCode * -1521134295 + rowCooridnate.GetHashCode();
            hashCode = hashCode * -1521134295 + columnCooridnate.GetHashCode();
            return hashCode;
        }
    }

    public class PositionConverter : TypeConverter
    {
        // Overrides the CanConvertFrom method of TypeConverter.
        // The ITypeDescriptorContext interface provides the context for the
        // conversion. Typically, this interface is used at design time to 
        // provide information about the design-time container.
        public override bool CanConvertFrom(ITypeDescriptorContext context,
           Type sourceType)
        {

            if (sourceType == typeof(string))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }
        // Overrides the ConvertFrom method of TypeConverter.
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                return new Position(value as string);
            }
            return base.ConvertFrom(context, culture, value);
        }
        // Overrides the ConvertTo method of TypeConverter.
        public override object ConvertTo(ITypeDescriptorContext context,
           CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return ((Position)value).ToString();
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
