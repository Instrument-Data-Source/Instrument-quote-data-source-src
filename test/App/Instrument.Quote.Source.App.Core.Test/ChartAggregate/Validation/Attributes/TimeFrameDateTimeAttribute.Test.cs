using System.ComponentModel.DataAnnotations;
using Instrument.Quote.Source.App.Core.ChartAggregate.Validation.Attributes;
using Instrument.Quote.Source.App.Core.Test.Tools;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;
namespace Instrument.Quote.Source.App.Core.Test.ChartAggregate.Validation.Attributes;

public class TimeFrameDateTimeAttribute_Test : BaseTest<TimeFrameDateTimeAttribute_Test>
{

  public TimeFrameDateTimeAttribute_Test(ITestOutputHelper output) : base(output)
  {

  }
  public static IEnumerable<DateTime> Correct_dt_m1
  {
    get
    {
      foreach (var hour in new[] { 0, 3, 23 })
      {
        foreach (var minute in new[] { 0, 1, 16, 59 })
        {
          yield return new DateTime(2020, 1, 1, hour, minute, 0);
          yield return new DateTime(2020, 1, 1, hour, minute, 0, 0, 0);
        }
      }
    }
  }

  public static IEnumerable<DateTime> Correct_dt_m5
  {
    get
    {
      foreach (var hour in new[] { 0, 3, 23 })
      {
        foreach (var minute in new[] { 0, 5, 15, 30, 50 })
        {
          yield return new DateTime(2020, 1, 1, hour, minute, 0);
          yield return new DateTime(2020, 1, 1, hour, minute, 0, 0, 0);
        }
      }
    }
  }

  public static IEnumerable<DateTime> Correct_dt_m10
  {
    get
    {
      foreach (var hour in new[] { 0, 3, 23 })
      {
        foreach (var minute in new[] { 0, 10, 30, 50 })
        {
          yield return new DateTime(2020, 1, 1, hour, minute, 0);
          yield return new DateTime(2020, 1, 1, hour, minute, 0, 0, 0);
        }
      }
    }
  }

  public static IEnumerable<DateTime> Correct_dt_m15
  {
    get
    {
      foreach (var hour in new[] { 0, 3, 23 })
      {
        foreach (var minute in new[] { 0, 15, 30, 45 })
        {
          yield return new DateTime(2020, 1, 1, hour, minute, 0);
          yield return new DateTime(2020, 1, 1, hour, minute, 0, 0, 0);
        }
      }
    }
  }

  public static IEnumerable<DateTime> Correct_dt_m30
  {
    get
    {
      foreach (var hour in new[] { 0, 3, 23 })
      {
        foreach (var minute in new[] { 0, 30 })
        {
          yield return new DateTime(2020, 1, 1, hour, minute, 0);
          yield return new DateTime(2020, 1, 1, hour, minute, 0, 0, 0);
        }
      }
    }
  }

  public static IEnumerable<DateTime> Correct_dt_H1
  {
    get
    {
      foreach (var hour in new[] { 0, 3, 23 })
      {
        yield return new DateTime(2020, 1, 1, hour, 0, 0);
        yield return new DateTime(2020, 1, 1, hour, 0, 0, 0, 0);
      }
    }
  }
  public static IEnumerable<DateTime> Correct_dt_H4
  {
    get
    {
      foreach (var hour in new[] { 0, 4, 8, 16, 20 })
      {
        yield return new DateTime(2020, 1, 1, hour, 0, 0);
        yield return new DateTime(2020, 1, 1, hour, 0, 0, 0, 0);
      }
    }
  }
  public static IEnumerable<DateTime> Correct_dt_D1
  {
    get
    {
      foreach (var day in new[] { 1, 2, 31, 16, 20 })
      {
        yield return new DateTime(2020, 1, day);
        yield return new DateTime(2020, 1, day, 0, 0, 0);
        yield return new DateTime(2020, 1, day, 0, 0, 0, 0, 0);
      }
    }
  }
  public static IEnumerable<DateTime> Correct_dt_W1
  {
    get
    {
      foreach (var day in new[] { 1, 8, 15, 22, 29 })
      {
        yield return new DateTime(2023, 5, day);
        yield return new DateTime(2023, 5, day, 0, 0, 0);
        yield return new DateTime(2023, 5, day, 0, 0, 0, 0, 0);
      }
    }
  }
  public static IEnumerable<DateTime> Correct_dt_M
  {
    get
    {
      foreach (var month in new[] { 1, 2, 3, 12, 6 })
      {
        yield return new DateTime(2023, month, 1);
        yield return new DateTime(2023, month, 1, 0, 0, 0);
        yield return new DateTime(2023, month, 1, 0, 0, 0, 0, 0);
      }
    }
  }

