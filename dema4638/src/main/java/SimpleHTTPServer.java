import com.sun.net.httpserver.HttpServer;

import java.net.InetAddress;
import java.net.InetSocketAddress;
import java.util.concurrent.Executors;
import java.util.concurrent.ThreadPoolExecutor;

public class SimpleHTTPServer {
    public static void main(String[] args) throws Exception{
        System.out.println("Hello");
        Library.initLibrary();
        ThreadPoolExecutor threadPoolExecutor = (ThreadPoolExecutor) Executors.newFixedThreadPool(10);
        HttpServer server = HttpServer.create(new InetSocketAddress(InetAddress.getLocalHost(), 80),10);
        server.createContext("/books", new  LibraryHttpHandler()); //"/Library"
        server.setExecutor(threadPoolExecutor);
        server.start();

    }
}
