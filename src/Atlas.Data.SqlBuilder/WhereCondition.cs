using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Atlas.Data.SqlBuilder.Enums;

namespace Atlas.Data.SqlBuilder
{
    public class WhereCondition : WhereConditionBase
    {
        protected internal string SqlQuery;

        protected internal Dictionary<string, object> Parameters = new Dictionary<string, object>();

        protected void ToSqlQuery(string columnname, ComparisonOperator oper, object values)
        {
            if (!(values is Array objectArray) || objectArray.Length == 0)
            {
                var nullstr = (oper == ComparisonOperator.In || oper == ComparisonOperator.NotIn) ? "(NULL)" : "NULL";
                SqlQuery = $"{columnname} {oper.ToSqlComperationOperator(true)} {nullstr}";
                return;
            }

            switch (oper)
            {
                case ComparisonOperator.Between when objectArray.Length != 2:
                    throw new ArgumentException("Array length must be 2 for Between Comparison Operator");

                case ComparisonOperator.Between:
                    var param0 = "@" + Guid.NewGuid();
                    var param1 = "@" + Guid.NewGuid();

                    SqlQuery = $"{columnname} {oper.ToSqlComperationOperator(values.IsNull())} {param0} AND {param1}";

                    Parameters.Add(param0, ((Array)values).GetValue(0));
                    Parameters.Add(param1, ((Array)values).GetValue(1));
                    return;

                case ComparisonOperator.In:
                case ComparisonOperator.NotIn:
                    var sbIn = new StringBuilder($"{columnname} {oper.ToSqlComperationOperator(values.IsNull())} (");

                    for (var i = 0; i < ((Array)values).Length; i++)
                    {
                        var paramName = "@" + Guid.NewGuid();
                        sbIn.Append(paramName);

                        if (i < ((Array)values).Length - 1)
                            sbIn.Append(',');

                        Parameters.Add(paramName, ((Array)values).GetValue(i));
                    }

                    sbIn.Append(")");
                    SqlQuery = sbIn.ToString();
                    return;
            }


            var sb = new StringBuilder();
            for (var i = 0; i < ((Array)values).Length; i++)
            {
                var val = ((Array)values).GetValue(i);
                var paramName = "@" + Guid.NewGuid();
                sb.Append($"{columnname} {oper.ToSqlComperationOperator(val.IsNull())} {paramName}");

                if (i < ((Array)values).Length - 1)
                    sb.Append(" AND ");

                Parameters.Add(paramName, val);
            }
            SqlQuery = sb.ToString();
        }

        public WhereCondition(string columnname, ComparisonOperator oper, params long?[] values)
        {
            ToSqlQuery(columnname, oper, values);
        }

        public WhereCondition(string columnname, ComparisonOperator oper, params DateTime?[] values)
        {
            ToSqlQuery(columnname, oper, values);
        }

        public WhereCondition(string columnname, ComparisonOperator oper, params string[] values)
        {
            ToSqlQuery(columnname, oper, values);
        }

        public WhereCondition(string columnname, ComparisonOperator oper, params bool?[] values)
        {
            ToSqlQuery(columnname, oper, values);
        }

        public WhereCondition(string columnname, ComparisonOperator oper, params DateTimeOffset?[] values)
        {
            ToSqlQuery(columnname, oper, values);
        }

        public WhereCondition(string columnname, ComparisonOperator oper, params TimeSpan?[] values)
        {
            ToSqlQuery(columnname, oper, values);
        }

        public WhereCondition(string columnname, ComparisonOperator oper, params Guid?[] values)
        {
            ToSqlQuery(columnname, oper, values);
        }

        public WhereCondition(string columnname, ComparisonOperator oper, params decimal?[] values)
        {
            ToSqlQuery(columnname, oper, values);
        }

        public WhereCondition(string columnname, ComparisonOperator oper, params double?[] values)
        {
            ToSqlQuery(columnname, oper, values);
        }

        public WhereCondition(string columnname, ComparisonOperator oper, params int?[] values)
        {
            ToSqlQuery(columnname, oper, values);
        }