  public static IEnumerable<object[]> Correct_case
  {
    get
    {
      yield return new object[] { Correct_dt_m1, TimeFrame.Enum.m1 };
      yield return new object[] { Correct_dt_m5, TimeFrame.Enum.m5 };
      yield return new object[] { Correct_dt_m10, TimeFrame.Enum.m10 };
      yield return new object[] { Correct_dt_m15, TimeFrame.Enum.m15 };
      yield return new object[] { Correct_dt_m30, TimeFrame.Enum.m30 };
      yield return new object[] { Correct_dt_H1, TimeFrame.Enum.H1 };
      yield return new object[] { Correct_dt_H4, TimeFrame.Enum.H4 };
      yield return new object[] { Correct_dt_D1, TimeFrame.Enum.D1 };
      yield return new object[] { Correct_dt_W1, TimeFrame.Enum.W1 };
      yield return new object[] { Correct_dt_M, TimeFrame.Enum.M };
    }
  }


  [Theory]
  [MemberData(nameof(Correct_case))]
  public void WHEN_correct_dt_for_timeframe_THEN_success(IEnumerable<DateTime> dts, TimeFrame.Enum tfEnum)
  {
    #region Array
    this.logger.LogDebug("Test ARRAY");

    var validationAttribute = new FitTimeFrameAttribute("TfId", true);

    IDictionary<object, object?>? items = new Dictionary<object, object?>();
    items.Add("TfId", (int)tfEnum);

    #endregion

    foreach (var dt in dts)
    {
      #region Act
      this.logger.LogDebug("Test ACT");

      var asserted_result = FitTimeFrameAttribute.IsValid(dt, tfEnum, out var assertedMessage);

      #endregion


      #region Assert
      this.logger.LogDebug("Test ASSERT");

      Expect("DTs is valid", () => Assert.True(asserted_result));
      #endregion
    }

  }


  public static IEnumerable<DateTime> InCorrect_dt_m1
  {
    get
    {
      foreach (var hour in new[] { 0, 3, 23 })
      {
        foreach (var minute in new[] { 0, 30, 59 })
        {
          foreach (var second in new[] { 1, 24, 59 })
          {
            yield return new DateTime(2020, 1, 1, hour, minute, second);

          }
          foreach (var millisecond in new[] { 1, 100, 999 })
          {
            yield return new DateTime(2020, 1, 1, hour, minute, 0, millisecond, 0);

          }
          foreach (var microsecond in new[] { 1, 100, 999 })
          {
            yield return new DateTime(2020, 1, 1, hour, minute, 0, 0, microsecond);
          }
        }
      }
    }
  }

  public static IEnumerable<DateTime> InCorrect_dt_m5
  {
    get
    {
      foreach (var incorrect_df in InCorrect_dt_m1)
      {
        yield return incorrect_df;
      }

      foreach (var hour in new[] { 0, 3, 23 })
      {
        foreach (var minute in new[] { 1, 31, 59 })
        {
          yield return new DateTime(2020, 1, 1, hour, minute, 0);
          yield return new DateTime(2020, 1, 1, hour, minute, 0, 0, 0);
        }
      }
    }
  }

  public static IEnumerable<DateTime> InCorrect_dt_m10
  {
    get
    {
      foreach (var incorrect_df in InCorrect_dt_m1)
      {
        yield return incorrect_df;
      }

      foreach (var hour in new[] { 0, 3, 23 })
      {
        foreach (var minute in new[] { 1, 31, 59 })
        {
          yield return new DateTime(2020, 1, 1, hour, minute, 0);
          yield return new DateTime(2020, 1, 1, hour, minute, 0, 0, 0);
        }
      }
    }
  }
  public static IEnumerable<DateTime> InCorrect_dt_m15
  {
    get
    {
      foreach (var incorrect_df in InCorrect_dt_m1)
      {
        yield return incorrect_df;
      }

      foreach (var hour in new[] { 0, 3, 23 })
      {
        foreach (var minute in new[] { 1, 9, 20, 59 })
        {
          yield return new DateTime(2020, 1, 1, hour, minute, 0);
          yield return new DateTime(2020, 1, 1, hour, minute, 0, 0, 0);
        }
      }
    }
  }
  public static IEnumerable<DateTime> InCorrect_dt_m30
  {
    get
    {
      foreach (var incorrect_df in InCorrect_dt_m1)
      {
        yield return incorrect_df;
      }

      foreach (var hour in new[] { 0, 3, 23 })
      {
        foreach (var minute in new[] { 1, 20, 31, 45, 59 })
        {
          yield return new DateTime(2020, 1, 1, hour, minute, 0);
          yield return new DateTime(2020, 1, 1, hour, minute, 0, 0, 0);
        }
      }
    }
  }
  public static IEnumerable<DateTime> InCorrect_dt_H1
  {
    get
    {
      var tf = TimeFrame.Enum.H1;
      foreach (var incorrect_df in InCorrect_dt_m1)
      {
        yield return incorrect_df;
      }

      foreach (var hour in new[] { 0, 3, 23 })
      {
        foreach (var minute in new[] { 1, 30, 59 })
        {
          yield return new DateTime(2020, 1, 1, hour, minute, 0);
          yield return new DateTime(2020, 1, 1, hour, minute, 0, 0, 0);
        }
      }
    }
  }
  public static IEnumerable<DateTime> InCorrect_dt_H4
  {
    get
    {
      var tf = TimeFrame.Enum.H4;
      foreach (var incorrect_df in InCorrect_dt_H1)
      {
        yield return incorrect_df;
      }
      foreach (var hour in new[] { 3, 9, 23 })
      {
        yield return new DateTime(2020, 1, 1, hour, 0, 0);
        yield return new DateTime(2020, 1, 1, hour, 0, 0, 0, 0);
      }
    }
  }

