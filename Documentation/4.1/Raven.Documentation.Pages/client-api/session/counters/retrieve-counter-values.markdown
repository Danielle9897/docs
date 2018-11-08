# Retrieve Counter Values  

You can retrieve the value of a single Counter, or the values of all the Counters of a document.

**In this page**:  
* [Retrieving a Counter's value](../../../client-api/session/counters/retrieve-counter-values#retrieving-a-counters-value)  
* [Retrieving all Counters of a document](../../../client-api/session/counters/retrieve-counter-values#retrieving-all-counters-of-a-document)  

{PANEL: }

##Retrieving a Counter's value
{NOTE: }
## `Get`  
Use `Get` to retrieve the current value of a Counter.  

## Syntax
{CODE-BLOCK:json}
long Get(string counterName)
{CODE-BLOCK/}

| Parameters: | |
|-|-|
|`counterName`|A string with the Counter's name|

| Return Value: | |
|-|-|
|`long` integer|Counter's current value|


`Get` is a member of the [CountersFor](../../../client-api/session/counters/counters-overview#counter-management-methods-and-the--structure) session object.  

*  To use it:  
  - Open a session.  
  - Use the session to load a document.  
  - Create an instance of `CountersFor`, and refer it to the document by giving it the value returned by `session.Load`.  
  - Execute `Get`. Provide it with the name of the Cuonter whose value you want to retrieve.  

##Sample:
{CODE counters_region_Get@ClientApi\Session\Counters\Counters.cs /}



{NOTE/}

##Retrieving all Counters of a document
{NOTE: }
## `GetAll`  
Retrieve all Counters (a list of Names and Values) of a chosen document.  

## Syntax
{CODE-BLOCK:json}
Dictionary<string, long?> GetAll()
{CODE-BLOCK/}

|-|
|Return Value:|
|An array of counter names and values|
| |

`GetAll` is a member of the [CountersFor](../../../client-api/session/counters/counters-overview#counter-management-methods-and-the--structure) session object.  

*  To use it:  
  - Open a session.  
  - Use the session to load a document.  
  - Create an instance of `CountersFor`, and refer it to the document by giving it the value returned by `session.Load`.  
  - Execute `CountersFor.GetAll` to get a list of the document's counters' names and values.  

##Sample:
{CODE counters_region_GetAll@ClientApi\Session\Counters\Counters.cs /}

{NOTE/}

{PANEL/}