        public WhereCondition(string columnname, ComparisonOperator oper, params short?[] values)
        {
            ToSqlQuery(columnname, oper, values);
        }

        public WhereCondition(string columnname, ComparisonOperator oper, params byte?[] values)
        {
            ToSqlQuery(columnname, oper, values);
        }

        public WhereCondition(string columnname, ComparisonOperator oper, params float?[] values)
        {
            ToSqlQuery(columnname, oper, values);
        }

        protected internal override string ToSql()
        {
            var sb = new StringBuilder(SqlQuery);

            foreach (var param in Parameters)
            {
                sb.Replace(param.Key, ValueToString(param.Value));
            }

            return sb.ToString();
        }

        private static string ValueToString(object value)
        {
            if (value == null)
                return "NULL";
            var valueType = value.GetType();

            if (valueType == typeof(DateTime?) || valueType == typeof(DateTime))
                return $"'{((DateTime)value):yyyy-MM-dd HH:mm:ss.ffff}'";

            if (valueType == typeof(DateTimeOffset?) || valueType == typeof(DateTimeOffset))
                return $"'{((DateTimeOffset)value).ToLocalTime():yyyy-MM-dd HH:mm:ss.ffff zzz}'";

            if (valueType == typeof(bool?) || valueType == typeof(bool))
                return ((bool)value) ? "TRUE" : "FALSE";

            if (valueType == typeof(TimeSpan?) || valueType == typeof(TimeSpan))
                return ((TimeSpan)value).Ticks.ToString();

            //if (valueType == typeof(float) || valueType == typeof(decimal) || valueType == typeof(double) || valueType==typeof(short))
            //    return Convert.ToDecimal(value).ToString(new CultureInfo("en-US"));

            //for long?, string, TimeSpan?, Guid?, decimal?, int?,  Byte?, float?
            return value.IsNumber() ? Convert.ToDecimal(value).ToString(new CultureInfo("en-US")) : $"'{value}'";
        }

        protected internal override Tuple<string, Dictionary<string, object>> ToSqlWithParameters()
        {
            return new Tuple<string, Dictionary<string, object>>(SqlQuery, Parameters);
        }
    }

    public class WhereCondition<T> : WhereCondition where T : class
    {
        protected const string ColumnNamePattern = "$$columnname$$";

        public WhereCondition(Expression<Func<T, object>> columnExpression, ComparisonOperator oper, params long?[] values) : base(ColumnNamePattern, oper, values)
        {
            SqlQuery = SqlQuery.Replace(ColumnNamePattern, Helpers.GetColumnName(columnExpression));
        }

        public WhereCondition(Expression<Func<T, object>> columnExpression, ComparisonOperator oper, params DateTime?[] values) : base(ColumnNamePattern, oper, values)
        {
            SqlQuery = SqlQuery.Replace(ColumnNamePattern, Helpers.GetColumnName(columnExpression));
        }

        public WhereCondition(Expression<Func<T, object>> columnExpression, ComparisonOperator oper, params string[] values) : base(ColumnNamePattern, oper, values)
        {
            SqlQuery = SqlQuery.Replace(ColumnNamePattern, Helpers.GetColumnName(columnExpression));
        }

        public WhereCondition(Expression<Func<T, object>> columnExpression, ComparisonOperator oper, params bool?[] values) : base(ColumnNamePattern, oper, values)
        {
            SqlQuery = SqlQuery.Replace(ColumnNamePattern, Helpers.GetColumnName(columnExpression));
        }

        public WhereCondition(Expression<Func<T, object>> columnExpression, ComparisonOperator oper, params DateTimeOffset?[] values) : base(ColumnNamePattern, oper, values)
        {
            SqlQuery = SqlQuery.Replace(ColumnNamePattern, Helpers.GetColumnName(columnExpression));
        }

        public WhereCondition(Expression<Func<T, object>> columnExpression, ComparisonOperator oper, params TimeSpan?[] values) : base(ColumnNamePattern, oper, values)
        {
            SqlQuery = SqlQuery.Replace(ColumnNamePattern, Helpers.GetColumnName(columnExpression));
        }

