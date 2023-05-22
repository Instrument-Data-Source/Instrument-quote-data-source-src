# Instrument Quote Source Service
- [Principles](./doc/Principles.md)
- [Components](./doc/Component.md)


## Functions
### [Instruments](./src/App/Instrument.Quote.Source.App.Core/InstrumentAggregate/Interface/IInstrumentSrv.cs)
- Add instrument
- Get List of instruments

### [TimeFrame](./src/App/Instrument.Quote.Source.App.Core/TimeFrameAggregate/Interface/ITimeFrameSrv.cs)
- Get all Timeframes

### [Candles](./src/App/Instrument.Quote.Source.App.Core/CandleAggregate/Interface/ICandleSrv.cs)
- Get candles for period
- Upload new candles
- Get all loaded periods
