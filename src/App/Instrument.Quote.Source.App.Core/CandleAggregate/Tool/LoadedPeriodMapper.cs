using Instrument.Quote.Source.App.Core.CandleAggregate.Dto;
using Instrument.Quote.Source.App.Core.CandleAggregate.Model;

namespace Instrument.Quote.Source.App.Core.CandleAggregate.Tool;

public static class LoadedPeriodMapper
{
  public static PeriodResponseDto ToDto(this LoadedPeriod loadedPeriod){
    return new PeriodResponseDto(){
      FromDate = loadedPeriod.FromDate,
      UntillDate = loadedPeriod.UntillDate
    };
  }
}