        public WhereCondition(Expression<Func<T, object>> columnExpression, ComparisonOperator oper, params Guid?[] values) : base(ColumnNamePattern, oper, values)
        {
            SqlQuery = SqlQuery.Replace(ColumnNamePattern, Helpers.GetColumnName(columnExpression));
        }

        public WhereCondition(Expression<Func<T, object>> columnExpression, ComparisonOperator oper, params decimal?[] values) : base(ColumnNamePattern, oper, values)
        {
            SqlQuery = SqlQuery.Replace(ColumnNamePattern, Helpers.GetColumnName(columnExpression));
        }

        public WhereCondition(Expression<Func<T, object>> columnExpression, ComparisonOperator oper, params double?[] values) : base(ColumnNamePattern, oper, values)
        {
            SqlQuery = SqlQuery.Replace(ColumnNamePattern, Helpers.GetColumnName(columnExpression));
        }

        public WhereCondition(Expression<Func<T, object>> columnExpression, ComparisonOperator oper, params int?[] values) : base(ColumnNamePattern, oper, values)
        {
            SqlQuery = SqlQuery.Replace(ColumnNamePattern, Helpers.GetColumnName(columnExpression));
        }

        public WhereCondition(Expression<Func<T, object>> columnExpression, ComparisonOperator oper, params short?[] values) : base(ColumnNamePattern, oper, values)
        {
            SqlQuery = SqlQuery.Replace(ColumnNamePattern, Helpers.GetColumnName(columnExpression));
        }

        public WhereCondition(Expression<Func<T, object>> columnExpression, ComparisonOperator oper, params byte?[] values) : base(ColumnNamePattern, oper, values)
        {
            SqlQuery = SqlQuery.Replace(ColumnNamePattern, Helpers.GetColumnName(columnExpression));
        }

        public WhereCondition(Expression<Func<T, object>> columnExpression, ComparisonOperator oper, params float?[] values) : base(ColumnNamePattern, oper, values)
        {
            SqlQuery = SqlQuery.Replace(ColumnNamePattern, Helpers.GetColumnName(columnExpression));
        }
    }

    public class WhereConditionGroup : WhereConditionBase
    {
        protected internal List<Tuple<GroupOperator, WhereConditionBase>> Conditions { get; } = new List<Tuple<GroupOperator, WhereConditionBase>>();

        public WhereConditionGroup(WhereConditionBase left, GroupOperator oper, WhereConditionBase right)
        {
            Conditions.Add(new Tuple<GroupOperator, WhereConditionBase>(GroupOperator.None, left));
            Conditions.Add(new Tuple<GroupOperator, WhereConditionBase>(oper, right));
        }

        public void AddCondition(GroupOperator oper, WhereConditionBase condition)
        {
            Conditions.Add(new Tuple<GroupOperator, WhereConditionBase>(oper, condition));
        }

        protected internal override string ToSql()
        {
            return $"({string.Concat(Conditions.Select(c => c.Item1.ToSqlGroupOperator() + c.Item2.ToSql()))})";
        }

        protected internal override Tuple<string, Dictionary<string, object>> ToSqlWithParameters()
        {
            var sb = new StringBuilder("(");
            var parameters = new Dictionary<string, object>();

            foreach (var condition in Conditions)
            {
                sb.Append(condition.Item1.ToSqlGroupOperator());

                var sqlWithParam = condition.Item2.ToSqlWithParameters();
                sb.Append(sqlWithParam.Item1);

                foreach (var param in sqlWithParam.Item2)
                {
                    parameters.Add(param.Key, param.Value);
                }
            }

            sb.Append(")");
            return new Tuple<string, Dictionary<string, object>>(sb.ToString(), parameters);

        }
    }

    public abstract class WhereConditionBase
    {
        protected internal abstract string ToSql();
        protected internal abstract Tuple<string, Dictionary<string, object>> ToSqlWithParameters();
    }
}
