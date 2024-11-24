namespace DO;

public record class Volunteer
{
    public int Id {  get; set; }
    public string Name { get; set; }
    public string PhonNumber {  get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Adress { get; set; }
    public double latitude { get; set; }

}
