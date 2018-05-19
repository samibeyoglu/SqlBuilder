using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using Atlas.Data.SqlBuilder.Enums;

namespace Atlas.Data.SqlBuilder
{
    internal static class Helpers
    {
        private const string DefaultSchema = "dbo";
        private const string DefaultForeignColumnName = "Id";

        internal static ConcurrentDictionary<string, TableMetaData> TableMetaDataList = new ConcurrentDictionary<string, TableMetaData>();

        internal static ConcurrentDictionary<string, ColumnMetaData> ColumnMetaDataList = new ConcurrentDictionary<string, ColumnMetaData>();

        internal static bool IsNumber(this object value)
        {
            return value is sbyte
                   || value is byte
                   || value is short
                   || value is ushort
                   || value is int
                   || value is uint
                   || value is long
                   || value is ulong
                   || value is float
                   || value is double
                   || value is decimal;
        }

        internal static bool IsNull(this object value)
        {
            return value == null;
        }

        internal static string ToSqlComperationOperator(this ComparisonOperator oper, bool isNull)
        {
            switch (oper)
            {
                case ComparisonOperator.Equal:
                    return isNull ? "IS" : "=";
                case ComparisonOperator.NotEqual:
                    return isNull ? "IS NOT" : "<>";
                case ComparisonOperator.GraterThan:
                    return ">";
                case ComparisonOperator.GraterOrEqual:
                    return ">=";
                case ComparisonOperator.LessThan:
                    return "<";
                case ComparisonOperator.LessThanOrEqual:
                    return "<=";
                case ComparisonOperator.In:
                    return "IN";
                case ComparisonOperator.NotIn:
                    return "NOT IN";
                case ComparisonOperator.Like:
                    return "LIKE";
                case ComparisonOperator.NotLike:
                    return "NOT LIKE";
                case ComparisonOperator.Between:
                    return "BETWEEN";
                default:
                    throw new ArgumentOutOfRangeException(nameof(oper), oper, null);
            }
        }

        internal static string ToSqlGroupOperator(this GroupOperator oper)
        {
            switch (oper)
            {
                case GroupOperator.None:
                    return string.Empty;
                case GroupOperator.Or:
                    return " OR ";
                case GroupOperator.And:
                    return " AND ";
                default:
                    throw new ArgumentOutOfRangeException(nameof(oper), oper, null);
            }
        }

        internal static string ToSqlJoin(this JoinType join)
        {
            switch (join)
            {
                case JoinType.Inner:
                    return "INNER JOIN";
                case JoinType.Left:
                    return "LEFT JOIN";
                case JoinType.Right:
                    return "RIGHT JOIN";
                default:
                    throw new ArgumentOutOfRangeException(nameof(join), join, null);
            }
        }

