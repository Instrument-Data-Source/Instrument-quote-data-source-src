using InsonusK.Xunit.ExpectationsTest;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Instrument.Quote.Source.App.Core.Test.TimeFrameAggregate.Model;

public class GetStartDateTimeFor_Test : ExpectationsTestBase
{

  public GetStartDateTimeFor_Test(ITestOutputHelper output, LogLevel logLevel = LogLevel.Debug) : base(output, logLevel)
  {

  }
  public static IEnumerable<object[]> ConvertTestCase
  {
    get
    {
      yield return new object[] {
        TimeFrame.Enum.M,
        new Dictionary<DateTime, IEnumerable<DateTime>> {
          {
            new DateTime(2023,12,1),
            new List<DateTime>(){
              new DateTime(2023,12,1),
              new DateTime(2023,12,15),
              new DateTime(2023,12,15,3,4,5),
              new DateTime(2023,12,31),
              new DateTime(2023,12,31,23,59,59),
            }
          }
        }
      };

      yield return new object[] {
        TimeFrame.Enum.W1,
        new Dictionary<DateTime, IEnumerable<DateTime>> {
          {
            new DateTime(2023,6,26),
            new List<DateTime>(){
              new DateTime(2023,6,26),
              new DateTime(2023,6,26,3,6,7),
              new DateTime(2023,6,30),
              new DateTime(2023,6,30,3,4,5),
              new DateTime(2023,7,2,23,59,59),
            }
          }
        }
      };

      yield return new object[] {
        TimeFrame.Enum.D1,
        new Dictionary<DateTime, IEnumerable<DateTime>> {
          {
            new DateTime(2023,6,26),
            new List<DateTime>(){
              new DateTime(2023,6,26),
              new DateTime(2023,6,26,3,6,7),
              new DateTime(2023,6,26,23,59,59),
            }
          }
        }
      };

      yield return new object[] {
        TimeFrame.Enum.H4,
        new Dictionary<DateTime, IEnumerable<DateTime>> {
          {
            new DateTime(2023,6,26,0,0,0),
            new List<DateTime>(){
              new DateTime(2023,6,26),
              new DateTime(2023,6,26,0,0,0),
              new DateTime(2023,6,26,3,6,7),
              new DateTime(2023,6,26,3,59,59),
            }
          },
          {
            new DateTime(2023,6,26,4,0,0),
            new List<DateTime>(){
              new DateTime(2023,6,26,4,0,0),
              new DateTime(2023,6,26,6,6,7),
              new DateTime(2023,6,26,7,59,59),
            }
          }
          ,
          {
            new DateTime(2023,6,26,20,0,0),
            new List<DateTime>(){
              new DateTime(2023,6,26,20,0,0),
              new DateTime(2023,6,26,23,6,7),
              new DateTime(2023,6,26,23,59,59),
            }
          }
        }
      };

      yield return new object[] {
        TimeFrame.Enum.H1,
        new Dictionary<DateTime, IEnumerable<DateTime>> {
          {
            new DateTime(2023,6,26,0,0,0),
            new List<DateTime>(){
              new DateTime(2023,6,26),
              new DateTime(2023,6,26,0,0,0),
              new DateTime(2023,6,26,0,6,7),
              new DateTime(2023,6,26,0,59,59),
            }
          },
          {
            new DateTime(2023,6,26,4,0,0),
            new List<DateTime>(){
              new DateTime(2023,6,26,4,0,0),
              new DateTime(2023,6,26,4,6,7),
              new DateTime(2023,6,26,4,59,59),
            }
          }
          ,
          {
            new DateTime(2023,6,26,23,0,0),
            new List<DateTime>(){
              new DateTime(2023,6,26,23,0,0),
              new DateTime(2023,6,26,23,6,7),
              new DateTime(2023,6,26,23,59,59),
            }
          }
        }
      };

      yield return new object[] {
        TimeFrame.Enum.m30,
        new Dictionary<DateTime, IEnumerable<DateTime>> {
          {
            new DateTime(2023,6,26,0,0,0),
            new List<DateTime>(){
              new DateTime(2023,6,26),
              new DateTime(2023,6,26,0,0,0),
              new DateTime(2023,6,26,0,6,7),
              new DateTime(2023,6,26,0,29,59),
            }
          },
          {
            new DateTime(2023,6,26,0,30,0),
            new List<DateTime>(){
              new DateTime(2023,6,26,0,30,0),
              new DateTime(2023,6,26,0,36,7),
              new DateTime(2023,6,26,0,59,59),
            }
          }
          ,
          {
            new DateTime(2023,6,26,23,30,0),
            new List<DateTime>(){
              new DateTime(2023,6,26,23,30,0),
              new DateTime(2023,6,26,23,36,7),
              new DateTime(2023,6,26,23,59,59),
            }
          }
        }
      };

      yield return new object[] {
        TimeFrame.Enum.m15,
        new Dictionary<DateTime, IEnumerable<DateTime>> {
          {
            new DateTime(2023,6,26,0,0,0),
            new List<DateTime>(){
              new DateTime(2023,6,26),
              new DateTime(2023,6,26,0,0,0),
              new DateTime(2023,6,26,0,6,7),
              new DateTime(2023,6,26,0,14,59),
            }
          },
          {
            new DateTime(2023,6,26,0,15,0),
            new List<DateTime>(){
              new DateTime(2023,6,26,0,15,0),
              new DateTime(2023,6,26,0,16,7),
              new DateTime(2023,6,26,0,29,59),
            }
          }
          ,
          {
            new DateTime(2023,6,26,23,45,0),
            new List<DateTime>(){
              new DateTime(2023,6,26,23,45,0),
              new DateTime(2023,6,26,23,46,7),
              new DateTime(2023,6,26,23,59,59),
            }
          }
        }
      };

      yield return new object[] {
        TimeFrame.Enum.m10,
        new Dictionary<DateTime, IEnumerable<DateTime>> {
          {
            new DateTime(2023,6,26,0,0,0),
            new List<DateTime>(){
              new DateTime(2023,6,26),
              new DateTime(2023,6,26,0,0,0),
              new DateTime(2023,6,26,0,6,7),
              new DateTime(2023,6,26,0,9,59),
            }
          },
          {
            new DateTime(2023,6,26,0,10,0),
            new List<DateTime>(){
              new DateTime(2023,6,26,0,10,0),
              new DateTime(2023,6,26,0,16,7),
              new DateTime(2023,6,26,0,19,59),
            }
          }
          ,
          {
            new DateTime(2023,6,26,23,50,0),
            new List<DateTime>(){
              new DateTime(2023,6,26,23,50,0),
              new DateTime(2023,6,26,23,56,7),
              new DateTime(2023,6,26,23,59,59),
            }
          }
        }
      };

      yield return new object[] {
        TimeFrame.Enum.m5,
        new Dictionary<DateTime, IEnumerable<DateTime>> {
          {
            new DateTime(2023,6,26,0,0,0),
            new List<DateTime>(){
              new DateTime(2023,6,26),
              new DateTime(2023,6,26,0,0,0),
              new DateTime(2023,6,26,0,2,7),
              new DateTime(2023,6,26,0,4,59),
            }
          },
          {
            new DateTime(2023,6,26,0,5,0),
            new List<DateTime>(){
              new DateTime(2023,6,26,0,5,0),
              new DateTime(2023,6,26,0,6,7),
              new DateTime(2023,6,26,0,9,59),
            }
          }
          ,
          {
            new DateTime(2023,6,26,23,55,0),
            new List<DateTime>(){
              new DateTime(2023,6,26,23,55,0),
              new DateTime(2023,6,26,23,56,7),
              new DateTime(2023,6,26,23,59,59),
            }
          }
        }
      };

      yield return new object[] {
        TimeFrame.Enum.m1,
        new Dictionary<DateTime, IEnumerable<DateTime>> {
          {
            new DateTime(2023,6,26,0,0,0),
            new List<DateTime>(){
              new DateTime(2023,6,26),
              new DateTime(2023,6,26,0,0,0),
              new DateTime(2023,6,26,0,0,7),
              new DateTime(2023,6,26,0,0,59),
            }
          },
          {
            new DateTime(2023,6,26,0,1,0),
            new List<DateTime>(){
              new DateTime(2023,6,26,0,1,0),
              new DateTime(2023,6,26,0,1,7),
              new DateTime(2023,6,26,0,1,59),
            }
          }
          ,
          {
            new DateTime(2023,6,26,23,59,0),
            new List<DateTime>(){
              new DateTime(2023,6,26,23,59,0),
              new DateTime(2023,6,26,23,59,7),
              new DateTime(2023,6,26,23,59,59),
            }
          }
        }
      };
    }
  }
  [Theory]
  [MemberData(nameof(ConvertTestCase))]
  public void WHEN_give_datetime_THEN_return_correct_convertatino(TimeFrame.Enum timeframe, Dictionary<DateTime, IEnumerable<DateTime>> testCases)
  {
    Logger.LogDebug("Test: {0}", timeframe);
    foreach (var kvp in testCases)
    {
      #region Array

      Logger.LogDebug("Test ARRAY {0}", kvp.Key);
      var expectedDateTime = kvp.Key;

      #endregion

      foreach (var usingDateTime in kvp.Value)
      {
        #region Act
        Logger.LogDebug("Test ACT");

        var assertedDataTime = timeframe.GetFromDateTimeFor(usingDateTime);

        #endregion


        #region Assert
        Logger.LogDebug("Test ASSERT");

        Expect($"DateTime {usingDateTime} convert correctly", () => Assert.Equal(expectedDateTime, assertedDataTime));

        #endregion
      }
    }


  }
}