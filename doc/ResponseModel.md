# Model of service responses

| Category             | Error      | BP Error | Model Error | Solution            |
| -------------------- | ---------- | -------- | ----------- | ------------------- |
| Success process      | Success    |          |             |                     |
| Business logic error | NotFound   | X        |             | Create entity       |
| Business logic error | Error      | X        |             | Use another BP flow |
| Business logic error | Validation | X        | X           | Change DTO          |
| Code error           | Exception  |          |             | Bugfix              |


## Error category
- [Success process](#success-process)
- [Business Logic error](#business-logic-error)
- [Code error](#code-error)

## Error tpyes
- [Success](#success-200)
- [NotFound](#notfound)
- [Error](#error)
- [ValidationError](#validation-error)
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
- [ValidationError](#validation-error)

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

#### Validation Error
Error in data model

**Example**
- Error in getted data
	- Change dto
	- Doesn't change if you change call order

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
