# Create or Modify a Counter  
{PANEL: }
## `Increment`  
Use `Increment` to **create** a new Counter, or **modify** the value of an existing one.  
If the Counter already exists, `Increment` will increase (or decrease) it value.  
If it doesn't exist, `Increment` will create it and provide its initial value.

## Syntax
{CODE-BLOCK:json}
void Increment(string counterName, long incrementation = 1)
{CODE-BLOCK/}

| Parameters | |
| ------------- | ------------- |
| `counterName` | A string with the Counter's name |
| `incrementation` | The Counter will be increased by this value, or by 1 if you ommit it. If this is a new Counter, this will be its initial value. |


`Increment` is a member of the [CountersFor](../../../client-api/session/counters/counters-overview#counter-management-methods-and-the--structure) session object.

*  To use it:  
  - Open a session.  
  - Use the session to load a document.  
  - Create an instance of `CountersFor`, and refer it to the document by giving it the value returned by `session.Load`.  
  - Execute `Increment`. Provide it with the Counter's name and the incrementation, or ommit the incrementation for an increase of 1.  
  - execute `session.SaveChanges` for the changes to take effect.  

##Note:
* **Modifying a Counter using `Increment` only takes effect when `session.SaveChanges()` is executed.**  
* **To decrease a Counter's value, use a negative incrementation.**  

##Sample:
{CODE counters_region_Increment@ClientApi\Session\Counters\Counters.cs /}

{PANEL/}
