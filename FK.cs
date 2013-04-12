using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MySqlNEaT
{
    public class FK : IEquatable<FK>
    {
        public string myTable { get; protected set; }
        public string myColumn { get; protected set; }
        public string refTable { get; protected set; }
        public string refColumn { get; protected set; }

        public FK(
            string myTable, string myColumn,
            string refTable, string refColumn,
            string displayColumn)
        {
            this.myTable = myTable;
            this.myColumn = myColumn;
            this.refTable = refTable;
            this.refColumn = refColumn;
        }

        public bool Equals(FK other)
        {
            if (other == null) return false;

            return this.myColumn == other.myColumn
                && this.myTable == other.myTable
                && this.refColumn == other.refColumn
                && this.refTable == other.refTable;
        }

        public override bool Equals(Object obj)
        {
            if (obj == null)
                return false;

            FK FKObj = obj as FK;
            if (FKObj == null)
                return false;
            else
                return Equals(FKObj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
