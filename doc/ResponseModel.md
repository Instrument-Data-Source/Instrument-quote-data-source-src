# Model of service responses

| Category             | Error        | BP Error | Model Error | Solution                   |
| -------------------- | ------------ | -------- | ----------- | -------------------------- |
| Success process      | Success      |          |             |                            |
| Business logic error | NotFound     | X        | X           | Create entity Or Change ID |
| Business logic error | Error        | X        |             | Use another BP flow        |
| Business logic error | DataModel VE |          | X           | Change DTO                 |
| Business logic error | BP ValidErr  | X        |             | Change DTO                 |
| Code error           | Exception    |          |             | Bugfix                     |


## Error category
- [Success process](#success-process)
- [Business Logic error](#business-logic-error)
- [Code error](#code-error)

## Error tpyes
- [Success](#success-200)
- [NotFound](#notfound)
- [Error](#error)
- [Data model validation Error](#data-model-validation-error)
- [Busines process validation Error](#busines-process-validation-error)
- [Exception](#exception)

## Description
### Success process
Success cases in business flow
- [Success](#success-200)

#### Success (200)
Success case of call, status code 200

### Business Logic error
Error described and predicted by business process OR made by external user actions
- [NotFound](#notfound)
- [Error](#error)
- [Data model validation Error](#data-model-validation-error)
- [Busines process validation Error](#busines-process-validation-error)

#### NotFound
Use non existed ID in call

**Example**
- Method use ID which was not created in system
  - Create entity and repeat call
 
#### Error
Errors predicted by **business process**

**Example**
- Call method in wrong time
  - Repeat call later
- Can not make a call, because entity has wrong status

#### Data model validation Error
Error in structure of data model

**Example**
- Errors in getted data: 
  - Field with count of candles in row is negative
  - Candle DateTime is out of chart bounds
  - High price is lower than open price
	
#### Busines process validation Error
Error in validation by business flow function
Difference to [Data model validation](#data-model-validation-error) that data model validation could be check with out connection to the service, using only data of DTO

**Example**
- Error in getted data:
	- Price decimal is too long for this instrument

### Code error
#### Exception
Error in code

**Example**
- Error made by code error or bug
  - bugfix
- Unexpected variable value
  - bugfix

## Cases
### DateTime must be fit TimeFrame
[FitTimeFrameAttribute](../src/App/Instrument.Quote.Source.App.Core/ChartAggregate/Validation/Attributes/FitTimeFrameAttribute.cs)

Context:
- This error made by user who make call => [Business logic error](#business-logic-error)
- You cann't change TimeFrame => [Validation error](#validation-error)

### Decimal part of candle price or volume is too long for instrument
[FitDecimalLenAttribute](../src/App/Instrument.Quote.Source.App.Core/ChartAggregate/Validation/Attributes/FitDecimalLenAttribute.cs)

Context:
- This error made by user => [Business logic error](#business-logic-error)
- Assume that previous steps was correct => [Validation error](#validation-error)
