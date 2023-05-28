# Instrument Quote Source Service
- [Principles](./doc/Principles.md)
- [Components](./doc/Component.drawio.svg)
- API on URL/swagger/index.html
- [ARD](./doc/ADR)

## Functions
### [Instruments](./src/App/Instrument.Quote.Source.App.Core/InstrumentAggregate/Interface/IInstrumentSrv.cs)
- Add instrument
- Get List of instruments
- Remove instrument

### [Instrument Type](./src/App/Instrument.Quote.Source.App.Core/InstrumentAggregate/Interface/IInstrumentTypeSrv.cs)
- Get Instrument Types

### [TimeFrame](./src/App/Instrument.Quote.Source.App.Core/TimeFrameAggregate/Interface/ITimeFrameSrv.cs)
- Get all Timeframes

### [Candles](./src/App/Instrument.Quote.Source.App.Core/CandleAggregate/Interface/ICandleSrv.cs)
- Get candles for period
- Upload new candles
- Get all loaded periods