        internal static ColumnMetaData GetColumnMetaData(Type entityType, string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
                throw new ArgumentNullException(nameof(propertyName), "Property name can not be null!");

            var tableKey = GetTableKey(entityType);

            var propertyInfo = entityType.GetProperties().FirstOrDefault(p => p.Name == propertyName);

            if (propertyInfo == null)
                throw new ArgumentNullException(nameof(propertyInfo), $"Property could not be found on the entity! Property Name:{propertyName}. Entity:{tableKey}");

            var columnKey = $"{tableKey}.{propertyName}";
            ColumnMetaData columnMetaData;

            if (!propertyInfo.PropertyType.IsValueType && propertyInfo.PropertyType != typeof(string))
            {
                var metaData = new NavigationColumnMetaData();
                var joinAttribute = propertyInfo.GetCustomAttributes(true).OfType<JoinAttribute>().FirstOrDefault();

                if (joinAttribute != null)
                {
                    metaData.ForeignSchema = joinAttribute.ForeignSchema;
                    metaData.ForeignTableName = joinAttribute.ForeignTableName;
                    metaData.Alias = joinAttribute.Alias;
                    metaData.ForeignColumnName = joinAttribute.ForeignColumnName;
                    metaData.JoinType = joinAttribute.JoinType;
                    metaData.ColumnName = joinAttribute.ColumnName;

                }

                if (string.IsNullOrWhiteSpace(metaData.ColumnName))
                    metaData.ColumnName = $"{propertyInfo.Name}{DefaultForeignColumnName}";

                var foreignTableType = propertyInfo.PropertyType.IsGenericType
                    ? propertyInfo.PropertyType.GenericTypeArguments.First()
                    : propertyInfo.PropertyType;

                var foreignTableMetaData = GetTableMetaData(foreignTableType);

                if (string.IsNullOrWhiteSpace(metaData.ForeignSchema))
                    metaData.ForeignSchema = foreignTableMetaData.Schema;

                if (string.IsNullOrWhiteSpace(metaData.ForeignTableName))
                    metaData.ForeignTableName = foreignTableMetaData.TableName;

                if (string.IsNullOrWhiteSpace(metaData.Alias))
                    metaData.Alias = $"{propertyInfo.Name}_{foreignTableMetaData.Schema}{foreignTableMetaData.TableName}";

                if (string.IsNullOrWhiteSpace(metaData.ForeignColumnName))
                    metaData.ForeignColumnName = DefaultForeignColumnName;

                columnMetaData = metaData;

            }
            else
            {
                columnMetaData = new ColumnMetaData();
                var columnAttribute = propertyInfo.GetCustomAttributes(true).OfType<ColumnAttribute>().FirstOrDefault();

                columnMetaData.ColumnName = columnAttribute?.ColumnName ?? propertyInfo.Name;
            }

            return ColumnMetaDataList.GetOrAdd(columnKey, columnMetaData);
        }

        private static string GetTableKey(Type entityType)
        {
            if (entityType == null)

                throw new ArgumentNullException(nameof(entityType));
            var tableKey = entityType.FullName;

            if (string.IsNullOrWhiteSpace(tableKey))
                throw new ArgumentNullException(nameof(entityType), $"Type fullname is null. Type:{entityType}");

            return tableKey;

        }

        internal static TableMetaData GetTableMetaData(Type entityType)
        {
            var tableKey = GetTableKey(entityType);

            if (TableMetaDataList.ContainsKey(tableKey))
                return TableMetaDataList[tableKey];

            var tableMetaData = new TableMetaData();
            var table = entityType.GetCustomAttributes(true).OfType<TableAttribute>().FirstOrDefault();

            if (table != null)
            {
                tableMetaData.TableName = table.Name;
                tableMetaData.Alias = table.Alias;
                tableMetaData.Schema = table.Schema;
            }

            if (string.IsNullOrWhiteSpace(tableMetaData.TableName))
                tableMetaData.TableName = entityType.Name;

            if (string.IsNullOrWhiteSpace(tableMetaData.Schema))
                tableMetaData.Schema = DefaultSchema;

            if (string.IsNullOrWhiteSpace(tableMetaData.Alias))
                tableMetaData.Alias = $"{tableMetaData.Schema}{entityType.Name}";

            return TableMetaDataList.GetOrAdd(tableKey, tableMetaData);
        }

        internal static MemberExpression GetMemberExpression(Expression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.MemberAccess:
                    return expression as MemberExpression;
                case ExpressionType.Convert:
                    return GetMemberExpression(((UnaryExpression)expression).Operand);
                case ExpressionType.Lambda:
                    return GetMemberExpression(((LambdaExpression)expression).Body);
            }

            throw new ArgumentException("Member expression expected");
        }

        internal static string GetColumnName<T>(Expression<Func<T, object>> columnExpression) where T : class
        {
            var me = GetMemberExpression(columnExpression);

            if (me.Expression.Type == typeof(T))
            {
                var tableMetaData = GetTableMetaData(me.Expression.Type);
                var columnMetaData = GetColumnMetaData(me.Expression.Type, me.Member.Name);
                return $"{tableMetaData.Alias}.[{columnMetaData.ColumnName}]";

            }
            else
            {
                var pme = GetMemberExpression(me.Expression);
                var foreignColumnMetaData = GetColumnMetaData(typeof(T), pme.Member.Name);
                return $"{((NavigationColumnMetaData)foreignColumnMetaData).Alias}.[{foreignColumnMetaData.ColumnName}]";
            }
        }
    }
}
