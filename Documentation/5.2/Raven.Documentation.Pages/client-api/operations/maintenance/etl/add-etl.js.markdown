# Operations: How to Add ETL

You can add ETL task by using **AddEtlOperation**.

## Usage

{CODE:nodejs add_etl_operation@ClientApi\Operations\addEtl.js  /}

| Parameters | | |
| ------------- | ----- | ---- |
| **configuration** | `EtlConfiguration<T>` | ETL configuration where `T` is connection string type |

## Example - Add Raven ETL

{CODE:nodejs add_raven_etl@ClientApi\Operations\addEtl.js  /}

## Example - Add Sql ETL

{CODE:nodejs add_sql_etl@ClientApi\Operations\addEtl.js  /}

## Example - Add OLAP ETL

{CODE:nodejs add_olap_etl@ClientApi\Operations\addEtl.js  /}

## Related Articles

### ETL

- [Basics](../../../../server/ongoing-tasks/etl/basics)
- [RavenDB ETL](../../../../server/ongoing-tasks/etl/raven)
- [SQL ETL](../../../../server/ongoing-tasks/etl/sql)

### Studio

- [RavenDB ETL Task](../../../../studio/database/tasks/ongoing-tasks/ravendb-etl-task)

### Connection Strings

- [Add](../../../../client-api/operations/maintenance/connection-strings/add-connection-string)
- [Get](../../../../client-api/operations/maintenance/connection-strings/get-connection-string)
- [Remove](../../../../client-api/operations/maintenance/connection-strings/remove-connection-string)
