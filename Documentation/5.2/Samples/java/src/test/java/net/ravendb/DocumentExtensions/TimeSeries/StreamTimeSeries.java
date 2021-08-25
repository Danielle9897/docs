import net.ravendb.client.documents.CloseableIterator;
import net.ravendb.client.documents.DocumentStore;
import net.ravendb.client.documents.IDocumentStore;
import net.ravendb.client.documents.commands.StreamResult;
import net.ravendb.client.documents.operations.timeSeries.TimeSeriesOperation;
import net.ravendb.client.documents.session.*;
import net.ravendb.client.documents.session.timeSeries.TimeSeriesEntry;
import net.ravendb.client.primitives.Reference;

import java.util.Arrays;
import java.util.List;

public class StreamTimeSeries {

    //region stream_methods
    <T> CloseableIterator<StreamResult<T>> stream(IDocumentQuery<T> query);

    <T> CloseableIterator<StreamResult<T>> stream(IDocumentQuery<T> query, Reference<StreamQueryStatistics> streamQueryStats);

    <T> CloseableIterator<StreamResult<T>> stream(IRawDocumentQuery<T> query);

    <T> CloseableIterator<StreamResult<T>> stream(IRawDocumentQuery<T> query, Reference<StreamQueryStatistics> streamQueryStats);
    //endregion

    public static void main(String args[]){
        try (IDocumentStore store = new DocumentStore( new String[]{ "http://localhost:8080" }, "Northwind")) {
            store.initialize();


            try (IDocumentSession session = store.openSession()) {

                //region direct
                ISessionDocumentTimeSeries timeseries = session.timeSeriesFor("HeartRate","user/1-A");
                List<TimeSeriesEntry> results = Arrays.asList(timeseries.get());
                //endregion


            }
            try (IDocumentSession session = store.openSession()) {
            //region query
            IRawDocumentQuery<Employee> query = session.advanced()
                    .rawQuery(Employee.class, "from Users select timeseries (from HeartRate)");

            CloseableIterator<StreamResult<Employee>> results = session.advanced().stream(query);

            while (results.hasNext()) {
                StreamResult<Employee> employee = results.next();
            }
            //endregion
            }
        }
    }

}


