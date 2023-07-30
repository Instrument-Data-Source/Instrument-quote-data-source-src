# Instrument Quote Source Service
- [Principles](./doc/Principles.md)
- [Components](./doc/Component.drawio.svg)
- API on URL/swagger/index.html
- [ARD](./doc/ADR)

## Functions
### [Instruments](./src/App/Instrument.Quote.Source.App.Core/InstrumentAggregate/Interface/IInstrumentSrv.cs)
- [Add instrument](./test/App/Instrument.Quote.Source.App.Test/InstrumentAggregate/IInstrumentSrv_Create_Test.cs)
- Get instrument
  - [Get  all](./test/App/Instrument.Quote.Source.App.Test/InstrumentAggregate/IInstrumentSrv_GetAll_Test.cs)
  - [Get by id](./test/App/Instrument.Quote.Source.App.Test/InstrumentAggregate/IInstrumentSrv_GetById_Test.cs)
  - [Get by code](./test/App/Instrument.Quote.Source.App.Test/InstrumentAggregate/IInstrumentSrv_GetByCode_Test.cs)
- [Remove instrument](./test/App/Instrument.Quote.Source.App.Test/InstrumentAggregate/IInstrumentSrv_Remove_Test.cs)

### [Instrument Type](./src/App/Instrument.Quote.Source.App.Core/InstrumentAggregate/Interface/IInstrumentTypeSrv.cs)
- Get Instrument Types
  - Get all
  - [Get by id](./test/App/Instrument.Quote.Source.App.Test/InstrumentAggregate/InstrumentType_GetById_Test.cs)
  - [Get by code](./test/App/Instrument.Quote.Source.App.Test/InstrumentAggregate/InstrumentType_GetByCode_Test.cs)

### [TimeFrame](./src/App/Instrument.Quote.Source.App.Core/TimeFrameAggregate/Interface/ITimeFrameSrv.cs)
- Get Timeframes
  - [Get all](./test/App/Instrument.Quote.Source.App.Test/TimeFrameAggregate/ITimeFrameSrv_GetAll_Test.cs) 
  - [Get by id](./test/App/Instrument.Quote.Source.App.Test/TimeFrameAggregate/ITimeFrameSrv_GetById_Test.cs) 
  - [Get by code](./test/App/Instrument.Quote.Source.App.Test/TimeFrameAggregate/ITimeFrameSrv_GetByCode_Test.cs) 

### [Chart](./src/App/Instrument.Quote.Source.App.Core/ChartAggregate/Interface/IChartSrv.cs)
- Get charts
  - [Get all](./test/App/Instrument.Quote.Source.App.Test/ChartAggregate/GetChart_GetAll_Test.cs)
  - [Get for instrument](./test/App/Instrument.Quote.Source.App.Test/ChartAggregate/GetChart_Get_Test.cs)

### [Candle](./src/App/Instrument.Quote.Source.App.Core/ChartAggregate/Interface/ICandlesSrv.cs)
- [Upload new](./test/App/Instrument.Quote.Source.App.Test/ChartAggregate/UploadChart.Test.cs)
- [Get for period](./test/App/Instrument.Quote.Source.App.Test/ChartAggregate/GetCandles.Test.cs)

