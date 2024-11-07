using System.ComponentModel.DataAnnotations;

public class BankAccount
{
    [Key]
    public string AccountId { get; set; }
    public decimal CurrentBalance { get; set; }
}