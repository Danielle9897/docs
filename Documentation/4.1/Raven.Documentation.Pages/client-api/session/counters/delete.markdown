# Delete a Counter  
{PANEL: }
## `Delete`  
Use `Delete` to remove a Counter from a document.  
There is no return value, and Delete will not generate an error if the Counter doesn't exist.  

## Syntax
{CODE-BLOCK:json}
void Delete(string counterName)
{CODE-BLOCK/}

| Parameters | |
| ------------- | ------------- |
| `counterName` | A string with the Counter's name |

`Delete` is a member of the [CountersFor](../../../client-api/session/counters/counters-overview#counter-management-methods-and-the--structure) session object.

*  To use it:  
  - Open a session.  
  - Use the session to load a document.  
  - Create an instance of `CountersFor`, and refer it to the document by giving it the value returned by `session.Load`.  
  - Execute `Delete`.  
  - execute `session.SaveChanges` for the changes to take effect.  

*  **Note**:  
  - A Counter you deleted will be removed only after the execution of `SaveChanges()`.  
  - Deleting a document, deletes its counters as well.  
  - There is no return value, and Delete will **not** generate an error if the Counter doesn't exist.  

##Sample:
{CODE counters_region_Delete@ClientApi\Session\Counters\Counters.cs /}
{PANEL/}
