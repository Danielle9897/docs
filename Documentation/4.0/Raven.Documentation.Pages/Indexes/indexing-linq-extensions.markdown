# Indexing LINQ extensions

Various indexing LINQ extensions are available to enhance the usability and reduce the complexity of the indexing functions. The available extensions are:

- [Boost](../indexes/indexing-linq-extensions#boost)
- [Reverse](../indexes/indexing-linq-extensions#reverse)
- [IfEntityIs](../indexes/indexing-linq-extensions#ifentityis)
- [WhereEntityIs](../indexes/indexing-linq-extensions#whereentityis)
- [ParseInt, ParseLong, ParseDecimal, ParseDouble](../indexes/indexing-linq-extensions#parsing-numbers)

{PANEL:**Boost**}

You can read more about boosting [here](../indexes/boosting).

{PANEL/}

{PANEL:**Reverse**}

**Strings** and **enumerables** can be reversed by using `Reverse` extension.

{CODE-TABS}
{CODE-TAB:csharp:Index indexes_1@Indexes/IndexingLinqExtensions.cs /}
{CODE-TAB:csharp:Query indexes_2@Indexes/IndexingLinqExtensions.cs /}
{CODE-TABS/}

{PANEL/}

{PANEL:**WhereEntityIs**}

`WhereEntityIs` can be used to check if given `Raven-Entity-Name` value in metadata for the given document matches any of the given values. This can be useful when indexing polymorphic data. Please visit dedicated article to get more information (or click [here](../indexes/indexing-polymorphic-data#other-ways)).

{PANEL/}

{PANEL:**IfEntityIs**}

`IfEntityIs` is similar to `WhereEntityIs`, yet it checks only against one value.

{PANEL/}

{PANEL:**Parsing numbers**}

String values can be safely parsed to `int`, `long`, `decimal` and `double` using appropriate methods:

- ParseInt,
- ParseLong,
- ParseDecimal,
- ParseDouble

There are two overrides for each method, first one returning default value in case of parsing failure, second one accepting value that should be returned when failure occurs.

{CODE-TABS}
{CODE-TAB:csharp:Index indexes_3@Indexes/IndexingLinqExtensions.cs /}
{CODE-TAB:csharp:Item indexes_4@Indexes/IndexingLinqExtensions.cs /}
{CODE-TAB:csharp:Example indexes_5@Indexes/IndexingLinqExtensions.cs /}
{CODE-TABS/}

{PANEL/}

## Related articles

- [Map indexes](../indexes/map-indexes)
- [Boosting](../indexes/boosting)