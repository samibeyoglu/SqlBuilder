using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Atlas.Data.SqlBuilder.Enums;

namespace Atlas.Data.SqlBuilder
{
    public class SqlBuilder<T> where T : class
    {
        protected WhereConditionBase WhereConditions;
        protected List<NavigationColumnMetaData> JoinList = new List<NavigationColumnMetaData>();
        protected List<string> SelectList = new List<string>();
        protected List<string> OrderByList = new List<string>();
        protected const string OrderByDescSuffix = "DESC";
        protected const string OrderByAscSuffix = "ASC";

        protected SqlBuilder()
        {

        }

        public static SqlBuilder<T> Initialize()
        {
            return new SqlBuilder<T>();
        }

        public SqlBuilder<T> Select(params Expression<Func<T, object>>[] columnExpressions)
        {
            foreach (var columnExpression in columnExpressions)
            {
                if (columnExpression.Body.NodeType == ExpressionType.Parameter)
                {
                    var tableMetaData = Helpers.GetTableMetaData(typeof(T));
                    SelectList.Add($"{tableMetaData.Alias}.*");
                }
                else
                {
                    var me = Helpers.GetMemberExpression(columnExpression.Body);
                    var columnMetaData = Helpers.GetColumnMetaData(me.Expression.Type, me.Member.Name);

                    if (columnMetaData is NavigationColumnMetaData data && me.Expression.Type == typeof(T))
                    {
                        SelectList.Add($"{data.Alias}.*");
                    }
                    else
                    {
                        SelectList.Add(Helpers.GetColumnName(columnExpression));
                    }
                }
            }

            return this;
        }

        public SqlBuilder<T> Join(params Expression<Func<T, object>>[] columnExpressions)
        {
            foreach (var columnExpression in columnExpressions)
            {

                var me = Helpers.GetMemberExpression(columnExpression);
                var columnMetaData = Helpers.GetColumnMetaData(typeof(T), me.Member.Name);

                if (columnMetaData is NavigationColumnMetaData)
                    JoinList.Add((NavigationColumnMetaData)columnMetaData);
                else

                    throw new ArgumentException($"Only navigation properties are allow to use in joins statements. Property Name:{me.Member.Name}", nameof(columnExpression));
            }

            return this;
        }

        public SqlBuilder<T> Where(Expression<Func<T, object>> columnExpression, ComparisonOperator oper, params string[] values)
        {
            var newCondition = new WhereCondition<T>(columnExpression, oper, values);
            return AddWhereCondition(newCondition);
        }

        public SqlBuilder<T> Where(Expression<Func<T, object>> columnExpression, ComparisonOperator oper, params long?[] values)
        {
            var newCondition = new WhereCondition<T>(columnExpression, oper, values);
            return AddWhereCondition(newCondition);
        }

        public SqlBuilder<T> Where(Expression<Func<T, object>> columnExpression, ComparisonOperator oper, params DateTime?[] values)
        {
            var newCondition = new WhereCondition<T>(columnExpression, oper, values);
            return AddWhereCondition(newCondition);
        }

        public SqlBuilder<T> Where(Expression<Func<T, object>> columnExpression, ComparisonOperator oper, params bool?[] values)
        {
            var newCondition = new WhereCondition<T>(columnExpression, oper, values);
            return AddWhereCondition(newCondition);
        }

        public SqlBuilder<T> Where(Expression<Func<T, object>> columnExpression, ComparisonOperator oper, params DateTimeOffset?[] values)
        {
            var newCondition = new WhereCondition<T>(columnExpression, oper, values);
            return AddWhereCondition(newCondition);
        }

        public SqlBuilder<T> Where(Expression<Func<T, object>> columnExpression, ComparisonOperator oper, params TimeSpan?[] values)
        {
            var newCondition = new WhereCondition<T>(columnExpression, oper, values);
            return AddWhereCondition(newCondition);
        }

        public SqlBuilder<T> Where(Expression<Func<T, object>> columnExpression, ComparisonOperator oper, params Guid?[] values)
        {
            var newCondition = new WhereCondition<T>(columnExpression, oper, values);
            return AddWhereCondition(newCondition);
        }

        public SqlBuilder<T> Where(Expression<Func<T, object>> columnExpression, ComparisonOperator oper, params decimal?[] values)
        {
            var newCondition = new WhereCondition<T>(columnExpression, oper, values);
            return AddWhereCondition(newCondition);
        }

        public SqlBuilder<T> Where(Expression<Func<T, object>> columnExpression, ComparisonOperator oper, params double?[] values)
        {
            var newCondition = new WhereCondition<T>(columnExpression, oper, values);
            return AddWhereCondition(newCondition);
        }

        public SqlBuilder<T> Where(Expression<Func<T, object>> columnExpression, ComparisonOperator oper, params int?[] values)
        {
            var newCondition = new WhereCondition<T>(columnExpression, oper, values);
            return AddWhereCondition(newCondition);
        }

        public SqlBuilder<T> Where(Expression<Func<T, object>> columnExpression, ComparisonOperator oper, params short?[] values)
        {
            var newCondition = new WhereCondition<T>(columnExpression, oper, values);
            return AddWhereCondition(newCondition);
        }

