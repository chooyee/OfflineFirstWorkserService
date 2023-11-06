
using System.Data;

namespace Model
{
    public struct DBTable
    {
        public string Name { get; set; }
        public List<DBColumn> Columns { get; set; }

        public DBTable()
        {
            Name = "";
            Columns = new List<DBColumn>();
        }

        public DBTable(string tableName)
        {
            Name = tableName;
            Columns = new List<DBColumn>();
        }

        public DBTable(string tableName, List<DBColumn> cols)
        {
            Name = tableName;
            Columns = cols;
        }

        public IEnumerable<string> GetAllColumnName()
        {
            return Columns.Select(x => x.Name);
        }

        public string GetCreateQuery()
        {
            string createTableStatement = $"CREATE TABLE IF NOT EXISTS {Name} (";

            foreach (var col in Columns)
            {
                string columnName = col.Name;
                string dataType = col.DataType;
                bool isNullable = col.IsNullable;
                bool isPrimaryKey = col.Primary;
                bool isAutoincrement = col.AutoIncrement;

                createTableStatement += $"{columnName} {dataType}";

                if (!isNullable)
                {
                    createTableStatement += " NOT NULL";
                }

                if (isPrimaryKey)
                {
                    createTableStatement += " PRIMARY KEY";
                }

                if (isAutoincrement)
                {
                    createTableStatement += " AUTOINCREMENT";
                }

                createTableStatement += ", ";
            }

            // Remove the trailing comma and space
            createTableStatement = createTableStatement.TrimEnd(',', ' ');

            createTableStatement += ");";

            return createTableStatement;
        }
    }

    public struct DBColumn
    {
        public string Name { get; set; }
        public string DataType { get; set; }
        public bool Primary { get; set; }
        public bool IsNullable { get; set; }
        public bool AutoIncrement { get; set; }
    }
}
