import org.json.simple.JSONObject;

public class Book {

    private String title;
    private String isbn;
    private String author;
    private int year;

    public Book(String title, String author, String isbn, int year){
        this.title = title;
        this.author = author;
        this.isbn = isbn;
        this.year = year;
    }

    public void updateTitle(String title){
        this.title = title;
    }

    public void updateAuthor(String author){
        this.author = author;
    }

    public void updateIsbn(String isbn){
        this.isbn = isbn;
    }

    public void updateYear(int year){
        this.year = year;
    }

    public String getTitle(){
        return this.title;
    }

    public String getAuthor(){
        return this.author;
    }

    public String getIsbn(){
        return this.isbn;
    }

    public int getYear(){
        return this.year;
    }

    public JSONObject convertToJson(){
        JSONObject obj = new JSONObject();
        obj.put("Pavadinimas", this.title);
        obj.put("Autorius", this.author);
        obj.put("ISBN", this.isbn);
        obj.put("Metai", this.year);
        return obj;
    }
}

//Test changes