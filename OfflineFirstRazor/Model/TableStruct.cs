using Factory.DB;
using Factory.Infra;

namespace Model
{
    [SqlTable("tbl_table_structure")]

    public class TableStruct
    {
        [SqlPrimaryKey]
        [SqlAutoIncrement]
        [SqlProperty("id", DataType.INT)]
        public int Id { get; set; }

        [SqlProperty("tableColumnName", DataType.TEXT)]
        public string TableColumnName { get { return Utilities.GetValidColumnName(SourceColumnName); } set { } }

        [SqlProperty("sourceColumnName", DataType.TEXT)]
        public string SourceColumnName { get; set; }

        [SqlProperty("colDataType", DataType.TEXT)]
        public string ColumnDataType { get; set; }

        [SqlProperty("colDataTypeLength", DataType.INT)]
        public int ColumnDataTypeLength { get; set; }

        [SqlProperty("loc", DataType.INT)]
        public int Location { get; set; }

        [SqlProperty("tableAlias", DataType.TEXT)]
        public string Source { get; set; }

        public TableStruct()
        {
        }

        public TableStruct(string sourceColumnName, int loc, string source)
        {
            Id = 0;
            SourceColumnName = sourceColumnName;
            Location = loc;
            Source = source;

            if (sourceColumnName.StartsWith(StructColumnPrivacy.ABMB1.Name, StringComparison.OrdinalIgnoreCase))
            {
                ColumnDataType = StructColumnPrivacy.ABMB1.Name;
            }
            else if (sourceColumnName.StartsWith(StructColumnPrivacy.ABMB2.Name, StringComparison.OrdinalIgnoreCase))
            {
                ColumnDataType = StructColumnPrivacy.ABMB2.Name;
            }
            else if (sourceColumnName.StartsWith(StructColumnPrivacy.TNG.Name, StringComparison.OrdinalIgnoreCase))
            {
                ColumnDataType = StructColumnPrivacy.TNG.Name;
            }

        }
    }
    public struct StructColumnPrivacy
    {
        public static StructColumnType ABMB1 = new StructColumnType("ABMB1", enumPrivacy.Abmb);
        public static StructColumnType ABMB2 = new StructColumnType("ABMB2", enumPrivacy.Common);
        public static StructColumnType TNG = new StructColumnType("tng", enumPrivacy.Common);
    }

    public struct StructColumnType
    {
        public string Name { get; set; }
        public enumPrivacy Privacy { get; set; }

        public StructColumnType(string name, enumPrivacy privacy)
        {
            Name = name;
            Privacy = privacy;
        }
    }

    public enum enumPrivacy
    {
        Common,
        Abmb,
        Tng
    }
    //public struct ColumnPrefix
    //{
    //    public const string ABMB = "ABMB";
    //    public const string ABMBTNG = "ABMBTNG";
    //    public const string TNG = "TNG";
    //}
}
