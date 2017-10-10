namespace LambdaSql.UnitTests.Entities
{
    public class Passport
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public string Number { get; set; }
        public int? NullablePersonId { get; set; }
    }
}
