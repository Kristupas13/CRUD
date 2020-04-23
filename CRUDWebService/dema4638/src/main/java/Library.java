import org.json.simple.JSONObject;

import javax.lang.model.type.IntersectionType;
import java.util.ArrayList;

public class Library {
    private static ArrayList<Book> booksList;

    public static void initLibrary() {
        booksList = new ArrayList<>();
        booksList.add(new Book("Avarinis nusileidimas", "Sandra Brown", "9786090138823", 2019));
        booksList.add(new Book("Po raudonu dangum", "Mark Sullivan", "9786098233254", 2019));
        booksList.add(new Book("Partizanu gretose", "Adolfas Ramanauskas-Vanagas", "9786094840425", 2020));
    }

    public static ArrayList<Book> getAllBooks() {
        return booksList;
    }

    public static JSONObject getBook(String isbn) {
        for (Book book : booksList) {
            if (book.getIsbn().equals(isbn)) {
                return book.convertToJson();
            }
        }
        return null;
    }

    public static void addNewBook(JSONObject obj) {
        String title = obj.get("Pavadinimas").toString();
        String author = obj.get("Autorius").toString();
        String isbn = obj.get("ISBN").toString();
        int year = Integer.parseInt(obj.get("Metai").toString());
        booksList.add(new Book(title, author, isbn, year));
    }

    public static boolean deleteBook(String isbn) {
        boolean bookWasFound = false;
        for (Book book : booksList) {
            if (book.getIsbn().equals(isbn)) {
                booksList.remove(book);
                bookWasFound = true;
                break;
            }
        }
        return bookWasFound;
    }

    public static boolean putBook(JSONObject obj, String isbn) {
        boolean bookExisted = false;
        for (Book book : booksList) {
            if (book.getIsbn().equals(isbn)) {
                bookExisted = true;
                String title = obj.get("Pavadinimas").toString();
                String author = obj.get("Autorius").toString();
                //String newIsbn = obj.get("ISBN").toString();
                int year = Integer.parseInt(obj.get("Metai").toString());
                book.updateAuthor(author);
                book.updateTitle(title);
                //book.updateIsbn(newIsbn);
                book.updateYear(year);
                break;
            }
        }

        if (bookExisted == false) {
            obj.put("ISBN", isbn);
            addNewBook(obj);
        }

        return bookExisted;
    }

    public static boolean checkExistence(JSONObject obj) {

        boolean bookExists = false;
        for (Book book : booksList) {
            if (book.getIsbn().equals(obj.get("ISBN").toString())) {
                bookExists = true;
            }
        }
        return bookExists;
    }
}
