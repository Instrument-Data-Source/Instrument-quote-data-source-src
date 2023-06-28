using Instrument.Quote.Source.App.Core.Test.Tools;

public class absMockFactory
{
  private HashSet<int> usedId = new HashSet<int>();
  /// <summary>
  /// Get next Random Id
  /// </summary>
  /// <returns></returns>
  protected int GetNextId()
  {
    var random = new Random();
    var id = random.Next(1, 1000);
    while (usedId.Contains(id))
    {
      id = random.Next(1, 1000);
    }
    return id;
  }
}