  public static IEnumerable<DateTime> InCorrect_dt_D1
  {
    get
    {
      foreach (var incorrect_df in InCorrect_dt_H1)
      {
        yield return incorrect_df;
      }
      foreach (var hour in new[] { 1, 8, 16, 23 })
      {
        yield return new DateTime(2020, 1, 1, hour, 0, 0);
        yield return new DateTime(2020, 1, 1, hour, 0, 0, 0, 0);
      }
    }
  }
  public static IEnumerable<DateTime> InCorrect_dt_W1
  {
    get
    {
      var tf = TimeFrame.Enum.W1;
      foreach (var incorrect_df in InCorrect_dt_D1)
      {
        yield return incorrect_df;
      }

      foreach (var Day in new[] { 16, 20, 21, 31 })
      {
        yield return new DateTime(2023, 5, Day);
        yield return new DateTime(2023, 5, Day, 0, 0, 0);
        yield return new DateTime(2023, 5, Day, 0, 0, 0, 0, 0);
      }
      yield return new DateTime(2023, 4, 1);
      yield return new DateTime(2023, 4, 1, 0, 0, 0);
      yield return new DateTime(2023, 4, 1, 0, 0, 0, 0, 0);
    }
  }
  public static IEnumerable<DateTime> InCorrect_dt_M
  {
    get
    {
      var tf = TimeFrame.Enum.W1;
      foreach (var incorrect_df in InCorrect_dt_D1)
      {
        yield return incorrect_df;
      }

      foreach (var Day in new[] { 2, 16, 21, 31 })
      {
        yield return new DateTime(2023, 5, Day);
        yield return new DateTime(2023, 5, Day, 0, 0, 0);
        yield return new DateTime(2023, 5, Day, 0, 0, 0, 0, 0);
      }
    }
  }
  public static IEnumerable<object[]> InCorrect_case
  {
    get
    {
      yield return new object[] { InCorrect_dt_m1, TimeFrame.Enum.m1 };
      yield return new object[] { InCorrect_dt_m5, TimeFrame.Enum.m5 };
      yield return new object[] { InCorrect_dt_m10, TimeFrame.Enum.m10 };
      yield return new object[] { InCorrect_dt_m15, TimeFrame.Enum.m15 };
      yield return new object[] { InCorrect_dt_m30, TimeFrame.Enum.m30 };
      yield return new object[] { InCorrect_dt_H1, TimeFrame.Enum.H1 };
      yield return new object[] { InCorrect_dt_H4, TimeFrame.Enum.H4 };
      yield return new object[] { InCorrect_dt_D1, TimeFrame.Enum.D1 };
      yield return new object[] { InCorrect_dt_W1, TimeFrame.Enum.W1 };
      yield return new object[] { InCorrect_dt_M, TimeFrame.Enum.M };
    }
  }


  [Theory]
  [MemberData(nameof(InCorrect_case))]
  public void WHEN_incorrect_dt_for_timeframe_THEN_invalid(IEnumerable<DateTime> dts, TimeFrame.Enum tfEnum)
  {
    #region Array
    this.logger.LogDebug("Test ARRAY");

    var validationAttribute = new FitTimeFrameAttribute("TfId", true);

    IDictionary<object, object?>? items = new Dictionary<object, object?>();
    items.Add("TfId", (int)tfEnum);

    #endregion

    foreach (var dt in dts)
    {
      #region Act
      this.logger.LogDebug("Test ACT");

      var asserted_result = FitTimeFrameAttribute.IsValid(dt, tfEnum, out var assertedMessage);

      #endregion


      #region Assert
      this.logger.LogDebug("Test ASSERT");

      Expect("DT is invalid", () => Assert.False(asserted_result));
      logger.LogInformation($"Validation message: {assertedMessage}");
      #endregion
    }
  }
}