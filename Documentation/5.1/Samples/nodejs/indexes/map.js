import { 
    DocumentStore,
    AbstractIndexCreationTask
} from "ravendb";

const store = new DocumentStore();


{
//region indexes_1
class Employees_ByFirstAndLastName extends AbstractIndexCreationTask {
    // ...
}
//endregion
}

class Employees_ByFirstAndLastName extends AbstractIndexCreationTask {
    //region indexes_2
    constructor() {
        super();

        this.map = `docs.Employees.Select(employee => new {     
            FirstName = employee.FirstName,     
            LastName = employee.LastName 
        })`;
    }
    //endregion
}

//region indexes_7
class Employees_ByFullName extends AbstractIndexCreationTask {
    constructor() {
        super();

        this.map = `docs.Employees.Select(employee => new {     
            FullName = (employee.FirstName + " ") + employee.LastName 
        })`;
    }
}
//endregion

//region indexes_1_0
class Employees_ByYearOfBirth extends AbstractIndexCreationTask {

    constructor() {
        super();

        this.map = `docs.Employees.Select(employee => new {     
            YearOfBirth = employee.Birthday.Year
        })`;
    }
}
//endregion

//region indexes_1_2
class Employees_ByBirthday extends AbstractIndexCreationTask {

    constructor() {
        super();

        this.map = `docs.Employees.Select(employee => new {     
            Birthday = employee.Birthday 
        })`;
    }
}
//endregion

//region indexes_1_4
class Employees_ByCountry extends AbstractIndexCreationTask {

    constructor() {
        super();

        this.map = `docs.Employees.Select(employee => new {     
            Country = employee.Address.Country 
        })`;
    }
}
//endregion

//region indexes_1_6
class Employees_Query extends AbstractIndexCreationTask {
    constructor() {
        super();

        this.map = `docs.Employees.Select(employee => new {     
            Query = new [] { 
                employee.FirstName, 
                employee.LastName, 
                employee.Title, 
                employee.Address.City 
            } 
        })`;

        this.index("Query", "Search");
    }
}
//endregion

class Employee { }

async function map() {
    
    const session = store.openSession();

    {
        //region indexes_4
        const employees1 = await session.query({ indexName: "Employees/ByFirstAndLastName" })
            .whereEquals("FirstName", "Robert")
            .all();

        const employees2 = await session.query({ indexName: "Employees/ByFirstAndLastName" })
            .whereEquals("FirstName", "Robert")
            .all();
        //endregion
    }

    {
        //region indexes_8
        const employees = await session
            .query({ indexName: "Employees/ByFullName" })
            .whereEquals("FullName", "Robert King")
            .ofType(Employee)
            .all();
        //endregion
    }

    {
        //region indexes_9
        const employees = await session
            .advanced
            .documentQuery({ indexName: "Employees/ByFullName" })
            .whereEquals("FullName", "Robert King")
            .all();
        //endregion
    }

    {
        //region indexes_6_1
        const employees = await session
            .query({ indexName: "Employees/ByYearOfBirth" })
            .whereEquals("YearOfBirth", 1963)
            .ofType(Employee)
            .all();
        //endregion
    }

    {
        //region indexes_5_1
        const startDate = new Date(1963, 1, 1);
        const endDate = new Date(1963, 12, 31, 23, 59, 59, 999);
        const employees = await session
            .query({ indexName: "Employees/ByBirthday" })
            .whereBetween("Birthday", startDate, endDate)
            .ofType(Employee)
            .all();
        //endregion
    }

    {
        //region indexes_7_1
        const employees = await session
            .query({ indexName: "Employees/ByCountry" })
            .whereEquals("Country", "USA")
            .ofType(Employee)
            .all();
        //endregion
    }

    {
        //region indexes_1_7
        const employees = await session
            .query({ indexName: "Employees/Query" })
            .search("Query", "John Doe")
            .ofType(Employee)
            .all();
        //endregion
    }
}