        public SqlBuilder<T> Where(Expression<Func<T, object>> columnExpression, ComparisonOperator oper, params byte?[] values)
        {
            var newCondition = new WhereCondition<T>(columnExpression, oper, values);
            return AddWhereCondition(newCondition);
        }

        public SqlBuilder<T> Where(Expression<Func<T, object>> columnExpression, ComparisonOperator oper, params float?[] values)
        {
            var newCondition = new WhereCondition<T>(columnExpression, oper, values);
            return AddWhereCondition(newCondition);
        }

        public SqlBuilder<T> Where(WhereConditionBase newCondition, GroupOperator groupOper = GroupOperator.None)
        {
            AddWhereCondition(newCondition, groupOper);
            return this;
        }

        public SqlBuilder<T> OrderBy(params Expression<Func<T, object>>[] columnExpressions)
        {
            return OrderBy(OrderByAscSuffix, columnExpressions);
        }

        public SqlBuilder<T> OrderByDesc(params Expression<Func<T, object>>[] columnExpressions)
        {
            return OrderBy(OrderByDescSuffix, columnExpressions);
        }

        public string ToSql()
        {
            var sb = new StringBuilder("SELECT\t");
            var selects = string.Join(",\r\n\t", SelectList);
            sb.AppendLine(string.IsNullOrWhiteSpace(selects) ? "*" : selects);

            var tableMetaData = Helpers.GetTableMetaData(typeof(T));
            sb.AppendLine($"FROM [{tableMetaData.Schema}].[{tableMetaData.TableName}] AS {tableMetaData.Alias}");
            JoinList.ForEach(j => sb.AppendLine(ToJoinSql(tableMetaData, j)));

            if (WhereConditions != null)
                sb.AppendLine($"WHERE {WhereConditions.ToSql()}");

            if (OrderByList.Any())
                sb.AppendLine($"ORDER BY {string.Join(", ", OrderByList)}");

            return sb.ToString();
        }

        public SqlCommand ToSqlCommand()
        {
            var sb = new StringBuilder("SELECT\t");
            var selects = string.Join(",\r\n\t", SelectList);
            sb.AppendLine(string.IsNullOrWhiteSpace(selects) ? "*" : selects);

            var tableMetaData = Helpers.GetTableMetaData(typeof(T));
            sb.AppendLine($"FROM [{tableMetaData.Schema}].[{tableMetaData.TableName}] AS {tableMetaData.Alias}");
            JoinList.ForEach(j => sb.AppendLine(ToJoinSql(tableMetaData, j)));

            var command = new SqlCommand();
            if (WhereConditions != null)
            {
                var sqlWithParameters = WhereConditions.ToSqlWithParameters();
                var sbw = new StringBuilder(sqlWithParameters.Item1, sqlWithParameters.Item1.Length);
                var counter = 0;
                foreach (var param in sqlWithParameters.Item2)
                {
                    var newParamName = "@p" + counter++;
                    command.Parameters.AddWithValue(newParamName, param.Value);
                    sbw.Replace(param.Key, newParamName);
                }
                sb.AppendLine($"WHERE {sbw}");

            }

            if (OrderByList.Any())
                sb.AppendLine($"ORDER BY {string.Join(", ", OrderByList)}");

            command.CommandText = sb.ToString();
            command.CommandType = CommandType.Text;
            return command;
        }

        private string ToJoinSql(TableMetaData tableMetaData, NavigationColumnMetaData j)
        {
            return $"\t{j.JoinType.ToSqlJoin()} [{j.ForeignSchema}].[{j.ForeignTableName}] AS {j.Alias}\r\n\t\tON {tableMetaData.Alias}.[{j.ColumnName}] = {j.Alias}.[{j.ForeignColumnName}]";
        }

        private SqlBuilder<T> AddWhereCondition(WhereConditionBase newCondition, GroupOperator groupOper = GroupOperator.None)
        {
            if (groupOper == GroupOperator.None)
                groupOper = GroupOperator.And;

            switch (WhereConditions)
            {
                case null:
                    WhereConditions = newCondition;
                    break;
                case WhereCondition condition:
                    WhereConditions = new WhereConditionGroup(condition, groupOper, newCondition);
                    break;
                case WhereConditionGroup conditionGroup:
                    conditionGroup.AddCondition(groupOper, newCondition);
                    break;
            }

            return this;
        }

        private SqlBuilder<T> OrderBy(string orderBy, params Expression<Func<T, object>>[] columnExpressions)
        {
            foreach (var columnExpression in columnExpressions)
            {
                var me = Helpers.GetMemberExpression(columnExpression);
                var columnMetaData = Helpers.GetColumnMetaData(me.Expression.Type, me.Member.Name);

                if (columnMetaData is NavigationColumnMetaData && me.Expression.Type == typeof(T))
                {
                    throw new ArgumentException("Only columns are allowed to order", nameof(columnExpressions));
                }
                else

                    OrderByList.Add($"{Helpers.GetColumnName(columnExpression)} {orderBy}");

            }
            return this;
        }
    }
}