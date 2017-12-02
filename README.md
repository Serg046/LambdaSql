# LambdaSql
Provides possibility to create sql query based on entity type and build query using OOP style.
It uses a member lambda expression to get column name and has corresponding methods for each sql clause.

## Structure
The main two types are SqlSelect and SqlFilter.
SqlSelect contains logic to create `select` sql query and SqlFilter is used for some filter like `where` or `having` clauses.
All features are **immutable**.

#### SqlSelect contract
```csharp
SqlSelect<T> Extend(Func<ISqlSelectQueryBuilder, ISqlSelectQueryBuilder> decorationCallback);
Type EntityType { get; }  
string CommandText { get; }  
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
#### SqlFilter contract
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
#### SqlFilterField contract
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
#### Entity types
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
#### Simple query
```csharp
var qry = new SqlSelect<Person>()
    .AddFields(p => p.Id)
    .Where(SqlFilter<Person>.From(p => p.Name).EqualTo("Sergey"));
    
Console.WriteLine(qry.CommandText);
Console.WriteLine("---");
Console.WriteLine(string.Join("; ", qry.Parameters
    .Select(p => $"Name = {p.ParameterName}, Value = {p.Value}")));
```
```
SELECT
    pe.Id
FROM
    Person pe
WHERE
    pe.Name = @w0
---
Name = @w0, Value = Sergey
```
#### Group by and having clauses:
```csharp
var qry = new SqlSelect<Person>()
    .AddFields(p => p.Name)
    .AddFields(SqlField<Person>.Count(p => p.Name))
    .Where(SqlFilter<Person>.From(p => p.Id).GreaterThan(5))
    .GroupBy(p => p.Name)
    .Having(SqlFilter<Person>.From(p => p.Name).EqualTo("Sergey"));

Console.WriteLine(qry.CommandText);
Console.WriteLine("---");
Console.WriteLine(string.Join("; ", qry.Parameters
    .Select(p => $"Name = {p.ParameterName}, Value = {p.Value}")));
```
```
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
#### Nested query
```csharp
var qry = new SqlSelect
(
    new SqlSelect<Person>()
        .AddFields(p => p.Id, p => p.Name)
        .Where(SqlFilter<Person>.From(p => p.Name).EqualTo("Sergey"))
    , new SqlAlias("inner")
).AddFields<Person>(p => p.Name);

Console.WriteLine(qry.CommandText);
Console.WriteLine("---");
Console.WriteLine(string.Join("; ", qry.Parameters
    .Select(p => $"Name = {p.ParameterName}, Value = {p.Value}")));
```
```
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
#### Inner join
```csharp
var joinByLambda = new SqlSelect<Person>()
    .InnerJoin<Person, Passport>((person, passport) => person.PassportId == passport.Id);
var joinByFilter = new SqlSelect<Person>()
    .InnerJoin<Passport>(SqlFilter<Passport>.From(p => p.Id).EqualTo<Person>(p => p.PassportId));

Console.WriteLine(joinByLambda.CommandText);
Console.WriteLine("---");
Console.WriteLine(joinByFilter.CommandText);
```
```
SELECT
    *
FROM
    Person pe
INNER JOIN
    Passport pa ON pe.PassportId = pa.Id
```
