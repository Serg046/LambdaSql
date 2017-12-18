# LambdaSql
Provides possibility to create sql query based on entity type and build query using OOP style.
It uses a member lambda expression to get column name and has corresponding methods for each sql clause.

## Structure
The main two types are SqlSelect and SqlFilter.
SqlSelect contains logic to create `select` sql query and SqlFilter is used for some filter like `where` or `having` clauses.
All features are **immutable**.

- SqlSelect contract
```csharp
SqlSelect<T> Extend(Func<ISqlSelectQueryBuilder, ISqlSelectQueryBuilder> decorationCallback);
Type EntityType { get; }  
string RawSql { get; }  
string ParametricSql { get; }  
DbParameter[] Parameters { get; }  
SqlSelect<T> Distinct();  
SqlSelect<T> AddFields(params Expression<Func<T, object>>[] fields);  
SqlSelect<T> GroupBy(params Expression<Func<T, object>>[] fields);  
SqlSelect<T> OrderBy(params Expression<Func<T, object>>[] fields);  
SqlSelect<T> InnerJoin<TLeft, TJoin>(...);  
SqlSelect<T> LeftJoin<TLeft, TJoin>(...);  
SqlSelect<T> RightJoin<TLeft, TJoin>(...);  
SqlSelect<T> FullJoin<TLeft, TJoin>(...);  
SqlSelect<T> Where(ISqlFilter filter);  
SqlSelect<T> Having(ISqlFilter filter);  
```
- SqlFilter contract
```csharp
string RawSql { get; }  
SqlParameter[] Parameters { get; }  
string ParametricSql { get; }  
SqlFilterField And<TFieldType>(...);  
SqlFilter<TEntity> And(SqlFilter<TEntity> filter);  
SqlFilterField Or<TFieldType>(...);  
SqlFilter<TEntity> Or(SqlFilter<TEntity> filter);  
SqlFilter<TEntity> AndGroup(SqlFilter<TEntity> filter);  
SqlFilter<TEntity> OrGroup(SqlFilter<TEntity> filter);  
```
- SqlFilterField contract
```csharp
TResult SatisfyLambda(Func<ISqlField, string> filter);  
TResult IsNull();  
TResult IsNotNull();  
TResult Like(string value);  
TResult EqualTo(TFieldType value);  
TResult EqualTo(Expression<Func<TEntity, TFieldType>> field);  
TResult NotEqualTo(TFieldType value);  
TResult NotEqualTo(Expression<Func<TEntity, TFieldType>> field);  
TResult GreaterThan(TFieldType value);  
TResult GreaterThan(Expression<Func<TEntity, TFieldType>> field);  
TResult GreaterThanOrEqual(TFieldType value);  
TResult GreaterThanOrEqual(Expression<Func<TEntity, TFieldType>> field);  
TResult LessThan(TFieldType value);  
TResult LessThan(Expression<Func<TEntity, TFieldType>> field);  
TResult LessThanOrEqual(TFieldType value);  
TResult LessThanOrEqual(Expression<Func<TEntity, TFieldType>> field);  
TResult In(params TFieldType[] values);  
TResult NotIn(params TFieldType[] values);  
```

