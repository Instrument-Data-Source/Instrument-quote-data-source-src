using System.ComponentModel.DataAnnotations;

namespace Instrument.Quote.Source.App.Core.JoinedChartAggregate.Config;

public class JoinedChartModuleConfig
{
  [Range(1, int.MaxValue)]
  public int MaxSplitBaseCandleCount { get; set; } = 10000;

  public bool background { get; set; } = true;
}