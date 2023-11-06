using Factory.DB;

namespace Model
{
    [SqlTable("tbl_source")]
    internal class TableSource
    {
        [SqlPrimaryKey]
        [SqlAutoIncrement]
        [SqlProperty("id", DataType.INT)]
        public int Id { get; set; }

        [SqlProperty("source", DataType.TEXT)]
        public string Source { get; set; }

        [SqlProperty("sourceType", DataType.TEXT)]
        public string SourceType { get; set; }

        public TableSource(string source, string sourceType)
        {
            Source = source;
            SourceType = sourceType;
        }

        public TableSource()
        {
        }
    }

    [SqlTable("tbl_idx_merge")]
    internal class TableIdxMerge
    {
        [SqlPrimaryKey]
        [SqlAutoIncrement]
        [SqlProperty("id", DataType.INT)]
        public int Id { get; set; }

        [SqlProperty("leftId", DataType.INT)]
        public string LeftId { get; set; }

        [SqlProperty("rightId", DataType.INT)]
        public string RightId { get; set; }

        [SqlProperty("tableAlias", DataType.TEXT)]
        public string TableAlis { get; set; }

    }

}
