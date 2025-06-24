using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using INFRASTRUCTURE.ExceptionHandler;

namespace INFRASTRUCTURE.Util;


public class QueryBuilder<TModel> where TModel : class
{
    /// <summary>
    /// Apply filters and relations to the query
    /// </summary>
    /// <param name="query">The query to apply the filters and relations to</param>
    /// <param name="filters">The filters to apply to the query, in the format of "field[operator]=value|field[operator]=value|..."</param>
    /// <param name="relations">The relations to apply to the query, separated by '|'</param>
    /// <returns>The query with the filters and relations applied</returns>
    public static IQueryable<TModel> Apply(IQueryable<TModel> query, string filters = "", string relations = "")
    {
        filters = filters ?? "";
        relations = relations ?? "";
        if (!string.IsNullOrEmpty(relations))
        {
            var relationships = relations.Split('|');
            foreach (var rel in relationships)
            {
                query = query.Include(rel);
            }
        }
        var matches = Regex.Matches(filters, @"[^|""\r\n]+=""[^""]*""|[^|""\r\n]+=[^|""\r\n]+");
        var filterList = matches.Cast<Match>().Select(m => m.Value).ToArray();
        foreach (var filter in filterList)
        {
            var match = Regex.Match(filter, @"^(?<path>[\w\.]+)\[(?<op>\w+)\]=(?<value>.+)$");
            if (!match.Success) throw new Error500Exception($"Invalid filter format: {filter}");
            string fld = match.Groups["path"].Value;
            string opt = match.Groups["op"].Value.ToLower();
            string val = match.Groups["value"].Value;
            // Remove quotes if val is a quoted string
            if (val.StartsWith("\"") && val.EndsWith("\""))
            {
                val = val.Substring(1, val.Length - 2);
            }
            query = query.Where(BuildExpression(fld, opt, val));
        }
        return query;
    }

    private static bool IsMultipleValues(string value)
    {
        // Match through regex, example: "1,2,3" or "1,2,3,4,5"
        return Regex.IsMatch(value, @"^\d+(,\d+)*$");
    }

    private static Expression<Func<TModel, bool>> BuildExpression(string path, string op, string value)
    {
        var parameter = Expression.Parameter(typeof(TModel), "x");
        Expression property = parameter;
        Type type = typeof(TModel);

        foreach (var part in path.Split('.'))
        {
            var propInfo = type.GetProperty(part, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance) 
                ?? throw new Error500Exception($"Property '{part}' not found on type '{type.Name}'");
            property = Expression.Property(property, propInfo);
            type = propInfo.PropertyType;
        }

        Expression comparison;
        
        if (op == "contains" && type == typeof(string) && !IsMultipleValues(value))
        {
            // For string contains operation
            var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) }) 
                ?? throw new Error500Exception($"Contains method not found on type '{typeof(string).Name}'");
            var constant = Expression.Constant(value, typeof(string));
            comparison = Expression.Call(property, containsMethod, constant);
        }
        else
        {
            try
            {
                var targetType = Nullable.GetUnderlyingType(type) ?? type;
                object typedValue = Convert.ChangeType(value, targetType);
                ConstantExpression? constant1 = null;
                ConstantExpression? constant2 = null;

                if (!IsMultipleValues(value))
                {
                    constant1 = Expression.Constant(typedValue, type);
                } 
                else
                {
                    var values = value.Split(',');
                    constant1 = Expression.Constant(values[0], type);
                    constant2 = Expression.Constant(values[1], type);

                    if (constant1 == null || constant2 == null)
                    {
                        throw new Error500Exception($"Invalid value format for property '{path}'");
                    }
                }

                comparison = op switch
                {
                    "eq"  => Expression.Equal(property, constant1),
                    "ne"  => Expression.NotEqual(property, constant1),
                    "gt"  => Expression.GreaterThan(property, constant1),
                    "gte" => Expression.GreaterThanOrEqual(property, constant1),
                    "lt"  => Expression.LessThan(property, constant1),
                    "lte" => Expression.LessThanOrEqual(property, constant1),
                    "between" => Expression.AndAlso(
                        Expression.GreaterThanOrEqual(property, constant1),
                        Expression.LessThanOrEqual(property, constant2!)
                    ),
                    _ => throw new Error500Exception($"Unsupported operator '{op}', valid operators are: eq, ne, gt, gte, lt, lte, between, contains")
                };
            }
            catch (FormatException)
            {
                throw new Error500Exception($"Invalid value format for property '{path}'");
            }
            catch (InvalidCastException)
            {
                throw new Error500Exception($"Cannot convert value to type '{type.Name}' for property '{path}'");
            }
        }

        return Expression.Lambda<Func<TModel, bool>>(comparison, parameter);
    }
}
