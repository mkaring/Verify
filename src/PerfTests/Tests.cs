[UsesVerify]
public class Tests
{
    [Fact]
    public Task Single()
    {
        var person = new Person
        {
            GivenNames = "John",
            FamilyName = "Smith",
            Spouse = "Jill",
            Address = new()
            {
                Street = "1 Puddle Lane",
                Suburb = "Gotham",
                Country = "USA"
            }
        };
        return Verify(person);
    }
}