## Usage
- Entity types
```csharp
public class Person
{
    public int Id { get; }
    public string Name { get; }
    public int PassportId { get; }
}

public class Passport
{
    public int Id { get; }
    public string Number { get; }
}
```
- Simple query
```csharp
var qry = new SqlSelect<Person>()
    .AddFields(p => p.Id)
    .Where(SqlFilter<Person>.From(p => p.Name).EqualTo("Sergey"));
    
Console.WriteLine(qry.ParametricSql);
Console.WriteLine("---");
Console.WriteLine(string.Join("; ", qry.Parameters
    .Select(p => $"Name = {p.ParameterName}, Value = {p.Value}")));
```
```sql
SELECT
    pe.Id
FROM
    Person pe
WHERE
    pe.Name = @w0
---
Name = @w0, Value = Sergey
```
- Group by and having clauses
```csharp
var qry = new SqlSelect<Person>()
    .AddFields(p => p.Name)
    .AddFields(SqlField<Person>.Count(p => p.Name))
    .Where(SqlFilter<Person>.From(p => p.Id).GreaterThan(5))
    .GroupBy(p => p.Name)
    .Having(SqlFilter<Person>.From(p => p.Name).EqualTo("Sergey"));

Console.WriteLine(qry.ParametricSql);
Console.WriteLine("---");
Console.WriteLine(string.Join("; ", qry.Parameters
    .Select(p => $"Name = {p.ParameterName}, Value = {p.Value}")));
```
```sql
SELECT
    pe.Name, COUNT(pe.Name)
FROM
    Person pe
WHERE
    pe.Id > @w0
GROUP BY
    pe.Name
HAVING
    pe.Name = @h0
---
Name = @w0, Value = 5; Name = @h0, Value = Sergey
```
- Nested query
```csharp
var qry = new SqlSelect
(
    new SqlSelect<Person>()
        .AddFields(p => p.Id, p => p.Name)
        .Where(SqlFilter<Person>.From(p => p.Name).EqualTo("Sergey"))
    , new SqlAlias("inner")
).AddFields<Person>(p => p.Name);

Console.WriteLine(qry.ParametricSql);
Console.WriteLine("---");
Console.WriteLine(string.Join("; ", qry.Parameters
    .Select(p => $"Name = {p.ParameterName}, Value = {p.Value}")));
```
```sql
SELECT
    inner.Name
FROM
(
    SELECT
        pe.Id, pe.Name
    FROM
        Person pe
    WHERE
        pe.Name = @w0
) AS inner
---
Name = @w0, Value = Sergey
```
- Inner join
```csharp
var joinByLambda = new SqlSelect<Person>()
    .InnerJoin<Person, Passport>((person, passport) => person.PassportId == passport.Id);
var joinByFilter = new SqlSelect<Person>()
    .InnerJoin<Passport>(SqlFilter<Passport>.From(p => p.Id).EqualTo<Person>(p => p.PassportId));

Console.WriteLine(joinByLambda.ParametricSql);
Console.WriteLine("---");
Console.WriteLine(joinByFilter.ParametricSql);
```
```sql
SELECT
    *
FROM
    Person pe
INNER JOIN
    Passport pa ON pe.PassportId = pa.Id
---
SELECT
    *
FROM
    Person pe
INNER JOIN
    Passport pa ON pa.Id = pe.PassportId
```
## Extensibility
- SqlSelectQueryBuilder contract
```csharp
ISqlSelectQueryBuilder ModifySelectFields(ModifyQueryPartCallback modificationCallback);
ISqlSelectQueryBuilder ModifyJoins(ModifyQueryPartCallback modificationCallback);
ISqlSelectQueryBuilder ModifyWhereFilters(ModifyQueryPartCallback modificationCallback);
ISqlSelectQueryBuilder ModifyGroupByFields(ModifyQueryPartCallback modificationCallback);
ISqlSelectQueryBuilder ModifyHavingFilters(ModifyQueryPartCallback modificationCallback);
ISqlSelectQueryBuilder ModifyOrderByFields(ModifyQueryPartCallback modificationCallback);
ISqlSelectQueryBuilder Modify(ModifyQueryPartCallback modificationCallback); //To modify the whole query
```
- MySql Limit
```csharp
public static ISqlSelect Limit(this ISqlSelect select, int count, int? offset = null)
{
    if (count <= 0)
    {
        throw new ArgumentException("Parameter \"count\" must be a possitive number");
    }
    if (offset.HasValue && offset <= 0)
    {
        throw new ArgumentException("Parameter \"offset\" must be a possitive number");
    }
    return offset == null
        ? select.Extend(queryBuilder => queryBuilder.Modify(query => $"{query}{Environment.NewLine}LIMIT {count}"))
        : select.Extend(queryBuilder => queryBuilder.Modify(query => $"{query}{Environment.NewLine}LIMIT {count} OFFSET {offset}"));
}

static void Main(string[] args)
{
    Console.WriteLine(new SqlSelect<Person>().Limit(10).RawSql);
    Console.WriteLine();
    Console.WriteLine(new SqlSelect<Person>().Limit(10, 5).RawSql);
}
```
```sql
SELECT
    *
FROM
    Person pe
LIMIT 10

SELECT
    *
FROM
    Person pe
LIMIT 10 OFFSET 5
```
- Oracle ROWNUM
```csharp
public static ISqlSelect Top(this ISqlSelect select, int count)
{
    if (count <= 0)
    {
        throw new ArgumentException("Parameter \"count\" must be a possitive number");
    }
    return select.Extend(queryBuilder => queryBuilder.ModifyWhereFilters(where => where.Length == 0
        ? $"{Environment.NewLine}WHERE{Environment.NewLine}    ROWNUM <= {count}"
        : $"{where} AND ROWNUM <= {count}"));
}

static void Main(string[] args)
{
    Console.WriteLine(new SqlSelect<Person>().Top(10).RawSql);
    Console.WriteLine();
    Console.WriteLine(new SqlSelect<Person>()
        .Where(SqlFilter<Person>.From(person => person.Name).EqualTo("Sergey"))
        .Top(10).RawSql);
}
```
```sql
SELECT
    *
FROM
    Person pe
WHERE
    ROWNUM <= 10

SELECT
    *
FROM
    Person pe
WHERE
    pe.Name = 'Sergey' AND ROWNUM <= 10
```
