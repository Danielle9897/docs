package net.ravendb.Indexes;

import net.ravendb.client.documents.DocumentStore;
import net.ravendb.client.documents.IDocumentStore;
import net.ravendb.client.documents.indexes.AbstractIndexCreationTask;
import net.ravendb.client.documents.indexes.FieldIndexing;
import net.ravendb.client.documents.queries.Query;
import net.ravendb.client.documents.session.IDocumentSession;

import java.time.LocalDate;
import java.time.temporal.ChronoUnit;
import java.util.Date;
import java.util.List;

public class Map {

    /*
    //region indexes_1
    public static class Employees_ByFirstAndLastName extends AbstractIndexCreationTask {
        // ...
    }
    //endregion
    */

    /*
    #region javaScriptindexes_1
    public static class Employees_ByFirstAndLastName extends AbstractJavaScriptIndexCreationTask {
       // ...
    }
    #endregion
    */

    public static class Employees_ByFirstAndLastName extends AbstractIndexCreationTask {
        //region indexes_2
        public Employees_ByFirstAndLastName() {
            map = "docs.Employees.Select(employee => new { " +
                "    FirstName = employee.FirstName, " +
                "    LastName = employee.LastName " +
                "})";
        }
        //endregion
    }

    //region indexes_7
    public static class Employees_ByFullName extends AbstractIndexCreationTask {
        public static class Result {
            private String fullName;

            public String getFullName() {
                return fullName;
            }

            public void setFullName(String fullName) {
                this.fullName = fullName;
            }
        }

        public Employees_ByFullName() {
            map = "docs.Employees.Select(employee => new { " +
                "    FullName = (employee.FirstName + \" \") + employee.LastName " +
                "})";
        }
    }
    //endregion

    //region indexes_1_0
    public static class Employees_ByYearOfBirth extends AbstractIndexCreationTask {
        public static class Result {
            private int yearOfBirth;

            public int getYearOfBirth() {
                return yearOfBirth;
            }

            public void setYearOfBirth(int yearOfBirth) {
                this.yearOfBirth = yearOfBirth;
            }
        }

        public Employees_ByYearOfBirth() {
            map = "docs.Employees.Select(employee => new { " +
                "    YearOfBirth = employee.Birthday.Year " +
                "})";
        }
    }
    //endregion

    //region indexes_1_2
    public static class Employees_ByBirthday extends AbstractIndexCreationTask {
        public static class Result {
            private Date birthday;

            public Date getBirthday() {
                return birthday;
            }

            public void setBirthday(Date birthday) {
                this.birthday = birthday;
            }
        }

        public Employees_ByBirthday() {
            map = "docs.Employees.Select(employee => new { " +
                "    Birthday = employee.Birthday " +
                "})";
        }
    }
    //endregion

    //region indexes_1_4
    public static class Employees_ByCountry extends AbstractIndexCreationTask {
        public static class Result {
            private String country;

            public String getCountry() {
                return country;
            }

            public void setCountry(String country) {
                this.country = country;
            }
        }

        public Employees_ByCountry() {
            map = "docs.Employees.Select(employee => new { " +
                "    Country = employee.Address.Country " +
                "})";
        }
    }
    //endregion

    //region indexes_1_6
    public static class Employees_Query extends AbstractIndexCreationTask {
        public static class Result {
            private String[] query;

            public String[] getQuery() {
                return query;
            }

            public void setQuery(String[] query) {
                this.query = query;
            }
        }

        public Employees_Query() {
            map = "docs.Employees.Select(employee => new { " +
                "    Query = new [] { employee.FirstName, employee.LastName, employee.Title, employee.Address.City } " +
                "})";
            index("query", FieldIndexing.SEARCH);
        }
    }
    //endregion

    private static class Employee {

    }

    public Map() {
        try (IDocumentStore store = new DocumentStore()) {
            try (IDocumentSession session = store.openSession()) {
                //region indexes_4
                List<Employee> employees1 = session.query(Employee.class, Employees_ByFirstAndLastName.class)
                    .whereEquals("FirstName", "Robert")
                    .toList();

                List<Employee> employees2 = session.query(Employee.class, Query.index("Employees/ByFirstAndLastName"))
                    .whereEquals("FirstName", "Robert")
                    .toList();
                //endregion
            }

            try (IDocumentSession session = store.openSession()) {
                //region indexes_8
                // notice that we're 'cheating' here
                // by marking result type in 'query' as 'Employees_ByFullName.Result'
                // and changing type using 'ofType' before sending query to server
                List<Employee> employees = session
                    .query(Employees_ByFullName.Result.class, Employees_ByFullName.class)
                    .whereEquals("FullName", "Robert King")
                    .ofType(Employee.class)
                    .toList();
                //endregion
            }

            try (IDocumentSession session = store.openSession()) {
                //region indexes_9
                List<Employee> employees = session
                    .advanced()
                    .documentQuery(Employee.class, Employees_ByFullName.class)
                    .whereEquals("FullName", "Robert King")
                    .toList();
                //endregion
            }

            try (IDocumentSession session = store.openSession()) {
                //region indexes_6_1
                List<Employee> employees = session
                    .query(Employees_ByYearOfBirth.Result.class, Employees_ByYearOfBirth.class)
                    .whereEquals("YearOfBirth", 1963)
                    .ofType(Employee.class)
                    .toList();
                //endregion
            }

            try (IDocumentSession session = store.openSession()) {
                //region indexes_5_1
                LocalDate startDate = LocalDate.of(1963, 1, 1);
                LocalDate endDate = startDate.plusYears(1).minus(1, ChronoUnit.MILLIS);
                List<Employee> employees = session
                    .query(Employees_ByBirthday.Result.class, Employees_ByBirthday.class)
                    .whereBetween("Birthday", startDate, endDate)
                    .ofType(Employee.class)
                    .toList();
                //endregion
            }

            try (IDocumentSession session = store.openSession()) {
                //region indexes_7_1
                List<Employee> employees = session
                    .query(Employees_ByCountry.Result.class, Employees_ByCountry.class)
                    .whereEquals("Country", "USA")
                    .ofType(Employee.class)
                    .toList();
                //endregion
            }

            try (IDocumentSession session = store.openSession()) {
                //region indexes_1_7
                List<Employee> employees = session
                    .query(Employees_Query.Result.class, Employees_Query.class)
                    .search("Query", "John Doe")
                    .ofType(Employee.class)
                    .toList();
                //endregion
            }

        }
    }

}
