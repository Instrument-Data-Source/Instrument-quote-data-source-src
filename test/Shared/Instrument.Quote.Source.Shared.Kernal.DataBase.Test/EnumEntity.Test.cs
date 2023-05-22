using Xunit.Abstractions;
using Instrument.Quote.Source.Shared.Kernal.DataBase;

namespace Instrument.Quote.Source.Shared.Kernal.DataBase.Test;

public class EnumEntity_Test
{
  ITestOutputHelper output;
  public class TestEnumEntity : EnumEntity<TestEnumEntity.Enum>
  {
    public TestEnumEntity(Enum Id) : base(Id)
    {
    }

    public TestEnumEntity(string Name) : base(Name)
    {
    }

    public enum Enum
    {
      Val1 = 1,
      Val2 = 2
    }
  }
  public EnumEntity_Test(ITestOutputHelper output)
  {
    this.output = output;
  }

  [Fact]
  public void WHEN_request_list_THEN_get_correct_list_of_entities()
  {
    // Array
    var expected_list = new List<TestEnumEntity>(){
        new TestEnumEntity(TestEnumEntity.Enum.Val1),
        new TestEnumEntity(TestEnumEntity.Enum.Val2)
      };

    // Act
    var asserted_list = TestEnumEntity.ToList();

    // Assert
    Assert.Equal(2, asserted_list.Count());
    foreach (var excpected_entity in expected_list)
    {
      Assert.Contains(excpected_entity, asserted_list);
    }
  }

  [Fact]
  public void WHEN_give_wrong_Name_THEN_get_error()
  {
    // Array

    // Act
    Assert.Throws<ArgumentOutOfRangeException>(() => new TestEnumEntity("Val33"));

    // Assert

  }

  [Fact]
  public void WHEN_give_wrong_id_THEN_get_error()
  {
    // Array

    // Act
    Assert.Throws<ArgumentOutOfRangeException>(() => new TestEnumEntity((TestEnumEntity.Enum)4));

    // Assert

  }

  [Fact]
  public void WHEN_set_enum_THEN_Id_Set_correctly()
  {
    // Array

    // Act
    var asserted_value = new TestEnumEntity(TestEnumEntity.Enum.Val1);

    // Assert
    Assert.Equal(asserted_value.Id, (int)TestEnumEntity.Enum.Val1);
  }
}