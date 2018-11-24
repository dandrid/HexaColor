using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexaColor.Model
{
    public class Position
    {
        public Position(int rowCooridnate, int columnCooridnate)
        {
            this.rowCooridnate = rowCooridnate;
            this.columnCooridnate = columnCooridnate;
        }
        public readonly int rowCooridnate;
        public readonly int columnCooridnate;

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
}
